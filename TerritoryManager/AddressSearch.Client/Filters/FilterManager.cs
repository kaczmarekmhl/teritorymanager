using System.Collections.Generic;
using System.Linq;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using AddressSearch.Comon.Data;

namespace AddressSearch.AdressProvider.Filters
{
    public static class FilterManager
    {
        /// <summary>
        ///     Returns filtered person list.
        /// </summary>
        public static List<Person> GetFilteredPersonList(List<Person> personList, List<IPersonFilter> filterList)
        {
            return personList.Where(p => !SatisfiesAnyPersonFilter(p, filterList)).ToList();
        }

        /// <summary>
        ///     Filters given person list by removing it's elements.
        /// </summary>
        public static void FilterPersonList(List<Person> personList, List<IPersonFilter> filterList)
        {
            personList.RemoveAll(p => SatisfiesAnyPersonFilter(p, filterList));
        }

        /// <summary>
        ///     Applies all filters to person.
        /// </summary>
        public static bool SatisfiesAnyPersonFilter(Person result, List<IPersonFilter> filterList)
        {
            return filterList.Any(filter => filter.SatisfiesCriteria(result) == true);
        }
    }
}
