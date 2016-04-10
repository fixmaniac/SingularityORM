using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;


namespace Singularity.ORM.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MendatoryAttribute : BusinessValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            base.ErrorMessage = "Wartość pola jest wymagana.";
            if (value != null
                && value.GetType().IsEnum
                && value.ToString().Equals("empty"))
                return new ValidationResult(ErrorMessage);
            else if (value != null
                && value.GetType() == typeof(String)
                && ((String)value) == String.Empty)
                return new ValidationResult(ErrorMessage);
            else if (value != null
                && value.GetType().IsEnum
                && value.ToString().Equals("0"))
                return new ValidationResult(ErrorMessage);
            else if (value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            else
                return ValidationResult.Success;
        }
    }
}
