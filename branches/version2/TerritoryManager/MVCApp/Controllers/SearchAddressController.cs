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

namespace MVCApp.Controllers
{
    [Authorize]
    public class SearchAddressController : Controller
    {
        #region IndexAction

        public ActionResult Index(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            ViewBag.DistrictId = district.Id;
            ViewBag.DistrictName = district.Name;            

            var personList = GetPersonListFromSession(district)
                .OrderBy(p => p.Name)
                .ToPagedList(1, 10);

            return View(personList);
        }

        #endregion

        #region GetPersonListAction

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult GetPersonList(int id, int page = 1)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpNotFoundResult();
            }

            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            var personList = GetPersonList(district)
                .OrderBy(p => p.Name)
                .ToPagedList(page, 10);

            return PartialView("_PersonList", personList);
        }

        /// <summary>
        /// Returns person list for the given district.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<Person> GetPersonList(District district)
        {
            List<Person> personList = new List<Person>();

            personList = GetPersonListFromSession(district);

            if (personList.Count == 0)
            {
                personList = GetPersonListFromKrak(district);
                PersistPersonListInSession(district, personList);
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
            var addressProvider = new AddressProvider();
            var personListFromKrak = addressProvider.getPersonList(district.PostCodeFirst, district.PostCodeLast);

            var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                    new ScandinavianSurname()
                };
            FilterManager.FilterPersonList(personListFromKrak, filterList);

            return personListFromKrak.Select(p => new Person(p, district)).ToList();
        }

        /// <summary>
        /// Loads person list from session.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns></returns>
        private List<Person> GetPersonListFromSession(District district)
        {
            string sessionKey = string.Format("PersonList_{0}", district.Id);

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
        private void PersistPersonListInSession(District district, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                Session[GetPersonListSessionKey(district)] = personList;
            }
        }

        /// <summary>
        /// Returns session key that will be used to persist person list.
        /// </summary>
        /// <param name="district">District that will be used by the session key.</param>
        /// <returns>Session key.</returns>
        private string GetPersonListSessionKey(District district)
        {
            return string.Format("PersonList_{0}", district.Id);
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
