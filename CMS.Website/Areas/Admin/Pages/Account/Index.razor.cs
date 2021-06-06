using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Areas.Admin.Pages.Shared;
using CMS.Website.Areas.Admin.Pages.Shared.Components;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Account
{
    public partial class Index : IDisposable
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
        public string keyword { get; set; }
        [Parameter]
        public int? roleId { get; set; }
        [Parameter]
        public int? p { get; set; }
        #endregion

        #region Model
        private List<SpAccountSearchResult> lstAccount;
        public int currentPage { get; set; }
        public int totalCount { get; set; }
        public int pageSize { get; set; } = 30;
        public int totalPages => (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));
        public ArticleSearchFilter modelFilter { get; set; }
        public string roleSelected { get; set; }
        List<AspNetRoles> lstRoles { get; set; }
        protected SetPwdModel setPwdModel { get; set; } = new();
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;
        protected ConfirmBase DeleteConfirmation { get; set; }
        List<string> lstAccountSelected { get; set; } = new List<string>();
        bool shouldRender { get; set; } = false;
        bool isCheck { get; set; }
        bool showSetPwdModal { get; set; }
        #endregion


        #region LifeCycle
      
        protected override void OnInitialized()
        {
            //Add for change location and seach not reload page
            NavigationManager.LocationChanged += HandleLocationChanged;
        }
        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            user = authState.User;

            await InitControl();
            await InitData();
        }

        protected override bool ShouldRender()
        {
            if (shouldRender)
            {
                shouldRender = false;
                return true;
            }
            return base.ShouldRender();
        }
        public void Dispose()
        {
            NavigationManager.LocationChanged -= HandleLocationChanged;
            GC.SuppressFinalize(this);
        }



        #endregion

        #region Init
        protected async Task InitControl()
        {
            //Binding Category
            var ListRoles = await Repository.AspNetUsers.AspNetRolesGetAll();
            if (ListRoles != null)
            {
                lstRoles = ListRoles.Select(x => new AspNetRoles { Id = x.Id.ToString(), Name = x.Name }).ToList();
            }

        }
        protected async Task InitData()
        {
            GetQueryStringValues();
            var modelFilter = new AccountSearchFilter();
            modelFilter.Keyword = keyword;
            var pRole = Guid.TryParse(roleSelected, out Guid role);
            if (pRole)
            {
                modelFilter.RoleId = role;
            }
            modelFilter.CurrentPage = p ?? 1;
            modelFilter.PageSize = 30;
            var result = await Repository.AspNetUsers.GetLstUsersPaging(modelFilter);

            lstAccount = result.Items;
            totalCount = result.TotalSize;

            //Init Selected 
            lstAccountSelected.Clear();
            StateHasChanged();
        }

        #endregion

        #region Event

      

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        protected async Task DeleteAccount(string userId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            if (userId == null) // Delete Demand
            {
                if (lstAccountSelected.Count == 0)
                {
                    toastService.ShowToast(ToastLevel.Warning, "Chưa chọn thành viên để xóa", "Thông báo");
                    return;
                }
            }
            else
            {
                lstAccountSelected.Clear();
                lstAccountSelected.Add(userId);
            }
            DeleteConfirmation.Show();
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                try
                {
                    foreach (var item in lstAccountSelected)
                    {
                        var currentUser = await Repository.AspNetUsers.FindAsync(item);
                        if (currentUser != null)
                        {
                            await Repository.AspNetUsers.AspNetUsersDelete(item);
                            await Repository.AspNetUsers.AspNetUserProfilesDeleteByUserId(item);
                            await Repository.AspNetUsers.AspNetUserRolesDelete(item);
                        }

                    }
                    toastService.ShowToast(ToastLevel.Success, "Xóa tài khoản thành công", "Thành công");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"ConfirmDelete_Click: {ex.ToString()}");
                    toastService.ShowToast(ToastLevel.Warning, "Có lỗi trong quá trình thực thi", "Lỗi!");
                }
                StateHasChanged();
                await InitData();
            }
        }

        protected void OnCheckBoxChange(bool headerChecked, string AspnetUserId, object isChecked)
        {
            if (headerChecked)
            {
                if ((bool)isChecked)
                {
                    lstAccountSelected.AddRange(lstAccount.Select(x => x.Id));
                    isCheck = true;
                }
                else
                {
                    isCheck = false;
                    lstAccountSelected.Clear();
                }
            }
            else
            {
                if ((bool)isChecked)
                {
                    if (!lstAccountSelected.Contains(AspnetUserId))
                    {
                        lstAccountSelected.Add(AspnetUserId);
                    }
                }
                else
                {
                    if (lstAccountSelected.Contains(AspnetUserId))
                    {
                        lstAccountSelected.Remove(AspnetUserId);
                    }
                }
            }
            StateHasChanged();

        }
        protected async Task OnShowSetPWD(string userName,bool isShow)
        {
            if(isShow)
            {
                var selectedUser = await UserManager.FindByNameAsync(userName);
                if (selectedUser != null)
                {
                    setPwdModel.UserName = userName;
                    showSetPwdModal = isShow;
                    StateHasChanged();
                }
            }
            else
            {
                showSetPwdModal = isShow;
                StateHasChanged();
            }
            
        }
        protected async Task OnSetPwd()
        {
            var selectedUser = await UserManager.FindByNameAsync(setPwdModel.UserName);
            if(selectedUser !=null)
            {
                string token = await UserManager.GeneratePasswordResetTokenAsync(selectedUser);
                var result = await UserManager.ResetPasswordAsync(selectedUser,token,  setPwdModel.Password);
                if(result.Succeeded)
                {
                    toastService.ShowSuccess("Cập nhật mật khẩu thành công", "Thông báo");
                    showSetPwdModal = false;
                    StateHasChanged();
                }
                else
                {
                    toastService.ShowError("Có lỗi trong quá trình cập nhật mật khẩu", "Thông báo");
                }
            }    
        }
        protected async void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {          
            await InitData();
        }
        protected void GetQueryStringValues()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("keyword", out var _keyword))
            {
                this.keyword = _keyword;
            }
            if (queryStrings.TryGetValue("roleId", out var _roleSelected))
            {
                this.roleSelected = _roleSelected;
            }
            if (queryStrings.TryGetValue("p", out var _p))
            {
                this.currentPage = Convert.ToInt32(_p);
                this.p = Convert.ToInt32(_p);
            }

        }
        #endregion

    }
}
