using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class Interview
    {
        public int InterviewId { get; set; }
        public int CandidateId { get; set; }
        public int InterviewerId { get; set; }
        public DateTime Date { get; set; }
        public string Feedback { get; set; }

        public Candidate Candidate { get; set; }
        public Employee Interviewer { get; set; }
    }
}