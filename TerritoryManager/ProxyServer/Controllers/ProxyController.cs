using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ProxyServer.Controllers
{
    public class ProxyController : Controller
    {
        private const string testHealthUrl = "http://www.krak.dk/";

        public ActionResult ProcessRequest()
        {
            // example of requested url: http://tereny-proxy-northeurope.azurewebsites.net/proxy?url=http://www.krak.dk
            string url = HttpContext.Request.QueryString["url"];

            if (string.IsNullOrEmpty(url))
            {
                return HttpNotFound("URL is null or empty.");
            }

            string content = grabContent(url);
            return Content(content);
        }

        public ActionResult HealthTest()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri(testHealthUrl));

            req.Timeout = 5000;

            using (var rsp = (HttpWebResponse)req.GetResponse())
            {
                if (rsp != null && HttpStatusCode.OK == rsp.StatusCode)
                {
                    // HTTP = 200 - Internet connection available, server online
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
        }

        /// <see>http://stackoverflow.com/questions/3447589/copying-http-request-inputstream</see>
        private string grabContent(string url)
        {
            string content;

            // Create a request for the URL. 		
            var req = HttpWebRequest.Create(url);
            req.Method = HttpContext.Request.HttpMethod;

            //-- No need to copy input stream for GET (actually it would throw an exception)
            if (req.Method != "GET")
            {
                req.ContentType = HttpContext.Request.ContentType;

                Request.InputStream.Position = 0;  //***** THIS IS REALLY IMPORTANT GOTCHA

                var requestStream = HttpContext.Request.InputStream;
                Stream webStream = null;
                try
                {
                    //copy incoming request body to outgoing request
                    if (requestStream != null && requestStream.Length > 0)
                    {
                        req.ContentLength = requestStream.Length;
                        webStream = req.GetRequestStream();
                        requestStream.CopyTo(webStream);
                    }
                }
                finally
                {
                    if (null != webStream)
                    {
                        webStream.Flush();
                        webStream.Close();
                    }
                }
            }

            // If required by the server, set the credentials.
            req.Credentials = CredentialCache.DefaultCredentials;
            req.Timeout = 10000;           

            // No more ProtocolViolationException!
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                // Display the status.
                //Console.WriteLine(response.StatusDescription);

                // Get the stream containing content returned by the server.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content. 
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }
    }
}
