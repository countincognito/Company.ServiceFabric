using Company.ServiceFabric.Common;
using Company.ServiceFabric.Logging.Serilog;
using Company.Utility.Logging.Serilog;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;

namespace Company.Engine.Registration.Service
{
    internal static class Hosting
    {
        public static void Register()
        {
            ServiceRuntime.RegisterServiceAsync(
                Naming.ServiceType<RegistrationEngine>(),
                (context) =>
                {
                    ConfigurationPackage configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject(@"Config");
                    ILogger serilog = new LoggerConfiguration()
                        .Enrich.FromServiceContext(context)
                        .Enrich.FromTrackingContext()
                        .Enrich.FromLoggingProxy()
                        .WriteTo.Seq(configPackage.Settings.Sections[@"ResourceSettings"].Parameters[@"seqLocation"].Value)
                        .CreateLogger();
                    Log.Logger = serilog;
                    return new RegistrationEngine(context, serilog);
                })
                .GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                Process.GetCurrentProcess().Id,
                Naming.ServiceType<RegistrationEngine>());
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
