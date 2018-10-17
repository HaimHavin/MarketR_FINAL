using System;
using System.ComponentModel.DataAnnotations;

namespace MarketR.Infrastructure
{
    public class PayReceiveAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Can only be used 1 or -1");
            }
            if (value.ToString() != "-1" || value.ToString() != "1")
            {
                return new ValidationResult("Can only be used 1 or -1");
            }
            return ValidationResult.Success;

        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                ErrorMessage = "Can only be used 1 or -1";
                return false;
            }
            if (value.ToString() != "-1" && value.ToString() != "1")
            {
                ErrorMessage = "Can only be used 1 or -1";
                return false;
            }
            return true;
        }
    }
}