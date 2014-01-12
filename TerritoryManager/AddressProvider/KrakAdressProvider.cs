namespace AddressSearch.AdressProvider
{
    using AddressSearch.AdressProvider.Entities;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    class KrakAddressProvider
    {
        public List<Person> getPersonList(string searchPhrase, List<SearchName> searchNameList)
        {
            ConcurrentBag<Person> personList = new ConcurrentBag<Person>();

            Parallel.ForEach(searchNameList, searchName =>
            {
                foreach (var person in getPersonList(searchPhrase, searchName))
                {
                    personList.Add(person);
                }
            });

            return removePersonListDuplicates(personList.ToList());
        }

        protected List<Person> getPersonList(string searchPhrase, SearchName searchName)
        {
            List<Person> resultList = new List<Person>();
            HtmlDocument doc = new HtmlDocument();
            int totalPageCount = 1;

            doc.LoadHtml(getKrakPersonHtml(searchName.Name, searchPhrase));

            totalPageCount = getTotalPageFromHtmlDocument(doc);

            for (int currentPage = 1; currentPage <= totalPageCount; currentPage++)
            {
                if (currentPage > 1)
                {
                    doc.LoadHtml(getKrakPersonHtml(searchName.Name, searchPhrase, currentPage));
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
            int postCode = 0;

            if(vCardNode == null)
            {
                return null;
            }

            //Parse tel
            HtmlNode tel = vCardNode.SelectSingleNode(".//span[contains(@class,'tel')]/a[@href]");
            string telReal = tel != null ? tel.GetAttributeValue("href", "").Replace("callto:", "") : "";
                       
            //Parse post code
            int.TryParse(getSingleNodeText(".//span[@class='postal-code']", vCardNode), out postCode);

            return new Person
                 {
                     SearchName = searchName,
                     Name = getSingleNodeText(".//span[@class='given-name']", vCardNode),
                     Lastname = getSingleNodeText(".//span[@class='family-name']", vCardNode),
                     StreetAddress = getSingleNodeText(".//span[@class='street-address']", vCardNode),
                     Locality = getSingleNodeText(".//span[@class='locality']", vCardNode),
                     PostCode = postCode,
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
            int totalPages = 1;
            var paginationNode = doc.DocumentNode.SelectSingleNode("//ul[@class='pagination']");

            if (paginationNode != null)
            {
                var totalPageNode = paginationNode.SelectSingleNode(".//li[@class='total']/a");

                if (totalPageNode != null)
                {
                    totalPages = int.Parse(totalPageNode.InnerText);
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

            return selectedNode.InnerHtml;
        }

        private string getKrakPersonHtml(string name, string searchPhrase, int page = 1)
        {
            int tryCount = 0;

            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            while (true)
            {
                try
                {
                    return webClient.DownloadString(getKrakPersonUrl(name, searchPhrase, page));
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

        private string getKrakPersonUrl(string name, string searchPhrase, int page = 1)
        {
            return string.Format("http://www.krak.dk/person/resultat/{0}/{1}/{2}", name, searchPhrase, page);
        }

        private List<Person> removePersonListDuplicates(List<Person> personList)
        {
            if (personList == null)
            {
                return null;
            }

            return personList.GroupBy(p => new { p.Name, p.Lastname, p.StreetAddress, p.PostCode }).Select(grp => grp.First()).ToList<Person>();
        }
    }
}