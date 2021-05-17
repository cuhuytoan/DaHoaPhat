using CMS.Data.ModelEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IArticleCategoryRepository : IRepositoryBase<ArticleCategory>
    {
        Task<List<ArticleCategory>> GetArticleCategoryById(int? ArticleCategoryId);
        Task<ArticleCategory> GetArticleCategoryByUrl(string Url);
    }
    public class ArticleCategoryRepository : RepositoryBase<ArticleCategory>, IArticleCategoryRepository
    {

        public ArticleCategoryRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {

        }


        public async Task<List<ArticleCategory>> GetArticleCategoryById(int? ArticleCategoryId)
        {

            if (ArticleCategoryId != null)
            {
                return await CmsContext.ArticleCategory.Where(p => p.Id == ArticleCategoryId)
                    .ToListAsync();
            }
            else
            {
                return await CmsContext.ArticleCategory.ToListAsync();
            }
        }

        public async Task<ArticleCategory> GetArticleCategoryByUrl(string Url)
        {

            return await CmsContext.ArticleCategory.FirstOrDefaultAsync(p => p.Url == Url);
        }
    }
}
