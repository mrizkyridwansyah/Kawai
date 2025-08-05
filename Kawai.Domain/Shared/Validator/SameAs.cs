using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Shared.Validator;

public class SameAs : ValidationAttribute
{
    protected string Compare { get; set; }
    public SameAs(string property)
    {
        Compare = property;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(Compare);
        if (property == null)
            throw new ArgumentException(@"Property " + Compare + " not found");
        
        if (String.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        string compareValue = property.GetValue(validationContext.ObjectInstance).ToString();
        if (string.Compare(value.ToString(), compareValue, StringComparison.OrdinalIgnoreCase) != 0)
            return new ValidationResult($"The {validationContext.DisplayName} must be same as {Compare}.", [validationContext.MemberName]);

        return ValidationResult.Success;
    }
}
