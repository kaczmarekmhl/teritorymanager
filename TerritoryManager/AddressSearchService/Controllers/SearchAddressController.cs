using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AddressSearchData;
using AddressSearchService.SearchEngine;

namespace AddressSearchService.Controllers
{
    [Route("api/[controller]")]
    public class SearchAddressController : Controller
    {
        [HttpGet]
        public IEnumerable<Person> Search(string country, string searchPhrase)
        {
            var addressProvider = new AddressProvider(new EnrioSearchStrategy(EnrioSearchStrategy.WebPageType.KrakDk));
            var task = addressProvider.GetPersonListAsync("2760");

            task.Wait();

            return task.Result;
            
        }
    }
}
