// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CMS.Data.ModelEntity
{
    public partial class ProductBrand
    {
        [Key]
        public int Id { get; set; }
        public int? ProductBrandCategoryId { get; set; }
        public int? ProductBrandTypeId { get; set; }
        public int? DepartmentManId { get; set; }
        [Column("ProductBrandModelManagement_ID")]
        public int? ProductBrandModelManagementId { get; set; }
        public int? ProductBrandStatusId { get; set; }
        public int? CountryId { get; set; }
        public int? LocationId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        [StringLength(1000)]
        public string Code { get; set; }
        [StringLength(1000)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string TradingName { get; set; }
        [StringLength(200)]
        public string BrandName { get; set; }
        [StringLength(50)]
        public string TaxCode { get; set; }
        [StringLength(50)]
        public string RegistrationNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? IssuedDate { get; set; }
        [StringLength(1000)]
        public string BusinessArea { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }
        [StringLength(200)]
        public string Telephone { get; set; }
        [StringLength(100)]
        public string Fax { get; set; }
        [StringLength(100)]
        public string Mobile { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Website { get; set; }
        [StringLength(200)]
        public string Facebook { get; set; }
        [StringLength(200)]
        public string Zalo { get; set; }
        [StringLength(200)]
        public string Hotline { get; set; }
        [StringLength(200)]
        public string Skype { get; set; }
        [Column("PRInfo")]
        [StringLength(4000)]
        public string Prinfo { get; set; }
        [StringLength(4000)]
        public string Agency { get; set; }
        [StringLength(4000)]
        public string Description { get; set; }
        [StringLength(500)]
        public string Image { get; set; }
        [StringLength(200)]
        public string PersonName { get; set; }
        [StringLength(500)]
        public string PersonAddress { get; set; }
        [StringLength(100)]
        public string PersonMobile { get; set; }
        [StringLength(100)]
        public string PersonEmail { get; set; }
        public int? Sort { get; set; }
        [Column("URL")]
        [StringLength(200)]
        public string Url { get; set; }
        public bool? Active { get; set; }
        [Column("HasQRCode")]
        public bool? HasQrcode { get; set; }
        public int? ViewCount { get; set; }
        public int? SellCount { get; set; }
        [StringLength(50)]
        public string AccountUserName { get; set; }
        [StringLength(200)]
        public string AccountEmail { get; set; }
        [StringLength(200)]
        public string DirectorName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DirectorBirthday { get; set; }
        [StringLength(500)]
        public string DirectorAddress { get; set; }
        [StringLength(200)]
        public string DirectorMobile { get; set; }
        [StringLength(200)]
        public string DirectorEmail { get; set; }
        [StringLength(200)]
        public string DirectorPosition { get; set; }
        [StringLength(450)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(450)]
        public string LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditDate { get; set; }
        public int? Checked { get; set; }
        [StringLength(450)]
        public string CheckBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CheckDate { get; set; }
        public int? Approved { get; set; }
        [StringLength(450)]
        public string ApproveBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ApproveDate { get; set; }
    }
}