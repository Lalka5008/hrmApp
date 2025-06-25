using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrm.Models
{
    public class Candidate
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ResumeLink { get; set; }
        public string Status { get; set; }

        public ICollection<Interview> Interviews { get; set; }
        public ICollection<JobOffer> JobOffers { get; set; }
    }
}
