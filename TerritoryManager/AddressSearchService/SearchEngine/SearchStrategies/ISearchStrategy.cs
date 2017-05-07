namespace AddressSearchService.SearchEngine
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AddressSearchData;
    using System;

    public interface ISearchStrategy
    {
        Task<SearchResult> GetPersonListAsync(string searchPhrase, List<SearchName> searchNameList, IProgress<int> progress = null);
    }
}
