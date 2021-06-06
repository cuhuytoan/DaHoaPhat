using CMS.Services.Repositories;
using System.Threading.Tasks;

namespace CMS.Services.RepositoriesBase
{
    public interface IRepositoryWrapper
    {
        IAccountRepository AspNetUsers { get; }
        IArticleRepository Article { get; }
        IArticleCategoryRepository ArticleCategory { get; }
        IPermissionRepository Permission { get; }
        IAdvertisingRepository Advertising { get; }
        ISettingRepository Setting { get; }
        IUserNotiRepository UserNoti { get; }
        IArticleCommentRepository ArticleComment { get; }
        IArticleCommentStaffRepository ArticleCommentStaff { get; }
        IMasterDataRepository MasterData { get; }
        IProductBrandRepository ProductBrand { get; }
        IProductCategoryRepository ProductCategory { get; }
        IProductCommentRepository ProductComment { get; }
        IProductCommentStaffRepository ProductCommentStaff { get; }
        IProductRepository Product { get; }
        IProductPictureRepository ProductPicture { get; }
        IProductPropertiesRepository ProductProperties { get; }
        void Save();

        Task<int> SaveChangesAsync();
    }
}