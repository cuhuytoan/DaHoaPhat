using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IProductCommentStaffRepository : IRepositoryBase<ProductCommentStaff>
    {
        Task<VirtualizeResponse<SpProductCommentStaffSearchResult>> ProductCommentStaffSearch(ProductCommentStaffSearchFilter model);

        Task<int> ProductCommentStaffPostComment(ProductCommentStaff model);

        Task<bool> ProductCommentStaffDelete(int articleCommentId);
    }
    public class ProductCommentStaffRepository : RepositoryBase<ProductCommentStaff>, IProductCommentStaffRepository
    {
        public ProductCommentStaffRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }
        public async Task<bool> ProductCommentStaffDelete(int articleCommentId)
        {
            var item = await CmsContext.ProductCommentStaff.FindAsync(articleCommentId);
            if (item != null)
            {
                CmsContext.ProductCommentStaff.Remove(item);
                await CmsContext.SaveChangesAsync();
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<VirtualizeResponse<SpProductCommentStaffSearchResult>> ProductCommentStaffSearch(ProductCommentStaffSearchFilter model)
        {
            var output = new VirtualizeResponse<SpProductCommentStaffSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();

            var result = await CmsContext.GetProcedures().SpProductCommentStaffSearchAsync(
                model.Keyword,
                model.ProductId,
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

        public async Task<int> ProductCommentStaffPostComment(ProductCommentStaff model)
        {
            CmsContext.Entry(model).State = model.Id > 0 ? EntityState.Modified : EntityState.Added;
            await CmsContext.SaveChangesAsync();
            CmsContext.ChangeTracker.Clear();

            return model.Id;
        }
    }
}
