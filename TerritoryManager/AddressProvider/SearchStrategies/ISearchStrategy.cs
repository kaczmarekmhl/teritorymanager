

namespace AddressSearch.AdressProvider.SearchStrategies
{
    using AddressSearchComon.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISearchStrategy
    {
        Task<List<Person>> GetPersonListAsync(string searchPhrase, List<SearchName> searchNameList, IProgress<int> progress = null);
    }
}
