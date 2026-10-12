using Microsoft.EntityFrameworkCore;
using SKU.Models;
using System.Collections.Generic;

namespace SKU.DbContexts
{
    public class SKUContext : DbContext
    {
        public SKUContext(DbContextOptions<SKUContext> options) : base(options)
        {
        }
        public DbSet<User> Sec_User { get; set; }
        public DbSet<UserType_Lookup> Sec_UserType { get; set; }

        public DbSet<UserLastLogins> Sec_Login { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserType_Lookup)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.UserTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserType_Lookup>().HasData(
                new UserType_Lookup { Id = 1, MajorCode = 1, MinorCode = 1, Name = "Customer", Description = "Human, buys from a store" },
                new UserType_Lookup { Id = 2, MajorCode = 1, MinorCode = 2, Name = "Admin", Description = "System administrator" }
            );
        }
    }
}


