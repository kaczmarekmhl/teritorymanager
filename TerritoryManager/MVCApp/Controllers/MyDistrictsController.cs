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
    public class MyDistrictsController : BaseController
    {
        public ActionResult Index()
        {
            var model =
                db.Districts
                .Where(t => t.AssignedTo.UserId == WebSecurity.CurrentUserId)
                .OrderBy(t => t.PostCodeFirst)
                .ThenBy(t => t.Name);

            ViewBag.PeoplePerDistrictCount = (from d in db.Districts
                                          join p in db.Persons on d.Id equals p.District.Id
                                          where d.AssignedToUserId == WebSecurity.CurrentUserId
                                          && p.Selected == true
                                          && (p.AddedByUserId == d.AssignedToUserId || IsSharingAdressesEnabled || p.DoNotVisit == true)
                                          group d by d.Id into grouped
                                          select new
                                          {
                                              userId = grouped.Key,
                                              personCount = grouped.Count()
                                          }).ToDictionary(d => d.userId, d => d.personCount);

            return View(model.ToList());
        }

        public ActionResult MapKml()
        {
            var freeDistricts =
                SetCurrentCongregationFilter(db.Districts)
                .Where(d => d.AssignedToUserId == WebSecurity.CurrentUserId)
                .OrderBy(t => t.PostCodeFirst)
                .ThenBy(t => t.Name)
                .ToList();

            var resultKmlDoc = new KmlDocument();

            foreach (var district in freeDistricts)
            {
                resultKmlDoc.MergeDocuments(district.GetDistrictBoundaryKmlDoc(false));
            }

            return this.Content(resultKmlDoc.ToString(), "text/xml");
        }
    }
}
