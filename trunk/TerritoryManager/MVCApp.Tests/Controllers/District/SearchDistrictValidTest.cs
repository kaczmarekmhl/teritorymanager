using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Collections.Generic;
using WebMatrix.WebData;
using MVCApp.Controllers;
using MVCApp.Tests.Mocks;

namespace MVCApp.Tests.Controllers.District
{
    [TestClass]
    public class SearchDistrictValidTest : SearchDistrictTestBase
    {

        [TestMethod]
        public void AdminDistrictSearch_CanSeeAllDistricts()
        {
            DistrictController.ControllerContext = new MockControllerContext(AdminUser, AdminRoles);

            ViewResult result = DistrictController.Index() as ViewResult;

            Assert.IsNull(result.ViewBag.ErrorMsg);
            Assert.AreEqual(result.ViewBag.DistrictSelectListItem.Count, 3);
        }

        [TestMethod]
        public void UserDistrictSearch_CanSeeOnlyDistrictsBelongingToUser()
        {
            DistrictController.ControllerContext = new MockControllerContext(TestUser, null);

            ViewResult result = DistrictController.Index() as ViewResult;

            Assert.IsNull(result.ViewBag.ErrorMsg);
            Assert.AreEqual(result.ViewBag.DistrictSelectListItem.Count, 2);
        }

        [TestMethod]
        public void AdminDistrictSearch_SelectsExistingDistrict()
        {
            DistrictController.ControllerContext = new MockControllerContext(AdminUser, AdminRoles);
            string districtIdToSelect = "2";

            ViewResult result = DistrictController.Search(districtIdToSelect) as ViewResult;                      

            Assert.IsNull(DistrictController.ViewBag.ErrorMsg);

            List<SelectListItem> items = DistrictController.ViewBag.DistrictSelectListItem;
            var districtSelected = items.Find(i => i.Selected == true);

            Assert.AreEqual(districtIdToSelect, districtSelected.Value);
        }

        [TestMethod]
        public void UserDistrictSearch_SelectsExistingDistrict()
        {
            DistrictController.ControllerContext = new MockControllerContext(TestUser, null);
            string districtIdToSelect = "2";

            ViewResult result = DistrictController.Search(districtIdToSelect) as ViewResult;

            Assert.IsNull(DistrictController.ViewBag.ErrorMsg);

            List<SelectListItem> items = DistrictController.ViewBag.DistrictSelectListItem;
            var districtSelected = items.Find(i => i.Selected == true);

            Assert.AreEqual(districtIdToSelect, districtSelected.Value);
        }
    }
}
