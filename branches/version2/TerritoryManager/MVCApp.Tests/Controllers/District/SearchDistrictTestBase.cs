using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCApp.Controllers;
using System.Web.Mvc;
using MVCApp.Tests.Mocks;
using System.Collections.Generic;

namespace MVCApp.Tests.Controllers
{
    [TestClass]
    public class SearchDistrictTestBase
    {
        public static string AdminUser = "admin";
        public static string TestUser = "testuser";
        public static HashSet<string> AdminRoles = new HashSet<string>() { "Admin" };
        
        public static DistrictController DistrictController;
        public static MockTerritoryDb MockDb;     

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext contex)
        {
            MockDb = new MockTerritoryDb();
            MockDb.AddSet<MVCApp.Models.District>(TestData.Districts);
            DistrictController = new DistrictController(MockDb);
        }
    }
}
