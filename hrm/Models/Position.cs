using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class Position
    {
        public int PositionId { get; set; }
        public string Title { get; set; }
        public string SalaryRange { get; set; }
        public bool IsLeadership { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<JobOffer> JobOffers { get; set; }
    }
}
