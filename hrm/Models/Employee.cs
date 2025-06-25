using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }

        public Department Department { get; set; }
        public Position Position { get; set; }

        // Navigation properties
        public ICollection<WorkSchedule> WorkSchedules { get; set; }
        public ICollection<TimeTracking> TimeTrackings { get; set; }
        // ... Add other navigation properties for related entities
    }
}
