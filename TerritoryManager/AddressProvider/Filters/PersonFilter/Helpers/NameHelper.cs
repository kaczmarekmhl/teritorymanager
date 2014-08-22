using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.Filters.PersonFilter.Helpers
{
    public class NameHelper
    {
        /// <summary>
        /// Check if given person name is exact match to the search name
        /// </summary>
        public static bool isNameExact(Person person)
        {
            return person.Name.Contains(person.SearchName.Name);
        }
    }
}
