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
using MVCApp.Filters;

namespace MVCApp.Controllers
{
    public class DistrictController : Controller
    {
        TerritoryDb _db = new TerritoryDb();

        //
        // GET: /Person/
        [ValidateDistrictBelongsToUser]
        public ActionResult Index(string searchDistrictID = null)
        {
            var userDistricts = from dist in _db.Districts
                                    orderby dist.PostCode
                                    where dist.BelongsToUser == "Bartek"
                                    select dist;
            var districtViewModel = new DistrictViewModel
            {
                PersonDistrictList = userDistricts.ToList()
            };        
            if (searchDistrictID == null)
            {
                return View("_SearchDistrictPartial", districtViewModel.PersonDistrictList);
            }

            var searchUserDistrict = userDistricts.Single(dist => dist.Id == searchDistrictID);
            // Retrieve persons data from DB
            var personList = (from person in _db.Persons
                              where !person.RemovedByUser 
                              where person.RemovedByUser != null
                              where person.District.Id == searchUserDistrict.Id
                              select person).ToList();

            // If no persons data in DB for a given district ID, downolad data from krak.dk
            if (personList.Count == 0)
            {
                var addressProvider = new AddressProvider();
                var personListFromKrak = addressProvider.getPersonList(int.Parse(searchUserDistrict.PostCode));

                // Store person list in DB before filtering it out
                _db.Persons.AddRange(personListFromKrak.Select(p => new PersonModel(p, searchUserDistrict)).ToList());
                _db.SaveChanges();

                var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                new NonPolishSurnameNonExactName(),
                new ScandinavianSurname()
            };
                FilterManager.FilterPersonList(personListFromKrak, filterList);
                personList = personListFromKrak.Select(p => new PersonModel(p, searchUserDistrict)).ToList();
            }
            districtViewModel.PersonList = personList;
            districtViewModel.SearchForDistrict = userDistricts.Single(dist => dist.PostCode == searchUserDistrict.PostCode);
            return View(districtViewModel);
        }

        //
        // GET: /Person/Details/5

        public ActionResult Details(int id)
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
