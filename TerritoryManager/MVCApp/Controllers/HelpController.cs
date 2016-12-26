using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HelpController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}