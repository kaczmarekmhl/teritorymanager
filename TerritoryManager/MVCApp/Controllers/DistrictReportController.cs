using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    [Authorize]
    public class DistrictReportController : BaseController
    {
        #region IndexAction

        [Authorize(Roles = "Admin")]
        public ActionResult Index(IEnumerable<DistrictReport> reports = null, DistrictReport.ReportStates state = DistrictReport.ReportStates.Pending)
        {
            if (reports == null)
            {
                reports = SetCurrentCongregationFilter(db.DistrictReports).Where(dr => dr.State == state).
                    ToList().
                    OrderBy(dr => dr.District.Number, new District.DistrictNumberComparer()).
                    ThenBy(dr => dr.Date);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", reports);
            }

            return View("Index", reports);
        }

        #endregion

        #region CreateAction

        public ActionResult Create(int Id, DateTime date, DistrictReport.ReportTypes type, DistrictReport.ReportStates state = DistrictReport.ReportStates.Pending)
        {
            var district = db.Districts.Find(Id);

            if (district.AssignedToUserId != WebSecurity.CurrentUserId)
            {
                return new HttpNotFoundResult();
            }

            var latestCompleteReport = district.Reports_LatestCompleteReport;

            if (latestCompleteReport != null && latestCompleteReport.Date.Date >= date.Date)
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

            ViewBag.ReportSuccessful = true;

            return PartialView("_ReportCompletion", district);
        }

        #endregion

        #region ApproveAction

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Approve(int[] selectedReportId)
        {
            if (selectedReportId != null && selectedReportId.Count() > 0)
            {
                var reports = db.DistrictReports.
                    Include("User").Include("District").
                    Where(dr => selectedReportId.Contains(dr.Id)).
                    ToList();

                reports.ForEach(dr => dr.State = DistrictReport.ReportStates.Approved);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region PendingReportsCountAction

        [ChildActionOnly]
        [Authorize(Roles = "Admin")]
        public PartialViewResult PendingReportsCount()
        {
            ViewBag.PendingReportsCount = SetCurrentCongregationFilter(db.DistrictReports).Count(dr => dr.State == DistrictReport.ReportStates.Pending);

            return PartialView("_PendingReportsCount");
        }
        #endregion

    }
}
