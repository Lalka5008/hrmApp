using hrm.Models;

public class Tax
{
    public int TaxId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public decimal IncomeTax { get; set; }
    public decimal PensionContribution { get; set; }
}