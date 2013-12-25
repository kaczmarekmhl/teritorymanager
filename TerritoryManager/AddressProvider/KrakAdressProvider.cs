namespace AddressSearch.AdressProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using AddressSearch.AdressProvider.Entities;
    using HtmlAgilityPack;

    public class KrakAddressProvider : IAddressProvider
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

            resultList = removePersonListDuplicates(resultList);

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
            HtmlNodeCollection vCardNodes = doc.DocumentNode.SelectNodes("//div[@class='hit vcard']");
            List<Person> personList = new List<Person>();

            if (vCardNodes == null)
            {
                var vCardSingleNode = doc.DocumentNode.SelectSingleNode("//div[@class='person-info vcard column']");
                var person = parseSinglePerson(vCardSingleNode, searchName);

                if(person != null)
                {
                    personList.Add(person);
                }

                return personList;
            }

            return parsePersonList(vCardNodes, searchName);
        }

        protected Person parseSinglePerson(HtmlNode vCardNode, SearchName searchName)
        {
            if(vCardNode == null)
            {
                return null;
            }

            HtmlNode tel = vCardNode.SelectSingleNode(".//span[contains(@class,'tel')]/a[@href]");
            string telReal = tel != null ? tel.GetAttributeValue("href", "").Replace("callto:", "") : "";

            return new Person
                 {
                     SearchName = searchName,
                     Name = getSingleNodeText(".//span[@class='given-name']", vCardNode),
                     Lastname = getSingleNodeText(".//span[@class='family-name']", vCardNode),
                     StreetAddress = getSingleNodeText(".//span[@class='street-address']", vCardNode),
                     Locality = getSingleNodeText(".//span[@class='locality']", vCardNode),
                     PostCode = getSingleNodeText(".//span[@class='postal-code']", vCardNode),
                     TelephoneNumber = telReal,
                     Latitude = getSingleNodeText(".//span[@class='latitude']", vCardNode),
                     Longitude = getSingleNodeText(".//span[@class='longitude']", vCardNode)
                 };
        }

        /// <summary>
        /// Parses krak person list
        /// </summary>
        protected List<Person> parsePersonList(HtmlNodeCollection vCardNodes, SearchName searchName)
        {
            List<Person> resultList = new List<Person>();

            foreach (HtmlNode vCardNode in vCardNodes)
            {
                var person = parseSinglePerson(vCardNode, searchName);

                if(person != null)
                {
                    resultList.Add(person);
                }
            }

            return resultList;
        }

        protected int getTotalPageFromHtmlDocument(HtmlDocument doc)
        {
            var totalPageNode = doc.DocumentNode.SelectSingleNode("//ul[@class='pagination']/li[@class='total']/a");
            return totalPageNode != null ? int.Parse(totalPageNode.InnerText) : 1;
        }

        private string getSingleNodeText(string xpath, HtmlNode node)
        {
            HtmlNode selectedNode = node.SelectSingleNode(xpath);

            if (selectedNode == null)
            {
                return "Not found";
            }

            return selectedNode.InnerHtml;
        }

        private string getKrakPersonHtml(string name, int postCode, int page = 1)
        {
            int tryCount = 0;

            while (true)
            {
                try
                {
                    return webClient.DownloadString(getKrakPersonUrl(name, postCode, page));
                }
                catch (WebException webException)
                {
                    tryCount++;
                    System.Threading.Thread.Sleep(1000);

                    if(tryCount >= 5)
                    {
                        throw webException;
                    }
                }
                catch(Exception)
                {
                    throw;
                }
            }
        }

        private string getKrakPersonUrl(string name, int postCode, int page = 1)
        {
            return string.Format("http://www.krak.dk/person/resultat/{0}/{1}/{2}", name, postCode, page);
        }

        private List<Person> removePersonListDuplicates(List<Person> personList)
        {
            if (personList == null)
            {
                return null;
            }

            return personList.GroupBy(p => new { p.Name, p.Lastname, p.StreetAddress }).Select(grp => grp.First()).ToList<Person>();
        }
    }
}