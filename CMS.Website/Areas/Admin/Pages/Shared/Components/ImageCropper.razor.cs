using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Areas.Admin.Pages.Shared;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Shared.Components
{
    public partial class ImageCropper 
    {
        public string response { get; set; }
        protected bool ShowConfirmation { get; set; }
        public string ImgData { get; set; }
        public async Task Show()
        {

            await JSRuntime.InvokeVoidAsync("Crop.image", DotNetObjectReference.Create(this));
            ShowConfirmation = true;
            StateHasChanged();
        }

        [Parameter]
        public EventCallback<bool> ConfirmationCropChanged { get; set; }

        protected async Task OnConfirmationChange(bool value)
        {
            ShowConfirmation = false;
            ImgData = response.ToString();
            await ConfirmationCropChanged.InvokeAsync(value) ;
        }
        [JSInvokable]
        public void ResponseMethod(string data)
        {
            response = data;
            StateHasChanged();
        }
    }
}
