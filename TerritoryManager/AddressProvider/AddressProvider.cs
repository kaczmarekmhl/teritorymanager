namespace AddressSearch.AdressProvider
{
    using AddressSearch.AdressProvider.Entities;
    using AddressSearch.AdressProvider.Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class AddressProvider
    {
        public static List<SearchName> polishSearchNameList;

        public AddressProvider()
        {
            LoadSearchNameList();
        }

        /// <summary>
        /// Returns person list for given post code.
        /// </summary>
        protected List<Person> getPersonList(string searchPhrase)
        {
            KrakAddressProvider krakAddressProvider = new KrakAddressProvider();
            return krakAddressProvider.getPersonList(searchPhrase, polishSearchNameList);
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

            return personList.GroupBy(p => new { p.Name, p.Lastname, p.StreetAddress, p.PostCode }).Select(grp => grp.First()).ToList<Person>();
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
