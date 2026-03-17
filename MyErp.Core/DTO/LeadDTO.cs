using MyErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.DTO
{
    public class LeadDTO
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

    }
}
