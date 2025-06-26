using hrm.Models;

public class EmployeeDocument
{
    public int DocId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public string Type { get; set; } // "Паспорт", "Трудовая книжка", "Диплом"
    public string FileUrl { get; set; }
}