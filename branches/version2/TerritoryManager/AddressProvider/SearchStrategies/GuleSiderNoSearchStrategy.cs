using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.SearchStrategies
{
    public class GuleSiderNoSearchStrategy : KrakDkSearchStrategy
    {
        public GuleSiderNoSearchStrategy()
        {
            webPageUrl = "http://www.gulesider.no/person/resultat/{0}/{1}/{2}";
        }
    }
}
