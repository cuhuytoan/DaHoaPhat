using System;

namespace CMS.Data.ModelFilter
{
    public class ProductSearchFilter
    {
        public string Keyword { get; set; }
        public int? ProductCategoryId { get; set; }
        public int? ProductManufactureId { get; set; }  
        public int? ProductStatusId { get; set; }  
        public int? CountryId { get; set; }  
        public int? LocationId { get; set; }  
        public int? DepartmentManId { get; set; }  
        public int? ProductBrandId { get; set; }  
        public int? ProductTypeId { get; set; }  
        public int? ExceptionId { get; set; }  
        public bool? ExceptionProductTop { get; set; }
        public int? FromPrice { get; set; }  
        public int? ToPrice { get; set; }
        public DateTime? FromDate { get; set; } 
        public DateTime? ToDate { get; set; }
        public bool? Efficiency { get; set; }  
        public bool? Active { get; set; }
        public string AssignBy { get; set; }  
        public string CreateBy { get; set; }  
        public string OrderBy { get; set; }
        public int PageSize { get; set; }  
        public int CurrentPage { get; set; }  
    }
}
