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
    public class MyDistrictsController : Controller
    {
        public ActionResult Index()
        {
            var model =
                db.Districts
                .Where(t => t.AssignedTo.UserId == WebSecurity.CurrentUserId)
                .OrderBy(t => t.PostCodeFirst);

            return View(model);
        }

        #region Database Access

        DistictManagerDb db;

        public MyDistrictsController()
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
