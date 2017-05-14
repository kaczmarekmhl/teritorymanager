namespace AddressSearch.AdressProvider.Filters.PersonFilter
{
    using AddressSearchComon.Data;
    using Helpers;

    /// <summary>
    ///     Satisfies criteria if person does not have typical polish surname.
    /// </summary>
    public class NonPolishSurname : IPersonFilter
    {
        readonly PolishSurnameRecogniser _polishSurnameRecognizer;

        public NonPolishSurname()
        {
            _polishSurnameRecognizer = new PolishSurnameRecogniser(); 
        }

        public virtual bool SatisfiesCriteria(Person person)
        {
            if (_polishSurnameRecognizer.ContainsPolishSurname(person.Lastname))
            {
                return false;
            }

            // Sometimes it happens that surename is in name
            if (_polishSurnameRecognizer.ContainsPolishSurname(person.Name, true))
            {
                return false;
            }

            return true;
        }


        
    }
}
