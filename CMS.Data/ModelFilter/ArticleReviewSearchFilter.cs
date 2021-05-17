namespace CMS.Data.ModelFilter
{
    public class ArticleReviewSearchFilter
    {
        public string Keyword { get; set; }
        public int? ArticleReviewId { get; set; }
        public int? ArticleReviewStatusId { get; set; }
        public int? PageSize = 100;
        public int? CurrentPage = 1;
    }
}
