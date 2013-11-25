namespace ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AddressSearch.AdressProvider;

    class Program
    {
        static void Main(string[] args)
        {
            var nameList = new List<string> { "anna", "michal", "tomasz", "jakub" };

            var addressProvider = new KrakAddressProvider();

            var list = addressProvider.getPersonList(2100, nameList);

            foreach (var person in list)
            {
                Console.WriteLine("{0} {1}; Address: {2}, {3}", person.Name, person.Lastname, person.StreetAddress, person.PostCode);
            }

            Console.ReadKey();
        }
    }
}
