using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Account
{
    public partial class ChangePassword : IDisposable
    {
        #region Inject   
        [Inject]
        IMapper Mapper { get; set; }
        [Inject]
        ILoggerManager Logger { get; set; }
        [Inject]
        UserManager<IdentityUser> UserManager { get; set; }
        [Inject]
        SignInManager<IdentityUser> SignInManager { get; set; }
        #endregion



        #region Parameter

        public ChangePwdModel changePwdModel { get; set; } = new ChangePwdModel();


        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;
        public string userId { get; set; }



        #endregion

        #region LifeCycle
      

        protected override async Task OnInitializedAsync()
        {
            //get claim principal
            var authState = await authenticationStateTask;
            user = authState.User;
            userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

          
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Init
       
        #endregion

        #region Event

        async Task PostChangePassword()
        {
            var currentUser = await UserManager.FindByIdAsync(userId);
            if (currentUser != null)
            {
                try
                {

                    var result = await UserManager.ChangePasswordAsync(currentUser, changePwdModel.CurrentPassword, changePwdModel.Password);
                    await UserManager.UpdateAsync(currentUser);
                    //await SignInManager.RefreshSignInAsync(currentUser);

                    if (result.Succeeded)
                    {
                        //ToastMessage
                        toastService.ShowToast(ToastLevel.Success, "Cập nhật mật khẩu thành công", "Thành công");
                    }
                    else
                    {
                        //ToastMessage
                        toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật", "Lỗi");
                    }

                }
                catch (Exception ex)
                {
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật {ex.ToString()}", "Lỗi");
                }

            }
            else
            {
                //ToastMessage
                toastService.ShowToast(ToastLevel.Error, $"Không tồn tại tài khoản cập nhật", "Lỗi");
            }

        }

        #endregion
    }
}
