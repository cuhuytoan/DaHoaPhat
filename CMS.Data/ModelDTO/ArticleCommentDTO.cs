using System;

namespace CMS.Data.ModelDTO
{
    public class ArticleCommentDTO
    {
        public int Id { get; set; }
        public int? ArticleId { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string AvatarUrl { get; set; }
    }
}