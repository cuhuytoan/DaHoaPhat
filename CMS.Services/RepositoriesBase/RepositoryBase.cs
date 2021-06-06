using CMS.Data.ModelEntity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.RepositoriesBase
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected CmsContext CmsContext { get; set; }
        private DbSet<T> table = null;

        public RepositoryBase(CmsContext CmsContext)
        {
            this.CmsContext = CmsContext;
            table = CmsContext.Set<T>();
        }

        public IQueryable<T> FindAll()
        {
            return CmsContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return CmsContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            CmsContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            CmsContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            CmsContext.Set<T>().Remove(entity);
        }

        public ValueTask<T> FindAsync(object id)
        {
            return CmsContext.Set<T>().FindAsync(id);
        }

        public T GetById(object id)
        {
            return CmsContext.Set<T>().Find(id);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return CmsContext.Set<T>().FirstOrDefault(expression);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await CmsContext.Set<T>().SingleOrDefaultAsync(expression);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await CmsContext.Set<T>().FirstOrDefaultAsync(expression);
        }

        public string FormatURL(string Title)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] bytes = Encoding.UTF8.GetBytes(Title.ToLower());
            string StrSource = encoder.GetString(bytes).Trim();

            StrSource = StrSource.Replace(((char)34).ToString(), "");
            StrSource = StrSource.Replace(((char)8220).ToString(), "");
            StrSource = StrSource.Replace(((char)8221).ToString(), "");
            StrSource = StrSource.Replace(" ", "-");

            var replaceChars = CmsContext.ReplaceChar.ToList();
            foreach (var charItem in replaceChars)
            {
                StrSource = StrSource.Replace(charItem.OldChar, charItem.NewChar).ToString();
            }

            StrSource = RepairURL(StrSource);
            return StrSource;
        }

        public string RepairURL(string URL)
        {
            string tmp = URL;

            foreach (char chr in URL)
            {
                if (!((chr >= 'a' && chr <= 'z') || (chr == '-') || (chr >= '0' && chr <= '9')))
                {
                    tmp = tmp.Replace(chr, '-');
                    CmsContext.Database.ExecuteSqlRaw("INSERT ReplaceChar (OldChar, NewChar) VALUES (@OldChar, @NewChar)",
                        new SqlParameter("@OldChar", chr),
                        new SqlParameter("@NewChar", '-')
                        );
                }
            }

            return tmp;
        }

        public async Task SendMail(string FromName, string ToEmail, string ToName, string Subject, string Body)
        {
            var setting = await CmsContext.Setting.FirstOrDefaultAsync();

            SmtpClient smtpClient = new();
            smtpClient.Host = setting.EmailSenderSmtp;
            smtpClient.Port = int.Parse(setting.EmailSenderPort);
            smtpClient.EnableSsl = setting.EmailSenderSsl.Value;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential(setting.EmailSender, setting.EmailSenderPassword);
            smtpClient.Timeout = 20000;

            MailAddress FromAddress = new MailAddress(setting.EmailSender, FromName);
            MailAddress ToAddress = new MailAddress(ToEmail, ToName);
            MailMessage Mailer = new MailMessage(FromAddress, ToAddress);
            Mailer.IsBodyHtml = true;
            Mailer.BodyEncoding = System.Text.Encoding.UTF8;
            Mailer.Subject = Subject;
            Mailer.Body = Body;

            try
            {
                smtpClient.Send(Mailer);
            }
            catch
            { }
        }
    }
}