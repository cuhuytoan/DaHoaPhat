namespace CMS.Data.ModelFilter
{
    public class ArticleReviewDetailSearchFilter
    {
        public string Keyword { get; set; }
        public int ArticleId { get; set; }
        public string CreateBy { get; set; }
        public int? PageSize = 30;
        public int? CurrentPage = 1;
    }
}
