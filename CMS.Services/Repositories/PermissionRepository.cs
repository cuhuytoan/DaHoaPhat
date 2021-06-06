using CMS.Data.ModelEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IPermissionRepository : IRepositoryBase<AspNetUsers>
    {
        #region Interface Article
        //Article
        bool CanViewArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage);

        bool CanAddNewArticle(ClaimsPrincipal user, string userId, ref string messsage);

        bool CanEditArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage);

        bool CanDeleteArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage);

        bool CanCommentArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage);

        bool CanCommentStaffArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage);

        bool CanEditCommentStaffArticle(ClaimsPrincipal user, string userId, int commentId, ref string messsage);

        bool CanEditCommentArticle(ClaimsPrincipal user, string userId, int commentId, ref string messsage);
        #endregion

        #region Interface Product
        //Product

        bool CanViewProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage);

        bool CanAddNewProduct(ClaimsPrincipal user, string userId, ref string messsage);

        bool CanEditProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage);

        bool CanDeleteProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage);

        bool CanCommentProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage);

        bool CanCommentStaffProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage);

        bool CanEditCommentStaffProduct(ClaimsPrincipal user, string userId, int commentId, ref string messsage);

        bool CanEditCommentProduct(ClaimsPrincipal user, string userId, int commentId, ref string messsage);
        #endregion


    }

    public class PermissionRepository : RepositoryBase<AspNetUsers>, IPermissionRepository
    {
        public PermissionRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }
        #region Article
        public bool CanAddNewArticle(ClaimsPrincipal user, string userId, ref string messsage)
        {
            if (!user.IsInRole("Bạn đọc"))
            {
                return true;
            }
            messsage = "Bạn không có quyền thêm mới bài viết";
            return false;
        }

        public bool CanCommentArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage)
        {
            var articleItem = CmsContext.Article.Find(articleId);
            if (articleItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && articleItem.CreateBy == userId)
                {
                    return true;
                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ArticleCategoryArticle.Where(x => x.ArticleId == articleId).Select(x => x.ArticleCategoryId).ToList();
                    var articleAssign = CmsContext.ArticleCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ArticleCategoryId != null && lstArtCategoryItem.Contains(x.ArticleCategoryId.Value)).ToList();
                    if (articleAssign != null && articleAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền bình luận bài viết";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền bình luận bài viết";
            return false;
        }
        public bool CanCommentStaffArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage)
        {
            var articleItem = CmsContext.Article.Find(articleId);
            if (articleItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && articleItem.CreateBy == userId)
                {
                    return true;
                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ArticleCategoryArticle.Where(x => x.ArticleId == articleId).Select(x => x.ArticleCategoryId).ToList();
                    var articleAssign = CmsContext.ArticleCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ArticleCategoryId != null && lstArtCategoryItem.Contains(x.ArticleCategoryId.Value)).ToList();
                    if (articleAssign != null && articleAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền bình luận bài viết";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền bình luận bài viết";
            return false;
        }
        public bool CanDeleteArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage)
        {
            var articleItem = CmsContext.Article.Find(articleId);
            if (articleItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && articleItem.CreateBy == userId)
                {
                    if (articleItem.ArticleStatusId == 1)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền xóa bài viết khi đã cập nhật trạng thái";
                        return false;
                    }

                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ArticleCategoryArticle.Where(x => x.ArticleId == articleId).Select(x => x.ArticleCategoryId).ToList();
                    var articleAssign = CmsContext.ArticleCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ArticleCategoryId != null && lstArtCategoryItem.Contains(x.ArticleCategoryId.Value)).ToList();
                    if (articleAssign != null && articleAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền xóa bài viết không thuộc chuyên mục";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền xóa bài viết";
            return false;
        }

        public bool CanEditArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage)
        {
            var articleItem = CmsContext.Article.Find(articleId);
            if (articleItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && articleItem.CreateBy == userId)
                {
                    if (articleItem.ArticleStatusId == 1 || articleItem.ArticleStatusId == 3)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền chỉnh sửa bài viết khi đã cập nhật trạng thái";
                        return false;
                    }

                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ArticleCategoryArticle.Where(x => x.ArticleId == articleId).Select(x => x.ArticleCategoryId).ToList();
                    var articleAssign = CmsContext.ArticleCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ArticleCategoryId != null && lstArtCategoryItem.Contains(x.ArticleCategoryId.Value)).ToList();
                    if (articleAssign != null && articleAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền chỉnh sửa bài viết không thuộc chuyên mục";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền chỉnh sửa bài viết";
            return false;
        }

        public bool CanEditCommentArticle(ClaimsPrincipal user, string userId, int commentId, ref string messsage)
        {
            var item = CmsContext.ArticleComment.Where(x => x.Id == commentId && x.CreateBy == userId);
            if (item != null)
            {
                return true;
            }
            return false;
        }

        public bool CanEditCommentStaffArticle(ClaimsPrincipal user, string userId, int commentId, ref string messsage)
        {
            var item = CmsContext.ArticleComment.Where(x => x.Id == commentId && x.CreateBy == userId);
            if (item != null)
            {
                return true;
            }
            return false;
        }

        public bool CanViewArticle(ClaimsPrincipal user, string userId, int articleId, ref string messsage)
        {
            var articleItem = CmsContext.Article.Find(articleId);
            if (articleItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && articleItem.CreateBy == userId)
                {
                    return true;
                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ArticleCategoryArticle.Where(x => x.ArticleId == articleId).Select(x => x.ArticleCategoryId).ToList();
                    var articleAssign = CmsContext.ArticleCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ArticleCategoryId != null && lstArtCategoryItem.Contains(x.ArticleCategoryId.Value)).ToList();
                    if (articleAssign != null && articleAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền xem bài viết";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền xem bài viết";
            return false;
        }
        #endregion

        #region Product
   
        public bool CanAddNewProduct(ClaimsPrincipal user, string userId, ref string messsage)
        {
            if (!user.IsInRole("Bạn đọc"))
            {
                return true;
            }
            messsage = "Bạn không có quyền thêm mới bài viết";
            return false;
        }

        public bool CanCommentProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage)
        {
            var productItem = CmsContext.Product.Find(productId);
            if (productItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && productItem.CreateBy == userId)
                {
                    return true;
                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ProductCategoryProduct.Where(x => x.ProductId == productId).Select(x => x.ProductCategoryId).ToList();
                    var productAssign = CmsContext.ProductCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ProductCategoryId != null && lstArtCategoryItem.Contains(x.ProductCategoryId.Value)).ToList();
                    if (productAssign != null && productAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền bình luận bài viết";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền bình luận bài viết";
            return false;
        }
        public bool CanCommentStaffProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage)
        {
            var productItem = CmsContext.Product.Find(productId);
            if (productItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && productItem.CreateBy == userId)
                {
                    return true;
                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ProductCategoryProduct.Where(x => x.ProductId == productId).Select(x => x.ProductCategoryId).ToList();
                    var productAssign = CmsContext.ProductCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ProductCategoryId != null && lstArtCategoryItem.Contains(x.ProductCategoryId.Value)).ToList();
                    if (productAssign != null && productAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền bình luận bài viết";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền bình luận bài viết";
            return false;
        }
        public bool CanDeleteProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage)
        {
            var productItem = CmsContext.Product.Find(productId);
            if (productItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && productItem.CreateBy == userId)
                {
                    if (productItem.ProductStatusId == 1)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền xóa bài viết khi đã cập nhật trạng thái";
                        return false;
                    }

                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ProductCategoryProduct.Where(x => x.ProductId == productId).Select(x => x.ProductCategoryId).ToList();
                    var productAssign = CmsContext.ProductCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ProductCategoryId != null && lstArtCategoryItem.Contains(x.ProductCategoryId.Value)).ToList();
                    if (productAssign != null && productAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền xóa bài viết không thuộc chuyên mục";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền xóa bài viết";
            return false;
        }

        public bool CanEditProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage)
        {
            var productItem = CmsContext.Product.Find(productId);
            if (productItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && productItem.CreateBy == userId)
                {
                    if (productItem.ProductStatusId == 1 || productItem.ProductStatusId == 3)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền chỉnh sửa bài viết khi đã cập nhật trạng thái";
                        return false;
                    }

                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ProductCategoryProduct.Where(x => x.ProductId == productId).Select(x => x.ProductCategoryId).ToList();
                    var productAssign = CmsContext.ProductCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ProductCategoryId != null && lstArtCategoryItem.Contains(x.ProductCategoryId.Value)).ToList();
                    if (productAssign != null && productAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền chỉnh sửa bài viết không thuộc chuyên mục";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền chỉnh sửa bài viết";
            return false;
        }

        public bool CanEditCommentProduct(ClaimsPrincipal user, string userId, int commentId, ref string messsage)
        {
            var item = CmsContext.ProductComment.Where(x => x.Id == commentId && x.CreateBy == userId);
            if (item != null)
            {
                return true;
            }
            return false;
        }

        public bool CanEditCommentStaffProduct(ClaimsPrincipal user, string userId, int commentId, ref string messsage)
        {
            var item = CmsContext.ProductComment.Where(x => x.Id == commentId && x.CreateBy == userId);
            if (item != null)
            {
                return true;
            }
            return false;
        }

        public bool CanViewProduct(ClaimsPrincipal user, string userId, int productId, ref string messsage)
        {
            var productItem = CmsContext.Product.Find(productId);
            if (productItem != null)
            {
                if (user.IsInRole("Quản trị hệ thống") || user.IsInRole("Lãnh đạo tòa soạn") || user.IsInRole("Lãnh đạo tòa soạn"))
                {
                    return true;
                }

                if ((user.IsInRole("Biên tập viên") || user.IsInRole("Cộng tác viên")) && productItem.CreateBy == userId)
                {
                    return true;
                }

                if (user.IsInRole("Phụ trách chuyên mục"))
                {
                    List<int> lstArtCategoryItem = CmsContext.ProductCategoryProduct.Where(x => x.ProductId == productId).Select(x => x.ProductCategoryId).ToList();
                    var productAssign = CmsContext.ProductCategoryAssign.Where(x => x.AspNetUsersId == userId && x.ProductCategoryId != null && lstArtCategoryItem.Contains(x.ProductCategoryId.Value)).ToList();
                    if (productAssign != null && productAssign.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        messsage = "Không có quyền xem bài viết";
                        return false;
                    }
                }
            }
            else
            {
                messsage = "Không tìm thấy bài viết";
                return false;
            }
            messsage = "Không có quyền xem bài viết";
            return false;
        }
       
        #endregion

    }
}