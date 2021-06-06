using CMS.Common;
using CMS.Data.ModelEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IAdvertisingRepository : IRepositoryBase<Advertising>
    {
        Task<List<AdvertisingBlock>> AdvertisingBlockGetAll(bool isMobile);

        Task<AdvertisingBlock> AdvertisingBlockGetById(int AdvertisingBlockId);

        Task<int> AdvertisingInsert(Advertising model, string UserId, IHostingEnvironment _env);

        Task AdvertisingUpdate(Advertising model, string UserId, IEnumerable<IFormFile> mainimages, IHostingEnvironment _env);

        Task AdvertisingDelete(int AdvertisingId);

        Task AdvertisingActive(int AdvertisingId, bool Active);

        Task<List<Advertising>> AdvertisingGetByAdvertisingBlockId(int id);
    }

    public class AdvertisingRepository : RepositoryBase<Advertising>, IAdvertisingRepository
    {
        public AdvertisingRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }

        public async Task<List<AdvertisingBlock>> AdvertisingBlockGetAll(bool isMobile)
        {
            return await CmsContext.AdvertisingBlock.Where(p => p.IsMobile == isMobile).ToListAsync();
        }

        public async Task<AdvertisingBlock> AdvertisingBlockGetById(int AdvertisingBlockId)
        {
            return await CmsContext.AdvertisingBlock.FirstOrDefaultAsync(p => p.Id == AdvertisingBlockId);
        }

        public async Task<int> AdvertisingInsert(Advertising model, string UserId, IHostingEnvironment _env)
        {
            model.Active = false;
            model.CreateBy = UserId;
            model.CreateDate = DateTime.Now;
            model.LastEditDate = DateTime.Now;
            model.LastEditBy = UserId;
            CmsContext.Advertising.Add(model);

            await CmsContext.SaveChangesAsync();
            return model.Id;
        }

        public async Task AdvertisingUpdate(Advertising model, string UserId, IEnumerable<IFormFile> mainimages, IHostingEnvironment _env)
        {
            var items = CmsContext.Advertising.FirstOrDefault(p => p.Id == model.Id);
            if (items != null)
            {
                items.Title = model.Title;
                items.Width = model.Width;
                items.Height = model.Height;
                items.Description = model.Description;
                items.Code = model.Code;
                items.Url = model.Url;
                items.Sort = model.Sort;
                items.Active = model.Active;
                items.LastEditDate = DateTime.Now;
                items.LastEditBy = UserId;
                await CmsContext.SaveChangesAsync();

                if (mainimages != null)
                {
                    await SaveImage(mainimages, model.Id, _env);
                }
            }
        }

        public async Task AdvertisingDelete(int id)
        {
            try
            {
                var items = CmsContext.Advertising.FirstOrDefault(p => p.Id == id);
                if (items != null)
                {
                    CmsContext.Advertising.Remove(items);
                    await CmsContext.SaveChangesAsync();
                }
            }
            catch
            {
            }
        }

        public async Task AdvertisingActive(int id, bool Active)
        {
            var advertising = await CmsContext.Advertising.FirstOrDefaultAsync(p => p.Id == id);
            if (advertising != null)
            {
                advertising.Active = Active;
                CmsContext.SaveChanges();
            }
        }

        public async Task<List<Advertising>> AdvertisingGetByAdvertisingBlockId(int AdvertisingBlockId)
        {
            return await CmsContext.Advertising.Where(p => p.AdvertisingBlockId == AdvertisingBlockId).OrderBy(p => p.Sort).ToListAsync();
        }

        private async Task SaveImage(IEnumerable<IFormFile> files, int id, IHostingEnvironment _env)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file == null) return;
                    var urlArticle = CreateAdvertisingURL(id);
                    var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    var fileName = String.Format("{0}-{1}.{2}", urlArticle, timestamp, file.ContentType.Replace("image/", ""));
                    var physicalPath = Path.Combine(_env.WebRootPath, "data/advertising/mainimages/original", fileName);
                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    try
                    {
                        Utils.EditSize(Path.Combine(_env.WebRootPath, "data/advertising/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/article/mainimages/small", fileName), 500, 500);
                        Utils.EditSize(Path.Combine(_env.WebRootPath, "data/advertising/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/article/mainimages/thumb", fileName), 120, 120);
                    }
                    catch
                    {
                    }

                    var advertising = CmsContext.Advertising.FirstOrDefault(x => x.Id == id);
                    advertising.Image = fileName;
                    CmsContext.SaveChanges();
                }
            }
        }

        private string CreateAdvertisingURL(int id)
        {
            try
            {
                var currentArticle = CmsContext.Advertising.FirstOrDefault(p => p.Id == id);
                return FormatURL(currentArticle?.Title) + "-" + id.ToString();
            }
            catch
            {
            }
            return "nourl";
        }
    }
}