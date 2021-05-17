using System;

namespace CMS.Data.ModelFilter
{
    public class AccountSearchFilter
    {
        public string Keyword { get; set; }
        public Guid? RoleId { get; set; }
        public bool? Active { get; set; }
        public int? PageSize = 10;
        public int? CurrentPage = 1;
    }
}
