using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class WorkSchedule
    {
        public int ScheduleId { get; set; }
        public int EmployeeId { get; set; }
        public string WorkDays { get; set; } // JSON
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }

        public Employee Employee { get; set; }
    }
}
