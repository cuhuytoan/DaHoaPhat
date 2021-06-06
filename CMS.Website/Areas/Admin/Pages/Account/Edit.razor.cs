using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace CMS.Website.Areas.Admin.Pages.Account
{
    public partial class Edit : IDisposable
    {
        #region Inject   
        [Inject]
        IMapper Mapper { get; set; }
        [Inject]
        ILoggerManager Logger { get; set; }
        [Inject]
        UserManager<IdentityUser> UserManager { get; set; }
        #endregion

        #region Parameter
        [Parameter]
        public string userId { get; set; }
        public AspNetUserInfoDTO userInfo { get; set; } = new AspNetUserInfoDTO();

        List<AspNetRoles> lstRole { get; set; }
        List<KeyValuePair<bool, string>> lstGender { get; set; }
        public DateTime MaxDate = new DateTime(2020, 12, 31);
        public DateTime MinDate = new DateTime(1950, 1, 1);
        private List<Location> lstLocation { get; set; } = new();
        private List<District> lstDistrict { get; set; } = new();
        private List<Ward> lstWard { get; set; } = new();
        private List<Department> lstDepartment { get; set; } = new();
        private List<Bank> lstBank { get; set; } = new();
        private bool enableAssignCategory;

        public List<int> SelectedCateValue { get; set; } = new List<int>();
        public List<string> SelectedCateName { get; set; } = new List<string>();
        private List<ArticleCategory> lstArticleCategory { get; set; } = new List<ArticleCategory>();


        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;


        #endregion

        #region LifeCycle
   
        protected override async Task OnInitializedAsync()
        {
            //get claim principal
            var authState = await authenticationStateTask;
            user = authState.User;

            await InitControl();
            await InitData();


        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Init
        protected async Task InitControl()
        {
            //Binding lstRole
            var resultLstRole = await Repository.AspNetUsers.AspNetRolesGetAll();
            if (resultLstRole != null)
            {
                lstRole = resultLstRole.Select(x => new AspNetRoles { Id = x.Id, Name = x.Name }).ToList();
            }
            //Binding lstGender
            List<KeyValuePair<bool, string>> lstGenderAdd = new List<KeyValuePair<bool, string>>();
            lstGenderAdd.Add(new KeyValuePair<bool, string>(true, "Nam"));
            lstGenderAdd.Add(new KeyValuePair<bool, string>(false, "Nữ"));
            lstGender = lstGenderAdd.ToList();

            lstLocation = await Repository.MasterData.LocationGetLstByCountryId(1);
            lstLocation = lstLocation.Select(x => new Location { Id = x.Id, Name = x.Name }).ToList();

            lstDepartment = await Repository.MasterData.DepartmentsGetLst();
            lstDepartment = lstDepartment.Select(x => new Department { Id = x.Id, Name = x.Name }).ToList();

            lstBank = await Repository.MasterData.BankGetLst();
            lstBank = lstBank.Select(x => new Bank { Id = x.Id, Name = x.Name }).ToList();

            var lstArticleCate = await Repository.ArticleCategory.GetArticleCategoryById(null);
            if (lstArticleCate != null)
            {
                lstArticleCategory = lstArticleCate.Select(x => new ArticleCategory { Id = x.Id, Name = x.Name }).ToList();
            }


        }
        protected async Task InitData()
        {

            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("userId", out var _userId))
            {
                this.userId = _userId;
            }
            if (userId != null)
            {
                var result = await Repository.AspNetUsers.AspNetUsersGetById(userId);
                if (result != null)
                {
                    var profile = await Repository.AspNetUsers.AspNetUserProfilesGetByUserId(userId);
                    var roles = await Repository.AspNetUsers.AspNetUserRolesGetByUserId(userId);
                    var lstcategoryAssign = await Repository.ArticleCategory.ArticleCategoryAssignsGetLstByUserId(userId);
                    userInfo.AspNetUsers = Mapper.Map<AspNetUsersDTO>(result);
                    userInfo.AspNetUserRoles = Mapper.Map<AspNetUserRolesDTO>(roles);
                    userInfo.AspNetUserProfiles = Mapper.Map<AspNetUserProfilesDTO>(profile);
                    userInfo.LstArtCatAssign = lstcategoryAssign;

                    if (userInfo.AspNetUserProfiles.LocationId > 0)
                    {
                        lstDistrict = userInfo.AspNetUserProfiles.LocationId == null ? new() : await Repository.MasterData.DistrictsGetLstByLocationId((int)userInfo.AspNetUserProfiles.LocationId);
                        lstDistrict = lstDistrict.Select(x => new District { Id = x.Id, Name = x.Name }).ToList();
                    }
                    if (userInfo.AspNetUserProfiles.DistrictId > 0)
                    {
                        lstWard = userInfo.AspNetUserProfiles.DistrictId == null ? new() : await Repository.MasterData.WardsGetLstByDistrictId((int)userInfo.AspNetUserProfiles.DistrictId);
                        lstWard = lstWard.Select(x => new Ward { Id = x.Id, Name = x.Name }).ToList();
                    }
                    if(userInfo.AspNetUserRoles.RoleId == "6df4162d-38a4-42e9-b3d3-a07a5c29215b")
                    {
                        //Get Lst ArticleCategory
                        var lstArtCateAssign = await Repository.ArticleCategory.ArticleCategoryAssignsGetLstByUserId(userId);
                        SelectedCateValue = lstArtCateAssign.Where(x => x.ArticleCategoryId !=null).Select(x => x.ArticleCategoryId.Value).ToList();
                        enableAssignCategory = true;
                    }    
                }
               
            }
        }
        #endregion

        #region Event

        async Task PostUserInfo()
        {
            if (userInfo.AspNetUserRoles.RoleId == "6df4162d-38a4-42e9-b3d3-a07a5c29215b")
            {
                if(SelectedCateValue.Count <1)
                {
                    toastService.ShowError("Thiếu thông tin chuyên mục cho tài khoản", "Thông báo");
                    return;
                }    
            }    
                var userExists = Repository.AspNetUsers.FirstOrDefault(p => p.UserName == userInfo.AspNetUsers.Email);
            if (userExists != null)
            {
                try
                {
                    await Repository.AspNetUsers.AspNetUserProfilesUpdate(
                      Mapper.Map<AspNetUserProfiles>(userInfo.AspNetUserProfiles));

                    await Repository.AspNetUsers.AspNetUserRolesUpdate(
                        Mapper.Map<AspNetUserRoles>(userInfo.AspNetUserRoles));

                    if(userInfo.AspNetUserRoles.RoleId == "6df4162d-38a4-42e9-b3d3-a07a5c29215b")
                    {
                        await Repository.ArticleCategory.ArticleCategoryAssignsUpdate(userExists.Id, SelectedCateValue);
                    }    
                    
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Success, "Cập nhật user thành công", "Thành công");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"PostUserInfo:{ex.ToString()}");
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật", "Lỗi");
                }

            }
            else
            {
                var user = new IdentityUser { UserName = userInfo.AspNetUsers.Email, Email = userInfo.AspNetUsers.Email };
                try
                {
                    var result = await UserManager.CreateAsync(user, userInfo.AspNetUsers.Password);
                    if (result.Succeeded)
                    {
                        //Insert new Profilers
                        userInfo.AspNetUserProfiles.UserId = user.Id;
                        await Repository.AspNetUsers.AspNetUserProfilesCreateNew(
                            Mapper.Map<AspNetUserProfiles>(userInfo.AspNetUserProfiles));
                        //Insert new Role
                        //userInfo.AspNetUserRoles.RoleId = RoleId;
                        userInfo.AspNetUserRoles.UserId = user.Id;
                        await Repository.AspNetUsers.AspNetUserRolesCreateNew(
                            Mapper.Map<AspNetUserRoles>(userInfo.AspNetUserRoles));

                        if (userInfo.AspNetUserRoles.RoleId == "6df4162d-38a4-42e9-b3d3-a07a5c29215b")
                        {
                            await Repository.ArticleCategory.ArticleCategoryAssignsUpdate(userExists.Id, SelectedCateValue);
                        }

                        //ToastMessage
                        toastService.ShowToast(ToastLevel.Success, "Cập nhật user thành công", "Thành công");
                        NavigationManager.NavigateTo("/Admin/Account/Index");
                    }
                }
                catch
                {
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật", "Lỗi");
                }
            }

        }
        private async void LocationSelected()
        {
            lstDistrict = userInfo.AspNetUserProfiles.LocationId == null ? new() : await Repository.MasterData.DistrictsGetLstByLocationId((int)userInfo.AspNetUserProfiles.LocationId);
            lstDistrict = lstDistrict.Select(x => new District { Id = x.Id, Name = x.Name }).ToList();
            StateHasChanged();
        }

        private async void DistrictSelected()
        {
            lstWard = userInfo.AspNetUserProfiles.DistrictId == null ? new() : await Repository.MasterData.WardsGetLstByDistrictId((int)userInfo.AspNetUserProfiles.DistrictId);
            lstWard = lstWard.Select(x => new Ward { Id = x.Id, Name = x.Name }).ToList();
            StateHasChanged();
        }

        private void OnChangedRole()
        {
          
            if (userInfo.AspNetUserRoles.RoleId == "6df4162d-38a4-42e9-b3d3-a07a5c29215b")
            {
                enableAssignCategory = true;
            }
            else
            {
                enableAssignCategory = false;
            }
            StateHasChanged();
        }

        private void OnArticleSelected()
        {
            SelectedCateName = lstArticleCategory.Where(p => SelectedCateValue.Contains(p.Id)).Select(p => p.Name).ToList();            
        }

        #endregion
    }
}
