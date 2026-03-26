using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.DTO
{
    public class SendLeadEmailDTO
    {
        public string LeadEmail { get; set; }
        public string LeadName { get; set; }
        public EmailTemplateType TemplateType { get; set; }
    }
    public enum EmailTemplateType
    {
        FirstContact = 1,
        OfferPrice = 2,
        FollowUp = 3,
        MeetingRequest = 4
    }
}
