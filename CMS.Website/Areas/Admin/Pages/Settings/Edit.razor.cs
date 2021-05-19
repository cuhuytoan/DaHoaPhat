using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
namespace CMS.Website.Areas.Admin.Pages.Settings
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
        public SettingDTO settings { get; set; } = new SettingDTO();
        //Noti Hub
        [CascadingParameter]
        protected HubConnection hubConnection { get; set; }
        [CascadingParameter]
        private GlobalModel globalModel { get; set; }

        #endregion

        #region LifeCycle
        protected override async Task OnParametersSetAsync()
        {

        }

        protected override async Task OnInitializedAsync()
        {           
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

        }
        protected async Task InitData()
        {
            var result = await Repository.Setting.GetSetting();
            if (result != null)
            {
                settings = Mapper.Map<SettingDTO>(result);
            }
        }
        #endregion

        #region Event
        async Task PostSetings()
        {
            settings.Id = await Repository.Setting.PostSetting(Mapper.Map<Setting>(settings));
            if (settings.Id > 0)
            {
                //ToastMessage
                toastService.ShowToast(ToastLevel.Success, "Cài đặt thành công", "Thành công");
            }
            else
            {
                //ToastMessage
                toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật ", "Lỗi");
            }
        }
        #endregion

    }
}
