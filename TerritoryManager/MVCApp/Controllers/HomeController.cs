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
            ViewBag.Welcome = "Welcome on my web site!";
            ViewBag.Message = "If you have an account, after log-in you can search for Poles that live in Denmark.";
            return View();
        }
    }
}
