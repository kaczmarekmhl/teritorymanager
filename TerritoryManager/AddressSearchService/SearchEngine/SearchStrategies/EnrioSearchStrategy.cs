namespace AddressSearchService.SearchEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressSearchData;

    public class EnrioSearchStrategy : ISearchStrategy
    {
        private int _completedNames;
        private int _totalNames;

        /// <summary>
        /// The web page type.
        /// </summary>
        public enum WebPageType
        {
            KrakDk,
            GulesiderNo
        }

        /// <summary>
        /// URL to one of the enrio's websites providing adresses.
        /// </summary>
        protected string WebPageUrl { get; private set; }

        public EnrioSearchStrategy(WebPageType webpageType)
        {
            if(webpageType == WebPageType.KrakDk)
            {
                WebPageUrl = "http://www.krak.dk/person/resultat/{0}/{1}/{2}";
            }
            else if(webpageType == WebPageType.GulesiderNo)
            { 
                WebPageUrl = "http://www.gulesider.no/person/resultat/{0}/{1}/{2}";
            }
            else
            {
                throw new InvalidOperationException("Unknown web page type");
            }
        }

        public virtual async Task<SearchResult> GetPersonListAsync(string searchPhrase, List<SearchName> searchNameList, IProgress<int> progress)
        {
            var searchResult = new SearchResult();
            _completedNames = 0;

            /*Slower version
             * foreach (var name in searchNameList)
            {
                personList.AddRange(await GetPersonListAsync(searchPhrase, name));
            }*/

            var taskList = (from name in searchNameList select GetPersonListAsync(searchPhrase, name, progress)).ToList();

            while(taskList.Count > 0)
            {              
                var finishedTask = await Task.WhenAny(taskList);

                PersonSearchResult result = finishedTask.GetAwaiter().GetResult();

                if (result.SearchCompletedSuccessfully)
                {
                    searchResult.PersonList.AddRange(result.PersonList);
                }
                else
                {
                    searchResult.FailedSearchNamesList.Add(result.SearchName);
                }

                taskList.Remove(finishedTask);
            }
            
            //Trace.TraceInformation("Request for {0} completed successfully :) {1}", searchPhrase, WebPageUrl);
            
            return searchResult;
        }

        private async Task<PersonSearchResult> GetPersonListAsync(string searchPhrase, SearchName searchName, IProgress<int> progress)
        {
            var personSearchResult = new PersonSearchResult() { SearchName = searchName };
            int totalPageCount = 1;

            try {
                
                //First page
                string personListHtml = await GetPersonHtmlAsync(searchName.Name, searchPhrase);
                personSearchResult.PersonList.AddRange(EnrioHtmlPageParser.GetPersonListFromHtml(personListHtml, searchName));

                //Next pages
                totalPageCount = EnrioHtmlPageParser.GetTotalPageFromHtml(personListHtml);

                var downloadTaskList = new List<Task<string>>();

                for (int currentPage = 2; currentPage <= totalPageCount; currentPage++)
                {
                    personListHtml = await GetPersonHtmlAsync(searchName.Name, searchPhrase, currentPage);
                    personSearchResult.PersonList.AddRange(EnrioHtmlPageParser.GetPersonListFromHtml(personListHtml, searchName));
                }

                personSearchResult.SearchCompletedSuccessfully = true;

                Interlocked.Increment(ref _completedNames);

                if (progress != null)
                {
                    progress.Report((int)Math.Floor((decimal)_completedNames * 100 / _totalNames));
                }
            }
            catch(Exception)
            {
                personSearchResult.SearchCompletedSuccessfully = false;
            }

            return personSearchResult;
        }        

        private async Task<string> GetPersonHtmlAsync(string name, string searchPhrase, int page = 1)
        {
            int tryCount = 0;

            while (true)
            {
                bool useProxy = tryCount % 2 == 0;
                bool setupWebRequestHeaders = tryCount % 3 != 1;

                using (var httpClient = SetupHttpClient(useProxy))
                {
                    if (setupWebRequestHeaders)
                    {
                        SetWebRequestHeaders(httpClient);
                    }

                    var url = GetPersonUrl(name, searchPhrase, page);

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

                                //Page generates 404 errors when no person was found
                                return string.Empty;
                            }

                            //Trace.TraceError("Request failed for {0} with reason {1}", url, response.ReasonPhrase);
                            tryCount++;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        //Trace.TraceError("Request was canceled");
                        tryCount++;
                    }

                    if (tryCount >= 8)
                    {
                        //Trace.TraceError("Search for name {0} failed (searchPhrase:{1})", name, searchPhrase);
                        throw new HttpRequestException(string.Format("Request for name {0} failed", name));
                    }
                }
            }
        }

        protected string GetPersonUrl(string name, string searchPhrase, int page = 1)
        {
            return string.Format(WebPageUrl, name, searchPhrase, page);
        }

        protected HttpClient SetupHttpClient(bool useProxy)
        {
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy("tereny-proxy-vm.trafficmanager.net:21777"),
                UseProxy = useProxy,
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

        private class PersonSearchResult
        {
            public List<Person> PersonList { get; set; } = new List<Person>();

            public SearchName SearchName { get; set; }

            public bool SearchCompletedSuccessfully { get; set; }
        }

        private class WebProxy : IWebProxy
        {
            private string _proxyUrl;

            public WebProxy(string proxyUrl)
            {
                _proxyUrl = proxyUrl;
            }

            public ICredentials Credentials { get; set; }

            public Uri GetProxy(Uri destination)
            {
                return new Uri(_proxyUrl);
            }

            public bool IsBypassed(Uri host)
            {
                return false;
            }
        }
    }
}