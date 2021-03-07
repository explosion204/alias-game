using System;
using AliasGame.Domain;
using AliasGame.Domain.Models;
using AliasGame.Infrastructure.Database;
using AliasGame.Infrastructure.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DbContextOptions = AliasGame.Infrastructure.Database.DbContextOptions;

namespace AliasGame.Infrastructure
{
    public static class DIExtensions
    {
        public static void AddRepositories(this IServiceCollection services, DbContextOptions options)
        {
            services.AddDbContext<AppDbContext>(x => x.UseNpgsql(options.ConnString), 
                ServiceLifetime.Transient);
            services.AddTransient<IRepository<Expression>, ExpressionRepository>();
            services.AddTransient<IRepository<Session>, SessionRepository>();
            services.AddTransient<IRepository<User>, UserRepository>();
        }

        public static void AddMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            services.AddSingleton(typeof(IMapper), mapperConfig.CreateMapper());
        }
    }
}