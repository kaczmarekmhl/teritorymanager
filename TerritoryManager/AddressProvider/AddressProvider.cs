namespace AddressSearch.AdressProvider
{
    using Entities;
    using Properties;
    using SearchStrategies;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AddressProvider
    {
        public static List<SearchName> PolishSearchNameList;
        private readonly ISearchStrategy _searchStrategy;

        public AddressProvider(ISearchStrategy searchStrategy)
        {
            _searchStrategy = searchStrategy;

            LoadSearchNameList();
        }

        /// <summary>
        /// Returns person list for given post code range.
        /// </summary>
        public async Task<List<Person>> GetPersonListAsync(int postCodeFirst, int? postCodeLast = null, IProgress<int> progress = null)
        {
            if(!postCodeLast.HasValue)
            {
                postCodeLast = postCodeFirst;
            }

            if (postCodeLast.Value < postCodeFirst)
            {
                throw new Exception("Invalid post code range");
            }

            var personList = new List<Person>();

            foreach (var searchPhrase in GetSearchPhrases(postCodeFirst, postCodeLast.Value))
            {
                personList.AddRange(await GetPersonListAsync(searchPhrase, progress));
            }

            personList = RemovePeopleOutsidePostCodeRange(personList, postCodeFirst, postCodeLast.Value);
            personList = RemovePersonListDuplicates(personList);

            return personList;
        }

        /// <summary>
        /// Returns person list for multiple search phrases.
        /// </summary>
        public async Task<List<Person>> GetPersonListAsync(List<string> searchPhraseList, IProgress<int> progress = null)
        {
            var personList = new List<Person>();

            foreach (var searchPhrase in searchPhraseList)
            {
                var personListPartial = await _searchStrategy.GetPersonListAsync(searchPhrase.Trim(), PolishSearchNameList, progress);

                personListPartial = RemovePeopleOutsidePostCodeRange(personListPartial, searchPhrase);

                personList.AddRange(personListPartial);
            }            

            return RemovePersonListDuplicates(personList);
        }

        /// <summary>
        /// Returns person list for given search phrase.
        /// </summary>
        public async Task<List<Person>> GetPersonListAsync(string searchPhrase, IProgress<int> progress = null)
        {
            var personList = await _searchStrategy.GetPersonListAsync(searchPhrase, PolishSearchNameList, progress);

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
            return personList.Where(person => person.PostCode >= postCodeFirst && person.PostCode <= postCodeLast).ToList();
        }

        /// <summary>
        /// Removes search results outside post code range.
        /// </summary>
        protected List<Person> RemovePeopleOutsidePostCodeRange(List<Person> personList, string searchPhrase)
        {
            int postCode;
            if (int.TryParse(searchPhrase, out postCode))
            {
                personList = RemovePeopleOutsidePostCodeRange(personList, postCode, postCode);
            }

            return personList;
        }

        /// <summary>
        /// Removes duplicates from person list.
        /// </summary>
        protected List<Person> RemovePersonListDuplicates(List<Person> personList)
        {
            //Convert to HashSet to remove duplicates
            return personList == null ? null : new HashSet<Person>(personList).ToList();
        }

        /// <summary>
        /// Returns list of phrases to search given post code range.
        /// </summary>
        protected HashSet<string> GetSearchPhrases(int postCodeFirst, int postCodeLast)
        {
            HashSet<string> phrases = new HashSet<string>();

            for (var postCode = postCodeFirst; postCode <= postCodeLast; postCode++)
            {
                if (_searchStrategy.GetType() == typeof(KrakDkSearchStrategy))
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
                else
                {
                    string postCodeString = postCode.ToString();

                    if (postCode < 1000)
                    {
                        postCodeString = string.Format("0{0}", postCodeString);
                    }

                    phrases.Add(postCodeString);
                }
            }

            return phrases;
        }

        protected void LoadSearchNameList()
        {
            // Load names only once
            if (PolishSearchNameList != null)
            {
                return;
            }

            PolishSearchNameList = new List<SearchName>();

            using (var reader = new StringReader(Resources.PolishNames))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        PolishSearchNameList.Add(new SearchName { Name = line.Trim() });
                    }
                }
            }            
        }
    }
}
