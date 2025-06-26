public class SalaryHistory
{
    public int HistoryId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public decimal OldSalary { get; set; }
    public decimal NewSalary { get; set; }
    public DateTime ChangeDate { get; set; }
}