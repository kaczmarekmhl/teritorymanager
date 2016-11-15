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
        private readonly CryptedData _cryptedData = new CryptedData();

        #region Properties

        private string _lastName;
        private string _streetAddress;
        private string _telephoneNumber;
        private string _longitude;
        private string _latitude;

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
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "PersonSurname")]
        public string Lastname
        {
            get
            {
                return MigrationVersion > 0 ? _lastName : _cryptedData.Lastname;
            }

            set
            {
                _lastName = value;
                _cryptedData.Lastname = value;
            }
        }

        /// <summary>
        /// Person street address.
        /// </summary>
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "PersonAddress")]
        public string StreetAddress
        {
            get
            {
                return MigrationVersion > 0 ? _streetAddress : _cryptedData.StreetAddress;
            }

            set
            {
                _streetAddress = value;              
                _cryptedData.StreetAddress = value;                
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
        [StringLength(30)]
        [Display(ResourceType = typeof(Strings), Name = "PersonTelephoneNum")]
        [DisplayFormat(NullDisplayText = " ")]
        public string TelephoneNumber
        {
            get
            {
                return MigrationVersion > 0 ? _telephoneNumber : _cryptedData.TelephoneNumber;
            }

            set
            {
                _telephoneNumber = value;
                _cryptedData.TelephoneNumber = value;                
            }
        }

        /// <summary>
        /// Person geographical longitude.
        /// </summary>
        [StringLength(15)]
        public string Longitude
        {
            get
            {
                return MigrationVersion > 0 ? _longitude : _cryptedData.Longitude;
            }

            set
            {              
                _longitude = value;               
                _cryptedData.Longitude = value;                
            }
        }

        /// <summary>
        /// Person geographical latitude.
        /// </summary>
        [StringLength(15)]
        public string Latitude
        {
            get
            {
                return MigrationVersion > 0 ? _latitude : _cryptedData.Latitude;
            }

            set
            {
                _latitude = value;              
                _cryptedData.Latitude = value;                
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

        /// <summary>
        /// Is this person added manually.
        /// </summary>
        public bool Manual { get; set; }

        /// <summary>
        /// Is this person marked as comming from new search update.
        /// </summary>
        public bool SearchUpdate { get; set; }

        /// <summary>
        /// Person name.
        /// </summary>
        [StringLength(100)]
        [Display(ResourceType = typeof(Strings), Name = "PersonRemarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Should this person be not visited.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "PersonDoNotVisit")]
        public bool DoNotVisit { get; set; }

        /// <summary>
        /// When this person required to be not visited
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(Strings), Name = "PersonDoNotVisitReportDate")]
        public DateTime? DoNotVisitReportDate { get; set; }

        /// <summary>
        /// Indicates wheather this person is visited by other publisher.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "PersonIsVisitedByOtherPublisher")]
        public bool IsVisitedByOtherPublisher { get; set; }

        /// <summary>
        /// The other publisher that is visiting this person.
        /// </summary>
        [StringLength(100)]
        [Display(ResourceType = typeof(Strings), Name = "PersonVisitingPublisher")]
        public string VisitingPublisher { get; set; }

        [NotMapped]
        public string PostCodeFormat
        {
            get
            {
                return PostCode.ToString().PadLeft(4, '0');
            }
        }

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
                return MigrationVersion > 0 ? null : _cryptedData.SerializeAndCrypt();
            }

            set
            {
                _cryptedData.DecryptAndDeserialize(value);
            }
        }

        /// <summary>
        /// Version of people migration
        /// </summary>
        public int MigrationVersion { get; set; }

        public void Migrate(int migrationVersion)
        {
            this.MigrationVersion = migrationVersion;

            _lastName = _cryptedData.Lastname;
            _streetAddress = _cryptedData.StreetAddress;
            _latitude = _cryptedData.Latitude;
            _longitude = _cryptedData.Longitude;
            _telephoneNumber = _cryptedData.TelephoneNumber;
        }

        public class CryptedData
        {
            public string Lastname { get; set; }

            //Will be removed
            public string StreetAddress { get; set; }

            public string TelephoneNumber { get; set; }

            //Will be removed
            public string Longitude { get; set; }

            //Will be removed
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
                if (cryptedValue == null)
                {
                    return;
                }

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
            this.MigrationVersion = MVCApp.Hubs.MigrationHub.CurrentMigrationVersion;
            this.Name = person.Name;
            this.Lastname = person.Lastname;
            this.StreetAddress = person.StreetAddress;

            this.TelephoneNumber = person.TelephoneNumber;
            this.Longitude = person.Longitude;
            this.Latitude = person.Latitude;
            this.District = district;
            this.AddedByUserId = WebSecurity.CurrentUserId;
            this.PostCode = person.PostCode;
        }

        #endregion
    }
}