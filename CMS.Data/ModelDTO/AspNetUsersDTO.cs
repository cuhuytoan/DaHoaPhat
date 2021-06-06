using CMS.Data.ModelEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.ModelDTO
{
    public class AspNetUsersDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập bắt buộc")]
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        [Required(ErrorMessage = "Email bắt buộc")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không chính xác")]
        public string ConfirmPassword { get; set; }

        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
    }

    public class AspNetUserProfilesDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string RegType { get; set; }
        public DateTime? RegisterDate { get; set; }
        public bool? Verified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime? LastActivityDate { get; set; }

        [Required(ErrorMessage = "Nhập họ tên")]
        public string FullName { get; set; }

        public bool? Gender { get; set; }
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Nhập cơ quan công tác")]
        public string Company { get; set; }

        public int? ProductBrandId { get; set; }
        public int? Rank { get; set; }
        public string Address { get; set; }
        public int? CountryId { get; set; }

        [Required(ErrorMessage = "Nhập tỉnh thành")]
        public int? LocationId { get; set; }

        [Required(ErrorMessage = "Nhập quận huyện")]
        public int? DistrictId { get; set; }

        [Required(ErrorMessage = "Nhập phường xã")]
        public int? WardId { get; set; }

        [Required(ErrorMessage = "Nhập số điện thoại")]
        public string Phone { get; set; }

        public string Email { get; set; }
        public string Website { get; set; }
        public string FacebookId { get; set; }
        public string Skype { get; set; }
        public string AvatarUrl { get; set; }
        public string Signature { get; set; }
        public int? AccountType { get; set; }
        public string Department { get; set; }
        public string BankAcc { get; set; }
        public int? BankId { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class AspNetUserRolesDTO
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }

    public class AspNetUserInfoDTO
    {
        public AspNetUsersDTO AspNetUsers { get; set; } = new();
        public AspNetUserProfilesDTO AspNetUserProfiles { get; set; } = new();
        public AspNetUserRolesDTO AspNetUserRoles { get; set; } = new();
        public List<ArticleCategoryAssign> LstArtCatAssign { get; set; } = new();
    }

    public class AspNetUserInfo
    {
        public AspNetUsers AspNetUsers { get; set; }
        public AspNetUserProfiles AspNetUserProfiles { get; set; }
        public AspNetUserRoles AspNetUserRoles { get; set; }
    }

    public class ChangePwdModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu ít nhất 6 kí tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu chưa đúng")]
        public string ConfirmPassword { get; set; }
    }
    public class SetPwdModel
    {       
        [Required(ErrorMessage ="Tài khoản không được trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu ít nhất 6 kí tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu chưa đúng")]
        public string ConfirmPassword { get; set; }
    }
}