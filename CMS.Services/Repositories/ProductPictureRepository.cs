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
    public interface IProductPictureRepository : IRepositoryBase<ProductPicture>
    {
        Task<List<ProductPicture>> ProductPictureGetLstByProductId(int productId);

        Task<bool> ProductPictureDeleteAllByProductId(int productId);

        Task<bool> ProductPictureInsert(List<ProductPicture> model, string userId,int productId);

        Task<bool> ProductPictureDeleteById(string productPictureId);

        
    }
    public class ProductPictureRepository : RepositoryBase<ProductPicture>, IProductPictureRepository
    {
        public ProductPictureRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }

        public async Task<bool> ProductPictureDeleteAllByProductId(int productId)
        {
            try
            {
                var item = await CmsContext.ProductPicture.Where(x => x.ProductId == productId).ToListAsync();
                if (item != null)
                {
                    CmsContext.ProductPicture.RemoveRange(item);
                    await CmsContext.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<bool> ProductPictureDeleteById(string productPictureId)
        {
            try
            {
                var item = await CmsContext.ProductPicture.FirstOrDefaultAsync(x => x.Id == productPictureId);
                if (item != null)
                {
                    CmsContext.ProductPicture.Remove(item);
                    await CmsContext.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<List<ProductPicture>> ProductPictureGetLstByProductId(int productId)
        {
            List<ProductPicture> output = new();
            try
            {
                output = await CmsContext.ProductPicture.Where(x => x.ProductId == productId).ToListAsync();
            }
            catch
            {

            }
            return output;
        }

        public async Task<bool> ProductPictureInsert(List<ProductPicture> model, string userId,int productId)
        {
            try
            {
                var productItem = await CmsContext.Product.FirstOrDefaultAsync(x => x.Id == productId);
                if (productItem != null)
                {
                    foreach (var p in model)
                    {
                        p.Id = Guid.NewGuid().ToString();
                        p.ProductId = productItem.Id;
                        p.CreateDate = DateTime.Now;
                        p.LastEditDate = DateTime.Now;
                        p.CreateBy = userId;
                        p.LastEditBy = userId;
                        CmsContext.ProductPicture.Add(p);
                        await CmsContext.SaveChangesAsync();
                    }
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> ProductPictureInsert(ProductPicture model, int productId)
        {
            throw new NotImplementedException();
        }
    }
}
