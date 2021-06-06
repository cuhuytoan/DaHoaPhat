using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Shared.Components
{
    public partial class ConfirmBase
    {
        protected bool ShowConfirmation { get; set; }

        [Parameter]
        public string ConfirmationTitle { get; set; } = "Xác nhận xóa";

        [Parameter]
        public string ConfirmationMessage { get; set; } = "Bạn có chắc chắn muốn xóa ?";

        public async Task Show()
        {
            ShowConfirmation = true;
            StateHasChanged();
        }

        [Parameter]
        public EventCallback<bool> ConfirmationChanged { get; set; }

        protected async Task OnConfirmationChange(bool value)
        {
            ShowConfirmation = false;
            await ConfirmationChanged.InvokeAsync(value);
        }
    }
}