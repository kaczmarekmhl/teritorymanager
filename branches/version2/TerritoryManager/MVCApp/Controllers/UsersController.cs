using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        #region IndexAction

        public ActionResult Index()
        {
            var model =
                db.UserProfiles
                .OrderBy(u => u.UserName);

            return View(model);
        }

        #endregion

        #region Database Access

        DistictManagerDb db;

        public UsersController()
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
