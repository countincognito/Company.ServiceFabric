using Company.ServiceFabric.Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Company.ServiceFabric.Client
{
    public class AuditableProxy
    {
        private static readonly IServiceProxyFactory _AuditableProxyFactory =
            new ServiceProxyFactory(handler => new AuditableFabricTransportServiceRemotingClientFactory(new FabricTransportServiceRemotingClientFactory()));

        public static I ForMicroservice<I>(ServicePartitionKey partitionKey = null) where I : class, IService
        {
            return Proxy.ForMicroservice<I>(_AuditableProxyFactory, partitionKey);
        }

        public static I ForComponent<I>(StatelessService caller, ServicePartitionKey partitionKey = null) where I : class, IService
        {
            return Proxy.ForService<I>(_AuditableProxyFactory, Addressing.Component<I>(caller), partitionKey);
        }

        public static I ForComponent<I>(StatefulService caller, ServicePartitionKey partitionKey = null) where I : class, IService
        {
            return Proxy.ForService<I>(_AuditableProxyFactory, Addressing.Component<I>(caller), partitionKey);
        }
    }
}
