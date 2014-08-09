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
            AddUsersAndRoles(context);
            AddCongregations(context);
            AddDistricts(context);
        }

        protected void AddUsersAndRoles(MVCApp.Models.DistictManagerDb context)
        {
            MVCApp.MvcApplication.InitializeSimpleMembershipProvider();
            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists(SystemRoles.Admin.ToString()))
            {
                roles.CreateRole(SystemRoles.Admin.ToString());
            }

            if (!roles.RoleExists(SystemRoles.Elder.ToString()))
            {
                roles.CreateRole(SystemRoles.Elder.ToString());
            }


            if (membership.GetUser("admin", false) == null)
            {
                membership.CreateUserAndAccount("admin", "Karslunde");
            }

            if (!roles.GetRolesForUser("admin").Contains(SystemRoles.Admin.ToString()))
            {
                roles.AddUsersToRoles(new[] { "admin" }, new[] { SystemRoles.Admin.ToString() });
            }
            
            if (membership.GetUser("jutlandia", false) == null)
            {
                membership.CreateUserAndAccount("jutlandia", "niebieskipomidor27");
            }

            if (!roles.GetRolesForUser("jutlandia").Contains(SystemRoles.Admin.ToString()))
            {
                roles.AddUsersToRoles(new[] { "jutlandia" }, new[] { SystemRoles.Admin.ToString() });
            }
            

            if (membership.GetUser("kurtp", false) != null && !roles.GetRolesForUser("kurtp").Contains(SystemRoles.Elder.ToString()))
            {
                roles.AddUsersToRoles(new[] { "kurtp" }, new[] { SystemRoles.Elder.ToString() });
            }

            if (membership.GetUser("zdzisiekw", false) != null && !roles.GetRolesForUser("zdzisiekw").Contains(SystemRoles.Elder.ToString()))
            {
                roles.AddUsersToRoles(new[] { "zdzisiekw" }, new[] { SystemRoles.Elder.ToString() });
            }

            if (membership.GetUser("slawekz", false) != null && !roles.GetRolesForUser("slawekz").Contains(SystemRoles.Elder.ToString()))
            {
                roles.AddUsersToRoles(new[] { "slawekz" }, new[] { SystemRoles.Elder.ToString() });
            }

            if (membership.GetUser("jakubw", false) != null && !roles.GetRolesForUser("jakubw").Contains(SystemRoles.Elder.ToString()))
            {
                roles.AddUsersToRoles(new[] { "jakubw" }, new[] { SystemRoles.Elder.ToString() });
            }

            if (membership.GetUser("bartoszs", false) != null && !roles.GetRolesForUser("bartoszs").Contains(SystemRoles.Elder.ToString()))
            {
                roles.AddUsersToRoles(new[] { "bartoszs" }, new[] { SystemRoles.Elder.ToString() });
            }

            if (membership.GetUser("testuser", false) == null)
            {
                membership.CreateUserAndAccount("testuser", "Karslunde");
            }
        }

        protected void AddCongregations(MVCApp.Models.DistictManagerDb context)
        {
            var cphName = "København Polsk Menighed";
            var jutName = "Skanderborg højvang menighed";

            context.Congregations.AddOrUpdate(
                c => c.Name,
                new Congregation
                {
                    Name = cphName
                },
                 new Congregation
                 {
                     Name = jutName
                 });

            var congregationCph = context.Congregations.First(c => c.Name.Equals(cphName));
            var congregationJut = context.Congregations.First(c => c.Name.Equals(jutName));

            var admin = context.UserProfiles.First(u => u.UserName == "admin");
            admin.Congregation = congregationCph;

            var jut = context.UserProfiles.First(u => u.UserName == "jutlandia");
            jut.Congregation = congregationJut;

            context.Entry(admin).State = EntityState.Modified;
            context.Entry(jut).State = EntityState.Modified;
            context.SaveChanges();
        }

        protected void AddDistricts(MVCApp.Models.DistictManagerDb context)
        {
            /*
            var user = context.UserProfiles.First(p => p.UserName == "testuser");            

            context.Districts.AddOrUpdate(
                t => t.Name,
                new District
                {
                    Number = "1",
                    Name = "Ballerup",
                    PostCodeFirst = 2750,
                    AssignedTo = user,
                    Congregation = congregation
                },
                new District
                {
                    Number = "2",
                    Name = "Hedehusene",
                    PostCodeFirst = 2640,
                    AssignedTo = user,
                    Congregation = congregation
                },
                new District
                {
                    Number = "3",
                    Name = "Ishoj",
                    PostCodeFirst = 2635,
                    AssignedTo = null,
                    Congregation = congregation
                },
                new District
                {
                    Number = "4",
                    Name = "Osterbro",
                    PostCodeFirst = 2100,
                    AssignedTo = user,
                    Congregation = congregation
                },
                new District
                {
                    Number = "5",
                    Name = "Norrebro",
                    PostCodeFirst = 2200,
                    AssignedTo = user,
                    Congregation = congregation
                },
                new District
                {
                    Number = "6",
                    Name = "Kobenhavn S",
                    PostCodeFirst = 2200,
                    AssignedTo = user,
                    Congregation = congregation
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
                    Congregation = congregation
               });
             */
        }
    }
}
