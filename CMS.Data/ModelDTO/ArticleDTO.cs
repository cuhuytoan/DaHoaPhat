using CMS.Data.ValidationCustomize;
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.ModelDTO
{
    public class ArticleDTO
    {
        public int? Id { get; set; }
        public int? ArticleTypeId { get; set; }
        public string ArticleCategoryIds { get; set; }
        public int? ProductBrandId { get; set; }
        public int? ArticleStatusId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bài viết")]
        [MaxWords(25, ErrorMessage = "Tên bài không vượt quá 25 từ")]
        public string Name { get; set; }

        public string SubTitle { get; set; }
        public string Image { get; set; }
        public string ImageDescription { get; set; }
        public string BannerImage { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tóm tắt")]
        [MaxWords(250, ErrorMessage = "Nội dung tóm tắt không quá 250 từ")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        //[MinWords(5000, ErrorMessage = "Nội dung tối thiểu 5000 từ")]
        //[MaxWords(10000, ErrorMessage = "Nội dung tối đa 10000 từ")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tác giả")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu")]
        public DateTime? StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc")]
        public DateTime? EndDate { get; set; } = DateTime.Now.AddYears(1);

        public bool? Active { get; set; }
        public int? Counter { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastEditBy { get; set; }
        public DateTime? LastEditDate { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }
        public bool? CanCopy { get; set; }
        public bool? CanComment { get; set; }
        public bool? CanDelete { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int? ArticleContentTypeId { get; set; }
    }
}