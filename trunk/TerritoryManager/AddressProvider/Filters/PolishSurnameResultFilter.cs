using AddressSearch.AdressProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.Filters
{
    public class PolishSurnameResultFilter : IResultFilter
    {
        public static List<string> polishSurnameSuffix = new List<string>{
            "ki", 
            "k",
            "r",
            "ka",
            "cz",
            "c"
        };

        public bool ApplyFilter(Person person)
        {
            if (!isNameExact(person))
            {
                foreach (var lastNamePart in person.Lastname.Split(new char[] { ' ', '-' }))
                {
                    return !polishSurnameSuffix.Any(suffix => lastNamePart.EndsWith(suffix));
                }
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
