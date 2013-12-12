using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCApp.Models;
using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Filters;
using System.Collections;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using MVCApp.Filters;
using System.Data.Entity;
using System.Net;
using WebMatrix.WebData;
using MVCApp.Exceptions;

namespace MVCApp.Controllers
{
    [Authorize]
    public class PersonsController : Controller
    {
        ITerritoryDb _db;

        // The constructor will execute when the app is run on a web server with SQL Server
        public PersonsController()
        {
            _db = new TerritoryDb();
        }

        // The constructor used for unit tests to fake a db
        public PersonsController(ITerritoryDb db)
        {
            _db = db;
        }


        // It is only assessible as a child action
        [ChildActionOnly]
        public PartialViewResult Search(string selectedDistrictId, bool displayOnlyDeletedPersons = false)
        {
            District selectedDistrict;
            List<Person> personList;
            GetDistrictFromDb(selectedDistrictId, out selectedDistrict, out personList);

            // If no persons data in DB for a given district ID, downolad data from krak.dk
            if (personList.Count == 0)
            {
                personList = GetPersonListFromKrak(selectedDistrict, personList);

                // Store person list in DB 
                _db.AddRange<Person>(personList);
                _db.SaveChanges();
            }

            personList.RemoveAll(p => p.RemovedByUser == !displayOnlyDeletedPersons);

            if (displayOnlyDeletedPersons)
            {

                return PartialView("_Restore", personList);
            }
            else
            {
                return PartialView("_Delete", personList);
            }
        }

        //
        // POST: /Persons/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]      
        public ActionResult Delete(int[] selectedPersons)
        {
            if (selectedPersons == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userDistrictId = UpdateSelectedPersonsInDb(selectedPersons, true);
            return RedirectToAction("Search", "District", new { DistrictSelectListItem = userDistrictId, displayOnlyDeletedPersons = false });
        }

        //
        // POST: /Persons/Restore/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Restore(int[] selectedPersons)
        {
            if (selectedPersons == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userDistrictId = UpdateSelectedPersonsInDb(selectedPersons, false);
            return RedirectToAction("Search", "District", new { DistrictSelectListItem = userDistrictId, displayOnlyDeletedPersons = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Map(int[] selectedPersons)
        {
            return View();
        }

        
        private List<Person> GetPersonListFromKrak(District selectedDistrict, List<Person> personList)
        {
            var addressProvider = new AddressProvider();
            var personListFromKrak = addressProvider.getPersonList(int.Parse(selectedDistrict.PostCode));

            var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                    new NonPolishSurnameNonExactName(),
                    new ScandinavianSurname()
                };
            FilterManager.FilterPersonList(personListFromKrak, filterList);

            personList = personListFromKrak.Select(p => new Person(p, selectedDistrict)).ToList();
            return personList;
        }

        private void GetDistrictFromDb(string selectedDistrictId, out District selectedDistrict, out List<Person> personList)
        {
            // Retrieve search district data and persons belonging to the district from DB 
            selectedDistrict = _db.Query<District>()
                .Include("PersonsFoundInDistrict")
                .Single(dist => dist.Id == selectedDistrictId);

            personList = null;
            if (selectedDistrict.PersonsFoundInDistrict != null)
            {
                personList = selectedDistrict.PersonsFoundInDistrict.ToList();
            }
        }

        private string UpdateSelectedPersonsInDb(int[] selectedPersons, bool removedByUser)
        {
            var personsToDelete = _db.Query<Person>().Include("District").Where(p => selectedPersons.Contains(p.Id)).ToList();
            personsToDelete.ForEach(p => p.RemovedByUser = removedByUser);
            _db.SaveChanges();
            return personsToDelete.First().District.Id;
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
