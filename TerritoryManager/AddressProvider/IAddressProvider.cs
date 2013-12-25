using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider
{
    public interface IAddressProvider
    {
        List<Person> getPersonList(int postCode, List<SearchName> searchNameList);
    }
}
