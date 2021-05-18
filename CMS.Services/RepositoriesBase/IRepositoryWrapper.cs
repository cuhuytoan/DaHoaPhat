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
        void Save();
        Task<int> SaveChangesAsync();
    }
}
