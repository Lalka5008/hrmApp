public class SickLeave
{
    public int SickLeaveId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string MedicalDocNumber { get; set; }
}