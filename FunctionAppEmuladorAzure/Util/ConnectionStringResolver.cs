using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppEmuladorAzure.Util;

public interface IConnectionStringResolver
{
    public string GetCadenaConexion();
}

public class ConnectionStringResolver : IConnectionStringResolver
{
    public string CadenaConexion { get; set; }

    public ConnectionStringResolver(IConfiguration configuration)
    {
        CadenaConexion = configuration.GetConnectionString("RepextConnectionString");
    }

    public string GetCadenaConexion()
    {
        return CadenaConexion;
    }
}
