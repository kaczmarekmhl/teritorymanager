using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.Filters
{
    public static class FilterManager
    {
        public static void ApplyFilter(List<Person> personList, List<IResultFilter> filterList)
        {
            personList.RemoveAll(p => ApplyFilter(p, filterList) == true);
        }

        public static bool ApplyFilter(Person result, List<IResultFilter> filterList)
        {
            foreach (var filter in filterList)
            {
                if (filter.ApplyFilter(result) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
