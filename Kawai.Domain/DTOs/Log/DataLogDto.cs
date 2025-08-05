using Kawai.Domain.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Domain.DTOs.Log;

public enum DataLogAction
{
    Create, Update, Delete
}

public class DataLogDto
{
    public string DocumentType { get; set; }
    public string ReferenceId { get; set; }
    public string EntityId { get; set; }
    public DataLogAction Action { get; set; }
    public string Activity { get; set; }
    public Dictionary<string, object> Before { get; set; }
    public Dictionary<string, object> After { get; set; }
}
