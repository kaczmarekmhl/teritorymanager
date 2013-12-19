using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Filters;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;
using AddressSearch.AdressProvider.Filters.PersonFilter.Helpers;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    [Authorize]
    public class SearchAddressController : Controller
    {
        const int personListPageSize = 100;

        #region IndexAction

        public ActionResult Index(int id, int page = 1)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            ViewBag.DistrictId = district.Id;
            ViewBag.DistrictName = district.Name;

            var personList = GetPersistedPersonList(district.Id, page);                

            return View(personList);
        }

        #endregion

        #region GetPersonListAction

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult GetPersonList(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            var personList = GetPersonList(district);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_PersonList", personList);
            }
            else
            {
                return RedirectToAction("Index", new { id = id });
            }
        }

        /// <summary>
        /// Returns person list for the given district.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        private IPagedList<Person> GetPersonList(District district)
        {
            var personList = GetPersistedPersonList(district.Id);

            if (personList.Count == 0)
            {
                var personListKrak = GetPersonListFromKrak(district);
                PreliminarySelection(personListKrak);
                PersistPersonList(district.Id, personListKrak);

            }

            personList = GetPersistedPersonList(district.Id);

            return personList;
        }

        /// <summary>
        /// Preliminary select people.
        /// </summary>
        /// <param name="personList">List with person models.</param>
        /// <returns>Preliminary selected list of person models.</returns>
        private List<Person> PreliminarySelection(List<Person> personList)
        {
            var polishSurnameRecogniser = new PolishSurnameRecogniser();

            foreach (var person in personList)
            {
                // If person has polish surname select it
                if (polishSurnameRecogniser.IsPolish(person.Name, person.Lastname) == true)
                {
                    person.Selected = true;
                }
            }

            return personList;
        }

        /// <summary>
        /// Loads person list from Krak website.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<Person> GetPersonListFromKrak(District district)
        {
            var personList = new List<Person>();
            var addressProvider = new AddressProvider();
            var personListFromKrak = addressProvider.getPersonList(district.PostCodeFirst, district.PostCodeLast);

            // Filtering
            var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                    new ScandinavianSurname()
                };
            FilterManager.FilterPersonList(personListFromKrak, filterList);

            // Conversion to model
            personList = personListFromKrak.Select(p => new Person(p, district)).ToList();         

            return personList;
        }

        /// <summary>
        /// Loads person list from session.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns></returns>
        private IPagedList<Person> GetPersistedPersonList(int districtId, int page = 1)
        {
            return db.Persons
                .Where(p => p.District.Id == districtId && p.AddedByUserId == WebSecurity.CurrentUserId)
                .OrderBy(p => p.Name)
                .ToPagedList(page, personListPageSize);
        }

        /// <summary>
        /// Persists person list in Session.
        /// </summary>
        /// <param name="district">District for which person list will be persisted.</param>
        /// <param name="personList">Person list to persist.</param>
        private void PersistPersonList(int districtId, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                db.Persons.AddRange(personList);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns session key that will be used to persist person list.
        /// </summary>
        /// <param name="districtId">District that will be used by the session key.</param>
        /// <returns>Session key.</returns>
        private string GetPersonListSessionKey(int districtId)
        {
            return string.Format("PersonList_{0}", districtId);
        }

        #endregion

        #region SelectPersonAction

        [HttpPost]
        public ActionResult SelectPerson(int districtId, int personId, bool selected)
        {
            Person person = db.Persons.Find(personId);

            if (person == null)
            {
                return new HttpNotFoundResult();
            }

            person.Selected = selected;
            db.SaveChanges();

            return new JsonResult()
            {
                Data = new { selected = person.Selected }
            };
        }
        #endregion

        #region Database Access

        DistictManagerDb db;

        public SearchAddressController()
        {
            db = new DistictManagerDb();
        }

        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
