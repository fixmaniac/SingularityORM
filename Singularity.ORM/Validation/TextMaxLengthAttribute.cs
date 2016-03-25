using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TextMaxLengthAttribute : BusinessValidationAttribute
    {
        public readonly int MaxLength;

        public TextMaxLengthAttribute(int maxLength)
        {
            this.MaxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.GetType() != typeof(string))
                return ValidationResult.Success;
            base.ErrorMessage = "Zbyt długi ciąg tekstowy.";
            return ((value as string).Length > this.MaxLength)
                     ? new ValidationResult(ErrorMessage)
                     : ValidationResult.Success;

        }
    }
}
