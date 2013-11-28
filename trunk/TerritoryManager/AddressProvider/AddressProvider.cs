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
        public List<Person> getPersonList(int postCode)
        {
            KrakAddressProvider krakAddressProvider = new KrakAddressProvider();
            return krakAddressProvider.getPersonList(postCode, LoadSearchNameList("PolishNameList.txt"));
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
