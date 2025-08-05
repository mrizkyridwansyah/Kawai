using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain;

public class RequiredIfEqual : ValidationAttribute
{
    public string CompareProperty;
    public object Value;
    public RequiredIfEqual(string property, object value)
    {
        CompareProperty = property;
        Value = value;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(CompareProperty);
        if (property == null)
            throw new ArgumentException(@"Property " + CompareProperty + " not found");

        var cmp = property.GetValue(validationContext.ObjectInstance);
        if (cmp == null)
            return ValidationResult.Success;

        bool isTrue = property.GetValue(validationContext.ObjectInstance).ToString() == Value.ToString();

        if (isTrue && (value == null ? true : string.IsNullOrEmpty(value.ToString())))
            return new ValidationResult($"The {validationContext.DisplayName} field is required", new[] { validationContext.MemberName });

        return ValidationResult.Success;
    }
}
