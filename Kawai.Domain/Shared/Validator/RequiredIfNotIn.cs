using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain;

public class RequiredIfNotIn : ValidationAttribute
{
    public string CompareProperty;
    public string[] Values;
    public RequiredIfNotIn(string property, params string[] values)
    {
        CompareProperty = property;
        Values = values;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(CompareProperty);
        if (property == null)
            throw new ArgumentException(@"Property " + CompareProperty + " not found");

        var cmp = property.GetValue(validationContext.ObjectInstance);
        if (cmp == null)
            return ValidationResult.Success;

        bool isTrue = !Values.Contains(property.GetValue(validationContext.ObjectInstance).ToString());

        if (isTrue && (value == null ? true : string.IsNullOrEmpty(value.ToString())))
            return new ValidationResult($"The {validationContext.DisplayName} field is required", new[] { validationContext.MemberName });

        return ValidationResult.Success;
    }
}
