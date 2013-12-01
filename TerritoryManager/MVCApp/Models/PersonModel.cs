using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AddressSearch.AdressProvider.Entities;
using System.Runtime.Serialization;

namespace MVCApp.Models
{
    public class PersonModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string StreetAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public DistrictModel District { get; set; }
        public string Notes { get; set; }
        public bool RemovedByUser { get; set; }

        public PersonModel()
        {
        }

        public PersonModel(Person person, DistrictModel district)
        {
            this.Name = person.Name;
            this.Lastname = person.Lastname;
            this.StreetAddress = person.StreetAddress;
            this.TelephoneNumber = person.TelephoneNumber;
            this.Longitude = person.Longitude;
            this.Latitude = person.Latitude;
            this.District = district;
        }
    }
}