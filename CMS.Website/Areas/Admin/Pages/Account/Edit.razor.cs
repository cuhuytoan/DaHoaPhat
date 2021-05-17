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

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;


        #endregion

        #region LifeCycle
        protected override async Task OnParametersSetAsync()
        {

        }

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
                    userInfo.AspNetUsers = Mapper.Map<AspNetUsersDTO>(result);
                    userInfo.AspNetUserRoles = Mapper.Map<AspNetUserRolesDTO>(roles);
                    userInfo.AspNetUserProfiles = Mapper.Map<AspNetUserProfilesDTO>(profile);
                }
            }
        }
        #endregion

        #region Event

        async Task PostUserInfo()
        {
            var userExists = Repository.AspNetUsers.FirstOrDefault(p => p.UserName == userInfo.AspNetUsers.Email);
            if (userExists != null)
            {
                try
                {
                    await Repository.AspNetUsers.AspNetUserProfilesUpdate(
                      Mapper.Map<AspNetUserProfiles>(userInfo.AspNetUserProfiles));

                    await Repository.AspNetUsers.AspNetUserRolesUpdate(
                        Mapper.Map<AspNetUserRoles>(userInfo.AspNetUserRoles));
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Success, "Cập nhật user thành công", "Thành công");
                }
                catch (Exception ex)
                {
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

        #endregion
    }
}
