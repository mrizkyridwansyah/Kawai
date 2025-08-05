using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Log;

public class DataLog
{
    public long Date { get; set; }

    public string UserId { get; set; }

    public string FullName { get; set; }

    public string UserAgent { get; set; }

    public string RemoteAddr { get; set; }

    public string Method { get; set; }

    public string RequestPath { get; set; }

    /// <summary>
    /// CREATE, UPDATE, DELETE
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// BISA DIISI SESUAI AKTIFITAS NYA APA KALO GA DIKIRIM AKAN SAMA DENGAN KOLOM ACTION
    /// </summary>
    public string Activity { get; set; }

    public string DocumentType { get; set; }

    public string EntityId { get; set; }

    public string ReferenceId { get; set; }

    public string Data { get; set; }

    public long ElapsedMilliseconds { get; set; }
}