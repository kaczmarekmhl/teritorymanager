using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class TerritoryDb : DbContext
    {
        public TerritoryDb() : base("name=DefaultConnection")
        {

        }
        public DbSet<PersonModel> Persons { get; set; }
        public DbSet<DistrictModel> Districts { get; set; }
    }
}