using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppEmuladorAzure.Util
{

    public interface IConnectionStorageResolver
    {
        string GetCadenaConexionStorage();
    }
    public class ConnectionStorageResolver : IConnectionStorageResolver
    {
        private string _cadenaConexionStorage { get; set; }

        public ConnectionStorageResolver(IConfiguration configuration)
        {
            _cadenaConexionStorage = configuration.GetConnectionString("StorageConnectionString");
        }

        public string GetCadenaConexionStorage()
        {
            return _cadenaConexionStorage;
        }
    }
}
