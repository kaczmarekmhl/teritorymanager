using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCApp;
using MVCApp.Controllers;

namespace MVCApp.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        //[ClassInitialize]
        //public static void ClassInit(TestContext contex)
        //{
        //    MockDb = new MockTerritoryDb();
        //    MockDb.AddSet<MVCApp.Models.District>(TestData.Districts);

        //}
        
        [TestMethod]
        public void IndexHomeController_ValidateMessages()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Welcome on my web site!", result.ViewBag.Welcome);
            Assert.AreEqual("If you have an account, after log-in you can search for Poles that live in Denmark.", result.ViewBag.Message);
        }
    }
}
