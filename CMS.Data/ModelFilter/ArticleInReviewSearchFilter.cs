namespace CMS.Data.ModelFilter
{
    public class ArticleInReviewSearchFilter
    {
        public string Keyword { get; set; }
        public int? ArticleReviewId { get; set; }
        public int? ArticleInReviewStatusId { get; set; }
        public string UserInReviewId { get; set; }
        public int? PageSize = 30;
        public int? CurrentPage = 1;
    }
}
