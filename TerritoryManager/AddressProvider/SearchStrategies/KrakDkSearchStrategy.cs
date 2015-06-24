namespace AddressSearch.AdressProvider.SearchStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities;
    using HtmlAgilityPack;
    using Newtonsoft.Json;

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

        private int _completedNames;
        private int _totalNames;

        /// <summary>
        /// URL to the Krak.dk web page or similar one.
        /// </summary>
        protected string WebPageUrl = "http://www.krak.dk/person/resultat/{0}/{1}/{2}";

        public virtual async Task<List<Person>> GetPersonListAsync(string searchPhrase, List<SearchName> searchNameList, IProgress<int> progress)
        {
            var personList = new List<Person>();
            _completedNames = 0;
            _totalNames = searchNameList.Count;

            /*Slower version
             * foreach (var name in searchNameList)
            {
                personList.AddRange(await GetPersonListAsync(searchPhrase, name));
            }*/

            var taskList = (from name in searchNameList select GetPersonListAsync(searchPhrase, name, progress)).ToList();

            await Task.WhenAll(taskList);

            foreach (var task in taskList)
            {
                personList.AddRange(task.GetAwaiter().GetResult());
            }

            if (_completedNames != _totalNames)
            {
                throw new Exception("All names have not been processed");
            }

            Trace.TraceInformation("Request for {0} completed successfully :) {1}", searchPhrase, WebPageUrl);
            
            return personList;
        }

        protected virtual async Task<List<Person>> GetPersonListAsync(string searchPhrase, SearchName searchName, IProgress<int> progress)
        {
            var resultList = new List<Person>();
            var doc = new HtmlDocument();
            int totalPageCount = 1;

            //First page
            doc.LoadHtml(await GetKrakPersonHtmlAsync(searchName.Name, searchPhrase));
            resultList.AddRange(GetPersonListFromHtmlDocument(doc, searchName));

            //Next pages
            totalPageCount = GetTotalPageFromHtmlDocument(doc);

            var downloadTaskList = new List<Task<string>>();

            for (int currentPage = 2; currentPage <= totalPageCount; currentPage++)
            {
                downloadTaskList.Add(GetKrakPersonHtmlAsync(searchName.Name, searchPhrase, currentPage));
            }

            while (downloadTaskList.Count > 0)
            {
                var finishedTask = await Task.WhenAny(downloadTaskList);
                downloadTaskList.Remove(finishedTask);

                doc.LoadHtml(finishedTask.GetAwaiter().GetResult());
                resultList.AddRange(GetPersonListFromHtmlDocument(doc, searchName));
            }

            Interlocked.Increment(ref _completedNames);

            if (progress != null)
            {
                progress.Report((int)Math.Floor((decimal)_completedNames * 100 / _totalNames));
            }

            return resultList;
        }

        protected List<Person> GetPersonListFromHtmlDocument(HtmlDocument doc, SearchName searchName)
        {
            var personNodes = doc.DocumentNode.SelectNodes("//article[contains(@class, 'hit')]");
            var personList = new List<Person>();

            return personNodes == null ? personList : ParsePersonList(personNodes, searchName);
        }

        protected Person ParseSinglePerson(HtmlNode personSingleNode, SearchName searchName)
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
                     Locality = GetSingleNodeText(".//span[@class='hit-address-locality']", personSingleNode),
                     PostCode = postCode,
                     TelephoneNumber = GetSingleNodeText(".//span[contains(@class, 'hit-phone-number')]", personSingleNode),
                     Latitude = location.Coordinate.Lat,
                     Longitude = location.Coordinate.Lon
                 };
        }

        /// <summary>
        /// Parses krak person list
        /// </summary>
        protected List<Person> ParsePersonList(HtmlNodeCollection personNodes, SearchName searchName)
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

        protected int GetTotalPageFromHtmlDocument(HtmlDocument doc)
        {
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

        private async Task<string> GetKrakPersonHtmlAsync(string name, string searchPhrase, int page = 1)
        {
            int tryCount = 0;

            while (true)
            {
                using (var httpClient = SetupHttpClient())
                {
                    if (tryCount % 2 == 1)
                    {
                        SetWebRequestHeaders(httpClient);
                    }

                    var url = GetKrakPersonUrl(name, searchPhrase, page);

                    try
                    {
                        //Trace.TraceInformation("Request start: " + url);

                        using (var response = await httpClient.GetAsync(url))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                //Trace.TraceInformation("Request competed: " + url);
                                return await response.Content.ReadAsStringAsync();
                            }
                            
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                //Trace.TraceInformation("Request completed: adresses not found");

                                //Krak generates 404 errors when no person was found
                                return string.Empty;
                            }

                            Trace.TraceError("Request failed for {0} with reason {1}", url, response.ReasonPhrase);
                            tryCount++;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Trace.TraceError("Request was canceled");
                        tryCount++;
                    }

                    if (tryCount >= 5)
                    {
                        Trace.TraceError("Search for name {0} failed (searchPhrase:{1})", name, searchPhrase);
                        throw new HttpRequestException(string.Format("Request for name {0} failed", name));
                    }
                }
            }
        }

        protected string GetKrakPersonUrl(string name, string searchPhrase, int page = 1)
        {
            return string.Format(WebPageUrl, name, searchPhrase, page);
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