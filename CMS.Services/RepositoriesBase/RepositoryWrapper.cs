using CMS.Data.ModelEntity;
using CMS.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CMS.Services.RepositoriesBase
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private IDbContextFactory<CmsContext> _cmsContext { get; set; }
        private IAccountRepository _accountRepository;
        private IArticleRepository _articleRepository;
        private IArticleCategoryRepository _articleCategoryRepository;
        private IPermissionRepository _permissionRepository;
        private IAdvertisingRepository _advertisingRepository;
        private ISettingRepository _setting;
        private IUserNotiRepository _userNoti;
        private IArticleCommentRepository _articleComment;
        private IArticleCommentStaffRepository _articleCommentStaff;
        private IMasterDataRepository _masterDate;
        private IProductBrandRepository _productBrand;
        private IProductCategoryRepository _productCategory;
        private IProductCommentRepository _productComment;
        private IProductCommentStaffRepository _productCommentStaff;
        private IProductRepository _product;
        private IProductPictureRepository _productPicture;
        private IProductPropertiesRepository _productProperties;

        public RepositoryWrapper(IDbContextFactory<CmsContext> CmsContext)
        {
            _cmsContext = CmsContext;
        }

        public IProductPropertiesRepository ProductProperties
        {
            get
            {
                if (_productProperties == null)
                {
                    _productProperties = new ProductPropertiesRepository(_cmsContext.CreateDbContext());
                }

                return _productProperties;
            }
        }

        public IProductPictureRepository ProductPicture
        {
            get
            {
                if (_productPicture == null)
                {
                    _productPicture = new ProductPictureRepository(_cmsContext.CreateDbContext());
                }

                return _productPicture;
            }
        }

        public IProductBrandRepository ProductBrand
        {
            get
            {
                if (_productBrand == null)
                {
                    _productBrand = new ProductBrandRepository(_cmsContext.CreateDbContext());
                }

                return _productBrand;
            }
        }

        public IProductCommentRepository ProductComment
        {
            get
            {
                if (_productComment == null)
                {
                    _productComment = new ProductCommentRepository(_cmsContext.CreateDbContext());
                }

                return _productComment;
            }
        }

        public IProductCommentStaffRepository ProductCommentStaff
        {
            get
            {
                if (_productCommentStaff == null)
                {
                    _productCommentStaff = new ProductCommentStaffRepository(_cmsContext.CreateDbContext());
                }

                return _productCommentStaff;
            }
        }


        public IProductRepository Product
        {
            get
            {
                if (_product == null)
                {
                    _product = new ProductRepository(_cmsContext.CreateDbContext());
                }

                return _product;
            }
        }

        public IProductCategoryRepository ProductCategory
        {
            get
            {
                if (_productCategory == null)
                {
                    _productCategory = new ProductCategoryRepository(_cmsContext.CreateDbContext());
                }

                return _productCategory;
            }
        }

        public IMasterDataRepository MasterData
        {
            get
            {
                if (_masterDate == null)
                {
                    _masterDate = new MasterDataRepository(_cmsContext.CreateDbContext());
                }

                return _masterDate;
            }
        }

        public IArticleCommentRepository ArticleComment
        {
            get
            {
                if (_articleComment == null)
                {
                    _articleComment = new ArticleCommentRepository(_cmsContext.CreateDbContext());
                }

                return _articleComment;
            }
        }

        public IArticleCommentStaffRepository ArticleCommentStaff
        {
            get
            {
                if (_articleCommentStaff == null)
                {
                    _articleCommentStaff = new ArticleCommentStaffRepository(_cmsContext.CreateDbContext());
                }

                return _articleCommentStaff;
            }
        }

        public IUserNotiRepository UserNoti
        {
            get
            {
                if (_userNoti == null)
                {
                    _userNoti = new UserNotiRepository(_cmsContext.CreateDbContext());
                }

                return _userNoti;
            }
        }

        public ISettingRepository Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = new SettingRepository(_cmsContext.CreateDbContext());
                }

                return _setting;
            }
        }

        public IAccountRepository AspNetUsers
        {
            get
            {
                if (_accountRepository == null)
                {
                    _accountRepository = new AccountRepository(_cmsContext.CreateDbContext());
                }

                return _accountRepository;
            }
        }

        public IAdvertisingRepository Advertising
        {
            get
            {
                if (_advertisingRepository == null)
                {
                    _advertisingRepository = new AdvertisingRepository(_cmsContext.CreateDbContext());
                }

                return _advertisingRepository;
            }
        }

        public IArticleRepository Article
        {
            get
            {
                if (_articleRepository == null)
                {
                    _articleRepository = new ArticleRepository(_cmsContext.CreateDbContext());
                }

                return _articleRepository;
            }
        }

        public IArticleCategoryRepository ArticleCategory
        {
            get
            {
                if (_articleCategoryRepository == null)
                {
                    _articleCategoryRepository = new ArticleCategoryRepository(_cmsContext.CreateDbContext());
                }

                return _articleCategoryRepository;
            }
        }

        public IPermissionRepository Permission
        {
            get
            {
                if (_permissionRepository == null)
                {
                    _permissionRepository = new PermissionRepository(_cmsContext.CreateDbContext());
                }

                return _permissionRepository;
            }
        }

        public void Save()
        {
            using var CmsContext = _cmsContext.CreateDbContext();
            CmsContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            using var CmsContext = _cmsContext.CreateDbContext();
            return CmsContext.SaveChangesAsync();
        }
    }
}