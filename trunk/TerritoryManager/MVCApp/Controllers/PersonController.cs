using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCApp.Models;
using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Entities;
using AddressSearch.AdressProvider.Filters;
using System.Collections;
using AddressSearch.AdressProvider.Filters.PersonFilter;

namespace MVCApp.Controllers
{
    public class PersonController : Controller
    {
        TerritoryDb _db = new TerritoryDb();

        //
        // GET: /Person/

        public ActionResult Index()
        {
            // Retrieve persons data from DB
            var personList = (from person in _db.Persons
                                where !person.RemovedByUser & person.RemovedByUser != null
                                select person).ToList();
          
            // If no persons data in DB for a given district ID, downolad data from krak.dk
            if (personList.Count == 0)
            {
                var addressProvider = new AddressProvider();
                var personListFromKrak = addressProvider.getPersonList(2100);
                var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                new NonPolishSurname(),
                new NonPolishSurnameNonExactName(),
                new ScandinavianSurname()
            };

                FilterManager.FilterPersonList(personListFromKrak, filterList);
                personList = personListFromKrak.Select(p => new PersonModel(p)).ToList();
            }
            return View(personList);
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
            var person = from p in _db.Persons 
                         where p.ID == id
                         select p;
            return View(person);
        }

        //
        // POST: /Person/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var person = from p in _db.Persons
                         where p.ID == id
                         select p;

            if (TryUpdateModel(person))
            {
                return RedirectToAction("Index");
            }
            return View(person);
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

        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
