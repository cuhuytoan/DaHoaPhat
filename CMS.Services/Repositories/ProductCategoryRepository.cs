using CMS.Data.ModelEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IProductCategoryRepository : IRepositoryBase<ProductCategory>
    {
        Task<List<ProductCategory>> GetProductCategoryById(int? ProductCategoryId);

        Task<List<ProductCategory>> GetProductCategoryByUserId(string UserId);

        Task<ProductCategory> GetProductCategoryByUrl(string Url);

        Task<List<ProductCategoryProduct>> GetLstProductCatebyProductId(int articleId);

        Task<List<ProductCategoryAssign>> ProductCategoryAssignsGetLstByUserId(string userId);

        
    }
    public class ProductCategoryRepository : RepositoryBase<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }
        public async Task<List<ProductCategoryAssign>> ProductCategoryAssignsGetLstByUserId(string userId)
        {
            List<ProductCategoryAssign> lstOutput = new();
            try
            {
                lstOutput = await CmsContext.ProductCategoryAssign.Where(x => x.AspNetUsersId == userId).ToListAsync();
            }
            catch
            {

            }
            return lstOutput;
        }

        public async Task ProductCategoryAssignsUpdate(string userId, List<int> articleCategoryId)
        {
            var listItem = new List<ProductCategoryAssign>();
            var item = await CmsContext.ProductCategoryAssign.Where(p => p.AspNetUsersId == userId).ToListAsync();
            if (item != null) // Update
            {
                CmsContext.ProductCategoryAssign.RemoveRange(item);
                await CmsContext.SaveChangesAsync();
            }
            //Add
            foreach (var p in articleCategoryId)
            {
                var itemArtCate = new ProductCategoryAssign();
                itemArtCate.AspNetUsersId = userId;
                itemArtCate.ProductCategoryId = p;
                listItem.Add(itemArtCate);
            }
            await CmsContext.ProductCategoryAssign.AddRangeAsync(listItem);
            await CmsContext.SaveChangesAsync();
        }

        public async Task<List<ProductCategory>> GetProductCategoryById(int? ProductCategoryId)
        {
            if (ProductCategoryId != null)
            {
                return await CmsContext.ProductCategory.Where(p => p.Id == ProductCategoryId)
                    .ToListAsync();
            }
            else
            {
                return await CmsContext.ProductCategory.ToListAsync();
            }
        }

        public async Task<ProductCategory> GetProductCategoryByUrl(string Url)
        {
            return await CmsContext.ProductCategory.FirstOrDefaultAsync(p => p.Url == Url);
        }

        public async Task<List<ProductCategory>> GetProductCategoryByUserId(string UserId)
        {
            List<ProductCategory> lstOutput = new();
            try
            {
                var lstCate = await CmsContext.ProductCategoryAssign.Where(x => x.AspNetUsersId == UserId).Select(x => x.ProductCategoryId).ToListAsync();
                if (lstCate != null)
                {
                    lstOutput = await CmsContext.ProductCategory.Where(x => lstCate.Contains(x.Id)).ToListAsync();
                }

            }
            catch
            {

            }
            return lstOutput;
        }

        public async Task<List<ProductCategoryProduct>> GetLstProductCatebyProductId(int articleId)
        {
            var lstArtCate = new List<ProductCategoryProduct>();
            try
            {
                lstArtCate = await CmsContext.ProductCategoryProduct.Where(p => p.ProductId == articleId).AsNoTracking().ToListAsync();
            }
            catch
            {
            }
            return lstArtCate;
        }
    }   
}
