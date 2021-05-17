namespace CMS.Data.ModelFilter
{
    public class ArticleInMagazinePaperSearchFilter
    {
        public string Keyword { get; set; }
        public int? MagazinePaperId { get; set; }
        public int? ArticleInMagazinePaperStatusId { get; set; }
        public int? PageSize = 30;
        public int? CurrentPage = 1;
    }
}
