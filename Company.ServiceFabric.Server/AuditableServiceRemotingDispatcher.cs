﻿using Company.ServiceFabric.Common;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace Company.ServiceFabric.Server
{
    public class AuditableServiceRemotingDispatcher
        : ServiceRemotingMessageDispatcher
    {
        public AuditableServiceRemotingDispatcher(ServiceContext serviceContext, IService service) :
            base(serviceContext, service)
        {
        }

        public async override Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            IServiceRemotingResponseMessage responseMessage =
                await base.HandleRequestResponseAsync(requestContext, AuditHelper.ProcessRequest(requestMessage));

            return AuditHelper.ProcessResponse(responseMessage);
        }

        public override Task<IServiceRemotingResponseMessageBody> HandleRequestResponseAsync(
            ServiceRemotingDispatchHeaders requestMessageDispatchHeaders,
            IServiceRemotingRequestMessageBody requestMessageBody,
            CancellationToken cancellationToken)
        {
            return base.HandleRequestResponseAsync(requestMessageDispatchHeaders, requestMessageBody, cancellationToken);
        }

        public override void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
        {
            base.HandleOneWayMessage(AuditHelper.ProcessRequest(requestMessage));
        }
    }
}
