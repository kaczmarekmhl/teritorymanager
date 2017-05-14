namespace AddressSearchComon.Data
{
    /// <summary>
    /// The name by which person is searched.
    /// </summary>
    public class SearchName
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is name a male name.
        /// </summary>
        public bool IsMale
        {
            get { return !Name.EndsWith("a"); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
