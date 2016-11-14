using MVCApp.Helpers;
using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MigrationController : BaseController
    { 
        public ActionResult Index()
        {
            return View();
        }
    }
}
