using AddressSearch.AdressProvider.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressSearch.AdressProvider.SearchStrategies
{
    using System;

    public interface ISearchStrategy
    {
        Task<List<Person>> GetPersonListAsync(string searchPhrase, List<SearchName> searchNameList, IProgress<int> progress = null);
    }
}
