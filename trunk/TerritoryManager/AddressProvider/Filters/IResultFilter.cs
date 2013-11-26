using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AddressSearch.AdressProvider.Filters
{
    public interface IResultFilter
    {
        bool ApplyFilter(Person person);
    }
}
