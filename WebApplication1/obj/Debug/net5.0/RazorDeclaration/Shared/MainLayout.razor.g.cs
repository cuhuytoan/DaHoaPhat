// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace WebApplication1.Shared
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using System.Net.Http.Json;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using Microsoft.AspNetCore.Components.WebAssembly.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using WebApplication1;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\Project\DaHoaPhat\WebApplication1\_Imports.razor"
using WebApplication1.Shared;

#line default
#line hidden
#nullable disable
    public partial class MainLayout : LayoutComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 20 "D:\Project\DaHoaPhat\WebApplication1\Shared\MainLayout.razor"
      
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/js/jquery.min.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/wow/wow.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/bootstrap/js/popper.min.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/bootstrap-select/bootstrap-select.min.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/bootstrap-touchspin/jquery.bootstrap-touchspin.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/magnific-popup/magnific-popup.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/counter/waypoints-min.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/counter/counterup.min.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/imagesloaded/imagesloaded.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/masonry/masonry-3.1.4.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/masonry/masonry.filter.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/owl-carousel/owl.carousel.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/scroll/scrollbar.min.js");
            //await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/plugins/lightgallery/js/lightgallery-all.min.js");
            await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/js/custom.js");
            await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/js/dz.carousel.js");
            await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/js/dz.ajax.js");
            await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/customCHT.js");

        }

    }
    

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IJSRuntime JSRuntime { get; set; }
    }
}
#pragma warning restore 1591
