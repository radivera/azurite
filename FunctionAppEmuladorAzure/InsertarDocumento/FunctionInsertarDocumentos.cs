using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using FunctionAppEmuladorAzure.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FunctionAppEmuladorAzure.InsertarDocumento
{
    public class FunctionInsertarDocumentos
    {
        private const int _Offset = -5;
        private DataTable documentsTable;
        private readonly ITableStorageRepository _tableStorageRepository;
        private IConfiguration _configuration;
        public FunctionInsertarDocumentos(ITableStorageRepository tableStorageRepository, IConfiguration configuration)
        {
            _tableStorageRepository = tableStorageRepository;
            _configuration = configuration;
        }
        [FunctionName("FunctionInsertarDocumentos")]
        public async Task RunAsync([QueueTrigger("documentos", Connection = "StorageConnectionString")] DocumentoModel myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            documentsTable = GetDocumentsTable();
            FillDocumentsTable(myQueueItem);
            string trakingId = Guid.NewGuid().ToString();
            bool exitoso =  _tableStorageRepository.InsertInTableStorage(documentsTable, trakingId);
            if (exitoso)
            {
                string blobContainerName = _configuration.GetConnectionString("BlobContainerMassiveInvoice"); 
                string storageConnection = _configuration.GetConnectionString("StorageConnectionString");

                byte[] byteArray = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(myQueueItem));
                MemoryStream invoicesBlob = new MemoryStream(byteArray);


                // Upload json text to the blob
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnection);
                BlobContainerClient containerClient = await GetContainerAsync(blobServiceClient, blobContainerName);
                BlobClient blob;

                string trackingId = "";
                bool retryUpload = true;

                while (retryUpload)
                {
                    try
                    {
                        DateTimeOffset? timestamp = DateTime.Now.AddHours(_Offset);
                        trackingId = timestamp?.ToString("yyyyMMddHHmmss");


                        blob = containerClient.GetBlobClient($"{trackingId}.json");
                        invoicesBlob.Position = 0;

                        // Try upload blob with current timestamp as name
                        await blob.UploadAsync(invoicesBlob);
                        retryUpload = false;
                    }
                    catch (RequestFailedException ex)
                    {
                        if (ex.ErrorCode != "BlobAlreadyExists")
                        {
                            throw ex;
                        }

                        continue;
                    }
                }


            }
        }

        private void FillDocumentsTable(DocumentoModel documento)
        {
            DataRow rowDocuments;
            rowDocuments = documentsTable.NewRow();

            rowDocuments["TenantNumber"] = documento.TenantNumber;
            rowDocuments["IssueDate"] = documento.IssueDate;
            rowDocuments["DocumentNumberFull"] = documento.DocumentNumberFull;
            rowDocuments["Amount_Due"] = documento.Amount_Due;
            rowDocuments["Supplier_PartyName"] = documento.Supplier_PartyName;
            rowDocuments["Supplier_PartyIdentification"] = documento.Supplier_PartyIdentification;
            rowDocuments["Customer_PartyName"] = documento.Customer_PartyName;
            rowDocuments["Customer_PartyIdentification"] = documento.Customer_PartyIdentification;
            rowDocuments["UUID_CUFE"] = documento.UUID_CUFE;

            documentsTable.Rows.Add(rowDocuments);

        }
        private DataTable GetDocumentsTable()
        {
            DataTable documentsDT = new DataTable();


            documentsDT.Columns.Add("TenantNumber", typeof(string));
            documentsDT.Columns.Add("IssueDate", typeof(string));
            documentsDT.Columns.Add("DocumentNumberFull", typeof(string));
            documentsDT.Columns.Add("Amount_Due", typeof(string));
            documentsDT.Columns.Add("Supplier_PartyName", typeof(string));
            documentsDT.Columns.Add("Supplier_PartyIdentification", typeof(string));
            documentsDT.Columns.Add("Customer_PartyName", typeof(string));
            documentsDT.Columns.Add("Customer_PartyIdentification", typeof(string));
            documentsDT.Columns.Add("UUID_CUFE", typeof(string));


            return documentsDT;
        }
        private async Task<BlobContainerClient> GetContainerAsync(BlobServiceClient client, string containerName)
        {
            BlobContainerClient container = client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return container;
        }


    }
}
