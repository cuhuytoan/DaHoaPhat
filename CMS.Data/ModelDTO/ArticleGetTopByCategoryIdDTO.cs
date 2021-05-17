using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.ModelDTO
{
    public class ArticleGetTopByCategoryIdDTO
    {
        [Key]
        public int NoItem { get; set; }
        public int Id { get; set; }
        public Nullable<int> ProductBrandId { get; set; }
        public Nullable<int> ArticleTypeId { get; set; }
        public string ArticleCategoryIds { get; set; }
        public string Name { get; set; }
        public string SubTitle { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> Counter { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public string URL { get; set; }
        public string Tags { get; set; }
    }
}
