using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Shared.Components
{
    public partial class PropertiesDynamic
    {
        protected bool ShowForm { get; set; }
        
        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public string ConfirmationTitle { get; set; } = "Chỉnh sửa thông số sản phẩm ";

        [Parameter]
        public string ConfirmationMessage { get; set; } = "Thông tin";

        public async Task Show()
        {
            ShowForm = true;
            StateHasChanged();
        }

        [Parameter]
        public EventCallback<bool> ConfirmationChanged { get; set; }

        protected async Task OnConfirmationChange(bool value)
        {
            ShowForm = false;
            await ConfirmationChanged.InvokeAsync(value);
        }
    }
}
