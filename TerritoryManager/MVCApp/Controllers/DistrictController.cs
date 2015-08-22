using MapLibrary;
using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    [Authorize]
    public class DistrictController : BaseController
    {
        #region IndexAction

        public ActionResult Index(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            ViewBag.IsAdressSharingEnabled = IsSharingAdressesEnabled;

            return View(district);
        }

        #endregion

        #region MapKmlAction

        public ActionResult MapKml(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            return this.Content(district.GetDistrictBoundaryKmlDoc().ToString(), "text/xml");
        }

        #endregion
    }
}
