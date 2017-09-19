﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class NotEqualToAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public NotEqualToAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (string)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (string)property.GetValue(validationContext.ObjectInstance);

            if (string.Equals(currentValue,comparisonValue))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
