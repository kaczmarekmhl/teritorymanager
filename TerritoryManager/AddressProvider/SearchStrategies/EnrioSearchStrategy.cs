namespace AddressSearch.AdressProvider.SearchStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressSearchComon.Types;
    using AddressSearchComon;
    using AddressSearchComon.Data;

    public class EnrioSearchStrategy : ISearchStrategy
    {
        private int _completedNames;
        private int _totalNames;

        EnrioWebPageType webpageType;

        public EnrioSearchStrategy(EnrioWebPageType webpageType)
        {
            this.webpageType = webpageType;
        }

        public virtual async Task<List<Person>> GetPersonListAsync(string searchPhrase, List<SearchName> searchNameList, IProgress<int> progress)
        {
            var personList = new List<Person>();
            _completedNames = 0;
            _totalNames = searchNameList.Count;

            /*Slower version
             * foreach (var name in searchNameList)
            {
                personList.AddRange(await GetPersonListAsync(searchPhrase, name));
            }*/

            var enrioAddressLoader = new EnrioAddressLoader(this.webpageType, true);

            var taskList = (from name in searchNameList select enrioAddressLoader.GetPersonListAsync(searchPhrase, name)).ToList();

            while (taskList.Count > 0)
            {
                var task = await Task.WhenAny(taskList);
                personList.AddRange(task.GetAwaiter().GetResult());
                taskList.Remove(task);

                Interlocked.Increment(ref _completedNames);

                if (progress != null)
                {
                    progress.Report((int)Math.Floor((decimal)_completedNames * 100 / _totalNames));
                }
            }
            
            if (_completedNames != _totalNames)
            {
                throw new Exception("All names have not been processed");
            }

            Trace.TraceInformation("Request for {0} completed successfully :) {1}", searchPhrase, webpageType);
            
            return personList;
        }        
    }
}