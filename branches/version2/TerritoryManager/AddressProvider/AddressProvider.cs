using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider
{
    public class AddressProvider
    {
        /// <summary>
        /// Returns person list for given post code.
        /// </summary>
        /// <param name="postCode"></param>
        /// <returns></returns>
        public List<Person> getPersonList(int postCode)
        {
            KrakAddressProvider krakAddressProvider = new KrakAddressProvider();
            return krakAddressProvider.getPersonList(postCode, LoadSearchNameList("Resources/PolishNameList.txt"));
        }

        /// <summary>
        /// Returns person list for given post code range.
        /// </summary>
        /// <param name="postCodeFirst"></param>
        /// <param name="postCodeLast"></param>
        /// <returns></returns>
        public List<Person> getPersonList(int postCodeFirst, int? postCodeLast)
        {
            if(!postCodeLast.HasValue)
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

        protected List<SearchName> LoadSearchNameList(string filePath)
        {
            List<SearchName> result = new List<SearchName>();

            foreach(string line in File.ReadAllLines(filePath))
            {
                if (!String.IsNullOrEmpty(line))
                {
                    result.Add(new SearchName { Name = line.Trim() });
                }
            }

            return result;
        }
    }
}
