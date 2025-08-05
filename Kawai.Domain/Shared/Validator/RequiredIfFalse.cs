using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain;

public class RequiredIfFalse : ValidationAttribute
{
    public string CompareProperty;

    public RequiredIfFalse(string property)
    {
        CompareProperty = property;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(CompareProperty);
        if (property == null)
            throw new ArgumentException(@"Property " + CompareProperty + " not found");

        bool isTrue = property.GetValue(validationContext.ObjectInstance).ToBoolean();

        if (!isTrue && (value == null || string.IsNullOrWhiteSpace(value?.ToString())))
            return new ValidationResult($"The {validationContext.DisplayName} field is required", new[] { validationContext.MemberName });

        return ValidationResult.Success;
    }
}
