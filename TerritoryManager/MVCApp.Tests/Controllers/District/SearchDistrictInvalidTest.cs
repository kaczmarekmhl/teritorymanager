using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MVCApp.Controllers;
using MVCApp.Tests.Mocks;
using System.Web.Mvc;

namespace MVCApp.Tests.Controllers.District
{
    [TestClass]
    public class SearchDistrictInvalidTest
    {
        public static string AdminUser = "admin";
        public static string TestUser = "testuser";
        public static HashSet<string> AdminRoles = new HashSet<string>() { "Admin" };

        public static DistrictController DistrictController;
        public static MockTerritoryDb MockDb;

        [TestInitialize()]
        public void TestInit()
        {
            MockDb = new MockTerritoryDb();
            MockDb.AddSet<MVCApp.Models.District>(TestData.Districts);
            MockDb.AddSet<MVCApp.Models.Person>(TestData.Persons);
            DistrictController = new DistrictController(MockDb);
        }

       
        [TestMethod]
        public void AdminDistrictSearch_NullDistrictId()
        {
            DistrictController.ControllerContext = new MockControllerContext(AdminUser, AdminRoles);

            HttpStatusCodeResult result = DistrictController.Search(null) as HttpStatusCodeResult;

            Assert.AreEqual(result.StatusCode, 400);
        }

        [TestMethod]
        public void AdminDistrictSearch_NonExistingDistrictId()
        {
            DistrictController.ControllerContext = new MockControllerContext(AdminUser, AdminRoles);

            HttpStatusCodeResult result = DistrictController.Search("non existing district id") as HttpStatusCodeResult;

            Assert.AreEqual(result.StatusCode, 400);
        }

        [TestMethod]
        public void UserDistrictSearch_NullDistrictId()
        {
            DistrictController.ControllerContext = new MockControllerContext(TestUser, AdminRoles);

            HttpStatusCodeResult result = DistrictController.Search(null) as HttpStatusCodeResult;

            Assert.AreEqual(result.StatusCode, 400);
        }

        [TestMethod]
        public void UserDistrictSearch_NonExistingDistrictId()
        {
            DistrictController.ControllerContext = new MockControllerContext(TestUser, AdminRoles);

            HttpStatusCodeResult result = DistrictController.Search("non existing district id") as HttpStatusCodeResult;

            Assert.AreEqual(result.StatusCode, 400);
        }

        [TestMethod]
        public void UserDistrictSearch_NoRightsToSeeDistrict()
        {
            DistrictController.ControllerContext = new MockControllerContext(TestUser, null);

            HttpStatusCodeResult result = DistrictController.Search("3") as HttpStatusCodeResult;

            Assert.AreEqual(result.StatusCode, 400);
        }
    }
}
