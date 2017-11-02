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
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "PersonSurname")]
        public string Lastname { get; set; }

        /// <summary>
        /// Person street address.
        /// </summary>
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "PersonAddress")]
        public string StreetAddress { get; set; }

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
        /// Legacy code crypting entity fields into one crypted string
        /// </summary>
        private class CryptedData
        {
            public string Lastname { get; set; }

            public string StreetAddress { get; set; }

            public string TelephoneNumber { get; set; }

            public string Longitude { get; set; }

            public string Latitude { get; set; }

            #region Serialization and encryption

            [NonSerialized]
            private readonly XmlSerializer _serializer;

            public CryptedData()
            {
                _serializer = new XmlSerializer(this.GetType());
            }

            /// <summary>
            /// Serialize object and return crypted string.
            /// </summary>
            /// <returns>Crypted string.</returns>
            public byte[] SerializeAndCrypt()
            {
                using (var textWriter = new StringWriter())
                {
                    _serializer.Serialize(textWriter, this);
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
                CryptedData deserializedData = (CryptedData)_serializer.Deserialize(new StringReader(serializedData));

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

        public Person(AddressSearch.Data.Person person, District district)
        {
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