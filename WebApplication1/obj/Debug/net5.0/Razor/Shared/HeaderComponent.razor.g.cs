#pragma checksum "D:\Project\DaHoaPhat\WebApplication1\Shared\HeaderComponent.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "11f27a29e500da12d0c848bd52f6416204a58d7f"
// <auto-generated/>
#pragma warning disable 1591
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
    public partial class HeaderComponent : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.AddMarkupContent(0, "<div class=\"sticky-header main-bar-wraper navbar-expand-lg\"><div class=\"main-bar clearfix \"><div class=\"container-fluid\"><div class=\"logo-header mostion\"><a href=\"index.html\"><img src=\"images/logo-black.png\" alt></a></div>\r\n\r\n            \r\n            <button class=\"navbar-toggler collapsed navicon justify-content-end\" type=\"button\" data-toggle=\"collapse\" data-target=\"#navbarNavDropdown\" aria-controls=\"navbarNavDropdown\" aria-expanded=\"false\" aria-label=\"Toggle navigation\"><span></span>\r\n                <span></span>\r\n                <span></span></button>\r\n\r\n            \r\n            <div class=\"extra-nav\"><div class=\"extra-cell\"><ul><li class=\"search-btn\"><a id=\"quik-search-btn\" href=\"javascript:;\"><i class=\"ti-search m-r10\"></i><span>Search</span></a></li>\r\n                        <li><button type=\"button\" data-toggle=\"modal\" data-target=\"#exampleModal\" class=\"btn secondry radius-no\">Subscribe</button></li></ul></div></div>\r\n\r\n            \r\n            <div class=\"dlab-quik-search\"><form action=\"#\"><input name=\"search\" value type=\"text\" class=\"form-control\" placeholder=\"Search...\">\r\n                    <span id=\"quik-search\"><i class=\"ti-search\"></i></span></form>\r\n                <span class=\"search-remove\" id=\"quik-search-remove\"><i class=\"ti-close\"></i></span></div>\r\n\r\n            \r\n            <div class=\"header-nav navbar-collapse collapse justify-content-center\" id=\"navbarNavDropdown\"><div class=\"logo-header\"><a href=\"index.html\"><img src=\"images/logo.png\" alt></a></div>\r\n                <ul class=\"nav navbar-nav\"><li class=\"active\"><a href=\"javascript:void(0);\">Home<i class=\"fa fa-chevron-down\"></i></a>\r\n                        <ul class=\"sub-menu\"><li><a href=\"index.html\">Home 01</a></li>\r\n                            <li><a href=\"index-2.html\">Home 02</a></li>\r\n                            <li><a href=\"index-3.html\">Home 03</a></li>\r\n                            <li><a href=\"index-4.html\">Home 04</a></li>\r\n                            <li><a href=\"index-5.html\">Home 05</a></li></ul></li>\r\n                    <li><a href=\"javascript:void(0);\">Post Layout<i class=\"fa fa-chevron-down\"></i></a>\r\n                        <ul class=\"sub-menu\"><li><a href=\"post-standart.html\">Post Standart</a></li>\r\n                            <li><a href=\"post-left-sidebar.html\">Post Left Sidebar</a></li>\r\n                            <li><a href=\"post-header-image.html\">Post Header Image</a></li>\r\n                            <li><a href=\"post-slide-show.html\">Post Slide Show</a></li>\r\n                            <li><a href=\"post-side-image.html\">Post Side Image</a></li>\r\n                            <li><a href=\"post-gallery.html\">Post Gallery</a></li>\r\n                            <li><a href=\"post-gallery-alternative.html\">Post Gallery Alt</a></li>\r\n                            <li><a href=\"post-link.html\">Post Link</a></li>\r\n                            <li><a href=\"post-audio.html\">Post Audio</a></li>\r\n                            <li><a href=\"post-video.html\">Post Video</a></li>\r\n                            <li><a href=\"post-pagination.html\">Post With Pagination</a></li>\r\n                            <li><a href=\"post-open-gutenberg.html\">Post Open Gutenberg</a></li></ul></li>\r\n                    <li class=\"has-mega-menu post-slider life-style\"><a href=\"javascript:void(0);\">Category<i class=\"fa fa-chevron-down\"></i></a>\r\n                        <div class=\"mega-menu\"><div class=\"life-style-bx\"><div class=\"life-style-tabs\"><ul><li><a href=\"javascript:void(0);\" id=\"st-all\" class=\"post-tabs active\">All</a></li>\r\n                                        <li><a href=\"javascript:void(0);\" id=\"st-beauty\" class=\"post-tabs\">Beauty</a></li>\r\n                                        <li><a href=\"javascript:void(0);\" id=\"st-lifestyle\" class=\"post-tabs\">Lifestyle</a></li>\r\n                                        <li><a href=\"javascript:void(0);\" id=\"st-fashion\" class=\"post-tabs\">Fashion</a></li>\r\n                                        <li><a href=\"javascript:void(0);\" id=\"st-travel\" class=\"post-tabs\">Travel</a></li></ul></div>\r\n                                <div class=\"life-style-post text-center\"><div id=\"all\" class=\"life-style-post-bx show\"><div class=\"header-blog-carousel owl-carousel owl-btn-center-lr\"><div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-standart.html\"><img src=\"images/category/pic1.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-standart.html\">Ready or Not, the Return into of the Hobo Bag Is Nigh</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-left-sidebar.html\"><img src=\"images/category/pic2.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-left-sidebar.html\">This Week on Instagram, Celebri ties Went All-In on Prints</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-full-width.html\"><img src=\"images/category/pic3.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title\"><h5 class=\"post-title\"><a href=\"post-header-image.html\">Anniversary With An Exhibition At Dallas Contemporary</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-slide-show.html\"><img src=\"images/category/pic4.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-slide-show.html\">La Dolce Vita Meets Old School on Beach Style </a></h5></div></div></div></div></div></div>\r\n                                    <div id=\"beauty\" class=\"life-style-post-bx\"><div class=\"header-blog-carousel owl-carousel owl-btn-center-lr\"><div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-standart.html\"><img src=\"images/category/pic1.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-standart.html\">Ready or Not, the Return into of the Hobo Bag Is Nigh</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-left-sidebar.html\"><img src=\"images/category/pic2.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-left-sidebar.html\">This Week on Instagram, Celebri ties Went All-In on Prints</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-full-width.html\"><img src=\"images/category/pic3.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title\"><h5 class=\"post-title\"><a href=\"post-header-image.html\">Anniversary With An Exhibition At Dallas Contemporary</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-slide-show.html\"><img src=\"images/category/pic4.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-slide-show.html\">La Dolce Vita Meets Old School on Beach Style </a></h5></div></div></div></div></div></div>\r\n                                    <div id=\"lifestyle\" class=\"life-style-post-bx\"><div class=\"header-blog-carousel owl-carousel owl-btn-center-lr\"><div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-standart.html\"><img src=\"images/category/pic1.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-standart.html\">Ready or Not, the Return into of the Hobo Bag Is Nigh</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-left-sidebar.html\"><img src=\"images/category/pic2.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-left-sidebar.html\">This Week on Instagram, Celebri ties Went All-In on Prints</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-full-width.html\"><img src=\"images/category/pic3.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title\"><h5 class=\"post-title\"><a href=\"post-header-image.html\">Anniversary With An Exhibition At Dallas Contemporary</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-slide-show.html\"><img src=\"images/category/pic4.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-slide-show.html\">La Dolce Vita Meets Old School on Beach Style </a></h5></div></div></div></div></div></div>\r\n                                    <div id=\"fashion\" class=\"life-style-post-bx\"><div class=\"header-blog-carousel owl-carousel owl-btn-center-lr\"><div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-standart.html\"><img src=\"images/category/pic1.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-standart.html\">Ready or Not, the Return into of the Hobo Bag Is Nigh</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-left-sidebar.html\"><img src=\"images/category/pic2.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-left-sidebar.html\">This Week on Instagram, Celebri ties Went All-In on Prints</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-full-width.html\"><img src=\"images/category/pic3.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title\"><h5 class=\"post-title\"><a href=\"post-header-image.html\">Anniversary With An Exhibition At Dallas Contemporary</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-slide-show.html\"><img src=\"images/category/pic4.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-slide-show.html\">La Dolce Vita Meets Old School on Beach Style </a></h5></div></div></div></div></div></div>\r\n                                    <div id=\"travel\" class=\"life-style-post-bx\"><div class=\"header-blog-carousel owl-carousel owl-btn-center-lr\"><div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-standart.html\"><img src=\"images/category/pic1.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-standart.html\">Ready or Not, the Return into of the Hobo Bag Is Nigh</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-left-sidebar.html\"><img src=\"images/category/pic2.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-left-sidebar.html\">This Week on Instagram, Celebri ties Went All-In on Prints</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-full-width.html\"><img src=\"images/category/pic3.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title\"><h5 class=\"post-title\"><a href=\"post-header-image.html\">Anniversary With An Exhibition At Dallas Contemporary</a></h5></div></div></div></div>\r\n                                            <div class=\"item\"><div class=\"blog-post blog-sm\"><div class=\"dlab-post-media\"><a href=\"post-slide-show.html\"><img src=\"images/category/pic4.jpg\" alt></a></div>\r\n                                                    <div class=\"dlab-post-info\"><div class=\"dlab-post-title \"><h5 class=\"post-title\"><a href=\"post-slide-show.html\">La Dolce Vita Meets Old School on Beach Style </a></h5></div></div></div></div></div></div></div></div></div></li>\r\n                    <li><a href=\"javascript:void(0);\">Shop<i class=\"fa fa-chevron-down\"></i></a>\r\n                        <ul class=\"sub-menu\"><li><a href=\"shop-product.html\">Product</a></li>\r\n                            <li><a href=\"shop-product-details.html\">Product Details</a></li>\r\n                            <li><a href=\"shop-cart.html\">Cart</a></li>\r\n                            <li><a href=\"shop-checkout.html\">Checkout</a></li></ul></li>\r\n                    <li><a href=\"javascript:void(0);\">Pages<i class=\"fa fa-chevron-down\"></i></a>\r\n                        <ul class=\"sub-menu\"><li><a href=\"about-me.html\">About</a></li>\r\n                            <li><a href=\"archive.html\">Archive</a></li>\r\n                            <li><a href=\"author.html\">Author</a></li>\r\n                            <li><a href=\"category.html\">Category</a></li>\r\n                            <li><a href=\"tags.html\">Tags</a></li>\r\n                            <li><a href=\"search-results.html\">Search results</a></li>\r\n                            <li><a href=\"coming-soon.html\">Coming Soon</a></li>\r\n                            <li><a href=\"sitedown.html\">Maintenance</a></li>\r\n                            <li><a href=\"error-404.html\">Error 404</a></li></ul></li>\r\n                    <li><a href=\"contact-me.html\">Contact</a></li></ul></div></div></div></div>");
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591