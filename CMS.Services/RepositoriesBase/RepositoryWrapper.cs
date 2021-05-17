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
        private IArticleReviewRepository _articleReview;
        private IMagazinePaperRepository _magazinePaper;

        public RepositoryWrapper(IDbContextFactory<CmsContext> CmsContext)
        {
            _cmsContext = CmsContext;

        }
        public IArticleReviewRepository ArticleReview
        {
            get
            {
                if (_articleReview == null)
                {
                    _articleReview = new ArticleReviewRepository(_cmsContext.CreateDbContext());
                }

                return _articleReview;
            }
        }
        public IMagazinePaperRepository MagazinePaper
        {
            get
            {
                if (_magazinePaper == null)
                {
                    _magazinePaper = new MagazinePaperRepository(_cmsContext.CreateDbContext());
                }

                return _magazinePaper;
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
