using Microsoft.AspNetCore.Components;

namespace CMS.Website.Areas.Identity.Pages.Account
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            NavigationManager.NavigateTo("/Login", true);
        }
    }
}
