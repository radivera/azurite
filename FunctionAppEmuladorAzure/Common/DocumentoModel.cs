using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppEmuladorAzure.Common
{
    public class DocumentoModel
    {
        public string TenantNumber { get; set; }
        public string IssueDate { get; set; }
        public string DocumentNumberFull { get; set; }
        public string Amount_Due { get; set; }
        public string Supplier_PartyName { get; set; }
        public string Supplier_PartyIdentification { get; set; }
        public string Customer_PartyName { get; set; }
        public string Customer_PartyIdentification { get; set; }
        public string UUID_CUFE { get; set; }

    }
}
