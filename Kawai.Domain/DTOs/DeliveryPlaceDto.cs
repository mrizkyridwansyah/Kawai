using Kawai.Domain.Shared;

namespace Kawai.Domain.DTOs;

public class DeliveryPlaceDto: DataTableDto
{
    public string Trade_Code { get; set; }
    public string Location_Code { get; set; }
    public string Location_Name { get; set; }
    public DateTime? Last_Update { get; set; }
    public string Last_User { get; set; }
    public DateTime? Register_Date { get; set; }
    public string DDLDescription { get; set; }

}
