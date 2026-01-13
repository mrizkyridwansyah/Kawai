namespace Kawai.Domain.DTOs;

public class ImportHistoryDto : DataTableDto
{
    public string Id {get;set;}
    public string Key {get;set;}
    public string Template {get;set;}
    public string Status {get;set;}
    public string FileName {get;set;}
    public string ContentType {get;set; }
    public long ProcessDuration {get;set;}
    public long SizeFile {get;set;}
    public int RowsCount {get;set;}
    public int ValidRowsCount {get;set;}
    public int InvalidRowsCount {get;set;}
    public string Date {get;set;}
    public string User {get;set;}
}