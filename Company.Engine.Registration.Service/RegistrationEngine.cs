using Company.Access.User.Interface;
using Company.Common.Data;
using Company.Engine.Registration.Interface;
using Company.ServiceFabric.Client;
using Company.ServiceFabric.Server;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Zametek.Utility.Logging;

namespace Company.Engine.Registration.Service
{
    internal sealed class RegistrationEngine
        : StatelessService, IRegistrationEngine
    {
        private IRegistrationEngine _Impl;
        private readonly ILogger _Logger;

        public RegistrationEngine(
            StatelessServiceContext context,
            ILogger logger)
            : base(context)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var userAccess = TrackingProxy.ForComponent<IUserAccess>(this);
            _Impl = LogProxy.Create<IRegistrationEngine>(new Impl.RegistrationEngine(userAccess, logger), logger, LogType.All);
            _Logger.Information("Constructed");
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(
                    (context) => new FabricTransportServiceRemotingListener(
                        context,
                        new TrackingServiceRemotingDispatcher(context, this),
                        new FabricTransportRemotingListenerSettings
                        {
                            EndpointResourceName = typeof(IRegistrationEngine).Name
                        }),
                    typeof(IRegistrationEngine).Name),
            };
        }

        protected override Task OnCloseAsync(CancellationToken cancellationToken)
        {
            _Logger.Information($"{nameof(OnCloseAsync)} Invoked");
            return base.OnCloseAsync(cancellationToken);
        }

        protected override void OnAbort()
        {
            _Logger.Information($"{nameof(OnAbort)} Invoked");
            base.OnAbort();
        }

        public Task<string> RegisterMemberAsync(RegisterRequest request)
        {
            return _Impl.RegisterMemberAsync(request);
        }
    }
}
