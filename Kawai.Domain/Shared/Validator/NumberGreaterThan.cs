using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Shared.Validator;

public class NumberGreaterThan : ValidationAttribute
{
    protected double Compare { get; set; }
    public NumberGreaterThan(double compareValue)
    {
        Compare = compareValue;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value.ToDouble() < Compare)
            return new ValidationResult($"The {validationContext.DisplayName} should greater than {Compare}.", [validationContext.MemberName]);

        return ValidationResult.Success;
    }
}
