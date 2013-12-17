using AddressSearch.AdressProvider.Entities;
using AddressSearch.AdressProvider.Filters.PersonFilter.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    /// <summary>
    ///     Satisfies criteria if person does not have typical polish surname.
    /// </summary>
    public class NonPolishSurname : IPersonFilter
    {
        PolishSurnameRecogniser polishSurnameRecognizer;

        public NonPolishSurname()
        {
            polishSurnameRecognizer = new PolishSurnameRecogniser(); 
        }

        public virtual bool SatisfiesCriteria(Person person)
        {
            if (polishSurnameRecognizer.ContainsPolishSurname(person.Lastname))
            {
                return false;
            }

            // Sometimes it happens that surename is in name
            if (polishSurnameRecognizer.ContainsPolishSurname(person.Name, true))
            {
                return false;
            }

            return true;
        }


        
    }
}
