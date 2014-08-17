using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.SearchStrategies
{
    public interface ISearchStrategy
    {
        List<Person> getPersonList(string searchPhrase, List<SearchName> searchNameList);
    }
}
