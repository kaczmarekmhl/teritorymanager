﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace MVCApp.Models
{
    public class Person
    {
        public int Id { get; set; }

        [StringLength(45)]
        [Display(Name="Name")]
        public string Name { get; set; }

        [StringLength(45)]
        [Display(Name="Surname")]
        public string Lastname { get; set; }

        [StringLength(45)]
        [Display(Name="Address")]
        public string StreetAddress { get; set; }

        [StringLength(30)]
        [Display(Name="Tel.")]
        [DisplayFormat(NullDisplayText="None")]
        public string TelephoneNumber { get; set; }

        [StringLength(15)]
        public string Longitude { get; set; }

        [StringLength(15)]
        public string Latitude { get; set; }

        public District District { get; set; }
        public string Notes { get; set; }
        public bool RemovedByUser { get; set; }

        public Person()
        {
        }

        public Person(AddressSearch.AdressProvider.Entities.Person person, District district)
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