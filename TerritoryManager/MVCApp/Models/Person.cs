using MVCApp.Crypt;
using MVCApp.Translate;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Xml.Serialization;
using WebMatrix.WebData;

namespace MVCApp.Models
{
    public class Person
    {
        private CryptedData cryptedData = new CryptedData();

        #region Properties

        /// <summary>
        /// Person  identificator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Person name.
        /// </summary>
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "PersonName")]
        public string Name { get; set; }

        /// <summary>
        /// Person surname.
        /// </summary>
        [NotMapped]
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "PersonSurname")]
        public string Lastname
        {
            get
            {
                return cryptedData.Lastname;
            }

            set
            {
                cryptedData.Lastname = value;
            }
        }

        /// <summary>
        /// Person street address.
        /// </summary>
        [NotMapped]
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "PersonAddress")]
        public string StreetAddress
        {
            get
            {
                return cryptedData.StreetAddress;
            }

            set
            {
                cryptedData.StreetAddress = value;
            }
        }

        /// <summary>
        /// Person post code.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DistrictPostCode")]
        public int PostCode { get; set; }

        /// <summary>
        /// Person telephone number.
        /// </summary>
        [NotMapped]
        [StringLength(30)]
        [Display(ResourceType = typeof(Strings), Name = "PersonTelephoneNum")]
        [DisplayFormat(NullDisplayText = "None")]
        public string TelephoneNumber
        {
            get
            {
                return cryptedData.TelephoneNumber;
            }

            set
            {
                cryptedData.TelephoneNumber = value;
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
                return cryptedData.Longitude;
            }

            set
            {
                cryptedData.Longitude = value;
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
                return cryptedData.Latitude;
            }

            set
            {
                cryptedData.Latitude = value;
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
        [Display(ResourceType = typeof(Strings), Name = "PersonSelected")]
        public bool Selected { get; set; }

        #endregion

        #region Crypted Properties

        /// <summary>
        /// Encrypted private data.
        /// </summary>
        [Column("cx")]
        public byte[] Crypt
        {
            get
            {
                return cryptedData.SerializeAndCrypt();
            }

            set
            {
                cryptedData.DecryptAndDeserialize(value);
            }
        }

        public class CryptedData
        {
            public string Lastname { get; set; }
            public string StreetAddress { get; set; }
            public string TelephoneNumber { get; set; }
            public string Longitude { get; set; }
            public string Latitude { get; set; }

            #region Serialization and encryption

            [NonSerialized]
            protected XmlSerializer serializer;

            public CryptedData()
            {
                serializer = new XmlSerializer(this.GetType());
            }

            /// <summary>
            /// Serialize object and return crypted string.
            /// </summary>
            /// <returns>Crypted string.</returns>
            public byte[] SerializeAndCrypt()
            {
                using (var textWriter = new StringWriter())
                {
                    serializer.Serialize(textWriter, this);
                    return Crypter.Encrypt(textWriter.ToString());
                }
            }

            /// <summary>
            /// Decrypt given string and deserialize data.
            /// </summary>
            /// <param name="cryptedValue">Encrypted string.</param>
            public void DecryptAndDeserialize(byte[] cryptedValue)
            {
                string serializedData = Crypter.Decrypt(cryptedValue);
                CryptedData deserializedData = (CryptedData)serializer.Deserialize(new StringReader(serializedData));

                this.Lastname = deserializedData.Lastname;
                this.StreetAddress = deserializedData.StreetAddress;
                this.TelephoneNumber = deserializedData.TelephoneNumber;
                this.Longitude = deserializedData.Longitude;
                this.Latitude = deserializedData.Latitude;
            }

            #endregion
        }

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

            int postCode = 0;
            int.TryParse(person.PostCode, out postCode);
            this.PostCode = postCode;
        }

        #endregion
    }
}