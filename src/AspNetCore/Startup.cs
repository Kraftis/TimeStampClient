using Disig.RemoteHsm.Web.Infrastructure.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore
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
            services.AddControllersWithViews();
            services.AddCors(
                options =>
                {
                    options.AddPolicy(name: "MyAllowSpecificOrigins",
                                      builder =>
                                      {
                                          builder.AllowAnyOrigin()
                                                 .AllowAnyMethod()
                                                 .AllowAnyHeader();
                                      });
                });
            services.AddProxy(opts =>

            {

                opts.FilterRequest = req => !string.Equals(req.Method, "OPTIONS", StringComparison.Ordinal) && !string.Equals(req.Method, "HEAD", StringComparison.Ordinal);

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("MyAllowSpecificOrigins");
            app.UseAuthorization();

            app.MapWhen(httpContext => httpContext.Request.Path.Value.StartsWith(@"/", StringComparison.OrdinalIgnoreCase), builder => builder.RunProxy(new Disig.RemoteHsm.Web.Infrastructure.Proxy.ProxyOptions()

            {

                Scheme = "https",

                Host = new Microsoft.AspNetCore.Http.HostString("freetsa.org", 443) //TSA hostname and port

            }));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
