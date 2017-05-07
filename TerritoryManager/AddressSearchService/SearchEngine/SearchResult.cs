namespace AddressSearchService.SearchEngine
{
    using System.Collections.Generic;
    using AddressSearchData;
    
    public class SearchResult
    {
        /// <summary>
        /// The list of found people.
        /// </summary>
        public List<Person> PersonList { get; set; } = new List<Person>();

        /// <summary>
        /// The search names for which search failed.
        /// </summary>
        public List<SearchName> FailedSearchNamesList { get; set; } = new List<SearchName>();
    }
}
