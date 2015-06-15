namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    using Entities;

    public interface IPersonFilter
    {
        bool SatisfiesCriteria(Person person);
    }
}
