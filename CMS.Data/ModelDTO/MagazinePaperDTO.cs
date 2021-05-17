using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.ModelDTO
{
    public class MagazinePaperDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Nhập tên đợt phản biện")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Nhập mô tả đợt phản biện")]
        public string Description { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Nhập ngày bắt đầu")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Nhập ngày kết thúc")]
        public DateTime? EndDate { get; set; }
        public string CreateBy { get; set; }
        public string LastEditBy { get; set; }
    }
    public class PostMagazinePaperDTO
    {
        public MagazinePaperDTO MagazinePaper { get; set; } = new MagazinePaperDTO();
        public List<int> LstArticleId { get; set; } = new List<int>();
    }
}
