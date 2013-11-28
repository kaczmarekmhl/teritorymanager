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
            var addressProvider = new AddressProvider();

            List<Person> resultList = addressProvider.getPersonList(2100);

            var filterList = new List<IResultFilter> {
                new SurnameResultFilter(),
                new PolishSurnameResultFilter()
            };

            FilterManager.ApplyFilter(resultList, filterList);

            Console.WriteLine(resultList.Count);

            foreach (var person in resultList)
            {
                Console.WriteLine("{0}\t{1}\t {2} \t{3}", person.Name, person.Lastname, person.StreetAddress, person.PostCode);       
            }
        }
    }
}
