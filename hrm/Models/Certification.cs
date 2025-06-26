using hrm.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Certification
{
    public int CertificationId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public string Name { get; set; }
    public DateTime? ExpiryDate { get; set; }

    [NotMapped]
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate < DateTime.Now;
}