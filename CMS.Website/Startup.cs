using Blazored.Toast;
using CMS.Website.Areas.Identity;
using CMS.Website.NotiHub;
using CMS.Website.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace CMS.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }
        public IWebHostEnvironment _env { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddRazorPages();
            services.AddServerSideBlazor().AddHubOptions(x => x.MaximumReceiveMessageSize = 102400000);
            // ===== Add Database ===============================
            services.ConfigureConnectDB(Configuration.GetConnectionString("CmsConnection"));
            // ===== Add Database Auth===========================
            services.ConfigureConnectDBAuth(Configuration.GetConnectionString("AuthConnection"));
            // ===== Config Identity=============================
            services.ConfigureDefaultIdentity();
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            //services.AddDatabaseDeveloperPageExceptionFilter();
            // ===== Add Logging ================================
            services.ConfigureLogging();


            // ===== Add RazorPage Authorize=====================
            services.ConfigureRazorPagesAuthorize();
            // ===== Config AutoMaper ===========================
            services.AddAutoMapper(typeof(Startup));
            // Add the Kendo UI services to the services container.
            services.AddTelerikBlazor();
            // ===== Add DI Services Wraper =====================
            services.ConfigureRepositoryWrapper();
            // ===== Add Services Transient Repository===========
            services.ConfigureTransient();
            // ===== Add Toast Blazor===========
            services.AddBlazoredToast();
            // ===== Add Noti===========
            //services.AddSignalR(hubOptions =>
            //{
            //    hubOptions.EnableDetailedErrors = true;
            //    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(60);
            //});
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddServerSideBlazor().AddCircuitOptions(o =>
            {
                if (_env.IsDevelopment()) //only add details when debugging
                {
                    o.DetailedErrors = true;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    OnPrepareResponse = ctx =>
            //    {
            //        const int durationInSeconds = 60 * 60 * 24;
            //        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            //            "public,max-age=" + durationInSeconds;
            //    }
            //});

            app.UseRouting();
            app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapHub<NotificationHubs>("/notificationHubs");
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapFallbackToAreaPage("/Admin/{*clientroutes:nonfile}", "/_HostAdmin", "Admin");

            });
        }
    }
}
