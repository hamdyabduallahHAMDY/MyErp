using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.DTO
{
    public class TicketDTO
    {
        public string Description { get; set; }
        public int TaxRegistrationId { get; set; }
        public string TaxRegistrationName { get; set; }
        public IFormFile attachment { get; set; }
    }
}