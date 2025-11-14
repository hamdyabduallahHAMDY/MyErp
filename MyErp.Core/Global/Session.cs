using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyErp.Core.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyErp.Core.Global
{
    public static class Session
    {
        public static IUnitOfWork UnitOfWork { get; set; }
    }

}
