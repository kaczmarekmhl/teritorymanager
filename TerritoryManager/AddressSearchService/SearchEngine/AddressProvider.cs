namespace AddressSearchService.SearchEngine
{
    using AddressSearchData;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AddressProvider
    {
        private readonly ISearchStrategy _searchStrategy;

        public AddressProvider(ISearchStrategy searchStrategy)
        {
            _searchStrategy = searchStrategy;
        }
                
        /// <summary>
        /// Returns person list for given search phrase.
        /// </summary>
        public async Task<List<Person>> GetPersonListAsync(string searchPhrase, IProgress<int> progress = null)
        {
            SearchResult searchResult = await _searchStrategy.GetPersonListAsync(searchPhrase, SearchNameProvider.GetSearchNameList(), progress);

            if(searchResult.FailedSearchNamesList.Count > 0)
            {
                //Ask other services to finish search for failed names
            }

            return RemovePersonListDuplicates(searchResult.PersonList);
        }

        /// <summary>
        /// Removes duplicates from person list.
        /// </summary>
        protected List<Person> RemovePersonListDuplicates(List<Person> personList)
        {
            //Convert to HashSet to remove duplicates
            return personList == null ? null : new HashSet<Person>(personList).ToList();
        }        
    }
}
