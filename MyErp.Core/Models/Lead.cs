using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class Lead : Common
    {
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string? AssignedTo { get; set; }
        public LeadStatus Status { get; set; }
        public EG_KSA Country { get; set; }
        public string? Notes { get; set; }
        public DateTime? DueDate { get; set; }
        public string? FeedBack { get; set; }
        public string? Website { get; set; }
        public string? Source { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastEdited { get; set; } 

    }
}
