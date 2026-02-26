using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Models
{
    
        public enum LogType
        {
            login = 0,
            logout = 1,
        }
        public enum Status
        {
            Open = 0,
            InProgress = 1,
            Closed = 2
        }
        public enum IsChecked
        {
           UnChecked = 0,
           Checked = 1 
        }


}
