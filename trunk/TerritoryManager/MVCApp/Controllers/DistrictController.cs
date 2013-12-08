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
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    [Authorize]
    public class DistrictController : Controller
    {
        ITerritoryDb _db;

        // The constructor will execute when the app is run on a web server
        // with SQL Server
        public DistrictController()
        {
            _db = new TerritoryDb();
        }

        // The constructor used for unit tests to fake a db
        public DistrictController(ITerritoryDb db)
        {
            _db = db;
        }

        //
        // GET: /District/

        public ActionResult Index()
        {
            // Populate search district menu with users's districts
            PopulateSearchDistrictMenu();
            return View();
        }

        //
        // Get: /District/Search

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string UserDistrict, bool DisplayDeletedPersons)
        {
            // Populate search district menu with users's districts
            PopulateSearchDistrictMenu(UserDistrict, DisplayDeletedPersons);
            // Retrieve search district data and persons belonging to the district from DB
            var searchUserDistrict = _db.Query<DistrictModel>().Include("PersonsFoundInDistrict").Single(dist => dist.Id == UserDistrict);
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
                _db.AddRange<PersonModel>(personList);
                _db.SaveChanges();
            }
            if (DisplayDeletedPersons)
            {
                return View("RestorePersons", personList.Where(p => p.RemovedByUser == true));
            }
            return View("DeletePersons", personList.Where(p => p.RemovedByUser == false));
        }

        private void PopulateSearchDistrictMenu(string selectedDistrictId = null, bool DisplayOnlyRemovedPersons = false) { 
            bool allDistricts = User.IsInRole("Admin");
            if (allDistricts)
            {
                PopulateSearchDistrictMenu(selectedDistrictId, DisplayOnlyRemovedPersons, allDistricts, null);
            }
            else
            {
                PopulateSearchDistrictMenu(selectedDistrictId, DisplayOnlyRemovedPersons, allDistricts);
            }
        }

        private void PopulateSearchDistrictMenu(string selectedDistrictId, bool DisplayOnlyRemovedPersons, bool allDistricts)
        {
            string userName = User.Identity.Name;
            PopulateSearchDistrictMenu(selectedDistrictId, DisplayOnlyRemovedPersons, allDistricts, userName);
        }

        public void PopulateSearchDistrictMenu(string selectedDistrictId, bool DisplayOnlyRemovedPersons, bool allDistricts, string userName)
        {
            IEnumerable<DistrictModel> userDistrictsFromDb;
            if (!allDistricts)
            {       
                userDistrictsFromDb = _db.Query<DistrictModel>().Where(d =>
                    d.BelongsToUser != null &&
                    d.BelongsToUser.UserName == userName);
            }
            else
            {
                userDistrictsFromDb = _db.Query<DistrictModel>();
            }

            if (userDistrictsFromDb == null || userDistrictsFromDb.Count() == 0)
            {
                ViewBag.ErrorMsg = "You don't have any district assigned to you.";
                return;
            }
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var district in userDistrictsFromDb.OrderBy( d => d.PostCode))
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
            return Search(UserDistrict: userDistrictId, DisplayDeletedPersons: false);
        }

        private string UpdateSelectedPersonsInDb(int[] selectedPersons, bool removedByUser)
        {
            var personsToDelete = _db.Query<PersonModel>().Include("District").Where(p => selectedPersons.Contains(p.Id)).ToList();
            personsToDelete.ForEach(p => p.RemovedByUser = removedByUser);
            _db.SaveChanges();
            return personsToDelete.First().District.Id;
        }

        //
        // POST: /District/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MinItemsSelected(1)]
        public ActionResult RestorePersons(int[] selectedPersons)
        {
            if (selectedPersons == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userDistrictId = UpdateSelectedPersonsInDb(selectedPersons, false);
            return Search(UserDistrict: userDistrictId, DisplayDeletedPersons: true);
        }

        //
        // GET: /District/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var person = _db.Query<PersonModel>().Single(p => p.Id == id);             
            return View(person);
        }

        //
        // POST: /District/Edit/5

        [HttpPost]
        public ActionResult Edit(PersonModel person)
        {
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
