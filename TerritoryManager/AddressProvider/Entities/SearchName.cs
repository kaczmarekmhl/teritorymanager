namespace AddressSearch.AdressProvider.Entities
{
    public class SearchName
    {
        public string Name { get; set; }

        public bool IsMale 
        { 
            get { return !Name.EndsWith("a"); } 
            private set { }
        }
    }
}
