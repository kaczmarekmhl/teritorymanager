using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace MVCApp.Tests.Controllers.District
{
    [TestClass]
    public class SearchDistrictMenu_AdminTest : TestBaseClass
    {

        [TestMethod]
        public void AdminCanSeeAllDistrictsInDropDownMenu()
        {
            DistrictController.PopulateSearchDistrictMenu("", false, true, null);
            ViewResult result = DistrictController.Index() as ViewResult;

            // Assert
           // Assert.AreEqual("Welcome on my web site!", result.ViewBag.Welcome);
           // Assert.AreEqual("If you have an account, after log-in you can search for Poles that live in Denmark.", result.ViewBag.Message);
        }
    }
}
