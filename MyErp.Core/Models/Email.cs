using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class Email : Common
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? attachments { get; set; }
        public string? WhatAppMessage { get; set; }
        public WhaOrEmail whatsoremail { get; set; }
    }
}
