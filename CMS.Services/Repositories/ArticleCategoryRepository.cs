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

        Task<List<ArticleCategory>> GetArticleCategoryByUserId(string UserId);

        Task<ArticleCategory> GetArticleCategoryByUrl(string Url);

        Task<List<ArticleCategoryArticle>> GetLstArticleCatebyArticleId(int articleId);

        Task<List<ArticleCategoryAssign>> ArticleCategoryAssignsGetLstByUserId(string userId);

        Task ArticleCategoryAssignsUpdate(string userId,List<int> articleCategoryId);
    }

    public class ArticleCategoryRepository : RepositoryBase<ArticleCategory>, IArticleCategoryRepository
    {
        public ArticleCategoryRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }

        public async Task<List<ArticleCategoryAssign>> ArticleCategoryAssignsGetLstByUserId(string userId)
        {
            List<ArticleCategoryAssign> lstOutput = new();
            try
            {
                lstOutput = await CmsContext.ArticleCategoryAssign.Where(x => x.AspNetUsersId == userId).ToListAsync();
            }
            catch
            {

            }
            return lstOutput;
        }

        public async Task ArticleCategoryAssignsUpdate(string userId, List<int> articleCategoryId)
        {
            var listItem = new List<ArticleCategoryAssign>();
            var item = await CmsContext.ArticleCategoryAssign.Where(p => p.AspNetUsersId == userId).ToListAsync();
            if (item != null) // Update
            {
                CmsContext.ArticleCategoryAssign.RemoveRange(item);
                await CmsContext.SaveChangesAsync();
            }
            //Add
            foreach (var p in articleCategoryId)
            {
                var itemArtCate = new ArticleCategoryAssign();
                itemArtCate.AspNetUsersId = userId;
                itemArtCate.ArticleCategoryId = p;
                listItem.Add(itemArtCate);
            }
            await CmsContext.ArticleCategoryAssign.AddRangeAsync(listItem);
            await CmsContext.SaveChangesAsync();
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

        public async Task<List<ArticleCategory>> GetArticleCategoryByUserId(string UserId)
        {
            List<ArticleCategory> lstOutput = new();
            try
            {
                var lstCate = await CmsContext.ArticleCategoryAssign.Where(x => x.AspNetUsersId == UserId).Select(x => x.ArticleCategoryId).ToListAsync();
                if(lstCate !=null )
                {
                    lstOutput = await CmsContext.ArticleCategory.Where(x => lstCate.Contains(x.Id)).ToListAsync();
                }    

            }
            catch
            {

            }
            return lstOutput;
        }

        public async Task<List<ArticleCategoryArticle>> GetLstArticleCatebyArticleId(int articleId)
        {
            var lstArtCate = new List<ArticleCategoryArticle>();
            try
            {
                lstArtCate = await CmsContext.ArticleCategoryArticle.Where(p => p.ArticleId == articleId).AsNoTracking().ToListAsync();
            }
            catch
            {
            }
            return lstArtCate;
        }
    }
}