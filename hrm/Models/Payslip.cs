using hrm.Models;

public class Payslip
{
    public int PayslipId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public string MonthYear { get; set; } // Формат "MM.YYYY"
    public decimal TotalAmount { get; set; }
}