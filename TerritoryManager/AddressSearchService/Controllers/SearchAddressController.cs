using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AddressSearchService.Controllers
{
    [Route("api/[controller]")]
    public class SearchAddressController : Controller
    {
        [HttpGet("{country}/{searchPhrases}")]
        public IEnumerable<string> Search(string country, List<string> searchPhrases)
        {
            return new string[] { "value1", "value2" };
        }
    }
}
