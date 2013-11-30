using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class DistrictModel
    {
        public string Id { get; set; }
        public string Name { get; set; }  
        public string PostCode { get; set; }
       // public UsersContext BelongsToUser { get; set; }
        public string BelongsToUser { get; set; }
        public ICollection<PersonModel> PersonsFoundInDistrict { get; set; }

        public DistrictModel(string postCode)
        {
            this.PostCode = postCode;
        }

        public DistrictModel()
        {
        }
    }
}