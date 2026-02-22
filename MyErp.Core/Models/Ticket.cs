using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class Ticket: Common
    {
        public string Description { get; set; }
        public int TaxRegistrationId { get; set; }
        public string TaxRegistrationName { get; set; }
        public byte[] Attachment { get; set; }
    }
}
