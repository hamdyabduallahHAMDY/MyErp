using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class ToDo : Common
    {
        public  string Title { get; set; }
        public string? Description { get; set; }
        public string? AssignedTo { get; set; }
        public string? CreatedBy { get; set; }
        public IsChecked ischecked { get; set; }
        public DateTime? LastCheckedAt { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Daily { get; set; }





    }
}
