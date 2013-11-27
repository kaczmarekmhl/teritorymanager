using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            PersonModelTest cmodel = new PersonModelTest();
            cmodel.FirstName = "Bartek";
            cmodel.LastName = "Gasior";

            return View(cmodel);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult Test(string str)
        {
           var message =  Server.HtmlEncode(str);

           //return RedirectPermanent("http://www.onet.pl");
           // return File(Server.MapPath("~/Content/Site.css"), "text/css");
           return Json(new { Name = "Bartek" }, JsonRequestBehavior.AllowGet);
        }
    }
}
