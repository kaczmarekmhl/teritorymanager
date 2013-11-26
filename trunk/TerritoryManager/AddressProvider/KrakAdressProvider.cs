namespace AddressSearch.AdressProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using AddressSearch.AdressProvider.Entities;
    using HtmlAgilityPack;

    public class KrakAddressProvider
    {
        WebClient webClient;

        public KrakAddressProvider()
        {
            webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
        }

        public List<Person> getPersonList(int postCode, List<SearchName> searchNameList)
        {
            List<Person> resultList = new List<Person>();

            foreach (var searchName in searchNameList)
            {
                resultList.AddRange(getPersonList(searchName, postCode));
            }

            return resultList;
        }

        protected List<Person> getPersonList(SearchName searchName, int postCode)
        {
            List<Person> resultList = new List<Person>();
            HtmlDocument doc = new HtmlDocument();
            int totalPageCount = 1;

            doc.LoadHtml(getKrakPersonHtml(searchName.Name, postCode));

            totalPageCount = getTotalPageFromHtmlDocument(doc);

            for (int currentPage = 1; currentPage <= totalPageCount; currentPage++)
            {
                if (currentPage > 1)
                {
                    doc.LoadHtml(getKrakPersonHtml(searchName.Name, postCode, currentPage));
                }

                resultList.AddRange(getPersonListFromHtmlDocument(doc, searchName));
            }

            return resultList;
        }

        protected List<Person> getPersonListFromHtmlDocument(HtmlDocument doc, SearchName searchName)
        {
            List<Person> resultList = new List<Person>();
            HtmlNodeCollection vCardNodes = doc.DocumentNode.SelectNodes("//div[@class='hit vcard']");
            string telReal;

            if (vCardNodes == null)
            {
                //TODO handle empty and one person
                return resultList;
            }

            foreach (HtmlNode vCardNode in vCardNodes)
            {
                HtmlNode tel = vCardNode.SelectSingleNode(".//span[contains(@class,'tel')]/a[@href]");
                telReal = tel != null ? tel.GetAttributeValue("href", "").Replace("callto:", "") : "";

                resultList.Add(new Person
                {
                    SearchName = searchName,
                    Name = vCardNode.SelectSingleNode(".//span[@class='given-name']").InnerText,
                    Lastname = vCardNode.SelectSingleNode(".//span[@class='family-name']").InnerText,
                    StreetAddress = vCardNode.SelectSingleNode(".//span[@class='street-address']").InnerText,
                    Locality = vCardNode.SelectSingleNode(".//span[@class='locality']").InnerText,
                    PostCode = vCardNode.SelectSingleNode(".//span[@class='postal-code']").InnerText,
                    TelephoneNumber = telReal,
                    Latitude = vCardNode.SelectSingleNode(".//span[@class='latitude']").InnerText,
                    Longitude = vCardNode.SelectSingleNode(".//span[@class='longitude']").InnerText
                });
            }

            return resultList;
        }

        protected int getTotalPageFromHtmlDocument(HtmlDocument doc)
        {
            var totalPageNode = doc.DocumentNode.SelectSingleNode("//ul[@class='pagination']/li[@class='total']/a");
            return totalPageNode != null ? int.Parse(totalPageNode.InnerText) : 1;
        }

        private string getKrakPersonHtml(string name, int postCode, int page = 1)
        {
            return webClient.DownloadString(getKrakPersonUrl(name, postCode, page));
        }

        private string getKrakPersonUrl(string name, int postCode, int page = 1)
        {
            return string.Format("http://www.krak.dk/person/resultat/{0}/{1}/{2}", name, postCode, page);
        }
    }
}