namespace Kawai.Domain.DTOs;

public class ClassificationDto: DataTableDto
{
    public string SeqNo { get; set; }
    public string TabHeader { get; set; }
    public string TableName { get; set; }
    public string Field_1 { get; set; }
    public string Field_2 { get; set; }

 
}

public class ClassificationTableDto
{
    public string TableName { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    


}
