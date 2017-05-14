namespace AddressSearch.Loader
{
    using AddressSearch.Comon.Data;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// This class is responsible for parsing Enrio's web page html document.
    /// </summary>
    public class EnrioHtmlPageParser
    {
        /// <summary>
        /// Gets people list from html document.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public static List<Person> GetPersonListFromHtml(string html, SearchName searchName)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var personNodes = doc.DocumentNode.SelectNodes("//article[contains(@class, 'hit')]");
            var personList = new List<Person>();

            return personNodes == null ? personList : ParsePersonList(personNodes, searchName);
        }

        /// <summary>
        /// Gets total page count from people list html document.
        /// </summary>
        /// <param name="html">The html document containing people list</param>
        /// <returns>The page count.</returns>
        public static int GetTotalPageFromHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            int totalPages = 1;
            var paginationNode = doc.DocumentNode.SelectSingleNode("//ol[@class='page-container']");

            if (paginationNode == null)
            {
                return totalPages;
            }

            var totalPageNode = paginationNode.SelectSingleNode(".//li[contains(@class, 'page-count')]/span");

            if (totalPageNode != null)
            {
                totalPages = int.Parse(totalPageNode.InnerText.Replace("...", ""));
            }
            else
            {
                //total node does not exist
                //try to search for maximum number
                foreach (var linkNode in paginationNode.Descendants("a"))
                {
                    int totalPagesTmp = 0;

                    if (!int.TryParse(linkNode.InnerText, out totalPagesTmp))
                    {
                        continue;
                    }

                    if (totalPagesTmp > totalPages)
                    {
                        totalPages = totalPagesTmp;
                    }
                }
            }

            return totalPages;
        }

        protected static Person ParseSinglePerson(HtmlNode personSingleNode, SearchName searchName)
        {
            int postCode = 0;

            if (personSingleNode == null)
            {
                return null;
            }

            //Parse location
            var location = ParseLocation(personSingleNode);

            if (location == null)
            {
                return null;
            }

            //Parse post code
            int.TryParse(GetSingleNodeText(".//span[@class='hit-postal-code']", personSingleNode), out postCode);

            //Parse name and last name
            string nameString = WebUtility.HtmlDecode(GetSingleNodeText(".//span[@class='hit-name-ellipsis']/a[@href]", personSingleNode));
            string[] nameParts = nameString.Split(' ');

            return new Person
            {
                SearchName = searchName,
                Name = string.Join(" ", nameParts.Take<string>(nameParts.Count() - 1)),
                Lastname = nameParts.Last<string>(),
                StreetAddress = WebUtility.HtmlDecode(GetSingleNodeText(".//span[@class='hit-street-address']", personSingleNode)),
                PostCode = postCode,
                TelephoneNumber = GetSingleNodeText(".//span[contains(@class, 'hit-phone-number')]", personSingleNode),
                Latitude = location.Coordinate.Lat,
                Longitude = location.Coordinate.Lon
            };
        }

        /// <summary>
        /// Parses person list
        /// </summary>
        protected static List<Person> ParsePersonList(HtmlNodeCollection personNodes, SearchName searchName)
        {
            var resultList = new List<Person>();

            foreach (HtmlNode singlePersonNode in personNodes)
            {
                var person = ParseSinglePerson(singlePersonNode, searchName);

                if (person != null)
                {
                    resultList.Add(person);
                }
            }

            return resultList;
        }

        private static string GetSingleNodeText(string xpath, HtmlNode node)
        {
            var selectedNode = node.SelectSingleNode(xpath);

            if (selectedNode == null)
            {
                return "Not found";
            }

            return selectedNode.InnerHtml.Replace("\n", "");
        }

        private static JsonLocation ParseLocation(HtmlNode personSingleNode)
        {
            var locNode = personSingleNode.SelectSingleNode(".//div[contains(@class, 'hit-address-location')]");

            if (locNode == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<JsonLocation>(locNode.GetAttributeValue("data-coordinate", ""));
        }

        private class JsonLocation
        {
            internal class JsonLocationCoordinate
            {
                public string Lat { get; set; }
                public string Lon { get; set; }
            }

            public string HitNumber { get; set; }
            public JsonLocationCoordinate Coordinate { get; set; }
        };
    }
}
