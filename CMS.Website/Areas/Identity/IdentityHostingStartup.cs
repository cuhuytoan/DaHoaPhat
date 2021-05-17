using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CMS.Website.Areas.Identity.IdentityHostingStartup))]
namespace CMS.Website.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}