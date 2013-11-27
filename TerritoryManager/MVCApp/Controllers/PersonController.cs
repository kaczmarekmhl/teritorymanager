using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCApp.Models;
using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Entities;
using AddressSearch.AdressProvider.Filters;

namespace MVCApp.Controllers
{
    public class PersonController : Controller
    {
        //
        // GET: /Person/

        public ActionResult Index()
        {
            var personSearchList = new List<SearchName>();

            personSearchList.Add(new SearchName
            {
                Name = "tomasz",
                IsMale = true
            });

            var addressProvider = new KrakAddressProvider();

            List<Person> resultList = addressProvider.getPersonList(2100, personSearchList);

            var filterList = new List<AddressSearch.AdressProvider.Filters.IResultFilter> {
                new SurnameResultFilter() 
            };

            FilterManager.ApplyFilter(resultList, filterList);

            return View(resultList);
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
    }
}
