using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KmlGenerator
{
    public class ExternalKmlProvider
    {
        /// <summary>
        /// Loads kml from external service for given post code.
        /// </summary>
        /// <param name="postCode">Danish post code.</param>
        /// <returns>Kml in string.</returns>
        public static string LoadKml(int postCode)
        {
            if (postCode <= 0)
            {
                return null;
            }

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            string kml = webClient.DownloadString(getUrl(postCode));

            if (String.IsNullOrEmpty(kml) || !KmlHelper.isValidKml(kml))
            {
                return null;
            }

            return kml;
        }

        protected static string getUrl(int postCode)
        {
            return string.Format("http://geo.oiorest.dk/postnumre/{0}/grænse.xml", postCode);
        }
    }
}
