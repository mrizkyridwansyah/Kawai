namespace Kawai.Domain.DTOs;

public class ShippingInstructionGridRowDto
{
    public bool Selected { get; set; } = true;
    public bool IsPicking { get; set; }
    public string SINo { get; set; }
    public DateTime? SIDate { get; set; }
    public string PONo { get; set; }
    public int SeqNo { get; set; }
    public string ItemCode { get; set; }
    public string PartNumber { get; set; }
    public string Description { get; set; }
    public string Unit { get; set; }
    public decimal QtyShipping { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal QtyStock { get; set; }
    public decimal QtyPicking { get; set; }
    public string SerialNoFrom { get; set; }
    public string SerialNoTo { get; set; }
    public List<ShippingInstructionGridSerialDto> Serials { get; set; } = [];
}

public class ShippingInstructionGridSerialDto
{
    public string PONo { get; set; }
    public string ItemCode { get; set; }
    public int SeqNo { get; set; }
    public string SerialNo { get; set; }
    public bool IsPicking { get; set; }
    public string Address { get; set; }
    public DateTime? PickingDate { get; set; }
    public string PickingTime { get; set; }
    public string PickingBy { get; set; }
}
