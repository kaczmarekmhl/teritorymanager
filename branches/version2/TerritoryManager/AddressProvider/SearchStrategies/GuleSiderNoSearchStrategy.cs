namespace AddressSearch.AdressProvider.SearchStrategies
{
    public class GuleSiderNoSearchStrategy : KrakDkSearchStrategy
    {
        public GuleSiderNoSearchStrategy()
        {
            WebPageUrl = "http://www.gulesider.no/person/resultat/{0}/{1}/{2}";
        }
    }
}
