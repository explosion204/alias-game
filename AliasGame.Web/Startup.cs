using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AliasGame.Hubs;
using AliasGame.Infrastructure;
using AliasGame.Infrastructure.Database;
using AliasGame.Service;
using AliasGame.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

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
            services.AddRepositories(new DbContextOptions()
            {
                ConnString = appOptions.ConnectionString
            });
            services.AddMapper();
            services.AddAuth(Configuration);
            services.AddAppServices(Configuration);
            services.AddSignalR();
            services.AddSignalRCore();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(e =>
            {
                e.MapHub<MessageHub>("/MessageHub");
                e.MapControllerRoute("default", "{controller=entry}/{action=index}");
            });
        }
    }
}