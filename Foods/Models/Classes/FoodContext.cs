using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Foods.Models.Classes
{
    public class FoodContext:DbContext
    {
        //ConnectionString
        public FoodContext():base("FoodContext")
        {

        }
        //DbSet<>
        public DbSet<Food> Foods { get; set; }
        public DbSet<TopFood> TopFoods { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<About> Abouts { get; set; }



    }
}