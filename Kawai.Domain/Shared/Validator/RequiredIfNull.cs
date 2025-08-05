using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain;

public class RequiredIfNull : ValidationAttribute
{
    protected string CompareProperty;
    public RequiredIfNull(string property)
    {
        CompareProperty = property;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(CompareProperty);
        if (property == null)
            throw new ArgumentException(@"Property " + CompareProperty + " not found");

        if (property.GetValue(validationContext.ObjectInstance) == null && value == null)
            return new ValidationResult($"The {validationContext.DisplayName} field is required", new[] { validationContext.MemberName });

        return ValidationResult.Success;
    }
}
