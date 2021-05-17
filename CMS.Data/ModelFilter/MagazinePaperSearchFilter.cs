namespace CMS.Data.ModelFilter
{
    public class MagazinePaperSearchFilter
    {
        public string Keyword { get; set; }
        public int? MagazinePaperId { get; set; }
        public int? MagazinePaperStatusId { get; set; }
        public int? PageSize = 100;
        public int? CurrentPage = 1;
    }
}
