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
    public enum CustomerStatus
    {
        planning = 0 ,
        todo = 1,
        InProgress = 2,
        Done = 3,
    }
    public enum ProjectType
    {
        NOne = 0,
        Development_Odoo = 1,
        Implementation_Odoo = 2,
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

    public enum Type
    {
        Invoice = 0,
        Sales = 1,
        Odoo = 2, 
        Odoo_Development = 3,
        Odoo_Implementation = 4,
        Admin = 5
    }
}
