using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class JobOffer
    {
        public int OfferId { get; set; }
        public int CandidateId { get; set; }
        public int PositionId { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }

        public Candidate Candidate { get; set; }
        public Position Position { get; set; }
    }
}
