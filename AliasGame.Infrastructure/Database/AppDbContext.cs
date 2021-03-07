using System;
using AliasGame.Domain.Models;
using AliasGame.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AliasGame.Infrastructure.Database
{
    internal class AppDbContext : DbContext
    {
        public DbSet<EfUser> Users { get; set; }
        public DbSet<EfExpression> Expressions { get; set; }
        public DbSet<EfSession> Sessions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}