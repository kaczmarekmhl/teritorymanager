using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class PersonModelTest
    {      
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public DistrictModel District { get; set; }
        public int PostCode { get; set; }
        public string Door { get; set; }
        public int Tel { get; set; }       
    }
}