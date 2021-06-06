using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IArticleCommentStaffRepository : IRepositoryBase<ArticleCommentStaff>
    {
        Task<VirtualizeResponse<SpArticleCommentStaffSearchResult>> ArticleCommentStaffSearch(ArticleCommentStaffSearchFilter model);

        Task<int> ArticleCommentStaffPostComment(ArticleCommentStaff model);

        Task<bool> ArticleCommentStaffDelete(int articleCommentId);
    }
    public class ArticleCommentStaffRepository : RepositoryBase<ArticleCommentStaff>, IArticleCommentStaffRepository
    {
        public ArticleCommentStaffRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }
        public async Task<bool> ArticleCommentStaffDelete(int articleCommentId)
        {
            var item = await CmsContext.ArticleCommentStaff.FindAsync(articleCommentId);
            if (item != null)
            {
                CmsContext.ArticleCommentStaff.Remove(item);
                await CmsContext.SaveChangesAsync();
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<VirtualizeResponse<SpArticleCommentStaffSearchResult>> ArticleCommentStaffSearch(ArticleCommentStaffSearchFilter model)
        {
            var output = new VirtualizeResponse<SpArticleCommentStaffSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();

            var result = await CmsContext.GetProcedures().SpArticleCommentStaffSearchAsync(
                model.Keyword,
                model.ArticleId,
                model.Active,
                model.CreateBy,
                model.PageSize,
                model.CurrentPage,
                itemCounts,
                returnValues
                );
            output.Items = result.ToList();
            output.TotalSize = (int)itemCounts.Value;
            return output;
        }

        public async Task<int> ArticleCommentStaffPostComment(ArticleCommentStaff model)
        {
            CmsContext.Entry(model).State = model.Id > 0 ? EntityState.Modified : EntityState.Added;            
            await CmsContext.SaveChangesAsync();
            CmsContext.ChangeTracker.Clear();

            return model.Id;
        }
    }
}
