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

    public enum LeadStatus
    {
        Cancel = 0,
        NotInterested = 1,
        Interested = 2,
        responding = 3,
        FollowUp = 4,
        Duplicated = 5,
        NotResponding = 6,
        NoAction = 7,
    }

    public enum EG_KSA
    {
        Egypt = 0,
        KSA = 1
    
    
    }
    public enum WhaOrEmail
    {
        WhatsApp = 0,
        Email = 1
    }


}
