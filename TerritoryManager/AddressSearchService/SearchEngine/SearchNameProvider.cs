using AddressSearchData;
using AddressSearchService.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AddressSearchService.SearchEngine
{
    /// <summary>
    /// Provides the list of names by which search is completed.
    /// </summary>
    public static class SearchNameProvider
    {
        private static Lazy<List<SearchName>> lazySearchNameList = new Lazy<List<SearchName>>(LoadSearchNameList);
        
        /// <summary>
        /// Returns search name list.
        /// </summary>
        /// <returns>Search name list</returns>
        public static List<SearchName> GetSearchNameList()
        {
            return lazySearchNameList.Value;
        }

        private static List<SearchName> LoadSearchNameList()
        {
            var searchNameList = new List<SearchName>();

            using (var reader = new StringReader(Resources.PolishNames))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        searchNameList.Add(new SearchName { Name = line.Trim() });
                    }
                }
            }

            return searchNameList;
        }
    }
}
