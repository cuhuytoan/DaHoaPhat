using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IProductBrandRepository : IRepositoryBase<ProductBrand>
    {

    }
    public class ProductBrandRepository : RepositoryBase<ProductBrand>, IProductBrandRepository
    {
        public ProductBrandRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }
    }
}
