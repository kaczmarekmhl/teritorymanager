using System.ComponentModel.DataAnnotations;

namespace MVCApp.Models
{
    public class Person
    {
        #region Properties

        /// <summary>
        /// Person  identificator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Person name.
        /// </summary>
        [StringLength(45)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Person surname.
        /// </summary>
        [StringLength(45)]
        [Display(Name = "Surname")]
        public string Lastname { get; set; }

        /// <summary>
        /// Person street address.
        /// </summary>
        [StringLength(45)]
        [Display(Name = "Address")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Person telephone number.
        /// </summary>
        [StringLength(30)]
        [Display(Name = "Tel.")]
        [DisplayFormat(NullDisplayText = "None")]
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Person geographical longitude.
        /// </summary>
        [StringLength(15)]
        public string Longitude { get; set; }

        /// <summary>
        /// Person geographical latitude.
        /// </summary>
        [StringLength(15)]
        public string Latitude { get; set; }

        /// <summary>
        /// Territory identificator.
        /// </summary>
        public Territory Territory { get; set; }

        /// <summary>
        /// Is this person selected as a Pole.
        /// </summary>
        [Display(Name = "Is Pole")]
        public bool Selected { get; set; }

        #endregion

        #region Constructors

        public Person()
        {
        }

        public Person(AddressSearch.AdressProvider.Entities.Person person, Territory territory)
        {
            this.Name = person.Name;
            this.Lastname = person.Lastname;
            this.StreetAddress = person.StreetAddress;
            this.TelephoneNumber = person.TelephoneNumber;
            this.Longitude = person.Longitude;
            this.Latitude = person.Latitude;
            this.Territory = territory;
        }

        #endregion
    }
}