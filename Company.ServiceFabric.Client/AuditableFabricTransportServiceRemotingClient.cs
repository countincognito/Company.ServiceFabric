using Company.ServiceFabric.Common;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using System;
using System.Fabric;
using System.Threading.Tasks;

namespace Company.ServiceFabric.Client
{
    public class AuditableFabricTransportServiceRemotingClient
      : IServiceRemotingClient
    {
        private readonly IServiceRemotingClient _InnerClient;

        public AuditableFabricTransportServiceRemotingClient(IServiceRemotingClient innerClient)
        {
            _InnerClient = innerClient ?? throw new ArgumentNullException(nameof(innerClient));
        }

        ~AuditableFabricTransportServiceRemotingClient()
        {
            var disposable = _InnerClient as IDisposable;
            disposable?.Dispose();
        }

        internal IServiceRemotingClient InnerClient
        {
            get
            {
                return _InnerClient;
            }
        }

        public string ListenerName
        {
            get
            {
                return _InnerClient.ListenerName;
            }
            set
            {
                _InnerClient.ListenerName = value;
            }
        }

        public ResolvedServiceEndpoint Endpoint
        {
            get
            {
                return _InnerClient.Endpoint;
            }
            set
            {
                _InnerClient.Endpoint = value;
            }
        }

        public ResolvedServicePartition ResolvedServicePartition
        {
            get
            {
                return _InnerClient.ResolvedServicePartition;
            }
            set
            {
                _InnerClient.ResolvedServicePartition = value;
            }
        }

        public async Task<IServiceRemotingResponseMessage> RequestResponseAsync(IServiceRemotingRequestMessage requestMessage)
        {
            IServiceRemotingResponseMessage responseMessage = await _InnerClient.RequestResponseAsync(AuditHelper.ProcessRequest(requestMessage));
            return AuditHelper.ProcessResponse(responseMessage);
        }

        public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
        {
            _InnerClient.SendOneWay(AuditHelper.ProcessRequest(requestMessage));
        }
    }
}
