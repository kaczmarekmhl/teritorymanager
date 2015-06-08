using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Filters;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using AddressSearch.AdressProvider.Filters.PersonFilter.Helpers;
using AddressSearch.AdressProvider.SearchStrategies;
using SearchEntities = AddressSearch.AdressProvider.Entities;
using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using MapLibrary;
using MVCApp.Translate;

namespace MVCApp.Helpers
{
    public class SearchAddress
    {
        #region SetProgressMessage
        public delegate void SetProgressMessageDelegate(string message);

        private SetProgressMessageDelegate _setProgressMessage;
        public SetProgressMessageDelegate SetProgressMessage
        {
            get {
                if (_setProgressMessage == null)
                {
                    _setProgressMessage = new SetProgressMessageDelegate(SetProgressMessageFake);
                }
                return _setProgressMessage; 
            }
            set { _setProgressMessage = value; }
        }

        public void SetProgressMessageFake(string message)
        {
        }

        #endregion

        public SearchAddress(DistictManagerDb db)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            this.db = db;
        }

        /// <summary>
        /// Returns newly found person list for the given district.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        public List<Person> SearchAndPersistNewPersonList(District district)
        {
            SetProgressMessage(Strings.SearchAddressWait);
            var personList = GetNewPersonListFromSearchEngine(district);

            SetProgressMessage(Strings.SearchAddressSave);
            personList = PreliminarySelection(personList);
            PersistPersonList(district.Id, personList);

            SetProgressMessage(Strings.SearchAddressComplete);
            return personList;
        }

        /// <summary>
        /// Loads persisted person list.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        public IQueryable<Person> GetPersistedPersonListQuery(int districtId, bool? searchUpdate = true)
        {
            return db.Persons
                .Where(p =>
                    p.District.Id == districtId &&
                    p.AddedByUserId == WebSecurity.CurrentUserId &&
                    p.Manual == false &&
                    (p.SearchUpdate == searchUpdate || searchUpdate == null))
                    .OrderBy(p => p.Name);
        }        

        private void SetProgressMessage2(string message)
        {
            if (SetProgressMessage != null)
            {
                SetProgressMessage(message);
            }
        }

        /// <summary>
        /// Loads person list from search engine and return only new persons found.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list of new people</returns>
        private List<Person> GetNewPersonListFromSearchEngine(District district)
        {
            var personListFromSearch = GetPersonListFromSearchEngine(district);

            SetProgressMessage(Strings.SearchAddressLoadingOldAdresses);
            var previousPersonList = GetPersistedPersonListQuery(district.Id, null).ToList();

            //List can be updated
            if (previousPersonList.Count > 0)
            {
                //convert person list to address provider entities
                var previousPersonListSearchEntitiy = previousPersonList.Select(p => new SearchEntities.Person()
                {
                    Name = p.Name,
                    Lastname = p.Lastname,
                    PostCode = p.PostCode,
                    StreetAddress = p.StreetAddress
                }).ToList();

                var newPersonList = new List<SearchEntities.Person>();
                var removedPersonList = new List<SearchEntities.Person>();

                GetAddressProviderForDistrict(district).GetDifferenceOfUpdatedPersonList(personListFromSearch, previousPersonListSearchEntitiy, out newPersonList, out removedPersonList);

                // Conversion to model
                return newPersonList.Select(p => new Person(p, district)).ToList();
            }
            else
            {
                // Conversion to model
                return personListFromSearch.Select(p => new Person(p, district)).ToList();
            }
        }

        /// <summary>
        /// Loads person list from search engine.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        private List<SearchEntities.Person> GetPersonListFromSearchEngine(District district)
        {
            //Load person list
            var addressProvider = GetAddressProviderForDistrict(district);
            var personListFromSearch = !String.IsNullOrEmpty(district.SearchPhrase) ? addressProvider.getPersonList(district.SearchPhrase) : addressProvider.getPersonList(district.PostCodeFirst, district.PostCodeLast);

            //Filter person list
            return FilterPersonListFromSearchEngine(district, personListFromSearch);
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
            if (String.IsNullOrEmpty(district.DistrictBoundaryKml))
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
        /// Persists person list.
        /// </summary>
        /// <param name="district">District for which person list will be persisted.</param>
        /// <param name="personList">Person list to persist.</param>
        private void PersistPersonList(int districtId, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        MarkPeopleAsOldSearch(districtId);

                        //Mark as new update
                        personList.ForEach(p => p.SearchUpdate = true);

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

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes people for given district.
        /// </summary>
        /// <param name="districtId">District id that the delete will be done for.</param>
        private void MarkPeopleAsOldSearch(int districtId)
        {
            //Entity framework does not support deleting data through direct SQL
            //We need to do it due to performance reasons
            string sqlDeleteStatement = "Update People SET SearchUpdate = 0 WHERE District_id = @districtId AND AddedByUserId = @userId AND Manual = 0";

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@districtId", districtId));
            parameterList.Add(new SqlParameter("@userId", WebSecurity.CurrentUserId));

            db.Database.ExecuteSqlCommand(sqlDeleteStatement, parameterList.ToArray());
            db.SaveChanges();
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

        protected DistictManagerDb db;
    }
}