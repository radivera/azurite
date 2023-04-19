using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppEmuladorAzure.InsertarDocumento
{
    public class DocumentEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }
        public string TenantNumber { get; set; }
        public string IssueDate { get; set; }
        public string DocumentNumberFull { get; set; }
        public string Amount_Due { get; set; }
        public string Supplier_PartyName { get; set; }
        public string Supplier_PartyIdentification { get; set; }
        public string Customer_PartyName { get; set; }
        public string Customer_PartyIdentification { get; set; }
        public string UUID_CUFE { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            this.TenantNumber = properties["TenantNumber"].StringValue;
            this.IssueDate = properties["IssueDate"].StringValue;
            this.DocumentNumberFull = properties["DocumentNumberFull"].StringValue;
            this.Amount_Due = properties["Amount_Due"].StringValue;
            this.Supplier_PartyName = properties["Supplier_PartyName"].StringValue;
            this.Supplier_PartyIdentification = properties["Supplier_PartyIdentification"].StringValue;
            this.Customer_PartyName = properties["Customer_PartyName"].StringValue;
            this.Customer_PartyIdentification = properties["Customer_PartyIdentification"].StringValue;
            this.UUID_CUFE = properties["UUID_CUFE"].StringValue;
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = TableEntity.WriteUserObject(this, operationContext);

            return properties;
        }
    }
}
