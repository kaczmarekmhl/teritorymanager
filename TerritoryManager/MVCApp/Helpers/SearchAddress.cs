namespace MVCApp.Helpers
{
    using AddressSearch;
    using AddressSearch.Filters;
    using AddressSearch.Filters.PersonFilter;
    using AddressSearch.Filters.PersonFilter.Helpers;
    using AddressSearch.Types;
    using MapLibrary;
    using MVCApp.Enums;
    using MVCApp.Models;
    using MVCApp.Translate;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using WebMatrix.WebData;
    using SearchEntities = AddressSearch.Data;

    public class SearchAddress
    {
        protected bool IsSharingAdressesEnabled;

        #region SetProgressMessage
        public delegate void SetProgressMessageDelegate(string message);

        private SetProgressMessageDelegate _setProgressMessage;
        public SetProgressMessageDelegate SetProgressMessage
        {
            get {
                return _setProgressMessage ?? (_setProgressMessage = SetProgressMessageFake);
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
			
            this.Db = db;
            this.IsSharingAdressesEnabled = UserContext.IsSharingAdressesEnabled(db);
        }

        /// <summary>
        /// Returns newly found person list for the given district.
        /// </summary>
        /// <param name="district">District that the search will be done for.</param>
        /// <returns>Person list</returns>
        public List<Person> SearchAndPersistNewPersonList(District district)
        {
			throw new InvalidOperationException("Address search disabled due to GDPR privacy requirements. Personal data cannot not be persisted in database.");

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
        /// <param name="districtId">District that the search will be done for.</param>
        /// <param name="searchUpdate">Update adresses</param>
        public IQueryable<Person> GetPersistedPersonListQuery(int districtId, bool? searchUpdate)
        {
            return Db.Persons
                .Where(p =>
                    p.District.Id == districtId &&
                    (p.AddedByUserId == WebSecurity.CurrentUserId || IsSharingAdressesEnabled || p.DoNotVisit || p.IsVisitedByOtherPublisher) &&
                    p.Manual == false &&
                    (p.SearchUpdate == searchUpdate || searchUpdate == null))
                    .OrderBy(p => p.Name);
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

                List<SearchEntities.Person> newPersonList;
                List<SearchEntities.Person> removedPersonList;

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

            //Search must be run in separate thread, otherwhise it will deadlock IIS 
            var personListFromSearch = !string.IsNullOrEmpty(district.SearchPhrase)
                ? Task.Run(async () => await addressProvider.GetPersonListAsync(district.SearchPhrases, GetAddressProviderProgress())).GetAwaiter().GetResult()
                : Task.Run(async () => await addressProvider.GetPersonListAsync(district.PostCodeFirst, district.PostCodeLast, GetAddressProviderProgress())).GetAwaiter().GetResult();

            //Filter person list
            return FilterPersonListFromSearchEngine(district, personListFromSearch);
        }

        /// <summary>
        /// Filters out people received from search engine.
        /// </summary>
        /// <param name="district">District.</param>
        /// <param name="personListFromSearch">List with person models.</param>
        /// <returns></returns>
        private static List<SearchEntities.Person> FilterPersonListFromSearchEngine(District district, List<SearchEntities.Person> personListFromSearch)
        {
            var filterList = new List<IPersonFilter> {
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
        private static List<SearchEntities.Person> FilterPeopleOutsideBoundary(List<SearchEntities.Person> personList, District district)
        {
            if (String.IsNullOrEmpty(district.DistrictBoundaryKml))
            {
                return personList;
            }

            var resultList = new List<SearchEntities.Person>();
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
        private static List<Person> PreliminarySelection(List<Person> personList)
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
        /// <param name="districtId">District for which person list will be persisted.</param>
        /// <param name="personList">Person list to persist.</param>
        private void PersistPersonList(int districtId, List<Person> personList)
        {
            if (personList.Count > 0)
            {
                using (var dbContextTransaction = Db.Database.BeginTransaction())
                {
                    try
                    {
                        MarkPeopleAsOldSearch(districtId);

                        //Mark as new update
                        personList.ForEach(p => p.SearchUpdate = true);

                        Db.Persons.AddRange(personList);

                        foreach (var validationResults in Db.GetValidationErrors())
                        {
                            if (!validationResults.IsValid)
                            {
                                var invalidEntity = (Person)validationResults.Entry.Entity;

                                Db.Persons.Remove(invalidEntity);
                                personList.Remove(invalidEntity);
                            }
                        }

                        Db.SaveChanges();

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
        /// Marks previous search as old.
        /// </summary>
        /// <param name="districtId">District id for which people will be marked as old.</param>
        private void MarkPeopleAsOldSearch(int districtId)
        {
            //Entity framework does not support deleting data through direct SQL
            //We need to do it due to performance reasons
            string sqlDeleteStatement;
            object[] parameterList;

            if (IsSharingAdressesEnabled)
            {
                sqlDeleteStatement = "Update People SET SearchUpdate = 0 WHERE District_id = @districtId AND Manual = 0";
                parameterList = new object[1];
            }
            else
            {
                sqlDeleteStatement = "Update People SET SearchUpdate = 0 WHERE District_id = @districtId AND (AddedByUserId = @userId OR DoNotVisit = 1)AND Manual = 0";
                parameterList = new object[2];
                parameterList[1] = new SqlParameter("@userId", WebSecurity.CurrentUserId);
            }

            parameterList[0] = new SqlParameter("@districtId", districtId);

            Db.Database.ExecuteSqlCommand(sqlDeleteStatement, parameterList);
            Db.SaveChanges();
        }

        /// <summary>
        /// Returns AddressProvider class for given district based on its country
        /// </summary>
        /// <param name="district">District</param>
        /// <returns>AddressProvider class</returns>
        private AddressProvider GetAddressProviderForDistrict(District district)
        {
            WebPageType webPageType;

            switch (district.Congregation.Country)
            {
                case Country.Denmark:
                    webPageType = WebPageType.KrakDk;
                    break;
                case Country.Norway:
                    webPageType = WebPageType.GulesiderNo;
                    break;
                default:
                    throw new Exception("Address provider cannot be returned for given country");
            }

            return new AddressProvider(webPageType);
        }

        protected IProgress<int> GetAddressProviderProgress()
        {
            var progress = new Progress<int>();
            progress.ProgressChanged += (s, e) =>
            {
                //Required to change language as this code is executed not in UI thread
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
                SetProgressMessage(string.Format(Strings.SearchAddressWaitProgress, e));
            };
            return progress;
        }

        protected DistictManagerDb Db;
    }
}