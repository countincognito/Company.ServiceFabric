using Company.ServiceFabric.Common;
using Company.ServiceFabric.Logging.Serilog;
using Company.Utility.Audit;
using Company.Utility.Logging.Serilog;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;

namespace Company.Api.Rest.Service
{
    internal static class Hosting
    {
        public static void Register()
        {
            ServiceRuntime.RegisterServiceAsync(
                Naming.ServiceType<RestApi>(),
                (context) =>
                {
                    ConfigurationPackage configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject(@"Config");
                    ILogger serilog = new LoggerConfiguration()
                        .Enrich.WithServiceContext(context)
                        .Enrich.WithAuditContext()
                        .WriteTo.Seq(configPackage.Settings.Sections[@"ResourceSettings"].Parameters[@"seqLocation"].Value)
                        .CreateLogger();
                    Log.Logger = serilog;
                    return new RestApi(context, serilog.ToGeneric<RestApi>());
                })
                .GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                Process.GetCurrentProcess().Id,
                Naming.ServiceType<RestApi>());
        }

        private static void Main()
        {
            try
            {
                Register();
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
