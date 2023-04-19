using FunctionAppEmuladorAzure.Common;
using FunctionAppEmuladorAzure.Util;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppEmuladorAzure.AgendarDocumento
{
    public interface IDocumentRepository
    {
        string GetCadenaConexion();
        Task<List<DocumentoModel>> GetDocumentos();
    }
    public class DocumentRepository : IDocumentRepository
    {

        private readonly ILogger<DocumentRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnectionStringResolver _connectionStringResolver;


        public DocumentRepository(
               ILogger<DocumentRepository> logger,
               IConfiguration configuration,
               IConnectionStringResolver connectionStringResolver
               )
        {
            _logger = logger;
            _configuration = configuration;
            _connectionStringResolver = connectionStringResolver;
        }
        public string GetCadenaConexion()
        {
            return _connectionStringResolver.GetCadenaConexion();
        }

        public async Task<List<DocumentoModel>> GetDocumentos()
        {
            string billforceConexion = _connectionStringResolver.GetCadenaConexion();
            string sqlCommand = @$"
                                    SELECT top 100	TenantNumber,
		                                    IssueDate,
		                                    DocumentNumberFull,
		                                    Amount_Due,
		                                    Supplier_PartyName,
		                                    Supplier_PartyIdentification,
		                                    Customer_PartyName,
		                                    Customer_PartyIdentification,
		                                    UUID_CUFE
                                    from repext.Documents 
                                    where TenantNumber='824574844' 
	                                    and DocumentTypeCode='FACTURA-UBL' 
	                                    and PaymentMeansId=2 
	                                    and Amount_Due > 0";

            try
            {
                List<DocumentoModel> documentos = new List<DocumentoModel>();

                using (SqlConnection con = new SqlConnection(billforceConexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await sdr.ReadAsync())
                            {
                                documentos.Add(new DocumentoModel
                                {
                                    TenantNumber = sdr["TenantNumber"].ToString(),
                                    IssueDate = sdr["IssueDate"].ToString(),
                                    DocumentNumberFull = sdr["DocumentNumberFull"].ToString(),
                                    Amount_Due = sdr["Amount_Due"].ToString(),
                                    Supplier_PartyName = sdr["Supplier_PartyName"].ToString(),
                                    Supplier_PartyIdentification = sdr["Supplier_PartyIdentification"].ToString(),
                                    Customer_PartyName = sdr["Customer_PartyName"].ToString(),
                                    Customer_PartyIdentification = sdr["Customer_PartyIdentification"].ToString(),
                                    UUID_CUFE = sdr["UUID_CUFE"].ToString()
                                });
                            }
                        }
                        con.Close();
                    }
                }

                return documentos;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.StackTrace);
                throw;
            }
        }
    }
}
