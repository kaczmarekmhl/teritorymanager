namespace AddressSearchData
{
    /// <summary>
    /// Person entity.
    /// </summary>
    public class Person 
    {   
        /// <summary>
        /// The name by which the person was searched.
        /// </summary>
        public SearchName SearchName { get; set; }

        /// <summary>
        /// Person's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Person's last name
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Person's street address
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Person's post code
        /// </summary>
        public int PostCode { get; set; }

        /// <summary>
        /// Person's telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Person's longitude
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// Person's latitude
        /// </summary>
        public string Latitude { get; set; }
    }
}
