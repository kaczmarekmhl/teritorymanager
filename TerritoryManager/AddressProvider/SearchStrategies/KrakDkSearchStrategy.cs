namespace AddressSearch.AdressProvider.SearchStrategies
{
    using AddressSearch.AdressProvider.Entities;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class KrakDkSearchStrategy : ISearchStrategy
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

        /// <summary>
        /// URL to the Krak.dk web page or similar one.
        /// </summary>
        protected readonly string webPageUrl = "http://www.krak.dk/person/resultat/{0}/{1}/{2}";

        public virtual async Task<List<Person>> getPersonListAsync(string searchPhrase, List<SearchName> searchNameList)
        {
            //ConcurrentBag<Person> personList = new ConcurrentBag<Person>();
            var personList = new List<Person>();

            /*Parallel.ForEach(Partitioner.Create(0, searchNameList.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    foreach (var person in getPersonListAsync(searchPhrase, searchNameList.ElementAt(i)).Result)
                    {
                        personList.Add(person);
                    }
                }
            });*/

            foreach (var name in searchNameList)
            {
                foreach (var person in await getPersonListAsync(searchPhrase, name))
                {
                    personList.Add(person);
                }
            }

            /*List<Task<List<Person>>> taskList = (from name in searchNameList select getPersonListAsync(searchPhrase, name)).ToList();

            await Task.WhenAll(taskList);

            foreach (var task in taskList)
            {
                personList.AddRange(task.Result);
            }*/

            return personList;
        }

        protected virtual async Task<List<Person>> getPersonListAsync(string searchPhrase, SearchName searchName)
        {
            //TODO concurency with result list?
            List<Person> resultList = new List<Person>();
            HtmlDocument doc = new HtmlDocument();
            int totalPageCount = 1;

            //First page
            doc.LoadHtml(await getKrakPersonHtmlAsync(searchName.Name, searchPhrase));
            resultList.AddRange(getPersonListFromHtmlDocument(doc, searchName));

            //Next pages
            totalPageCount = getTotalPageFromHtmlDocument(doc);

            var downloadTaskList = new List<Task<string>>();

            for (int currentPage = 2; currentPage <= totalPageCount; currentPage++)
            {
                downloadTaskList.Add(getKrakPersonHtmlAsync(searchName.Name, searchPhrase, currentPage));
            }

            while (downloadTaskList.Count > 0)
            {
                var finishedTask = await Task.WhenAny(downloadTaskList);
                downloadTaskList.Remove(finishedTask);

                doc.LoadHtml(finishedTask.Result);
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

            if (personSingleNode == null)
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

            if (locNode == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<JsonLocation>(locNode.GetAttributeValue("data-coordinate", ""));
        }

        private async Task<string> getKrakPersonHtmlAsync(string name, string searchPhrase, int page = 1)
        {
            int tryCount = 0;

            while (true)
            {
                using (var httpClient = SetupHttpClient())
                {
                    SetWebRequestHeaders(httpClient);

                    string url = getKrakPersonUrl(name, searchPhrase, page);

                    Trace.TraceInformation("Request start: " + url);

                    HttpClient client = new HttpClient();

                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Trace.TraceInformation("Request end: " + url);

                            return await response.Content.ReadAsStringAsync();
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            Trace.TraceInformation("Request end: adresses not found");

                            //Krak generates 404 errors when no person was found
                            return String.Empty;
                        }
                        else
                        {
                            Trace.TraceError("Request failed: " + response.ReasonPhrase);
                            tryCount++;

                            if (tryCount >= 5)
                            {
                                throw new HttpRequestException("Request failed with message: " + response.RequestMessage);
                            }
                        }
                    }
                }
            }
        }

        protected string getKrakPersonUrl(string name, string searchPhrase, int page = 1)
        {
            return string.Format(webPageUrl, name, searchPhrase, page);
        }

        protected HttpClient SetupHttpClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy("tereny-proxy-vm.trafficmanager.net:21777", false),
                UseProxy = true,
                UseCookies = true
            };

            return new HttpClient(httpClientHandler);
        }

        protected void SetWebRequestHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "pl-PL,pl;q=0.8,en-US;q=0.6,en;q=0.4");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36");
        }
    }
}