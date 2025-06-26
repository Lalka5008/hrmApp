using hrm.Models;

public class CompanyAsset
{
    public int AssetId { get; set; }
    public string Name { get; set; }
    public int? AssignedTo { get; set; }
    public Employee AssignedEmployee { get; set; }
}