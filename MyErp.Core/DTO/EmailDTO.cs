using Microsoft.AspNetCore.Http;
using MyErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.DTO
{
    public class EmailDTO
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public IFormFile? attachments { get; set; }
        public WhaOrEmail whatsoremail { get; set; }
    }
}
