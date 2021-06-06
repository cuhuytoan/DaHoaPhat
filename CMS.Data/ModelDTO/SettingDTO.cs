using System.ComponentModel.DataAnnotations;

namespace CMS.Data.ModelDTO
{
    public class SettingDTO
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Domain { get; set; }

        [StringLength(500)]
        public string WebsiteName { get; set; }

        [StringLength(500)]
        public string AdminName { get; set; }

        [StringLength(500)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmailSupport { get; set; }

        [StringLength(500)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmailOrder { get; set; }

        [StringLength(500)]
        public string EmailSenderSmtp { get; set; }

        [StringLength(500)]
        public string EmailSenderPort { get; set; }

        public bool EmailSenderSsl { get; set; }

        [StringLength(500)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmailSender { get; set; }

        [StringLength(500)]
        public string EmailSenderPassword { get; set; }

        [StringLength(500)]
        public string Telephone { get; set; }

        public bool AppStatus { get; set; }
        public int? Counter { get; set; }
        public int DefaultLanguageId { get; set; }
        public int DefaultSkinId { get; set; }

        [StringLength(500)]
        public string MetaDescriptionDefault { get; set; }

        [StringLength(500)]
        public string MetaKeywordsDefault { get; set; }

        [StringLength(500)]
        public string MetaTitleDefault { get; set; }

        [StringLength(1000)]
        public string GoogleAnalyticsCode { get; set; }

        [StringLength(1000)]
        public string OtherCode { get; set; }

        [StringLength(50)]
        public string FacebookPageId { get; set; }

        [StringLength(50)]
        public string FacebookAppId { get; set; }

        [StringLength(50)]
        public string FacebookAdmin { get; set; }

        [StringLength(50)]
        public string TwitterId { get; set; }

        [StringLength(200)]
        public string VbeeAppId { get; set; }

        [StringLength(50)]
        public string VbeeUserId { get; set; }
    }
}