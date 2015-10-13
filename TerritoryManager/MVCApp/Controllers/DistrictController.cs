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

            if (string.IsNullOrEmpty(district.DistrictBoundaryKml))
            {
                if (!string.IsNullOrEmpty(district.SearchPhrase))
                {
                    foreach (var searchPhrase in district.SearchPhrases)
                    {
                        int postCode;

                        if(int.TryParse(searchPhrase, out postCode))
                        {
                            ViewBag.MapCenterAddress = string.Format("{0:0000}", postCode);
                            break;
                        }
                    }                    
                }
                else if (district.PostCodeFirst > 0)
                { 
                    ViewBag.MapCenterAddress = string.Format("{0:0000}", district.PostCodeFirst);
                }

                if (CurrentCongregation.Country == Enums.Country.Norway)
                {
                    ViewBag.MapCountryCode = "NO";
                }
                else if (CurrentCongregation.Country == Enums.Country.Denmark)
                {
                    ViewBag.MapCountryCode = "DK";
                }
            }

            return View(district);
        }

        #endregion

        #region ListDistrictReportsAction

        [ChildActionOnly]
        [Authorize(Roles = "Admin")]
        public PartialViewResult ListDistrictReports(IEnumerable<DistrictReport> reports)
        {
            return PartialView("_ListDistrictReports", reports.OrderByDescending(r => r.Date));
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
