using AddressSearch.AdressProvider.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressSearch.AdressProvider.SearchStrategies
{
    public interface ISearchStrategy
    {
        Task<List<Person>> GetPersonListAsync(string searchPhrase, List<SearchName> searchNameList);
    }
}
