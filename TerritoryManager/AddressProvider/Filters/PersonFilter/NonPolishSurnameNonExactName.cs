namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    using Entities;
    using Helpers;

    /// <summary>
    ///     Satisfies criteria if person's name is different that the SearchName.Name
    ///     and his surname is not typical polish surname.
    /// </summary>
    public class NonPolishSurnameNonExactName : NonPolishSurname
    {
        public override bool SatisfiesCriteria(Person person)
        {
            if (!NameHelper.IsNameExact(person))
            {
                return base.SatisfiesCriteria(person);
            }

            return false;
        }
    }
}
