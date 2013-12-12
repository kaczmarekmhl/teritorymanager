using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace MVCApp.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        ITerritoryDb _db;

        // The constructor will execute when the app is run on a web server with SQL Server
        public PersonController()
        {
            _db = new TerritoryDb();
        }

        // The constructor used for unit tests to fake a db
        public PersonController(ITerritoryDb db)
        {
            _db = db;
        }
        //
        // GET: /Persons/Edit/5

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var person = _db.Query<Person>().Include("District").Single(p => p.Id == id);
            return View(person);
        }


        //
        // POST: /Persons/Edit/5

        [HttpPost]
        public ActionResult Edit(Person person)
        {
            if (TryUpdateModel(person))
            {
                var personFromDb = _db.Query<Person>().Include("District").Single(p => p.Id == person.Id);
                personFromDb.Notes = person.Notes;
                _db.SaveChanges();
                return RedirectToAction("Search", "District", new { DistrictSelectListItem = personFromDb.District.Id, displayOnlyDeletedPersons = false });
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
