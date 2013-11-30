namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVCApp.Models.TerritoryDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MVCApp.Models.TerritoryDb";
        }

        protected override void Seed(MVCApp.Models.TerritoryDb context)
        {           
        }
    }
}
