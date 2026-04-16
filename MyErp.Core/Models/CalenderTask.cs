using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class CalenderTask : Common
    {
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string allday { get; set; }

        public int ReminderMinutesBefore { get; set; }

        public DateTime ReminderTime { get; set; }

        public bool IsReminderSent { get; set; } = false;
        public string AssignedTo { get; set; }
        public string? MettingLink { get; set; }
        public string? Description { get; set; }
    



    }
}
