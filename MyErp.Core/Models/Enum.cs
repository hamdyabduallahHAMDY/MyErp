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
            todo = 0,
            InProgress = 1,
            Closed = 2,
            archived = 3
    }
        public enum IsChecked
        {

        todo = 0,
        InProgress = 1,
        Closed = 2,
        archived = 3
    }


}
