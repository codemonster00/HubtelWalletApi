using HubTelWalletApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HubTelWalletApi.Context
{
    public  class AppDbContext:DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

           builder.Entity<Wallet>().HasQueryFilter(w => !w.IsDeleted);

           
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

    }
}