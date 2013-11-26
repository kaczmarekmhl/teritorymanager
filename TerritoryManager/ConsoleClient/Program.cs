namespace ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AddressSearch.AdressProvider;
    using AddressSearch.AdressProvider.Entities;
    using AddressSearch.AdressProvider.Filters;

    class Program
    {
        static void Main(string[] args)
        {
            var personSearchList = new List<SearchName>();

            personSearchList.Add(new SearchName
                {
                    Name = "tomasz",
                    IsMale = true
                });

            var addressProvider = new KrakAddressProvider();

            List<Person> resultList = addressProvider.getPersonList(2100, personSearchList);

            var filterList = new List<IResultFilter> {
                new SurnameResultFilter() 
            };

            FilterManager.ApplyFilter(resultList, filterList);

            foreach (var person in resultList)
            {
                Console.WriteLine("{0} {1}; Address: {2}, {3}", person.Name, person.Lastname, person.StreetAddress, person.PostCode);       
            }
            Console.ReadKey();
        }
    }
}
