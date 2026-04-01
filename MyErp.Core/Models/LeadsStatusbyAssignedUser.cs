using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class LeadsStatusbyAssignedUser
    {
        public string Name { get; set; }
        public int Cancel { get; set; }
        public int NotInterested { get; set; }
        public int Interested { get; set; }
        public int NotResponding { get; set; }
        public int Responding { get; set; }
        public int FollowUp { get; set; }


    }
}
