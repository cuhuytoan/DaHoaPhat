namespace CMS.Data.ModelFilter
{
    public class ArticleCommentSearchFilter
    {
        public string Keyword { get; set; }
        public int ArticleId { get; set; }
        public bool? Active { get; set; }
        public string CreateBy { get; set; }
        public int? PageSize = 100;
        public int? CurrentPage = 1;
    }
}
