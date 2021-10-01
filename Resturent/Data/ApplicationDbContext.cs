using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurent.Models;
using Resturent.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Resturent.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Coupon> Coupon { get; set; }
    }
}
