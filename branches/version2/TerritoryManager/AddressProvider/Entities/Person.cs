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
        
        public override bool Equals(System.Object obj)
        {
            if (obj == null || !(obj is Person))
            {
                return false;
            }

            var p = (Person)obj;

            if (Name.Equals(p.Name)
                && Lastname.Equals(p.Lastname)
                && PostCode.Equals(p.PostCode))
            {
                if (!StreetAddress.Equals(p.StreetAddress))
                {
                    return CompareStreetAddress(StreetAddress, p.StreetAddress);
                }

                return true;
            }

            return false;
        }

        private bool CompareStreetAddress(String oldStreet, String newStreet)
        {
            var separators = new char[] { '.',',',';',' ' };

            oldStreet = String.Join("", oldStreet.Split(separators));
            newStreet = String.Join("", newStreet.Split(separators));

            return oldStreet.Equals(newStreet);
        }

        public override int GetHashCode()
        {
            return PostCode.GetHashCode();
        }
    }
}
