using MyErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = MyErp.Core.Models.Type;

namespace MyErp.Core.DTO
{
    public class ToDoDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? AssignedTo { get; set; }
        public string? CreatedBytodo { get; set; }
        public int ischecked { get; set; }
        public DateTime? LastCheckedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Daily { get; set; }
        public ProjectType? ProjectType { get; set; }
        public string? CustomerName { get; set; }
    }
    public class ToDoDTOUpdate
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? AssignedTo { get; set; }
        public string? CreatedBytodo { get; set; }
        public int ischecked { get; set; }
        public DateTime? LastCheckedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Daily { get; set; }
        public ProjectType? ProjectType { get; set; }
        public string? CustomerName { get; set; }
    }
}
