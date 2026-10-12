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
    }
}


