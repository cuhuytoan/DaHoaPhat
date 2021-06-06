using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Shared
{
    public partial class AdminLayout
    {
        private HubConnection hubConnection;
        private GlobalModel globalModel { get; set; } = new GlobalModel();

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        protected override void OnInitialized()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            //get claim principal
            var authState = await authenticationStateTask;
            globalModel.user = authState.User;
            globalModel.userId = globalModel.user.FindFirstValue(ClaimTypes.NameIdentifier);
            hubConnection = new HubConnectionBuilder()
              .WithUrl(NavigationManager.ToAbsoluteUri("/notificationHubs"), options =>
              {
                  options.Headers.Add("USERID", globalModel.userId);
              })
              .WithAutomaticReconnect()
              .Build();
            await hubConnection.StartAsync();
            hubConnection.On<string, string, string, string, string>("ReceiveMessage", (toUserId, subject, content, url, imageUrl) =>
            {
                //ToastMessage
                if (globalModel.userId == toUserId)
                {
                    toastService.ShowToast(ToastLevel.Info, $"{subject}", "Bạn có thông báo mới");
                    globalModel.lstUserNoti.Insert(0, new SpUserNotifySearchResult
                    {
                        Subject = subject,
                        Content = content,
                        URL = url,
                        CreateDate = DateTime.Now
                    });
                    globalModel.totalUnread += 1;
                    StateHasChanged();
                }
            });

            await InitData();
        }

        private async Task InitData()
        {
            var profiles = await Repository.AspNetUsers.AspNetUserProfilesGetByUserId(globalModel.userId);
            if (profiles != null)
            {
                globalModel.avatar = profiles.AvatarUrl;
            }
            var result = await Repository.UserNoti.GetAllNoti(null, globalModel.userId, null, 3, 1);
            globalModel.lstUserNoti = result.Items;
            globalModel.totalUnread = result.TotalSize;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/plugins/components/jquery/dist/jquery.min.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/cubic-html/bootstrap/dist/js/bootstrap.min.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/cubic-html/js/sidebarmenu.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/cubic-html/js/jquery.slimscroll.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/cubic-html/js/waves.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/cubic-html/js/custom.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/cubic-html/js/jasny-bootstrap.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "https://cdnjs.cloudflare.com/ajax/libs/OwlCarousel2/2.3.4/owl.carousel.min.js");
                //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "https://cdnjs.cloudflare.com/ajax/libs/Dropify/0.2.2/js/dropify.min.js");

                //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/customcht.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Cubic/plugins/components/styleswitcher/jQuery.style.switcher.js");
                //Cropper Js
                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/cropperjs/cropper.js");

                await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/cropperjs/_scripts.js");
            }
        }
    }
}