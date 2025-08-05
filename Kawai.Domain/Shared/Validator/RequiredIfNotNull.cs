using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain;

public class RequiredIfNotNull : ValidationAttribute
{
    protected string[] CompareProperty;
    public RequiredIfNotNull(params string[] property)
    {
        CompareProperty = property;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        foreach (var r in CompareProperty)
        {
            var property = validationContext.ObjectType.GetProperty(r);
            if (property == null)
                throw new ArgumentException(@"Property " + r + " not found");

            if (property.GetValue(validationContext.ObjectInstance) != null && value == null)
                return new ValidationResult($"The {validationContext.DisplayName} field is required", new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}
