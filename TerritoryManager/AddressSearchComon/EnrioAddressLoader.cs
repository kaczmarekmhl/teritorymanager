using AddressSearchComon.Data;
using AddressSearchComon.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AddressSearchComon
{
    public class EnrioAddressLoader
    {
        /// <summary>
        /// URL to one of the enrio's websites providing adresses.
        /// </summary>
        protected string WebPageUrl { get; private set; }

        private bool enableProxy;

        public EnrioAddressLoader(EnrioWebPageType webpageType, bool enableProxy = false)
        {
            if (webpageType == EnrioWebPageType.KrakDk)
            {
                WebPageUrl = "http://www.krak.dk/person/resultat/{0}/{1}/{2}";
            }
            else if (webpageType == EnrioWebPageType.GulesiderNo)
            {
                WebPageUrl = "http://www.gulesider.no/person/resultat/{0}/{1}/{2}";
            }
            else
            {
                throw new InvalidOperationException("Unknown web page type");
            }

            this.enableProxy = enableProxy;
        }

        /// <summary>
        /// Retrieves person list for the given person name and search phrase.
        /// </summary>
        /// <param name="searchPhrase">The search phrase.</param>
        /// <param name="searchName">The person name.</param>
        /// <returns>List of people.</returns>
        public async Task<List<Person>> GetPersonListAsync(string searchPhrase, SearchName searchName)
        {
            var personSearchResult = new List<Person>();
            int totalPageCount = 1;

            //First page
            string personListHtml = await GetPersonHtmlAsync(searchName.Name, searchPhrase);
            personSearchResult.AddRange(EnrioHtmlPageParser.GetPersonListFromHtml(personListHtml, searchName));

            //Next pages
            totalPageCount = EnrioHtmlPageParser.GetTotalPageFromHtml(personListHtml);

            var downloadTaskList = new List<Task<string>>();

            for (int currentPage = 2; currentPage <= totalPageCount; currentPage++)
            {
                personListHtml = await GetPersonHtmlAsync(searchName.Name, searchPhrase, currentPage);
                personSearchResult.AddRange(EnrioHtmlPageParser.GetPersonListFromHtml(personListHtml, searchName));
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
                Proxy = new WebProxy("http://tereny-proxy-vm.trafficmanager.net:21777"),
                UseProxy = this.enableProxy && useProxy,
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
