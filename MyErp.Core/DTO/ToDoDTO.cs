using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.DTO
{
    public class ToDoDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? AssignedTo { get; set; }
        public string? CreatedBy { get; set; }
        public int ischecked { get; set; }
    }
}
