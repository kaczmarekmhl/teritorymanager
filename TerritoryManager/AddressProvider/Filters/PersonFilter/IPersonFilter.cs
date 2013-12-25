namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    using AddressSearch.AdressProvider.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IPersonFilter
    {
        bool SatisfiesCriteria(Person person);
    }
}
