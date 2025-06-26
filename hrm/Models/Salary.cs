using System.ComponentModel.DataAnnotations.Schema;

public class Salary
{
    public int SalaryId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal Bonus { get; set; }

    [NotMapped]
    public decimal Total => BaseAmount + Bonus;
}