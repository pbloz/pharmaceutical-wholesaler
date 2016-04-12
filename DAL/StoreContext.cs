using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Hurtownia.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Hurtownia.DAL
{
    public class StoreContext : IdentityDbContext<ApplicationUser>
    {
        public StoreContext()
            : base("DefaultConnection")
        {

        }
        static StoreContext()
        {
            Database.SetInitializer<StoreContext>(new StoreInitializer());
        }
        public static StoreContext Create()
        {
            return new StoreContext();
        }
        //tabela w bazie danych
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<BonusCode> BonusCodes { get; set; }

        public DbSet<Slider> Sliders { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Manufacture> Manufactures { get; set; }


        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}

    }
}