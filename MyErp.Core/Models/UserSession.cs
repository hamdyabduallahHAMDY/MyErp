using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class UserSession : Common
    {
        public string Regestrationnumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime datetime { get; set; } = DateTime.Now;
        public LogType logType { get; set; } 
    }
}
