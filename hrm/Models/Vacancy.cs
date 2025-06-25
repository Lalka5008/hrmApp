using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{

    public class Vacancy
    {
        public int VacancyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }

        public Department Department { get; set; }

    }
}