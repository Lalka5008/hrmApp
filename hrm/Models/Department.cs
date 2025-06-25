using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int? ManagerId { get; set; }

        public Employee Manager { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
    }
}
