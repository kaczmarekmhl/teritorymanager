namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    using AddressSearch.AdressProvider.Entities;
    using AddressSearch.AdressProvider.Filters.PersonFilter.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///     Satisfies criteria if person's name is different that the SearchName.Name
    ///     and his surname is not typical polish surname.
    /// </summary>
    public class NonPolishSurnameNonExactName : NonPolishSurname
    {
        public override bool SatisfiesCriteria(Person person)
        {
            if (!NameHelper.isNameExact(person))
            {
                return base.SatisfiesCriteria(person);
            }

            return false;
        }
    }
}
