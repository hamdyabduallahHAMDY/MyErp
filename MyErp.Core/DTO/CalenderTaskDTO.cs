using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.DTO
{
    public class CalenderTaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string allday { get; set; }
        public DateTime ReminderTime { get; set; }
        public string AssignedTo { get; set; }
        public string? MettingLink { get; set; }
        public string? Description { get; set; }

    }
}
