using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class DistrictViewModel
    {
        public List<DistrictModel> PersonDistrictList { get; set; }
        public List<PersonModel> PersonList { get; set; }
        public DistrictModel SearchForDistrict { get; set; }
    }
}