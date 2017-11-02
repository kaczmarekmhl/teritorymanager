using AddressSearch.Data;

namespace AddressSearch.Filters.PersonFilter
{
    public interface IPersonFilter
    {
        bool SatisfiesCriteria(Person person);
    }
}
