using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    /// <summary>
    ///     Satisfies criteria if person's name is different that the SearchName.Name
    ///     and his surname is not typical polish surname.
    /// </summary>
    public class NonPolishSurnameNonExactName : NonPolishSurname
    {
        public override bool SatisfiesCriteria(Person person)
        {
            if (!isNameExact(person))
            {
                return base.SatisfiesCriteria(person);
            }

            return false;
        }

        /// <summary>
        /// Check if given person name is exact match to the search name
        /// </summary>
        protected bool isNameExact(Person person)
        {
            return person.Name.Contains(person.SearchName.Name);
        }
    }
}
