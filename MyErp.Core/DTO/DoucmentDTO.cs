using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.DTO
{
    public   class DocumentDTO
    {
        public string Name { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
