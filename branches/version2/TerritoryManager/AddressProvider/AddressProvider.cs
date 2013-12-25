namespace AddressSearch.AdressProvider
{
    using AddressSearch.AdressProvider.Entities;
    using AddressSearch.AdressProvider.Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;

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
        /// <param name="postCode"></param>
        /// <returns></returns>
        public List<Person> getPersonList(int postCode)
        {
            KrakAddressProvider krakAddressProvider = new KrakAddressProvider();
            return krakAddressProvider.getPersonList(postCode, polishSearchNameList);
        }

        /// <summary>
        /// Returns person list for given post code range.
        /// </summary>
        /// <param name="postCodeFirst"></param>
        /// <param name="postCodeLast"></param>
        /// <returns></returns>
        public List<Person> getPersonList(int postCodeFirst, int? postCodeLast)
        {
            if(!postCodeLast.HasValue || postCodeFirst == postCodeLast.Value)
            {
                return getPersonList(postCodeFirst);
            }

            if (postCodeLast.Value < postCodeFirst)
            {
                throw new Exception("Invalid post code range");
            }

            KrakAddressProvider krakAddressProvider = new KrakAddressProvider();
            List<Person> personList = new List<Person>();

            for (int postCode = postCodeFirst; postCode <= postCodeLast.Value; postCode++)
            {
                personList.AddRange(getPersonList(postCode));
            }

            return personList;
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
