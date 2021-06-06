using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IProductCommentRepository : IRepositoryBase<ProductComment>
    {
        Task<VirtualizeResponse<SpProductCommentSearchResult>> ProductCommentSearch(ProductCommentSearchFilter model);

        Task<int> ProductCommentPostComment(ProductComment model);

        Task<bool> ProductCommentDelete(int articleCommentId);
    }
    public class ProductCommentRepository : RepositoryBase<ProductComment>, IProductCommentRepository
    {
        public ProductCommentRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }
        public async Task<bool> ProductCommentDelete(int articleCommentId)
        {
            var item = await CmsContext.ProductComment.FindAsync(articleCommentId);
            if (item != null)
            {
                CmsContext.ProductComment.Remove(item);
                await CmsContext.SaveChangesAsync();
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<VirtualizeResponse<SpProductCommentSearchResult>> ProductCommentSearch(ProductCommentSearchFilter model)
        {
            var output = new VirtualizeResponse<SpProductCommentSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();

            var result = await CmsContext.GetProcedures().SpProductCommentSearchAsync(
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

        public async Task<int> ProductCommentPostComment(ProductComment model)
        {
            CmsContext.Entry(model).State = model.Id > 0 ? EntityState.Modified : EntityState.Added;

            await CmsContext.SaveChangesAsync();
            CmsContext.ChangeTracker.Clear();
            return model.Id;
        }
    } 
}
