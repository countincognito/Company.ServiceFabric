using Company.ServiceFabric.Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Company.ServiceFabric.Client
{
    public class TrackingProxy
    {
        private static readonly IServiceProxyFactory _TrackingProxyFactory =
            new ServiceProxyFactory(handler => new TrackingFabricTransportServiceRemotingClientFactory(new FabricTransportServiceRemotingClientFactory()));

        public static I ForMicroservice<I>(ServicePartitionKey partitionKey = null) where I : class, IService
        {
            return Proxy.ForMicroservice<I>(_TrackingProxyFactory, partitionKey);
        }

        public static I ForComponent<I>(StatelessService caller, ServicePartitionKey partitionKey = null) where I : class, IService
        {
            return Proxy.ForService<I>(_TrackingProxyFactory, Addressing.Component<I>(caller), partitionKey);
        }

        public static I ForComponent<I>(StatefulService caller, ServicePartitionKey partitionKey = null) where I : class, IService
        {
            return Proxy.ForService<I>(_TrackingProxyFactory, Addressing.Component<I>(caller), partitionKey);
        }
    }
}
