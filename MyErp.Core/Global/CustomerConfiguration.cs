using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyErp.Core.Models;

namespace MyErp.Core.Global
{
    public class CustomerConfiguration
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ActivityCode { get; set; }

        public string IssuerId { get; set; }

        public string IssuerName { get; set; }

        public string TokenPin { get; set; }

        public string PosSerial { get; set; }

        public string PosVersion { get; set; }

        public string PosDocumentVersion { get; set; }

        public string DocumentTypeVersion { get; set; }

        public string InvoiceLicenseType { get; set; }

        public string PosClientId { get; set; }

        public string PosClientSecret { get; set; }

       // public EnvironmentPortal EnvironmentPortal { get; set; }

        public string DataBaseType { get; set; }

        public static string ConnectionString { get; set; }

        public bool IsSign { get; set; }
    }
}
