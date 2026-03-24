using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class Goal : Common
    {
        public int Num { get; set; }
        public string? AssignedTo { get; set; }
        public string? Description { get; set; }

    }
}
