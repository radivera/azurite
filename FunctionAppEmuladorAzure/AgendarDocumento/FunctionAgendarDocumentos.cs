using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using FunctionAppEmuladorAzure.Common;

namespace FunctionAppEmuladorAzure.AgendarDocumento
{
    public class FunctionAgendarDocumentos
    {


        private readonly IDocumentRepository _documentoRepository;

        public FunctionAgendarDocumentos(

          IDocumentRepository documentoRepository)
        {

            _documentoRepository = documentoRepository;
        }


        [FunctionName(nameof(FunctionAgendarDocumentos))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, [Queue("documentos"), StorageAccount("StorageConnectionString")] ICollector<DocumentoModel> queueCollector)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
           List<DocumentoModel> documentolist = await _documentoRepository.GetDocumentos();
            foreach (var item in documentolist)
            {
                queueCollector.Add(item);
            }
            var responseMessage = "Mensajes encolados exitosamente";
            return new OkObjectResult(responseMessage);
        }
    }
}
