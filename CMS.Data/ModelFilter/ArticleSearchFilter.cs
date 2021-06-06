using System;

namespace CMS.Data.ModelFilter
{
    public class ArticleSearchFilter
    {
        public string Keyword { get; set; }
        public string Tags { get; set; }
        public int? ArticleCategoryId { get; set; }
        public int? ArticleStatusId { get; set; }
        public int? ProductBrandId { get; set; }
        public int? ArticleTypeId { get; set; }
        public int? ExceptionId { get; set; }
        public bool? ExceptionArticleTop { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool? Efficiency { get; set; }
        public bool? Active { get; set; }
        public string AssignBy { get; set; }
        public string CreateBy { get; set; }
        public int? PageSize = 10;
        public int? CurrentPage = 1;
        public int StartIndex { get; set; }


    }
}
