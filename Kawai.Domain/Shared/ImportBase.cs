using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Shared;

public abstract class ImportBase
{
    public int RowNumber { get; set; }  // untuk tracking baris Excel

    public string Errors { get; set; } = "";

    public bool IsValid()
    {
        Errors = "";

        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();

        // Fix: Use the correct namespace for Validator
        bool valid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results, true);

        if (!valid)
        {
            foreach (var r in results)
            {
                Errors += (r.ErrorMessage ?? "Validation error") + Environment.NewLine;
            }
        }

        return valid;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ImportableAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Property)]
public class ReferenceSheetAttribute : Attribute
{
    public string StoreProcedure { get; }
    public ReferenceSheetAttribute(string sp)
    {
        StoreProcedure = sp;
    }
}

