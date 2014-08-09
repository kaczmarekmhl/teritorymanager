using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        #region IndexAction

        public ActionResult Index()
        {
            var model =
                SetCurrentCongregationFilter(db.UserProfiles)
                .OrderBy(u => u.UserName);

            return View(model);
        }

        #endregion

    }
}
