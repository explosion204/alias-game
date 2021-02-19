using System;
using AliasGame.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AliasGame.Infrastructure.Database
{
    internal class AppDbContext : IdentityDbContext<EfUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<EfExpression> Expressions { get; set; }
        public DbSet<EfSession> Sessions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
        
    }
}