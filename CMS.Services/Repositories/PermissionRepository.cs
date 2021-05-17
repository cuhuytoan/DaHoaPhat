using CMS.Data.ModelEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{

    public interface IPermissionRepository : IRepositoryBase<AspNetUsers>
    {
        Task<bool> CanEditArticle(int ArticleId, string UserId);
        Task<bool> CanDeleteArticle(int ArticleId, string UserId);
    }
    public class PermissionRepository : RepositoryBase<AspNetUsers>, IPermissionRepository
    {

        public PermissionRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {

        }

        public bool CanAddNewArticle(string UserId)
        {
            bool Result = true;

            return Result;
        }

        public async Task<bool> CanEditArticle(int ArticleId, string UserId)
        {
            var articleItem = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId && p.CreateBy == UserId);
            if (articleItem != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanDeleteArticle(int ArticleId, string UserId)
        {
            var articleItem = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId && p.CreateBy == UserId);
            if (articleItem != null)
            {
                return true;
            }
            return false;
        }
    }
}
