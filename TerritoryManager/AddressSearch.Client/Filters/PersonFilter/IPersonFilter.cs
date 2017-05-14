using AddressSearch.Comon.Data;

namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    public interface IPersonFilter
    {
        bool SatisfiesCriteria(Person person);
    }
}
