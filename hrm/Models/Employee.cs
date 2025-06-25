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

        // Названия вместо объектов (для простоты)
        public string DepartmentName { get; set; }
        public string PositionTitle { get; set; }
    }
}