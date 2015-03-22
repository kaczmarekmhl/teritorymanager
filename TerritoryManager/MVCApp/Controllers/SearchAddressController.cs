using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Filters;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using AddressSearch.AdressProvider.Filters.PersonFilter.Helpers;
using AddressSearch.AdressProvider.SearchStrategies;
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

            var personList = GetPersistedPersonList(district.Id, page);

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
                SearchAddressOnKrak(district)
                .OrderBy(p => p.Name)
                .ToPagedList(1, personListPageSize);

            if (Request.IsAjaxRequest())
            {
                ViewBag.DistrictId = district.Id;

                if (personList.Count == 0)
                {
                    ViewBag.NoPersonFound = true;
                }
                else
                {
                    ViewBag.SearchComplete = true;
                }

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
        private List<Person> SearchAddressOnKrak(District district)
        {
            var personList = GetPersonListFromKrak(district);
            personList = PreliminarySelection(personList);
            PersistPersonList(district.Id, personList);

            return personList;
        }

        /// <summary>
        /// Deletes people for given district.
        /// </summary>
        /// <param name="districtId">District id that the delete will be done for.</param>
        private void DeletePeopleInDistrict(int districtId)
        {
            //Entity framework does not support deleting data through direct SQL
            //We need to do it due to performance reasons
            string sqlDeleteStatement = "DELETE FROM People WHERE District_id = @districtId AND AddedByUserId = @userId";

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
        /// Filters our people outside district boundary.
        /// </summary>
        /// <param name="personList">List with person models.</param>
        /// <param name="district">District</param>
        /// <returns>List with people inside district boundary.</returns>
        private List<AddressSearch.AdressProvider.Entities.Person> FilterPeopleOutsideBoundary(List<AddressSearch.AdressProvider.Entities.Person> personList, District district)
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
        /// Loads person list from Krak website.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<Person> GetPersonListFromKrak(District district)
        {
            var personList = new List<Person>();
            var addressProvider = GetAddressProviderForDistrict(district);
            var personListFromKrak = !String.IsNullOrEmpty(district.SearchPhrase)? addressProvider.getPersonList(district.SearchPhrase) : addressProvider.getPersonList(district.PostCodeFirst, district.PostCodeLast);
            
            // Filtering
            var filterList = new List<AddressSearch.AdressProvider.Filters.PersonFilter.IPersonFilter> {
                    new ScandinavianSurname()
                };
            FilterManager.FilterPersonList(personListFromKrak, filterList);

            personListFromKrak = FilterPeopleOutsideBoundary(personListFromKrak, district);

            // Conversion to model
            personList = personListFromKrak.Select(p => new Person(p, district)).ToList();

            return personList;
        }

        /// <summary>
        /// Loads persisted person list.
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
        /// Persists person list.
        /// </summary>
        /// <param name="district">District for which person list will be persisted.</param>
        /// <param name="personList">Person list to persist.</param>
        private void PersistPersonList(int districtId, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                DeletePeopleInDistrict(districtId);

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
