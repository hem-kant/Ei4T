using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ei4T.CoreServiceClientL.CoreServiceFramework
{
    public static class CoreServiceConnectionScheme
    {
        public const String netTcp = "net.tcp";
        public const String wsHttp = "http";
        public const String undefined = "undefined";
    }

    public static class CoreServiceFactory
    {
        public static ICoreServiceFrameworkContext GetCoreServiceContext(Uri endpointUri, NetworkCredential windowsNetworkCredentials)
        {
            switch (endpointUri.Scheme.ToLower())
            {
                case CoreServiceConnectionScheme.wsHttp:
                    return GetWsHttpContext(endpointUri, windowsNetworkCredentials);
                case CoreServiceConnectionScheme.netTcp:
                    return GetNetTcpContext(endpointUri, windowsNetworkCredentials);
                default:
                    throw new ArgumentException("The uri connection scheme specified [{0}] is invalid; a valid scheme (ie. http for WsHttp, or net.tcp for NetTcp must be specified).");
            }
        }

        public static ICoreServiceFrameworkContext GetWsHttpContext(Uri endpointUri, NetworkCredential windowsNetworkCredentials)
        {
            return new CoreServiceFrameworkWsHttpContext(endpointUri, windowsNetworkCredentials);
        }

        public static ICoreServiceFrameworkContext GetNetTcpContext(Uri endpointUri, NetworkCredential windowsNetworkCredentials)
        {
            return new CoreServiceFrameworkNetTcpContext(endpointUri, windowsNetworkCredentials);
        }
    }
}
