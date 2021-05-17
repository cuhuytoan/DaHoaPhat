using CMS.Data.ModelEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.ModelDTO
{
    public class ArticleReviewDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Nhập tên đợt phản biện")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Nhập mô tả đợt phản biện")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Nhập ngày bắt đầu")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Nhập ngày kết thúc")]
        public DateTime? EndDate { get; set; }
        public string CreateBy { get; set; }
        public string LastEditBy { get; set; }
    }
    public class PostArticleReviewDTO
    {
        public ArticleReviewDTO ArticleReview { get; set; } = new ArticleReviewDTO();
        public List<int> LstArticleId { get; set; } = new List<int>();
        public List<string> LstReviewPerson { get; set; } = new List<string>();
    }
    public class ArticleReviewPersonDTO
    {

        public int Id { get; set; }
        public int? ArticleReviewId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastEditDate { get; set; }
        public string LastEditBy { get; set; }
    }
    public class ArticleReviewArticleDTO
    {
        public int Id { get; set; }
        public int? ArticleReviewId { get; set; }
        public int? ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string Author { get; set; }
        public string MainImage { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastEditDate { get; set; }
        public string LastEditBy { get; set; }
    }
    public class ArticleReviewDetailDTO
    {

        public int Id { get; set; }
        public int? ArticleId { get; set; }
        [Required(ErrorMessage = "Nhập bình luận")]
        [MinLength(50, ErrorMessage = "Tối thiểu 50 kí tự")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Nhập bình luận")]
        [MinLength(50, ErrorMessage = "Tối thiểu 50 kí tự")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Nhập bình luận")]
        [MinLength(50, ErrorMessage = "Tối thiểu 50 kí tự")]
        public string Layout { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastEditDate { get; set; }
        public string LastEditBy { get; set; }
        public int? ArticleReviewStatusId { get; set; }
    }
    public class ArticleReviewColectionDTO
    {
        public SpArticleInReviewSearchResult ArticleSearch { get; set; } = new SpArticleInReviewSearchResult();
        public List<SpArticleReviewDetailSearchResult> LstArticleInReview { get; set; } = new List<SpArticleReviewDetailSearchResult>();
    }

}


