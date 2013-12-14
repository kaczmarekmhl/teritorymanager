using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class Territory
    {
        #region Properties

        /// <summary>
        /// Territory record identificator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Territory number.
        /// </summary>
        [StringLength(20)]
        public string Number { get; set; }

        /// <summary>
        /// Territory name.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string Name { get; set; }  

        /// <summary>
        /// First post code of territory.
        /// </summary>
        [Required]
        [Display(Name="First Post Code")]
        public int PostCodeFirst  { get; set; }

        /// <summary>
        /// Last post code of territory
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
        /// The UserProfile.UserId that is assigned to this territory.
        /// </summary>
        public int? AssignedToUserId { get; set; }

        /// <summary>
        /// The UserProfile that is assigned to this territory.
        /// </summary>
        public virtual UserProfile AssignedTo{ get; set; }


        /// <summary>
        /// The list of Person found in this territory.
        /// </summary>
        public ICollection<Person> PersonList { get; set; }

        #endregion

        #region Constructors

        public Territory(string postCode)
        {
            this.PostCode = postCode;
        }

        public Territory()
        {
        }

        #endregion
    }
}