using BoldSign.Api;
using InsuranceDemo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InsuranceDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            this.HostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TemplateDetails>();
            services.AddControllersWithViews();
            services.Configure<RouteOptions>(option =>
            {
                option.LowercaseUrls = true;
                option.LowercaseQueryStrings = true;
                // option.AppendTrailingSlash = true;
            });
            services.AddMvc();
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
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.Use((context, next) =>
            {
                var templateDetails = context.RequestServices.GetRequiredService<TemplateDetails>();
                context.Items["TemplateDetails"] = templateDetails;
                return next();
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/",
                    defaults: new { controller = "Home", action = "Index" });
                endpoints.MapControllerRoute(
                    name: "claim-form-basic-details",
                    pattern: "/basic-details/{id?}",
                    defaults: new { controller = "Home", action = "ClaimFormBasicDetails" });
                endpoints.MapControllerRoute(
                    name: "claim-form-address-information",
                    pattern: "/address-information/{id?}",
                    defaults: new { controller = "Home", action = "ClaimFormAddressInformation" });
                endpoints.MapControllerRoute(
                    name: "sign-document",
                    pattern: "/sign-document/{id?}",
                    defaults: new { controller = "Home", action = "SignDocument" });
                endpoints.MapControllerRoute(
                    name: "thank-you",
                    pattern: "/thank-you/",
                    defaults: new { controller = "Home", action = "SignCompleted" });
                endpoints.MapControllerRoute(
                    name: "download-document",
                    pattern: "/download/",
                    defaults: new { controller = "Home", action = "DownloadDocument" });
                endpoints.MapControllerRoute(
                    name: "about-us",
                    pattern: "/about-us/",
                    defaults: new { controller = "Home", action = "AboutUs" });
            });
        }
    }
}