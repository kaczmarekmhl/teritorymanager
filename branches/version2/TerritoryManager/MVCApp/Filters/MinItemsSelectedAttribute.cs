using MVCApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCApp.Filters
{
    public class MinItemsSelectedAttribute : ValidationAttribute
    {
        private readonly int _minItems;       
        private TerritoryDb _db = new TerritoryDb();

        public MinItemsSelectedAttribute(int minItems)
            : base("You must select at least {0} items")
        {
            _minItems = minItems;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            if (value == null) return new ValidationResult(errorMessage); 
            if (value != null) 
            {
                var valueCollection = value as ICollection;
                if (valueCollection == null || valueCollection.Count < _minItems)
                {
                    return new ValidationResult(errorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}