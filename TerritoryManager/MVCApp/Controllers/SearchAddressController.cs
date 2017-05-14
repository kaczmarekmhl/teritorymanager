using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Filters;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using AddressSearch.AdressProvider.Filters.PersonFilter.Helpers;
using AddressSearch.AdressProvider.SearchStrategies;
using SearchEntities = AddressSearchComon.Data;
using MapLibrary;
using MVCApp.Models;
using Novacode;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using MVCApp.Helpers;

namespace MVCApp.Controllers
{
    [Authorize]
    public class SearchAddressController : BaseController
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
            ViewBag.NewResults = true;
            ViewBag.PersonListOutdated = district.IsPersonListOutdated();

            var personList = GetPersistedPersonListPaged(district.Id, page, true);

            return View(personList);
        }

        #endregion

        #region PreviousSearchAction

        public ActionResult PreviousSearch(int id, int page = 1)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            ViewBag.DistrictId = district.Id;
            ViewBag.DistrictName = district.Name;
            ViewBag.NewResults = false;

            var personList = GetPersistedPersonListPaged(district.Id, page, false);

            return View(personList);
        }

        #endregion

        #region SearchOnKrakAction

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult GetAddressList(int id, bool addressFound)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }   

            if (Request.IsAjaxRequest())
            {
                ViewBag.DistrictId = district.Id;

                var personList = GetPersistedPersonListPaged(district.Id, 1, true);

                if (addressFound == false)
                {
                    ViewBag.NoPersonFound = true;
                }
                else
                {
                    ViewBag.SearchComplete = true;
                }

                ViewBag.NewResults = true;

                return PartialView("_PersonList", personList);
            }
            else
            {
                return RedirectToAction("Index", new { id = id });
            }
            
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

            if ((person.DoNotVisit || person.IsVisitedByOtherPublisher) && selected == false)
            {
                // Deselecting person that should not be visited or is visited by another publisher is not valid
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

        #region DeleteSearch

        public ActionResult DeleteSearch(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            //Entity framework does not support deleting data through direct SQL
            //We need to do it due to performance reasons
            string sqlDeleteStatement;
            object[] parameterList;

            if (IsSharingAdressesEnabled)
            {
                sqlDeleteStatement = "Delete from People WHERE District_id = @districtId AND Manual = 0 AND DoNotVisit = 0";
                parameterList = new object[1];
            }
            else
            {
                sqlDeleteStatement = "Delete People WHERE District_id = @districtId AND AddedByUserId = @userId AND Manual = 0 AND DoNotVisit = 0";
                parameterList = new object[2];
                parameterList[1] = new SqlParameter("@userId", WebSecurity.CurrentUserId);
            }

            parameterList[0] = new SqlParameter("@districtId", district.Id);

            db.Database.ExecuteSqlCommand(sqlDeleteStatement, parameterList);
            db.SaveChanges();

            return RedirectToAction("Index", new { id = id });
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads persisted person list.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns></returns>
        private IPagedList<Person> GetPersistedPersonListPaged(int districtId, int page = 1, bool? searchUpdate = true)
        {
            var search = new SearchAddress(db);
            
            return search.GetPersistedPersonListQuery(districtId, searchUpdate).ToPagedList(page, personListPageSize);
            
        }                     

        #endregion
               
    }
}
