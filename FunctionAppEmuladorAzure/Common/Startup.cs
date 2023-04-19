using FunctionAppEmuladorAzure.AgendarDocumento;
using FunctionAppEmuladorAzure.Common;
using FunctionAppEmuladorAzure.InsertarDocumento;
using FunctionAppEmuladorAzure.Util;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionAppEmuladorAzure.Common;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddTransient<IDocumentRepository, DocumentRepository>()
            .AddTransient<IConnectionStringResolver, ConnectionStringResolver>()
            .AddTransient<ITableStorageRepository, TableStorageRepository>()
            .AddTransient<IConnectionStorageResolver, ConnectionStorageResolver>();
    }
}
