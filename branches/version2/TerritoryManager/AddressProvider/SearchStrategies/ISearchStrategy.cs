using AddressSearch.AdressProvider.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressSearch.AdressProvider.SearchStrategies
{
    public interface ISearchStrategy
    {
        Task<List<Person>> getPersonListAsync(string searchPhrase, List<SearchName> searchNameList);
    }
}
