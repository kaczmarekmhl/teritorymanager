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
            var territory = db.Territories.Find(id);

            if (territory == null)
            {
                return new HttpNotFoundResult();
            }

            ViewBag.TerritoryId = territory.Id;
            ViewBag.TerritoryName = territory.Name;            

            var personList = GetPersonListFromSession(territory)
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

            var territory = db.Territories.Find(id);

            if (territory == null)
            {
                return new HttpNotFoundResult();
            }

            var personList = GetPersonList(territory)
                .OrderBy(p => p.Name)
                .ToPagedList(page, 10);

            return PartialView("_PersonList", personList);
        }

        /// <summary>
        /// Returns person list for the given territory.
        /// </summary>
        /// <param name="territory">Territory that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<Person> GetPersonList(Territory territory)
        {
            List<Person> personList = new List<Person>();

            personList = GetPersonListFromSession(territory);

            if (personList.Count == 0)
            {
                personList = GetPersonListFromKrak(territory);
                PersistPersonListInSession(territory, personList);
            }

            return personList;
        }

        /// <summary>
        /// Loads person list from Krak website.
        /// </summary>
        /// <param name="territory">Territory that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<Person> GetPersonListFromKrak(Territory territory)
        {
            var addressProvider = new AddressProvider();
            var personListFromKrak = addressProvider.getPersonList(territory.PostCodeFirst, territory.PostCodeLast);

            var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                    new ScandinavianSurname()
                };
            FilterManager.FilterPersonList(personListFromKrak, filterList);

            return personListFromKrak.Select(p => new Person(p, territory)).ToList();
        }

        /// <summary>
        /// Loads person list from session.
        /// </summary>
        /// <param name="territory">Territory that the search will be done for.</param>
        /// <returns></returns>
        private List<Person> GetPersonListFromSession(Territory territory)
        {
            string sessionKey = string.Format("PersonList_{0}", territory.Id);

            if (Session[sessionKey] != null)
            {
                return (List<Person>)Session[sessionKey];
            }

            return new List<Person>();
        }

        /// <summary>
        /// Persists person list in Session.
        /// </summary>
        /// <param name="territory">Territory for which person list will be persisted.</param>
        /// <param name="personList">Person list to persist.</param>
        private void PersistPersonListInSession(Territory territory, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                Session[GetPersonListSessionKey(territory)] = personList;
            }
        }

        /// <summary>
        /// Returns session key that will be used to persist person list.
        /// </summary>
        /// <param name="territory">Territory that will be used by the session key.</param>
        /// <returns>Session key.</returns>
        private string GetPersonListSessionKey(Territory territory)
        {
            return string.Format("PersonList_{0}", territory.Id);
        }

        #endregion

        #region Database Access

        TerritoryDb db;

        public SearchAddressController()
        {
            db = new TerritoryDb();
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
