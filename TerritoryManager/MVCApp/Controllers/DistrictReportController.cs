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

        public ActionResult Create(int Id, int UserId, DateTime date, DistrictReport.ReportTypes type, DistrictReport.ReportStates state = DistrictReport.ReportStates.Pending)
        {
            var district = db.Districts.Find(Id);

            var latestCompleteReport = district.Reports_LatestCompleteReport;

            if (latestCompleteReport != null && latestCompleteReport.Date.Date >= date.Date)
            {
                return new HttpNotFoundResult();
            }

            var user = db.UserProfiles.Find(UserId);

            var districtReport = new DistrictReport()
            {
                District = district,
                User = user,
                Date = date,
                Type = type,
                State = state
            };

            district.DistrictReports.Add(districtReport);
            db.SaveChanges();

            ViewBag.ReportSuccessful = true;

            return ListDistrictReports(district);
        }

        #endregion

        #region GetReportCompletionForm

        public PartialViewResult GetReportCompletionForm(District district)
        {
            if (User.IsInRole("Admin"))
            {
                //Select dropdown values
                ViewBag.UserSelectList = GetUserSelectionList(district.AssignedToUserId);
            }

            return PartialView("_ReportCompletion", district);
        }

        #endregion

        #region ListDistrictReportsAction

        [ChildActionOnly]
        public PartialViewResult ListDistrictReports(District district)
        {
            var reports = district.DistrictReports;

            if (!User.IsInRole("Admin"))
            {
                reports = reports.Where(r => r.UserId == WebSecurity.CurrentUserId).ToList();
            }

            return PartialView("_ListDistrictReports", reports
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.Id));
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
