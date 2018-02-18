using Company.Access.User.Interface;
using Company.Common.Data;
using Company.Engine.Registration.Interface;
using Company.ServiceFabric.Client;
using Company.ServiceFabric.Server;
using Company.Utility.Audit;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace Company.Engine.Registration.Service
{
    internal sealed class RegistrationEngine
        : StatelessService, IRegistrationEngine
    {
        private IRegistrationEngine _Impl;
        private readonly ILogger<IRegistrationEngine> _Logger;

        public RegistrationEngine(
            StatelessServiceContext context,
            ILogger<IRegistrationEngine> logger)
            : base(context)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var userAccess = AuditableProxy.ForComponent<IUserAccess>(this);
            _Impl = AuditableWrapper.Create<IRegistrationEngine, Impl.RegistrationEngine>(
                new Impl.RegistrationEngine(userAccess, logger),
                logger);
            _Logger.LogInformation("Constructed");
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(
                    (context) => new FabricTransportServiceRemotingListener(
                        context,
                        new AuditableServiceRemotingDispatcher(context, this),
                        new FabricTransportRemotingListenerSettings
                        {
                            EndpointResourceName = typeof(IRegistrationEngine).Name
                        }),
                    typeof(IRegistrationEngine).Name),
            };
        }

        protected override Task OnCloseAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation($"{nameof(OnCloseAsync)} Invoked");
            return base.OnCloseAsync(cancellationToken);
        }

        protected override void OnAbort()
        {
            _Logger.LogInformation($"{nameof(OnAbort)} Invoked");
            base.OnAbort();
        }

        public Task<string> RegisterMemberAsync(RegisterRequest request)
        {
            return _Impl.RegisterMemberAsync(request);
        }
    }
}
