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

            return View(model.ToList());
        }

    }
}
