using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AddressSearch.AdressProvider.CustomWebClient
{
    /// <summary>
    /// A Cookie-aware WebClient that will store authentication cookie information and persist it through subsequent requests.
    /// </summary>
    public class CookieAwareWebClient : WebClient
    {
        // Absolute Uri to proxy servers managed by Azure Traffic Manager
        private const string proxyAddressAndPath = "http://tereny-proxy.trafficmanager.net/proxy?url=";

        //Properties to handle implementing a timeout
        private int? _timeout = null;
        public int? Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        //A CookieContainer class to house the Cookie once it is contained within one of the Requests
        public CookieContainer CookieContainer { get; private set; }

        //Constructor
        public CookieAwareWebClient()
        {
           CookieContainer = new CookieContainer();
        }

        //Method to handle setting the optional timeout (in milliseconds)
        public void SetTimeout(int timeout)
        {
            _timeout = timeout;
        }

        //This handles using and storing the Cookie information as well as managing the Request timeout
        protected override WebRequest GetWebRequest(Uri address)
        {
            // example of uri that proxy can process: http://tereny-proxy-northeurope.azurewebsites.net/proxy?url=http://www.krak.dk
            Uri uriProxy = new Uri(proxyAddressAndPath + address.AbsoluteUri);

            //Handles the CookieContainer
            var request = (HttpWebRequest)base.GetWebRequest(uriProxy);
            request.CookieContainer = CookieContainer;
            request.KeepAlive = true;

            //Sets the Timeout if it exists
            if (_timeout.HasValue)
            {
                request.Timeout = _timeout.Value;
            }

            return request;
        }
    }
}
