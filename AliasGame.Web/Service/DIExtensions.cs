using System;
using System.Text;
using AliasGame.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AliasGame.Service
{
    public static class DIExtensions
    {
        public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var appOptions = new ApplicationOptions();
            configuration.GetSection(ApplicationOptions.SectionName).Bind(appOptions);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appOptions.SecretKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            var appOptions = new ApplicationOptions();
            configuration.GetSection(ApplicationOptions.SectionName).Bind(appOptions);

            services.AddTransient<IDataManager, DataManager>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IExpressionService, ExpressionService>();
            services.AddTransient<IUserService>(
                x => new UserService(x.GetService<IDataManager>(), appOptions.SecretKey)
            );
        }
    }
}