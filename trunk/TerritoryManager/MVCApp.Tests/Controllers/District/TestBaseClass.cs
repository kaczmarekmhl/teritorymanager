using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCApp.Controllers;
using System.Web.Mvc;
using MVCApp.Tests.Mocks;
using MVCApp.Models;
using System.Collections.Generic;

namespace MVCApp.Tests.Controllers
{
    [TestClass]
    public class TestBaseClass
    {
        public static MockTerritoryDb MockDb;
        public static DistrictController DistrictController;
        public static string Admin;


        [ClassInitialize()]
        public static void ClassInit()
        {
            MockDb = new MockTerritoryDb();
            MockDb.AddSet<DistrictModel>(TestData.Districts);
            DistrictController = new DistrictController(MockDb);        
        }

    }
}
