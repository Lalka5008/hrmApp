using hrm.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class EmployeeTraining
{
    public int TrainingId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public int CourseId { get; set; }
    public TrainingCourse Course { get; set; }
    public DateTime? CompletionDate { get; set; }

    [NotMapped]
    public bool IsCompleted => CompletionDate.HasValue;
}