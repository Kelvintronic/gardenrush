using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using gardenrush.lib.Data;
using gardenrush.lib.services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace gardenrush.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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

            services.AddDbContext<GardenRushDbContext>(options =>
                        options.UseMySql(
                        Configuration.GetConnectionString("GardenRushConnection")));

            services.AddScoped<IGardenRepository, GardenRepository>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
