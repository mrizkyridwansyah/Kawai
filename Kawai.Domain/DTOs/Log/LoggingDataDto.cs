using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Domain.DTOs.Log;

public class LoggingDataDto: DataTableDto
{
    public long Id { get; set; }
    public long Date { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string UserAgent { get; set; }
    public string RemoteAddr { get; set; }
    public string Method { get; set; }
    public string RequestPath { get; set; }
    public string Action { get; set; }
    public string Activity { get; set; }
    public string DocumentType { get; set; }
    public string EntityId { get; set; }
    public string ReferenceId { get; set; }
    public string Data { get; set; }
    public long ElapsedMilliseconds { get; set; }
}
