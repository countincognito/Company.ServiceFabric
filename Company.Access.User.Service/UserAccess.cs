using Company.Access.User.Interface;
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

namespace Company.Access.User.Service
{
    internal sealed class UserAccess
        : StatelessService, IUserAccess
    {
        private IUserAccess _Impl;
        private readonly ILogger<IUserAccess> _Logger;

        public UserAccess(
            StatelessServiceContext context,
            ILogger<IUserAccess> logger)
            : base(context)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _Impl = AuditableWrapper.Create<IUserAccess, Impl.UserAccess>(
                new Impl.UserAccess(logger),
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
                            EndpointResourceName = typeof(IUserAccess).Name
                        }),
                    typeof(IUserAccess).Name),
            };
        }

        protected override Task OnCloseAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation($"{nameof(OnCloseAsync)} invoked");
            return base.OnCloseAsync(cancellationToken);
        }

        protected override void OnAbort()
        {
            _Logger.LogInformation($"{nameof(OnAbort)} invoked");
            base.OnAbort();
        }

        public Task<bool> CheckUserExistsAsync(string email)
        {
            return _Impl.CheckUserExistsAsync(email);
        }

        public Task<string> CreateUserAsync(string email)
        {
            return _Impl.CreateUserAsync(email);
        }
    }
}
