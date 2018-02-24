using Company.Api.Rest.Impl;
using Company.Api.Rest.Interface;
using Company.Manager.Membership.Interface;
using Company.ServiceFabric.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Company.Api.Rest.Service
{
    internal sealed class RestApi
        : StatelessService, IRestApi
    {
        private readonly ILogger<IRestApi> _Logger;
        private readonly string _ApiCertThumbprint;

        public RestApi(
            StatelessServiceContext context,
            ILogger<IRestApi> logger)
            : base(context)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ConfigurationPackage configPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject(@"Config");
            _ApiCertThumbprint = configPackage.Settings.Sections[@"ResourceSettings"].Parameters[@"apiCertThumbprint"].Value;
            _Logger.LogInformation("Constructed");
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            IEnumerable<EndpointResourceDescription> endpoints = Context.CodePackageActivationContext.GetEndpoints()
                .Where(endpoint => endpoint.Protocol == EndpointProtocol.Http || endpoint.Protocol == EndpointProtocol.Https);

            return endpoints.Select(endpoint =>
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, typeof(IRestApi).Name, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                            .UseKestrel(options =>
                            {
                                if (endpoint.Protocol == EndpointProtocol.Http)
                                {
                                    options.Listen(IPAddress.Any, endpoint.Port);
                                }
                                else if (endpoint.Protocol == EndpointProtocol.Https)
                                {
                                    options.Listen(IPAddress.Any, endpoint.Port, listenOptions => listenOptions.UseHttps(GetCertificate(_ApiCertThumbprint)));
                                }
                            })
                            .ConfigureServices(
                                services => services
                                    .AddTransient(typeof(IMembershipManager), _ => AuditableProxy.ForMicroservice<IMembershipManager>())
                                    .AddSingleton(typeof(ILogger<IRestApi>), _Logger)
                                    .AddSingleton(serviceContext))
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseStartup<Startup>()
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url)
                            .Build();
                    })));
        }

        public static X509Certificate2 GetCertificate(string thumbprint)
        {
            // Strip any non-hexadecimal values and make uppercase.
            string cleanedThumbprint = Regex.Replace(thumbprint, @"[^\da-fA-F]", string.Empty).ToUpper();
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates;
                var signingCert = certCollection.Find(X509FindType.FindByThumbprint, cleanedThumbprint, false);
                if (signingCert.Count == 0)
                {
                    throw new FileNotFoundException(string.Format("Cert with thumbprint: '{0}' not found in local machine cert store.", cleanedThumbprint));
                }

                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
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
    }
}
