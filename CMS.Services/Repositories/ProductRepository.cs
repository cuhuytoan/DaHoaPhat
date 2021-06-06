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
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<int?> ProductInsert(Product model, string userId, int productStatusId, List<int> productCategoryProduct);

        Task ProductUpdate(Product model, string UserId, int ProductStatusId, List<int> productCategoryProduct);

        Task ProductDelete(int ProductId);

        Task<Product> ProductGetById(int ProductId);

        Task<VirtualizeResponse<SpProductSearchResult>> ProductSearchWithPaging(ProductSearchFilter model);

        void ProductTopCategorySave(int ProductId);

        void ProductTopParentCategorySave(int ProductId);

        Task ProductUpdateStatusType(string userId, int ProductId, int StatusTypeId);

        Task<string> CreateProductURL(int ProductId);

        Task<List<ProductStatus>> GetLstProductStatus();

        Task<List<ProductStatus>> GetLstProductStatusByUserId(string userId);

        Task<List<ProductAttachFile>> ProductAttachGetLstByProductId(int productId);

        Task<bool> ProductAttachDelete(int productAttachFileId);

        Task<bool> ProductAttachInsert(List<ProductAttachFile> model);

        Task<List<ProductType>> ProductTypeGetLst();

        Task<List<ProductManufacture>> ProductManufacturesGetLst();

       
    }
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }
        public async Task<int?> ProductInsert(Product model, string userId, int productStatusId, List<int> productCategoryProduct)
        {
            // Add một lần          
            model.ProductBrandId = 0;
            model.ProductStatusId = productStatusId;
            model.QrcodePublic = Utils.GetRandomText(6);
            model.CreateBy = userId;
            model.CreateDate = DateTime.Now;
            model.LastEditDate = DateTime.Now;
            model.LastEditBy = userId;
            model.CanCopy = true;
            model.CanComment = true;
            model.CanDelete = true;
            model.Active = true;
            model.Counter = 0;
            model.SellCount = 0;
            model.ProductBrandId = 0;
            model.Approved = -1;
            model.Checked = -1;
            CmsContext.Product.Add(model);

            await CmsContext.SaveChangesAsync();
            //Insert productCategoryProduct
            await ProductSetProductCategory(model.Id, productCategoryProduct);
            return model.Id;
        }

        public async Task ProductUpdate(Product model, string userId, int productStatusId, List<int> productCategoryProduct)
        {
            try
            {
                await CmsContext.SaveChangesAsync();
                var items = CmsContext.Product.FirstOrDefault(p => p.Id == model.Id);
                if (items != null)
                {                    
                    model.LastEditBy = userId;
                    model.LastEditDate = DateTime.Now;                    
                    if (string.IsNullOrEmpty(items.Url))
                    {
                        items.Url = await CreateProductURL(items.Id);
                    }
                    CmsContext.Entry(items).CurrentValues.SetValues(model);
                    await CmsContext.SaveChangesAsync();
                    //Insert productCategoryProduct
                    await ProductSetProductCategory(model.Id, productCategoryProduct);
                }
            }
            catch(Exception ex)
            {
            }
        }

        public async Task ProductDelete(int ProductId)
        {
            try
            {
                var items = await CmsContext.Product.FirstOrDefaultAsync(p => p.Id == ProductId);
                if (items != null)
                {
                    CmsContext.Product.Remove(items);
                    await CmsContext.SaveChangesAsync();
                }
            }
            catch
            {
            }
        }

        public async Task<Product> ProductGetById(int ProductId)
        {
            return await CmsContext.Product.FirstOrDefaultAsync(p => p.Id == ProductId);
        }


        public async Task<VirtualizeResponse<SpProductSearchResult>> ProductSearchWithPaging(ProductSearchFilter model)
        {
            var output = new VirtualizeResponse<SpProductSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();

            var result = await CmsContext.GetProcedures().SpProductSearchAsync(
            model.Keyword,
            model.ProductCategoryId,
            model.ProductManufactureId,
            model.ProductStatusId,
            model.CountryId,
            model.LocationId,
            model.DepartmentManId,
            model.ProductBrandId,
            model.ProductTypeId,
            model.ExceptionId,
            model.ExceptionProductTop,
            model.FromPrice,
            model.ToPrice,
            model.FromDate,
            model.ToDate,
            model.Efficiency,
            model.Active,
            model.AssignBy,
            model.CreateBy,
            model.OrderBy,
            model.PageSize,
            model.CurrentPage, itemCounts, returnValues
            );
            output.Items = result.ToList();
            output.TotalSize = (int)itemCounts.Value;
            return output;
        }

        public async Task ProductSetProductCategory(int ProductId, List<int> productCategoryProduct)
        {
            var listItem = new List<ProductCategoryProduct>();
            var item = await CmsContext.ProductCategoryProduct.Where(p => p.ProductId == ProductId).ToListAsync();
            if (item != null) // Update
            {
                CmsContext.ProductCategoryProduct.RemoveRange(item);
                await CmsContext.SaveChangesAsync();
            }
            //Add
            foreach (var p in productCategoryProduct)
            {
                var itemArtCate = new ProductCategoryProduct();
                itemArtCate.ProductId = ProductId;
                itemArtCate.ProductCategoryId = p;
                listItem.Add(itemArtCate);
            }
            await CmsContext.ProductCategoryProduct.AddRangeAsync(listItem);
            await CmsContext.SaveChangesAsync();
        }

        public async Task<string> CreateProductURL(int ProductId)
        {
            try
            {
                var currentProduct = await CmsContext.Product.FirstOrDefaultAsync(p => p.Id == ProductId);
                return FormatURL(currentProduct?.Name) + "-" + ProductId.ToString();
            }
            catch
            {
            }
            return "nourl";
        }

        public string ProductGetStatusString(int? ProductStatusId)
        {
            string Result = "";
            var currentProductStatus = CmsContext.ProductStatus.FirstOrDefault(p => p.Id == ProductStatusId);

            if (currentProductStatus.Id == 0)
            {
                Result = "<label class='badge badge-info'>Đã lưu</label>";
            }
            if (currentProductStatus.Id == 1)
            {
                Result = "<label class='badge badge-warning'>Chờ duyệt</label>";
            }
            if (currentProductStatus.Id == 2)
            {
                Result = "<label class='badge badge-success'>Đã đăng</label>";
            }

            return Result;
        }

        public void ProductTopCategorySave(int ProductId)
        {
            var ProductCategoryProduct_Item = CmsContext.ProductCategoryProduct.FirstOrDefault(p => p.ProductId == ProductId);
            int ProductCategoryId = ProductCategoryProduct_Item.ProductCategoryId;

            //var ProductTop_Items = CmsContext.ProductTop.Where(p => p.ProductCategoryId == ProductCategoryId);
            //CmsContext.ProductTop.RemoveRange(ProductTop_Items);

            ProductTop ProductTop_Item = new ProductTop();
            ProductTop_Item.ProductCategoryId = ProductCategoryId;
            ProductTop_Item.ProductId = ProductId;
            CmsContext.ProductTop.Add(ProductTop_Item);

            CmsContext.SaveChanges();
        }

        public void ProductTopParentCategorySave(int ProductId)
        {
            var ProductCategoryProduct_Item = CmsContext.ProductCategoryProduct.FirstOrDefault(p => p.ProductId == ProductId);
            int ProductCategoryId = ProductCategoryProduct_Item.ProductCategoryId;

            var ProductCategory_Item = CmsContext.ProductCategory.FirstOrDefault(p => p.Id == ProductCategoryId);

            if (ProductCategory_Item.ParentId != null)
            {
                int ProductCategoryParentId = ProductCategory_Item.ParentId.Value;
                //var ProductTop_Items = CmsContext.ProductTop.Where(p => p.ProductCategoryId == ProductCategoryParentId);
                //CmsContext.ProductTop.RemoveRange(ProductTop_Items);

                ProductTop ProductTop_Item = new ProductTop();
                ProductTop_Item.ProductCategoryId = ProductCategoryParentId;
                ProductTop_Item.ProductId = ProductId;
                CmsContext.ProductTop.Add(ProductTop_Item);

                CmsContext.SaveChanges();
            }
        }

        public async Task ProductUpdateStatusType(string userId, int ProductId, int StatusTypeId)
        {
            var product = await CmsContext.Product.FirstOrDefaultAsync(p => p.Id == ProductId);
            if (product != null)
            {
                if (StatusTypeId == 2 || StatusTypeId == 3) // Kiểm tra
                {
                    product.Checked = 1;
                    product.CheckBy = userId;
                    product.CheckDate = DateTime.Now;
                }
                else if (StatusTypeId == 4) //Duyệt
                {
                    product.Approved = 1;
                    product.ApproveBy = userId;
                    product.ApproveDate = DateTime.Now;
                }
                else if (StatusTypeId == 0) //Từ chối đăng
                {
                    product.Approved = 0;
                    product.ApproveBy = userId;
                    product.ApproveDate = DateTime.Now;
                }
                product.ProductStatusId = StatusTypeId;
                await CmsContext.SaveChangesAsync();
            }
        }

        public async Task<List<ProductStatus>> GetLstProductStatus()
        {
            return await CmsContext.ProductStatus.ToListAsync();
        }

        public async Task<List<ProductAttachFile>> ProductAttachGetLstByProductId(int productId)
        {
            return await CmsContext.ProductAttachFile.Where(p => p.ProductId == productId).ToListAsync();
        }

        public async Task<bool> ProductAttachDelete(int productAttachFileId)
        {
            var item = await CmsContext.ProductAttachFile.FindAsync(productAttachFileId);
            if (item != null)
            {
                CmsContext.ProductAttachFile.Remove(item);
                await CmsContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ProductAttachInsert(List<ProductAttachFile> model)
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



        public async Task<List<ProductStatus>> GetLstProductStatusByUserId(string userId)
        {
            var lstRole = await CmsContext.AspNetUserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToListAsync();
            if (lstRole != null && lstRole.Count > 0)
            {
                if (lstRole.Contains("6df4162d-38a4-42e9-b3d3-a07a5c29215b")) // Phụ trách chuyên mục
                {
                    List<int> lstStatusId = new List<int> { 1, 2, 3 }; //Trạng thái mới gửi và đã kiểm tra
                    return await CmsContext.ProductStatus.Where(x => lstStatusId.Contains(x.Id)).ToListAsync();
                }
                else
                {
                    return await CmsContext.ProductStatus.ToListAsync();
                }
            }
            else
            {
                return await CmsContext.ProductStatus.ToListAsync();
            }


        }

        public async Task<List<ProductType>> ProductTypeGetLst()
        {
            return await CmsContext.ProductType.AsNoTracking().ToListAsync();
        }

        public async Task<List<ProductManufacture>> ProductManufacturesGetLst()
        {
            return await CmsContext.ProductManufacture.AsNoTracking().ToListAsync();
        }
    }   
}
