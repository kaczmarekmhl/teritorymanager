namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<MVCApp.Models.TerritoryDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MVCApp.Models.TerritoryDb";
        }

        protected override void Seed(MVCApp.Models.TerritoryDb context)
        {
            SeedMembership();
        }

        private void SeedMembership()
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
            if (membership.GetUser("testuser", false) == null)
            {
                membership.CreateUserAndAccount("testuser", "Karslunde");
            }
            if (!roles.GetRolesForUser("admin").Contains("Admin"))
            {
                roles.AddUsersToRoles(new[] { "admin" }, new[] {"Admin"});
            }
        }
    }
}
