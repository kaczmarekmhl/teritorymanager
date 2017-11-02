using AddressSearch.Data;

namespace AddressSearch.Filters.PersonFilter.Helpers
{
    public class NameHelper
    {
        /// <summary>
        /// Check if given person name is exact match to the search name
        /// </summary>
        public static bool IsNameExact(Person person)
        {
            return person.Name.Contains(person.SearchName.Name);
        }
    }
}
