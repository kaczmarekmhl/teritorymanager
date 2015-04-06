namespace AddressSearch.AdressProvider
{
    using AddressSearch.AdressProvider.Entities;
    using AddressSearch.AdressProvider.Properties;
    using AddressSearch.AdressProvider.SearchStrategies;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class AddressProvider
    {
        public static List<SearchName> polishSearchNameList;
        private ISearchStrategy searchStrategy;

        public AddressProvider(ISearchStrategy searchStrategy)
        {
            this.searchStrategy = searchStrategy;

            LoadSearchNameList();
        }

        /// <summary>
        /// Returns person list for given post code range.
        /// </summary>
        public List<Person> getPersonList(int postCodeFirst, int? postCodeLast = null)
        {
            if(!postCodeLast.HasValue)
            {
                postCodeLast = postCodeFirst;
            }

            if (postCodeLast.Value < postCodeFirst)
            {
                throw new Exception("Invalid post code range");
            }

            List<Person> personList = new List<Person>();

            foreach (var searchPhrase in GetSearchPhrases(postCodeFirst, postCodeLast.Value))
            {
                personList.AddRange(getPersonList(searchPhrase));
            }

            personList = RemovePeopleOutsidePostCodeRange(personList, postCodeFirst, postCodeLast.Value);
            personList = RemovePersonListDuplicates(personList);

            return personList;
        }

        /// <summary>
        /// Returns person list for given search phrase.
        /// </summary>
        public List<Person> getPersonList(string searchPhrase)
        {
            var personList = searchStrategy.getPersonList(searchPhrase, polishSearchNameList);

            return RemovePersonListDuplicates(personList);
        }

        public void GetDifferenceOfUpdatedPersonList(List<Person> updatedPersonList, List<Person> outdatedPersonList, out List<Person> newPersonList, out List<Person> removedPersonList)
        {
            // Get new people list
            var newPersonSet = new HashSet<Person>(updatedPersonList);
            newPersonSet.ExceptWith(outdatedPersonList);
            newPersonList = newPersonSet.ToList();

            // Get removed people list
            var removedPersonSet = new HashSet<Person>(outdatedPersonList);
            removedPersonSet.ExceptWith(updatedPersonList);
            removedPersonList = removedPersonSet.ToList();
        }  

        /// <summary>
        /// Removes search results outside post code range.
        /// </summary>
        protected List<Person> RemovePeopleOutsidePostCodeRange(List<Person> personList, int postCodeFirst, int postCodeLast)
        {
            var resultList = new List<Person>();

            foreach (var person in personList)
            {
                if (person.PostCode >= postCodeFirst && person.PostCode <= postCodeLast)
                {
                    resultList.Add(person);
                }
            }

            return resultList;
        }

        /// <summary>
        /// Removes duplicates from person list.
        /// </summary>
        protected List<Person> RemovePersonListDuplicates(List<Person> personList)
        {
            if (personList == null)
            {
                return null;
            }

            //Convert to HashSet to remove duplicates
            return new HashSet<Person>(personList).ToList<Person>();
        }

        /// <summary>
        /// Returns list of phrases to search given post code range.
        /// </summary>
        protected HashSet<string> GetSearchPhrases(int postCodeFirst, int postCodeLast)
        {
            HashSet<string> phrases = new HashSet<string>();

            for (int postCode = postCodeFirst; postCode <= postCodeLast; postCode++)
            {
                if (postCode >= 1000 && postCode <= 1499)
                {
                    phrases.Add("København K");
                }
                else if (postCode >= 1500 && postCode <= 1799)
                {
                    phrases.Add("København V");
                }
                else if (postCode >= 1800 && postCode <= 1999)
                {
                    phrases.Add("Frederiksberg C");
                }
                else
                {
                    phrases.Add(postCode.ToString());
                }
            }

            return phrases;
        }

        protected void LoadSearchNameList()
        {
            // Load names only once
            if (polishSearchNameList != null)
            {
                return;
            }

            polishSearchNameList = new List<SearchName>();

            using (var reader = new StringReader(Resources.PolishNames))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        polishSearchNameList.Add(new SearchName { Name = line.Trim() });
                    }
                }
            }            
        }
    }
}
