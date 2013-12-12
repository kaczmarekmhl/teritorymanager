namespace ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AddressSearch.AdressProvider;
    using AddressSearch.AdressProvider.Entities;
    using AddressSearch.AdressProvider.Filters;
    using AddressSearch.AdressProvider.Filters.PersonFilter;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var addressProvider = new AddressProvider();
            int postCode;

            if (!int.TryParse(args[0], out postCode))
            {
                Console.WriteLine("Usage: ConsoleClient [postCode]");
            }

            List<Person> resultList = addressProvider.getPersonList(postCode);

            WriteResultToFile(String.Format("result{0}_Full.txt", postCode), resultList);

            var filteredResultList = FilterManager.GetFilteredPersonList(resultList, new List<IPersonFilter> {
                new ScandinavianSurname(),
            });
            WriteResultToFile(String.Format("result{0}_Scand.txt", postCode), filteredResultList);


            filteredResultList = FilterManager.GetFilteredPersonList(resultList, new List<IPersonFilter> {
                new ScandinavianSurname(),
                new NonPolishSurnameNonExactName()
            });
            WriteResultToFile(String.Format("result{0}_NonExact.txt", postCode), filteredResultList);


            filteredResultList = FilterManager.GetFilteredPersonList(resultList, new List<IPersonFilter> {
                new ScandinavianSurname(),
                new NonPolishSurname()
            });
            WriteResultToFile(String.Format("result{0}_Polish.txt", postCode), filteredResultList);
            
        }

        public static void WriteResultToFile(string filePath, List<Person> personList)
        {
            List<string> fileLines = new List<string>();

            fileLines.Add("Count: " + personList.Count);

            foreach (var person in personList)
            {
                fileLines.Add(String.Format("{0}; {1};  Address: {2} \t{3}", person.Name, person.Lastname, person.StreetAddress, person.PostCode));
            }

            File.WriteAllLines(filePath, fileLines, Encoding.UTF8);
        }
    }
}
