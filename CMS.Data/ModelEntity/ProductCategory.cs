// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CMS.Data.ModelEntity
{
    public partial class ProductCategory
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [Column("URL")]
        [StringLength(500)]
        public string Url { get; set; }
        [StringLength(500)]
        public string Image { get; set; }
        public string Description { get; set; }
        public int? Sort { get; set; }
        public int? Counter { get; set; }
        public bool? DisplayMenu { get; set; }
        [StringLength(50)]
        public string MenuColor { get; set; }
        public bool? Active { get; set; }
        public bool? CanDelete { get; set; }
        [StringLength(500)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(500)]
        public string LastEditedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditedDate { get; set; }
    }
}