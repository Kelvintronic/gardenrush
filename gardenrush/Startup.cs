using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using gardenrush.Areas.Identity;
using gardenrush.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using gardenrush.Services;
using gardenrush.lib.Data;
using gardenrush.lib.services;
using Microsoft.AspNetCore.HttpOverrides;

namespace gardenrush
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // configure Kestrel
            services.Configure<KestrelServerOptions>(options =>
            {
                byte[] localhost = { 127, 0, 0, 1 };
                IPAddress address = new IPAddress(localhost);
                options.Listen(address, Int32.Parse(Configuration["GardenRushPorts:Https"]), listenOptions =>
                {
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                        listenOptions.UseHttps(Configuration["SSL_Cert:Path"],
                                                Configuration["SSL_Cert:Password"]);
                    else
                        listenOptions.UseHttps();
                });
            });

            // Identity database connection
            services.AddDbContext<GardenIdentityDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("IdentityConnection")));

            // If custom pages are implemented then user AddIdentity() and remove the UI Nuget package
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<GardenIdentityDbContext>();

            // The following two statements are required to share the login cookies between apps on the same domain
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("../"))
                .SetApplicationName("Kelvintronic");
            }
            else
            {
                services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/_Engineering/01 SoftwareProjects/"))
                .SetApplicationName("Kelvintronic");
            }

            // see above: required to share cookie
            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = ".AspNet.SharedCookie";
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            services.AddSingleton<TurnService>();

            services.AddDbContext<GardenRushDbContext>(options =>
            options.UseMySql(
            Configuration.GetConnectionString("GardenRushConnection")));

            services.AddScoped<IGardenRepository, GardenRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // required to allow the app to run as a subdirectory
            app.UsePathBase("/gardenrush");

            // using Microsoft.AspNetCore.HttpOverrides;
            // required if app is behind another web server such as Apache
            // Note: Apache must be set up as a proxy with header forwarding enabled
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
