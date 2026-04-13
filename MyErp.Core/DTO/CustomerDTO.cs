using MyErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = MyErp.Core.Models.Type;

namespace MyErp.Core.DTO
{
    public class CustomerDTO
    {
        public string Name { get; set; }
        public string? TaxRegistrationNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? AnyDesk { get; set; }
        public Type Type { get; set; }
        public string? POC { get; set; }
        public string? allowance { get; set; }
        public string? Description { get; set; }
        public CustomerStatus? customerStatus { get; set; }

    }
}
