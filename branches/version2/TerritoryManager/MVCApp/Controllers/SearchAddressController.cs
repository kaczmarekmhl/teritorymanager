using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Filters;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using AddressSearch.AdressProvider.Filters.PersonFilter.Helpers;
using AddressSearch.AdressProvider.SearchStrategies;
using SearchEntities = AddressSearch.AdressProvider.Entities;
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
        public ActionResult SearchOnKrak(int id)
        {
            HttpContext.Server.ScriptTimeout = 30 * 60;

            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }
                        
            var personList =
                GetPersonList(district)
                .OrderByDescending(p => p.NewPersonUpdate)
                .ThenBy(p => p.Name)
                .ToPagedList(1, personListPageSize);

            if (Request.IsAjaxRequest())
            {
                ViewBag.DistrictId = district.Id;

                if (personList.Count == 0)
                {
                    ViewBag.NoPersonFound = true;

                    // Load previous results
                    personList = GetPersistedPersonListPaged(district.Id, 1, true);
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

            person.Selected = selected;
            db.SaveChanges();

            return new JsonResult()
            {
                Data = new { selected = person.Selected }
            };
        }
        #endregion
        
        #region Helpers

        /// <summary>
        /// Returns person list for the given district.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<Person> GetPersonList(District district)
        {
            List<Person> previousPersonList = new List<Person>();

            var personList = GetPersonListFromSearchEngine(district, out previousPersonList);
            personList = PreliminarySelection(personList);
            PersistPersonList(district.Id, personList);

            return personList;
        }
                
        /// <summary>
        /// Loads person list from search engine.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<Person> GetPersonListFromSearchEngine(District district, out List<Person> previousPersonList)
        {
            var addressProvider = GetAddressProviderForDistrict(district);
            var personListFromSearch = !String.IsNullOrEmpty(district.SearchPhrase) ? addressProvider.getPersonList(district.SearchPhrase) : addressProvider.getPersonList(district.PostCodeFirst, district.PostCodeLast);

            personListFromSearch = FilterPersonListFromSearchEngine(district, personListFromSearch);

            previousPersonList = GetPersistedPersonListQuery(district.Id, null).ToList();

            //List can be updated
            if (previousPersonList.Count > 0)
            {
                var outdatedPersonList = previousPersonList.Select(p => new SearchEntities.Person()
                    {
                        Name = p.Name,
                        Lastname = p.Lastname,
                        PostCode = p.PostCode,
                        StreetAddress = p.StreetAddress
                    }).ToList();

                var newPersonList = new List<SearchEntities.Person>();
                var removedPersonList = new List<SearchEntities.Person>();

                addressProvider.GetDifferenceOfUpdatedPersonList(personListFromSearch, outdatedPersonList, out newPersonList, out removedPersonList);

                previousPersonList.ForEach(p => p.NewPersonUpdate = false);

                // Conversion to model
                return newPersonList.Select(p => new Person(p, district)).ToList();
            }

            // Conversion to model
            return personListFromSearch.Select(p => new Person(p, district)).ToList();
        }

        /// <summary>
        /// Filters out people received from search engine.
        /// </summary>
        /// <param name="district">District.</param>
        /// <param name="personListFromSearch">List with person models.</param>
        /// <returns></returns>
        private List<SearchEntities.Person> FilterPersonListFromSearchEngine(District district, List<SearchEntities.Person> personListFromSearch)
        {
            var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                    new ScandinavianSurname()
                };
            FilterManager.FilterPersonList(personListFromSearch, filterList);

            personListFromSearch = FilterPeopleOutsideBoundary(personListFromSearch, district);

            return personListFromSearch;
        }

        /// <summary>
        /// Filters out people outside district boundary.
        /// </summary>
        /// <param name="personList">List with person models.</param>
        /// <param name="district">District</param>
        /// <returns>List with people inside district boundary.</returns>
        private List<SearchEntities.Person> FilterPeopleOutsideBoundary(List<SearchEntities.Person> personList, District district)
        {
            if (String.IsNullOrEmpty(district.DistrictBoundaryKml) || !IsDistrictPartial(district))
            {
                return personList;
            }

            var resultList = new List<AddressSearch.AdressProvider.Entities.Person>();
            var kmlDoc = new KmlDocument(district.DistrictBoundaryKml);

            foreach (var person in personList)
            {
                if (kmlDoc.IsPointInsideBounday(person.Longitude, person.Latitude))
                {
                    resultList.Add(person);
                }
            }

            return resultList;
        }

        /// <summary>
        /// Deletes people for given district.
        /// </summary>
        /// <param name="districtId">District id that the delete will be done for.</param>
        private void MarkPeopleAsOldSearch(int districtId)
        {
            //Entity framework does not support deleting data through direct SQL
            //We need to do it due to performance reasons
            string sqlDeleteStatement = "Update People SET NewPersonUpdate = 0 WHERE District_id = @districtId AND AddedByUserId = @userId AND Manual = 0";

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@districtId", districtId));
            parameterList.Add(new SqlParameter("@userId", WebSecurity.CurrentUserId));

            db.Database.ExecuteSqlCommand(sqlDeleteStatement, parameterList.ToArray());
            db.SaveChanges();
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
        /// Loads persisted person list.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns></returns>
        private IPagedList<Person> GetPersistedPersonListPaged(int districtId, int page = 1, bool? newPersonUpdate = true)
        {
            return GetPersistedPersonListQuery(districtId, newPersonUpdate).ToPagedList(page, personListPageSize);
        }

        /// <summary>
        /// Loads persisted person list.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        private IQueryable<Person> GetPersistedPersonListQuery(int districtId, bool? newPersonUpdate = true)
        {
            return db.Persons
                .Where(p => 
                    p.District.Id == districtId && 
                    p.AddedByUserId == WebSecurity.CurrentUserId && 
                    p.Manual == false &&
                    (p.NewPersonUpdate == newPersonUpdate || newPersonUpdate == null))
                    .OrderBy(p => p.Name);
        }

        /// <summary>
        /// Persists person list.
        /// </summary>
        /// <param name="district">District for which person list will be persisted.</param>
        /// <param name="personList">Person list to persist.</param>
        private void PersistPersonList(int districtId, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                MarkPeopleAsOldSearch(districtId);

                //Mark as new update
                personList.ForEach(p => p.NewPersonUpdate = true);

                db.Persons.AddRange(personList);

                foreach (var validationResults in db.GetValidationErrors())
                {
                    if (!validationResults.IsValid)
                    {
                        var invalidEntity = (Person)validationResults.Entry.Entity;

                        db.Persons.Remove(invalidEntity);
                        personList.Remove(invalidEntity);
                    }
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Checks if given district is partial.
        /// </summary>
        /// <param name="district">District that should ne checked</param>
        /// <returns>If given district is partial.</returns>
        private bool IsDistrictPartial(District district)
        {
            return true;
            /*return db.Districts
                .Count(d => d.Id != district.Id
                    &&( d.PostCodeFirst == district.PostCodeFirst
                    || d.PostCodeLast == district.PostCodeFirst
                    || d.PostCodeFirst == district.PostCodeLast
                    || d.PostCodeLast == district.PostCodeLast))
                    > 0;*/
        }

        /// <summary>
        /// Returns AddressProvider class for given district based on its country
        /// </summary>
        /// <param name="district">District</param>
        /// <returns>AddressProvider class</returns>
        private AddressProvider GetAddressProviderForDistrict(District district)
        {
            ISearchStrategy searchStrategy;

            switch (district.Congregation.Country)
            {
                case Enums.Country.Denmark:
                    searchStrategy = new KrakDkSearchStrategy();
                    break;
                case Enums.Country.Norway:
                    searchStrategy = new GuleSiderNoSearchStrategy();
                    break;
                default:
                    throw new Exception("Address provider cannot be returned for given country");
            }

            return new AddressProvider(searchStrategy);
        }

        #endregion
               
    }
}
