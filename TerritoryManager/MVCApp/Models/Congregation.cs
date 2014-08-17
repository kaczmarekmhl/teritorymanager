using MVCApp.Enums;
using MVCApp.Translate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class Congregation
    {
        #region Properties

        /// <summary>
        /// Dictrict record identificator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Congregation's name.
        /// </summary>
        [StringLength(45)]
        [Display(ResourceType = typeof(Strings), Name = "CongregationName")]
        public string Name { get; set; }

        /// <summary>
        /// Congregation's country.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Country")]
        public Country Country { get; set; }

        #endregion
    }
}