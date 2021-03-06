﻿namespace MapLibrary
{
    using System;
    using System.Net;
    using System.Text;

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

            if (String.IsNullOrEmpty(kml) || !KmlValidator.isValid(kml))
            {
                return null;
            }

            return kml;
        }

        protected static string getUrl(int postCode)
        {
            return string.Format("http://geo.oiorest.dk/postnumre/{0}/grænse", postCode);
        }
    }
}
