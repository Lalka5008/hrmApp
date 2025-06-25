using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class TimeTracking
    {
        public int TrackId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan ClockIn { get; set; }
        public TimeSpan ClockOut { get; set; }
        public bool IsLate { get; set; }

        public Employee Employee { get; set; }
    }
}
