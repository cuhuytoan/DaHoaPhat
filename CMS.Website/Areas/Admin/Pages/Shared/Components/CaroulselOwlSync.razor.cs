using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Shared.Components
{
    public partial class CaroulselOwlSync
    {    
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                return JSRuntime.InvokeVoidAsync("InitOwlSync").AsTask();
            }
            return Task.CompletedTask;
        }
    }
}
