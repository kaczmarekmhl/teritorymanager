using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class DistrictModel
    {   
        [Key]
        [StringLength(10)]
        public string Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }  

        [Required]
        [StringLength(4)]
        [Display(Name="Code")]
        public string PostCode { get; set; }

        public virtual UserProfile BelongsToUser { get; set; }

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