using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain;

public class RequiredIfNotNullOrZero : ValidationAttribute
{
    protected string[] CompareProperty;
    public RequiredIfNotNullOrZero(params string[] property)
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

            var val = property.GetValue(validationContext.ObjectInstance);

            if (val != null && value == null && val.ToString().ToLong() != 0)
                return new ValidationResult($"The {validationContext.DisplayName} field is required", new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}

