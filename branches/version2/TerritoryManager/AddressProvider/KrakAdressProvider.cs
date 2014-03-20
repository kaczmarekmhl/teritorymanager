namespace AddressSearch.AdressProvider
{
    using AddressSearch.AdressProvider.Entities;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    class KrakAddressProvider
    {
        class JsonLocation
        {
            internal class JsonLocationCoordinate
            {
                public string Lat { get; set; }
                public string Lon { get; set; }
            }

            public string HitNumber { get; set; }
            public JsonLocationCoordinate Coordinate { get; set; }
        };

        public List<Person> getPersonList(string searchPhrase, List<SearchName> searchNameList)
        {
            ConcurrentBag<Person> personList = new ConcurrentBag<Person>();

            Parallel.ForEach(Partitioner.Create(0, searchNameList.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    foreach (var person in getPersonList(searchPhrase, searchNameList.ElementAt(i)))
                    {
                        personList.Add(person);
                    }
                }                
            });

            /*foreach (var name in searchNameList)
            {
                foreach (var person in getPersonList(searchPhrase, name))
                {
                    personList.Add(person);
                }
            }*/

            return personList.ToList();
        }

        protected List<Person> getPersonList(string searchPhrase, SearchName searchName)
        {
            List<Person> resultList = new List<Person>();
            HtmlDocument doc = new HtmlDocument();
            int totalPageCount = 1;

            doc.LoadHtml(getKrakPersonHtml(searchName.Name, searchPhrase));

            totalPageCount = getTotalPageFromHtmlDocument(doc);

            for (int currentPage = 1; currentPage <= totalPageCount; currentPage++)
            {
                if (currentPage > 1)
                {
                    doc.LoadHtml(getKrakPersonHtml(searchName.Name, searchPhrase, currentPage));
                }

                resultList.AddRange(getPersonListFromHtmlDocument(doc, searchName));
            }

            return resultList;
        }

        protected List<Person> getPersonListFromHtmlDocument(HtmlDocument doc, SearchName searchName)
        {
            HtmlNodeCollection personNodes = doc.DocumentNode.SelectNodes("//article[contains(@class, 'hit')]");
            List<Person> personList = new List<Person>();

            if (personNodes == null)
            {
                return personList;
            }

            return parsePersonList(personNodes, searchName);
        }

        protected Person parseSinglePerson(HtmlNode personSingleNode, SearchName searchName)
        {
            int postCode = 0;

            if(personSingleNode == null)
            {
                return null;
            }

            //Parse location
            var location = parseLocation(personSingleNode);

            if (location == null)
            {
                return null;
            }
                       
            //Parse post code
            int.TryParse(getSingleNodeText(".//span[@class='hit-postal-code']", personSingleNode), out postCode);
    
            //Parse name and last name
            string nameString = System.Net.WebUtility.HtmlDecode(getSingleNodeText(".//span[@class='hit-name-ellipsis']/a[@href]", personSingleNode));
            string[] nameParts = nameString.Split(' ');

            return new Person
                 {
                     SearchName = searchName,
                     Name = String.Join(" ", nameParts.Take<string>(nameParts.Count() - 1)),
                     Lastname = nameParts.Last<string>(),
                     StreetAddress = System.Net.WebUtility.HtmlDecode(getSingleNodeText(".//span[@class='hit-street-address']", personSingleNode)),
                     Locality = getSingleNodeText(".//span[@class='hit-address-locality']", personSingleNode),
                     PostCode = postCode,
                     TelephoneNumber = getSingleNodeText(".//span[contains(@class, 'hit-phone-number')]", personSingleNode),
                     Latitude = location.Coordinate.Lat,
                     Longitude = location.Coordinate.Lon
                 };
        }

        /// <summary>
        /// Parses krak person list
        /// </summary>
        protected List<Person> parsePersonList(HtmlNodeCollection personNodes, SearchName searchName)
        {
            List<Person> resultList = new List<Person>();

            foreach (HtmlNode singlePersonNode in personNodes)
            {
                var person = parseSinglePerson(singlePersonNode, searchName);

                if (person != null)
                {
                    resultList.Add(person);
                }
            }

            return resultList;
        }

        protected int getTotalPageFromHtmlDocument(HtmlDocument doc)
        {
            int totalPages = 1;
            var paginationNode = doc.DocumentNode.SelectSingleNode("//ol[@class='page-container']");

            if (paginationNode != null)
            {
                var totalPageNode = paginationNode.SelectSingleNode(".//li[contains(@class, 'page-count')]/span");

                if (totalPageNode != null)
                {
                    totalPages = int.Parse(totalPageNode.InnerText.Replace("...", ""));
                }
                else
                {
                    int totalPagesTmp = 0;

                    //total node does not exist
                    //try to search for maximum number
                    foreach (var linkNode in paginationNode.Descendants("a"))
                    {
                        if (int.TryParse(linkNode.InnerText, out totalPagesTmp))
                        {
                            if (totalPagesTmp > totalPages)
                            {
                                totalPages = totalPagesTmp;
                            }
                        }
                    }

                }
            }

            return totalPages;
        }

        private string getSingleNodeText(string xpath, HtmlNode node)
        {
            HtmlNode selectedNode = node.SelectSingleNode(xpath);

            if (selectedNode == null)
            {
                return "Not found";
            }

            return selectedNode.InnerHtml.Replace("\n", "");
        }

        private JsonLocation parseLocation(HtmlNode personSingleNode)
        {
            HtmlNode locNode = personSingleNode.SelectSingleNode(".//div[contains(@class, 'hit-address-location')]");
            return JsonConvert.DeserializeObject<JsonLocation>(locNode.GetAttributeValue("data-coordinate", ""));
        }

        private string getKrakPersonHtml(string name, string searchPhrase, int page = 1)
        {
            int tryCount = 0;

            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            while (true)
            {
                try
                {
                    return webClient.DownloadString(getKrakPersonUrl(name, searchPhrase, page));
                }
                catch (WebException webException)
                {
                    tryCount++;
                    System.Threading.Thread.Sleep(1000);

                    if (tryCount >= 5)
                    {
                        throw webException;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private string getKrakPersonUrl(string name, string searchPhrase, int page = 1)
        {
            return string.Format("http://www.krak.dk/person/resultat/{0}/{1}/{2}", name, searchPhrase, page);
        }
    }
}