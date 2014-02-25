using MapLibrary;
using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FreeDistrictsController : Controller
    {
        #region IndexAction
        
        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region MapKmlAction

        public ActionResult MapKml()
        {
            var freeDistricts =
                db.Districts
                .Where(d => d.AssignedToUserId == null)
                .OrderBy(t => t.PostCodeFirst)
                .ThenBy(t => t.Name);

            var resultKmlDoc = new KmlDocument();

            foreach (var district in freeDistricts)
            {
                if (!district.Name.Contains("- T"))
                {
                    // District is divided - do not display it as free
                    if(db.Districts.Count(d => d.Name.Contains(district.Name + " - T")) > 0)
                    {
                        continue;
                    }
                }

                district.LoadExternalDistrictBoundaryKml();

                var districtKmlDoc = new KmlDocument(district.DistrictBoundaryKml);
                resultKmlDoc.MergeDocuments(districtKmlDoc);
            }

            return this.Content(resultKmlDoc.GetKmlWithPlacemarks().ToString(), "text/xml");
        }

        #endregion

        #region Database Access

        DistictManagerDb db;

        public FreeDistrictsController()
        {
            db = new DistictManagerDb();
        }

        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
