using hrm.Models;

public class Vacation
{
    public int VacationId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } // "Ежегодный", "Дополнительный" и т.д.
}