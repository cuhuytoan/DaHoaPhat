﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CMS.Data.ModelEntity
{
    public partial class ProductBlock
    {
        [Key]
        public int Id { get; set; }
        public int? ProductCategoryId { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Image { get; set; }
        public bool? Active { get; set; }
        public int? Sort { get; set; }
        [Column("Style_ID")]
        public int? StyleId { get; set; }
        [StringLength(256)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(256)]
        public string LastEditedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditedDate { get; set; }
        public bool? CanDelete { get; set; }
    }
}