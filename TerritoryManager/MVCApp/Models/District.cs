using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class District
    {
        #region Properties

        /// <summary>
        /// Dictrict record identificator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dictrict number.
        /// </summary>
        [StringLength(20)]
        public string Number { get; set; }

        /// <summary>
        /// Dictrict name.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string Name { get; set; }  

        /// <summary>
        /// First post code of dictrict.
        /// </summary>
        [Required]
        [Display(Name="First Post Code")]
        public int PostCodeFirst  { get; set; }

        /// <summary>
        /// Last post code of dictrict
        /// </summary>
        [Display(Name = "Last Post Code")]
        public int? PostCodeLast { get; set; }
        
        /// <summary>
        /// Post code range used for display.
        /// </summary>
        [NotMapped]
        [Display(Name = "Post Code")]
        public string PostCode 
        { 
            get
            {
                if (!PostCodeLast.HasValue || PostCodeFirst == PostCodeLast.Value)
                {
                    return PostCodeFirst.ToString();
                }
                else
                {
                    return string.Format("{0}-{1}", PostCodeFirst, PostCodeLast);
                }
            }

            private set {} 
        }

        /// <summary>
        /// The UserProfile.UserId that is assigned to this dictrict.
        /// </summary>
        public int? AssignedToUserId { get; set; }

        /// <summary>
        /// The UserProfile that is assigned to this dictrict.
        /// </summary>
        public virtual UserProfile AssignedTo{ get; set; }


        /// <summary>
        /// The list of Person found in this dictrict.
        /// </summary>
        public ICollection<Person> PersonList { get; set; }

        #endregion

        #region Constructors

        public District(string postCode)
        {
            this.PostCode = postCode;
        }

        public District()
        {
        }

        #endregion
    }
}