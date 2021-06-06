using System;
using System.Collections.Generic;

namespace CMS.Data.ModelDTO
{
    public class ArticleCategoryDTO
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int? Sort { get; set; }
        public int? Counter { get; set; }
        public bool? DisplayMenu { get; set; }
        public string MenuColor { get; set; }
        public bool? Active { get; set; }
        public bool? CanDelete { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastEditedBy { get; set; }
        public DateTime? LastEditedDate { get; set; }
        public bool? hasChildren { get; set; }
    }

    public class ArticleMenuDTO
    {
        public ArticleCategoryDTO MainMenu { get; set; }

        public List<ArticleCategoryDTO> SubMenu { get; set; }
    }
}