using hrm.Models;

public class TrainingCourse
{
    public int CourseId { get; set; }
    public string Name { get; set; }
    public int DurationHours { get; set; }
    public string Provider { get; set; }

    public ICollection<EmployeeTraining> EmployeeTrainings { get; set; }
}