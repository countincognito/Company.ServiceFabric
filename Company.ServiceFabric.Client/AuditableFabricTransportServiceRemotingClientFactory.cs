using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace Company.ServiceFabric.Client
{
    public class AuditableFabricTransportServiceRemotingClientFactory
        : IServiceRemotingClientFactory
    {
        private readonly IServiceRemotingClientFactory _InnerClientFactory;

        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientConnected;
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientDisconnected;

        public AuditableFabricTransportServiceRemotingClientFactory(IServiceRemotingClientFactory innerClientFactory)
        {
            _InnerClientFactory = innerClientFactory ?? throw new ArgumentNullException(nameof(innerClientFactory));
            _InnerClientFactory.ClientConnected += OnClientConnected;
            _InnerClientFactory.ClientDisconnected += OnClientDisconnected;
        }

        private void OnClientConnected(
            object sender,
            CommunicationClientEventArgs<IServiceRemotingClient> e)
        {
            ClientConnected?.Invoke(this, new CommunicationClientEventArgs<IServiceRemotingClient>()
            {
                Client = e.Client
            });
        }

        private void OnClientDisconnected(
            object sender,
            CommunicationClientEventArgs<IServiceRemotingClient> e)
        {
            ClientDisconnected?.Invoke(this, new CommunicationClientEventArgs<IServiceRemotingClient>()
            {
                Client = e.Client
            });
        }

        public async Task<IServiceRemotingClient> GetClientAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var client = await _InnerClientFactory.GetClientAsync(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);
            return new AuditableFabricTransportServiceRemotingClient(client);
        }

        public async Task<IServiceRemotingClient> GetClientAsync(
            ResolvedServicePartition previousRsp,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var client = await _InnerClientFactory.GetClientAsync(
                previousRsp,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);
            return new AuditableFabricTransportServiceRemotingClient(client);
        }

        public Task<OperationRetryControl> ReportOperationExceptionAsync(
            IServiceRemotingClient client,
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            return _InnerClientFactory.ReportOperationExceptionAsync(
                // This expects a an object of type FabricTransportServiceRemotingClient, hence
                // why we need to expose the InnerClient here.
                // https://github.com/Azure/service-fabric-services-and-actors-dotnet/issues/43
                ((AuditableFabricTransportServiceRemotingClient)client).InnerClient,
                exceptionInformation,
                retrySettings,
                cancellationToken);
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return _InnerClientFactory.GetRemotingMessageBodyFactory();
        }
    }
}
