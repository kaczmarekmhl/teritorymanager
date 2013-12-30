namespace MVCApp.Migrations
{
    using MVCApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<MVCApp.Models.DistictManagerDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MVCApp.Models.DistictManagerDb";
        }

        protected override void Seed(MVCApp.Models.DistictManagerDb context)
        {
            MVCApp.MvcApplication.InitializeSimpleMembershipProvider();
            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;
            if (!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }
            if (membership.GetUser("admin", false) == null)
            {
                membership.CreateUserAndAccount("admin", "Karslunde");
            }
            
            /*if (membership.GetUser("testuser", false) == null)
            {
                membership.CreateUserAndAccount("testuser", "Karslunde");
            }
            if (!roles.GetRolesForUser("admin").Contains("Admin"))
            {
                roles.AddUsersToRoles(new[] { "admin" }, new[] { "Admin" });
            }

            var user = context.UserProfiles.First(p => p.UserName == "testuser");

            context.Districts.AddOrUpdate(
                t => t.Name,
                new District
                {
                    Number = "1",
                    Name = "Ballerup",
                    PostCodeFirst = 2750,
                    AssignedTo = user,
                },
                new District
                {
                    Number = "2",
                    Name = "Hedehusene",
                    PostCodeFirst = 2640,
                    AssignedTo = user,
                },
                new District
                {
                    Number = "3",
                    Name = "Ishoj",
                    PostCodeFirst = 2635,
                    AssignedTo = null,
                },
                new District
                {
                    Number = "4",
                    Name = "Osterbro",
                    PostCodeFirst = 2100,
                    AssignedTo = user,
                },
                new District
                {
                    Number = "5",
                    Name = "Norrebro",
                    PostCodeFirst = 2200,
                    AssignedTo = user,
                },
                new District
                {
                    Number = "6",
                    Name = "Kobenhavn S",
                    PostCodeFirst = 2200,
                    AssignedTo = user,
                },
                new District
                {
                    Number = "7A",
                    Name = "Kobenhavn V.A",
                    PostCodeFirst = 1501,
                    PostCodeLast = 1550,
                    AssignedTo = user,
                },
                new District
                {
                    Number = "7B",
                    Name = "Kobenhavn V.B",
                    PostCodeFirst = 1551,
                    PostCodeLast = 1600,
                    AssignedTo = user,
                });*/
        }
    }
}
