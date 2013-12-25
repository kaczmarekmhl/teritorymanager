using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider
{
    public class AddressProviderTool
    {
        private IAddressProvider addressProvider { get; set; }
        private List<SearchName> searchNameList;

        public AddressProviderTool()
        {
            searchNameList = LoadSearchNameList("Resources/PolishNameList.txt");
            addressProvider = new KrakAddressProvider();
        }

        public AddressProviderTool(string[] nameList)
            : this(nameList, new KrakAddressProvider())
        {            
        }

        public AddressProviderTool(IAddressProvider addressProvider)
        {
            searchNameList = LoadSearchNameList("Resources/PolishNameList.txt");
            this.addressProvider = addressProvider;
        }        
        
        public AddressProviderTool(string[] nameList, IAddressProvider addressProvider)
        {
            searchNameList = LoadSearchNameList(nameList);
            this.addressProvider = addressProvider;
        }
        
        public List<Person> getPersonList(int postCode)
        {
            return addressProvider.getPersonList(postCode, searchNameList);
        }

        protected List<SearchName> LoadSearchNameList(string filePath)
        {
            return LoadSearchNameList(File.ReadAllLines(filePath));
        }

        protected List<SearchName> LoadSearchNameList(string[] nameList)
        {
            List<SearchName> result = new List<SearchName>();

            foreach (string name in nameList)
            {
                if (!String.IsNullOrEmpty(name))
                {
                    result.Add(new SearchName { Name = name.Trim() });
                }
            }

            return result;
        }

    }
}
