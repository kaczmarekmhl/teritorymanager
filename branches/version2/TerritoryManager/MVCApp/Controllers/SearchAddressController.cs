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

            var personList = GetPersonListFromSession(district.Id)
                .OrderBy(p => p.Name)
                .ToPagedList(page, personListPageSize);

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

            var personList = GetPersonList(district)
                .OrderBy(p => p.Name)
                .ToPagedList(1, personListPageSize);

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
        private List<Person> GetPersonList(District district)
        {
            List<Person> personList = new List<Person>();

            personList = GetPersonListFromSession(district.Id);

            if (personList.Count == 0)
            {
                personList = GetPersonListFromKrak(district);
                PersistPersonListInSession(district.Id, personList);
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
            int id = 1;

            // Filtering
            var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                    new ScandinavianSurname()
                };
            FilterManager.FilterPersonList(personListFromKrak, filterList);

            // Conversion to model
            personList = personListFromKrak.Select(p => new Person(id++, p, district)).ToList();

            // Preliminary selection
            var polishSurnameRecogniser = new PolishSurnameRecogniser();

            foreach (var person in personList)
            {
                // If person has polish surname select it
                if (polishSurnameRecogniser.ContainsPolishSurname(person.Lastname) == true)
                {
                    person.Selected = true;
                }
            }

            return personList;
        }

        /// <summary>
        /// Loads person list from session.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns></returns>
        private List<Person> GetPersonListFromSession(int districtId)
        {
            string sessionKey = string.Format("PersonList_{0}", districtId);

            if (Session[sessionKey] != null)
            {
                return (List<Person>)Session[sessionKey];
            }

            return new List<Person>();
        }

        /// <summary>
        /// Persists person list in Session.
        /// </summary>
        /// <param name="district">District for which person list will be persisted.</param>
        /// <param name="personList">Person list to persist.</param>
        private void PersistPersonListInSession(int districtId, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                Session[GetPersonListSessionKey(districtId)] = personList;
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
            List<Person> personList = GetPersonListFromSession(districtId);
            Person person = personList.Single(p => p.Id == personId);

            if (person == null)
            {
                return new HttpNotFoundResult();
            }

            person.Selected = selected;
            PersistPersonListInSession(districtId, personList);

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
