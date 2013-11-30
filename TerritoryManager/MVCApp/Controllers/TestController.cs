using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCApp.Models;

namespace MVCApp.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Person/

        public ActionResult Index()
        {
            var model =
                from p in _persons
                orderby p.Street
                select p;

            return View(_persons);
        }

        //
        // GET: /Person/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Person/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Person/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Person/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Person/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Person/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Person/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        static DistrictModel _dis = new DistrictModel
        {
            Id = "1",
            Name = "Ballerup",
            PostCode = "2750"
        };

        static List<PersonModelTest> _persons = new List<PersonModelTest> {
            new PersonModelTest {
               District = _dis,
               FirstName = "Bartek",
               LastName = " Gasior",
               Street = "Magleparken",
               StreetNo = "12",
               Door = "st. P",
               PostCode = 2750,
               Tel = 52527568,             
            },
            new PersonModelTest {
               District = _dis,
               FirstName = "Maciek",
               LastName = " Gasior",
               Street = "Magleparken",
               StreetNo = "12",
               Door = "st. P",
               PostCode = 2750,
               Tel = 52527568,             
            }
        };
    }
}
