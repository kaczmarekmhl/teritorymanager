using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/

        public ActionResult Index(List<Person> personList)
        {
            return View();
        }

    }
}
