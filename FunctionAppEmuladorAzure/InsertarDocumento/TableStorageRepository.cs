using FunctionAppEmuladorAzure.Common;
using FunctionAppEmuladorAzure.Util;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppEmuladorAzure.InsertarDocumento
{
    public interface ITableStorageRepository
    {
        CloudTable GetDataTable(string TableName);
        bool InsertInTableStorage(
            DataTable documentsTable,
            string trakingId);

    }

    public class TableStorageRepository : ITableStorageRepository
    {

        private readonly IConnectionStorageResolver _config;

        public TableStorageRepository(IConnectionStorageResolver config)
        {
            _config = config;
        }


        public CloudTable GetDataTable(string TableName)
        {
            string connectionString = _config.GetCadenaConexionStorage();

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(TableName);

            return table;
        }

        public  bool InsertInTableStorage(DataTable documentsTable, string trakingId)
        {
            // DocumentsToSync
            DocumentEntity documentEntity = new DocumentEntity();
            CloudTable documentsTableStorage = GetDataTable("Documents");

            foreach (DataRow row in documentsTable.Rows)
            {
                documentEntity.PartitionKey = trakingId;
                documentEntity.RowKey = Guid.NewGuid().ToString();
                documentEntity.Timestamp = DateTimeOffset.Now;
                documentEntity.TenantNumber = row["TenantNumber"].ToString();
                documentEntity.IssueDate = row["IssueDate"].ToString();
                documentEntity.DocumentNumberFull = row["DocumentNumberFull"].ToString();
                documentEntity.Amount_Due = row["Amount_Due"].ToString();
                documentEntity.Supplier_PartyName = row["Supplier_PartyName"].ToString();
                documentEntity.Supplier_PartyIdentification = row["Supplier_PartyIdentification"].ToString();
                documentEntity.Customer_PartyName = row["Customer_PartyName"].ToString();
                documentEntity.Customer_PartyIdentification = row["Customer_PartyIdentification"].ToString();
                documentEntity.UUID_CUFE = row["UUID_CUFE"].ToString();

                TableOperation insertOperation = TableOperation.InsertOrMerge(documentEntity);
                TableResult result =  documentsTableStorage.Execute(insertOperation);

            }
            return true;
        }

       
    }
}
