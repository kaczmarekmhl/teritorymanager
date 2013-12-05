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
using System.Data.Entity;
using System.Net;

namespace MVCApp.Controllers
{
    public class DistrictController : Controller
    {
        TerritoryDb _db = new TerritoryDb();

        //
        // GET: /District/
       
        public ActionResult Index()
        {
            // Populate search district menu with users's districts
            PopulateSearchDistrictMenu("Bartek");
            return View();
        }

        //
        // Get: /District/Search

        [HttpGet]
        [ValidateDistrictBelongsToUser]
        public ActionResult Search(string UserDistrict, bool DisplayDeletedPersons)
        {
            // Populate search district menu with users's districts
            PopulateSearchDistrictMenu("Bartek", UserDistrict, DisplayDeletedPersons);
            // Retrieve search district data and persons belonging to the district from DB
            var searchUserDistrict = _db.Districts.Include("PersonsFoundInDistrict").Single(dist => dist.Id == UserDistrict);
            var personList = searchUserDistrict.PersonsFoundInDistrict;
            // If no persons data in DB for a given district ID, downolad data from krak.dk
            if (personList.Count == 0)
            {
                var addressProvider = new AddressProvider();
                var personListFromKrak = addressProvider.getPersonList(int.Parse(searchUserDistrict.PostCode));
                var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                new NonPolishSurnameNonExactName(),
                new ScandinavianSurname()
                };
                FilterManager.FilterPersonList(personListFromKrak, filterList);
                personList = personListFromKrak.Select(p => new PersonModel(p, searchUserDistrict)).ToList();
                // Store person list in DB before filtering it out
                _db.Persons.AddRange(personList);
                _db.SaveChanges();
            }
            if (DisplayDeletedPersons)
            {
                return View("RestorePersons", personList.Where(p => p.RemovedByUser == true));
            }
            return View("DeletePersons", personList.Where(p => p.RemovedByUser == false));
        }

        private void PopulateSearchDistrictMenu(string user, string selectedDistrictId = null, bool DisplayOnlyRemovedPersons = false)
        {
            var userDistrictsFromDb = from dist in _db.Districts
                                      orderby dist.PostCode
                                      where dist.BelongsToUser == user
                                      select dist;
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var district in userDistrictsFromDb)
            {
                items.Add(
                    district.Id.Equals(selectedDistrictId)
                    ? new SelectListItem { Text = string.Format("{0} {1}", district.PostCode, district.Name), Value = district.Id, Selected = true }
                    : new SelectListItem { Text = string.Format("{0} {1}", district.PostCode, district.Name), Value = district.Id }
                    );
            }
            ViewBag.UserDistrict = items;
            ViewBag.DisplayOnlyRemovedPersons = DisplayOnlyRemovedPersons;
        }

        //
        // POST: /District/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePersons(int[] selectedPersons)
        {
            if (selectedPersons == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userDistrictId = UpdateSelectedPersonsInDb(selectedPersons, true);
            return RedirectToAction("Search", "District", new { UserDistrict = userDistrictId, DisplayDeletedPersons = false });
        }

        private string UpdateSelectedPersonsInDb(int[] selectedPersons, bool removedByUser)
        {
            var personsToDelete = _db.Persons.Include("District").Where(p => selectedPersons.Contains(p.ID)).ToList();
            personsToDelete.ForEach(p => p.RemovedByUser = removedByUser);
            _db.SaveChanges();            
            return personsToDelete.First().District.Id;
        }

        //
        // POST: /District/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestorePersons(int[] selectedPersons)
        {
            if (selectedPersons == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userDistrictId = UpdateSelectedPersonsInDb(selectedPersons, false);
            return RedirectToAction("Search", "District", new { UserDistrict = userDistrictId, DisplayDeletedPersons = true });
        }

        //
        // GET: /District/Edit/5

        public ActionResult Edit(int id)
        {
            var person = from p in _db.Persons
                         where p.ID == id
                         select p;
            return View(person);
        }

        //
        // POST: /District/Edit/5

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
