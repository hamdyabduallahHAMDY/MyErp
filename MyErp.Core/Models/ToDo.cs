using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    public class ToDo : Common
    {
        public string Title { get; set; }
        public IsChecked ischecked { get; set; }
        public DateTime? LastCheckedAt { get; set; }
    }
}
