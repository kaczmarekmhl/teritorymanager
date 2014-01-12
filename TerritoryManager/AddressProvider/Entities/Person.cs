namespace AddressSearch.AdressProvider.Entities
{
    using System;

    public class Person
    {
        public SearchName SearchName { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public int PostCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1}; {2}, {3}", Name, Lastname, StreetAddress, PostCode);
        }
    }
}
