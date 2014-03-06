using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    public class DistrictReportController : Controller
    {

        [Authorize(Roles = "Admin")]
        public ActionResult Index(IEnumerable<DistrictReport> reports = null, DistrictReport.ReportStates state = DistrictReport.ReportStates.Pending)
        {
            if (reports == null)
            {
                reports = db.DistrictReports.
                    Where(dr => dr.State == state).
                    OrderBy(dr => dr.District.Number).
                    ThenBy(dr => dr.Date).
                    ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", reports);
            }

            return View("Index", reports);
        }


        [Authorize]
        public ActionResult Create(int Id, DateTime date, DistrictReport.ReportTypes type, DistrictReport.ReportStates state = DistrictReport.ReportStates.Pending)
        {
            var district = db.Districts.Find(Id);

            if (district.AssignedToUserId != WebSecurity.CurrentUserId)
            {
                return new HttpNotFoundResult();
            }

            var latestCompleteReport = district.Reports_LatestCompleteReport;

            if (latestCompleteReport != null && latestCompleteReport.Date >= date)
            {
                return new HttpNotFoundResult();
            }

            var districtReport = new DistrictReport()
            {
                District = district,
                User = district.AssignedTo,
                Date = date,
                Type = type,
                State = state
            };

            district.DistrictReports.Add(districtReport);
            db.SaveChanges();

            return PartialView("_ReportCompletion", district);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Approve(int[] selectedReportId)
        {
            var reports = db.DistrictReports.
                Include("User").Include("District").
                Where(dr => selectedReportId.Contains(dr.Id)).
                ToList();

            if (selectedReportId.Count() > 0)
            {
                reports.ForEach(dr => dr.IsApproved = true);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        #region Database Access

        DistictManagerDb db;

        public DistrictReportController()
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
