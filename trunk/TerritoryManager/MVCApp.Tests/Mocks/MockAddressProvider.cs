using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Entities;

namespace MVCApp.Tests.Controllers.Mocks
{
    class MockAddressProvider : IAddressProvider
    {
        Dictionary<int, List<Person>> searchResultList;

        public MockAddressProvider(Dictionary<int, List<Person>> searchResultList)
        {
            this.searchResultList = searchResultList;
        }
        
        List<Person> IAddressProvider.getPersonList(int postCode, List<SearchName> searchNameList)
        {
            return searchResultList[postCode];
        }
    }
}
