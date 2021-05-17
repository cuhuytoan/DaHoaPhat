using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{

    public interface IArticleCommentRepository : IRepositoryBase<ArticleComment>
    {
        Task<VirtualizeResponse<SpArticleCommentSearchResult>> ArticleCommentSearch(ArticleCommentSearchFilter model);
        Task<int> ArticleCommentPostComment(ArticleComment model);
        Task<bool> ArticleCommentDelete(int articleCommentId);
    }
    public class ArticleCommentRepository : RepositoryBase<ArticleComment>, IArticleCommentRepository
    {
        public ArticleCommentRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {

        }

        public async Task<bool> ArticleCommentDelete(int articleCommentId)
        {
            var item = await CmsContext.ArticleComment.FindAsync(articleCommentId);
            if (item != null)
            {
                CmsContext.ArticleComment.Remove(item);
                await CmsContext.SaveChangesAsync();
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<VirtualizeResponse<SpArticleCommentSearchResult>> ArticleCommentSearch(ArticleCommentSearchFilter model)
        {
            var output = new VirtualizeResponse<SpArticleCommentSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();

            var result = await CmsContext.GetProcedures().SpArticleCommentSearchAsync(
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

        public async Task<int> ArticleCommentPostComment(ArticleComment model)
        {
            CmsContext.Entry(model).State = EntityState.Added;
            await CmsContext.SaveChangesAsync();

            return model.Id;
        }
    }
}
