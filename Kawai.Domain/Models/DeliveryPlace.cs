using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class DeliveryPlace
{
    public string Trade_Code { get; set; }
    public string Location_Code { get; set; }
    public string Location_Name { get; set; }
}
