using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasGame.Infrastructure;
using AliasGame.Infrastructure.Database;
using AliasGame.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AliasGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appOptions = new ApplicationOptions();
            Configuration.GetSection(ApplicationOptions.SectionName).Bind(appOptions);
            
            services.AddControllers();
            services.AddControllersWithViews().AddSessionStateTempDataProvider();
            services.AddSession();
            services.AddRepositories(new DbContextOptions()
            {
                ConnString = appOptions.ConnectionString
            });
            services.ConfigureIdentity(x =>
            {
                x.User.RequireUniqueEmail = true;
                x.Password.RequiredLength = 8;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireDigit = false;
            });
            services.AddUserManager();
            services.AddMapper();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(e =>
            {
                e.MapControllerRoute("default", "{controller=entry}/{action=index}");
            });
        }
    }
}