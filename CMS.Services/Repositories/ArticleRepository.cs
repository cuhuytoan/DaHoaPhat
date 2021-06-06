using CMS.Common;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IArticleRepository : IRepositoryBase<Article>
    {      
        Task<int?> ArticleInsert(Article model, string userId, int articleStatusId, List<int> articleCategoryArticle);
      
        Task ArticleUpdate(Article model, string UserId, int ArticleStatusId, List<int> articleCategoryArticle);
       
        Task ArticleDelete(int ArticleId);

        Task<Article> ArticleGetById(int ArticleId);

        Task<VirtualizeResponse<SpArticleSearchResult>> ArticleSearchWithPaging(ArticleSearchFilter model);

        void ArticleTopCategorySave(int ArticleId);

        void ArticleTopParentCategorySave(int ArticleId);

        Task ArticleUpdateStatusType(string userId, int ArticleId, int StatusTypeId);

        Task<string> CreateArticleURL(int ArticleId);

        Task<List<ArticleStatus>> GetLstArticleStatus();

        Task<List<ArticleStatus>> GetLstArticleStatusByUserId(string userId);

        Task<List<ArticleAttachFile>> ArticleAttachGetLstByArticleId(int articleId);

        Task<bool> ArticleAttachDelete(int articleAttachFileId);

        Task<bool> ArticleAttachInsert(List<ArticleAttachFile> model);

      
    }

    public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
    {
        public ArticleRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }

        public async Task<int?> ArticleInsert(Article model, string userId, int articleStatusId, List<int> articleCategoryArticle)
        {
            // Add một lần
            model.ArticleTypeId = 1;
            model.ProductBrandId = 0;
            model.ArticleStatusId = articleStatusId;
            model.CreateBy = userId;
            model.CreateDate = DateTime.Now;
            model.LastEditDate = DateTime.Now;
            model.LastEditBy = userId;
            model.CanCopy = true;
            model.CanComment = true;
            model.CanDelete = true;
            model.Active = true;
            model.Counter = 0;
            model.ProductBrandId = 0;
            model.Approved = -1;
            model.Checked = -1;
            CmsContext.Article.Add(model);

            await CmsContext.SaveChangesAsync();
            //Insert articleCategoryArticle
            await ArticleSetArticleCategory(model.Id, articleCategoryArticle);
            return model.Id;
        }

        public async Task ArticleUpdate(Article model, string userId, int articleStatusId, List<int> articleCategoryArticle)
        {
            try
            {
                var items = CmsContext.Article.FirstOrDefault(p => p.Id == model.Id);
                if (items != null)
                {            
                    model.LastEditBy = userId;
                    model.LastEditDate = DateTime.Now;

                    if (string.IsNullOrEmpty(items.Url))
                    {
                        items.Url = await CreateArticleURL(items.Id);
                    }
                    CmsContext.Entry(items).CurrentValues.SetValues(model);
                    await CmsContext.SaveChangesAsync();
                    //Insert articleCategoryArticle
                    await ArticleSetArticleCategory(model.Id, articleCategoryArticle);
                }
            }
            catch
            {
            }
        }

        public async Task ArticleDelete(int ArticleId)
        {
            try
            {
                var items = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId);
                if (items != null)
                {
                    CmsContext.Article.Remove(items);
                    await CmsContext.SaveChangesAsync();
                }
            }
            catch
            {
            }
        }

        public async Task<Article> ArticleGetById(int ArticleId)
        {
            return await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId);
        }        

        public async Task<VirtualizeResponse<SpArticleSearchResult>> ArticleSearchWithPaging(ArticleSearchFilter model)
        {
            var output = new VirtualizeResponse<SpArticleSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();

            var result = await CmsContext.GetProcedures().SpArticleSearchAsync(
            model.Keyword,            
            model.ArticleCategoryId,
            model.ArticleStatusId,
            model.ProductBrandId,
            model.ArticleTypeId,
            model.ExceptionId,
            model.ExceptionArticleTop,
            model.FromDate,
            model.ToDate,
            model.Efficiency,
            model.Active,
            model.AssignBy,
            model.CreateBy,
            model.PageSize,
            model.CurrentPage, itemCounts, returnValues
            );
            output.Items = result.ToList();
            output.TotalSize = (int)itemCounts.Value;
            return output;
        }

        public async Task ArticleSetArticleCategory(int ArticleId, List<int> articleCategoryArticle)
        {
            var listItem = new List<ArticleCategoryArticle>();
            var item = await CmsContext.ArticleCategoryArticle.Where(p => p.ArticleId == ArticleId).ToListAsync();
            if (item != null) // Update
            {
                CmsContext.ArticleCategoryArticle.RemoveRange(item);
                await CmsContext.SaveChangesAsync();
            }
            //Add
            foreach (var p in articleCategoryArticle)
            {
                var itemArtCate = new ArticleCategoryArticle();
                itemArtCate.ArticleId = ArticleId;
                itemArtCate.ArticleCategoryId = p;
                listItem.Add(itemArtCate);
            }
            await CmsContext.ArticleCategoryArticle.AddRangeAsync(listItem);
            await CmsContext.SaveChangesAsync();
        }

        public async Task<string> CreateArticleURL(int ArticleId)
        {
            try
            {
                var currentArticle = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId);
                return FormatURL(currentArticle?.Name) + "-" + ArticleId.ToString();
            }
            catch
            {
            }
            return "nourl";
        }

        public void ArticleTopCategorySave(int ArticleId)
        {
            var ArticleCategoryArticle_Item = CmsContext.ArticleCategoryArticle.FirstOrDefault(p => p.ArticleId == ArticleId);
            int ArticleCategoryId = ArticleCategoryArticle_Item.ArticleCategoryId;

            //var ArticleTop_Items = CmsContext.ArticleTop.Where(p => p.ArticleCategoryId == ArticleCategoryId);
            //CmsContext.ArticleTop.RemoveRange(ArticleTop_Items);

            ArticleTop ArticleTop_Item = new ArticleTop();
            ArticleTop_Item.ArticleCategoryId = ArticleCategoryId;
            ArticleTop_Item.ArticleId = ArticleId;
            CmsContext.ArticleTop.Add(ArticleTop_Item);

            CmsContext.SaveChanges();
        }

        public void ArticleTopParentCategorySave(int ArticleId)
        {
            var ArticleCategoryArticle_Item = CmsContext.ArticleCategoryArticle.FirstOrDefault(p => p.ArticleId == ArticleId);
            int ArticleCategoryId = ArticleCategoryArticle_Item.ArticleCategoryId;

            var ArticleCategory_Item = CmsContext.ArticleCategory.FirstOrDefault(p => p.Id == ArticleCategoryId);

            if (ArticleCategory_Item.ParentId != null)
            {
                int ArticleCategoryParentId = ArticleCategory_Item.ParentId.Value;
                //var ArticleTop_Items = CmsContext.ArticleTop.Where(p => p.ArticleCategoryId == ArticleCategoryParentId);
                //CmsContext.ArticleTop.RemoveRange(ArticleTop_Items);

                ArticleTop ArticleTop_Item = new ArticleTop();
                ArticleTop_Item.ArticleCategoryId = ArticleCategoryParentId;
                ArticleTop_Item.ArticleId = ArticleId;
                CmsContext.ArticleTop.Add(ArticleTop_Item);

                CmsContext.SaveChanges();
            }
        }
               
        public async Task ArticleUpdateStatusType(string userId,int ArticleId, int StatusTypeId)
        {
            var article = await CmsContext.Article.FirstOrDefaultAsync(p => p.Id == ArticleId);
            if (article != null)
            {
                if(StatusTypeId == 2 || StatusTypeId == 3) // Kiểm tra
                {
                    article.Checked = 1;
                    article.CheckBy = userId;
                    article.CheckDate = DateTime.Now;
                }
                else if(StatusTypeId == 4) //Duyệt
                {
                    article.Approved = 1;
                    article.ApproveBy = userId;
                    article.ApproveDate = DateTime.Now;
                }
                else if (StatusTypeId == 0) //Từ chối đăng
                {
                    article.Approved = 0;
                    article.ApproveBy = userId;
                    article.ApproveDate = DateTime.Now;
                }
                article.ArticleStatusId = StatusTypeId;
                await CmsContext.SaveChangesAsync();
            }
        }

        public async Task<List<ArticleStatus>> GetLstArticleStatus()
        {
            return await CmsContext.ArticleStatus.ToListAsync();
        }

        public async Task<List<ArticleAttachFile>> ArticleAttachGetLstByArticleId(int articleId)
        {
            return await CmsContext.ArticleAttachFile.Where(p => p.ArticleId == articleId).ToListAsync();
        }

        public async Task<bool> ArticleAttachDelete(int articleAttachFileId)
        {
            var item = await CmsContext.ArticleAttachFile.FindAsync(articleAttachFileId);
            if (item != null)
            {
                CmsContext.ArticleAttachFile.Remove(item);
                await CmsContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ArticleAttachInsert(List<ArticleAttachFile> model)
        {
            try
            {
                foreach (var p in model)
                {
                    CmsContext.Entry(p).State = EntityState.Added;
                    await CmsContext.SaveChangesAsync();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

   

        public async Task<List<ArticleStatus>> GetLstArticleStatusByUserId(string userId)
        {
            var lstRole = await CmsContext.AspNetUserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToListAsync();
            if(lstRole !=null && lstRole.Count > 0)
            {
                if(lstRole.Contains("6df4162d-38a4-42e9-b3d3-a07a5c29215b")) // Phụ trách chuyên mục
                {
                    List<int> lstStatusId = new List<int> { 1, 2,3 }; //Trạng thái mới gửi và đã kiểm tra
                    return await CmsContext.ArticleStatus.Where(x => lstStatusId.Contains(x.Id)).ToListAsync();
                }
                else
                {
                    return await CmsContext.ArticleStatus.ToListAsync();
                }
            }
            else
            {
                return await CmsContext.ArticleStatus.ToListAsync();
            }


        }
    }
}