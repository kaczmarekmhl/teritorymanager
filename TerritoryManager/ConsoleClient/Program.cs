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
    using AddressSearch.AdressProvider.SearchStrategies;

    class Program
    {
        static void Main(string[] args)
        {
            var addressProvider = new AddressProvider(new KrakDkSearchStrategy());

            if (String.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Usage: ConsoleClient [searchPhrase]");
            }

            var searchPhrase = args[0];

            var start = DateTime.Now;

            var progress = new Progress<int>();
            progress.ProgressChanged += (s, e) =>
            {
                Console.Clear();
                Console.WriteLine(e);
            };

            List<Person> resultList = addressProvider.GetPersonListAsync(searchPhrase, progress).Result;
            
            Console.WriteLine(searchPhrase + ": count: " + resultList.Count);
            Console.WriteLine("Time: " + (DateTime.Now - start).TotalSeconds);
            Console.ReadKey();

            WriteResultToFile(String.Format("result{0}_Full.txt", searchPhrase), resultList);
            
            /*var filteredResultList = FilterManager.GetFilteredPersonList(resultList, new List<IPersonFilter> {
                new ScandinavianSurname(),
            });
            WriteResultToFile(String.Format("result{0}_Scand.txt", searchPhrase), filteredResultList);
            */
            /*
            filteredResultList = FilterManager.GetFilteredPersonList(resultList, new List<IPersonFilter> {
                new ScandinavianSurname(),
                new NonPolishSurnameNonExactName()
            });
            WriteResultToFile(String.Format("result{0}_NonExact.txt", postCode), filteredResultList);


            filteredResultList = FilterManager.GetFilteredPersonList(resultList, new List<IPersonFilter> {
                new ScandinavianSurname(),
                new NonPolishSurname()
            });
            WriteResultToFile(String.Format("result{0}_Polish.txt", postCode), filteredResultList);*/
            
        }

        public static void WriteResultToFile(string filePath, List<Person> personList)
        {
            List<string> fileLines = new List<string>();

            fileLines.Add("Count: " + personList.Count);

            foreach (var person in personList.OrderBy(p => p.Name).ThenBy(p => p.Lastname))
            {
                fileLines.Add(String.Format("{0}; {1};  Address: {2} \t{3}", person.Name, person.Lastname, person.StreetAddress, person.PostCode));
            }

            File.WriteAllLines(filePath, fileLines, Encoding.UTF8);
        }
    }
}
