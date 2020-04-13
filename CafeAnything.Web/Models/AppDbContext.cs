using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CafeAnything.Web.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("CafeAnything2020")
        {

        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Categories> Categories { get; set; }

        public DbSet<Tables> Tables { get; set; }

        public DbSet<OrderCart> OrderCarts { get; set; }
    }

}