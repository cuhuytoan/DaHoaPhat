namespace CMS.Data.ModelFilter
{
    public class ProductCommentStaffSearchFilter
    {
        public string Keyword { get; set; }
        public int ProductId { get; set; }
        public bool? Active { get; set; }
        public string CreateBy { get; set; }
        public int? PageSize = 100;
        public int? CurrentPage = 1;
    }
}
