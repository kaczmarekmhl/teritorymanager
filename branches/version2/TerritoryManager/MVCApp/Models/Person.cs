using MVCApp.Crypt;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebMatrix.WebData;

namespace MVCApp.Models
{
    public class Person
    {
        private string lastname;
        private string streetAddress;
        private string telephoneNumber;
        private string longitude;
        private string latitude;


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
        [NotMapped]
        [StringLength(45)]
        [Display(Name = "Surname")]
        public string Lastname
        {
            get
            {
                return String.IsNullOrEmpty(lastname) ? lastname = Crypter.Decrypt(LastnameCrypt) : lastname;
            }

            set
            {
                lastname = value;
                LastnameCrypt = Crypter.Encrypt(value);
            }
        }

        /// <summary>
        /// Person street address.
        /// </summary>
        [NotMapped]
        [StringLength(45)]
        [Display(Name = "Address")]
        public string StreetAddress
        {
            get
            {
                return String.IsNullOrEmpty(streetAddress) ? streetAddress = Crypter.Decrypt(StreetAddressCrypt) : streetAddress;
            }

            set
            {
                streetAddress = value;
                StreetAddressCrypt = Crypter.Encrypt(value);
            }
        }

        /// <summary>
        /// Person telephone number.
        /// </summary>
        [NotMapped]
        [StringLength(30)]
        [Display(Name = "Tel.")]
        [DisplayFormat(NullDisplayText = "None")]
        public string TelephoneNumber
        {
            get
            {
                return String.IsNullOrEmpty(telephoneNumber) ? telephoneNumber = Crypter.Decrypt(TelephoneNumberCrypt) : telephoneNumber;
            }

            set
            {
                telephoneNumber = value;
                TelephoneNumberCrypt = Crypter.Encrypt(value);
            }
        }

        /// <summary>
        /// Person geographical longitude.
        /// </summary>
        [NotMapped]
        [StringLength(15)]
        public string Longitude
        {
            get
            {
                return String.IsNullOrEmpty(longitude) ? longitude = Crypter.Decrypt(LongitudeCrypt) : longitude;
            }

            set
            {
                longitude = value;
                LongitudeCrypt = Crypter.Encrypt(value);
            }
        }

        /// <summary>
        /// Person geographical latitude.
        /// </summary>
        [NotMapped]
        [StringLength(15)]
        public string Latitude
        {
            get
            {
                return String.IsNullOrEmpty(latitude) ? latitude = Crypter.Decrypt(LatitudeCrypt) : latitude;
            }

            set
            {
                latitude = value;
                LatitudeCrypt = Crypter.Encrypt(value);
            }
        }

        /// <summary>
        /// District that this person belongs to.
        /// </summary>
        public District District { get; set; }

        /// <summary>
        /// The UserProfile.UserId that added this person.
        /// </summary>
        public int? AddedByUserId { get; set; }

        /// <summary>
        /// Is this person selected as a Pole.
        /// </summary>
        [Display(Name = "Is Pole")]
        public bool Selected { get; set; }

        #endregion

        #region Crypted Properties

        /// <summary>
        /// Person surname encrypted.
        /// </summary>
        [Column("x1")]
        public string LastnameCrypt { get; set; }

        /// <summary>
        /// Person street address encrypted.
        /// </summary>
        [Column("x2")]
        public string StreetAddressCrypt { get; set; }

        /// <summary>
        /// Person telephone number encrypted.
        /// </summary>
        [Column("x3")]
        public string TelephoneNumberCrypt { get; set; }

        /// <summary>
        /// Person geographical longitude encrypted.
        /// </summary>
        [Column("x4")]
        public string LongitudeCrypt { get; set; }

        /// <summary>
        /// Person geographical latitude encrypted.
        /// </summary>
        [Column("x5")]
        public string LatitudeCrypt { get; set; }

        #endregion

        #region Constructors

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
            this.AddedByUserId = WebSecurity.CurrentUserId;
        }

        #endregion
    }
}