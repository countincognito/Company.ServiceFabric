using Microsoft.ServiceFabric.Services.Remoting;
using Serilog;
using Serilog.Core.Enrichers;
using System;
using System.Diagnostics;
using System.Fabric;

namespace Company.ServiceFabric.Logging.Serilog
{
    public static class Extensions
    {
        public static ILogger Enrich(this ILogger logger, StatelessServiceContext context)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var properties = new PropertyEnricher[]
            {
                new PropertyEnricher(nameof(context.ServiceName), context.ServiceName.ToString()),
                new PropertyEnricher(nameof(context.ServiceTypeName), context.ServiceTypeName),
                new PropertyEnricher(nameof(context.ReplicaOrInstanceId), context.ReplicaOrInstanceId.ToString()),
                new PropertyEnricher(nameof(context.PartitionId), context.PartitionId),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.ApplicationName), context.CodePackageActivationContext.ApplicationName),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.ApplicationTypeName), context.CodePackageActivationContext.ApplicationTypeName),
                new PropertyEnricher(nameof(context.NodeContext.NodeName), context.NodeContext.NodeName),
                new PropertyEnricher(nameof(context.TraceId), context.TraceId),
            };
            return logger.ForContext(properties);
        }

        public static Microsoft.Extensions.Logging.ILogger<T> Enrich<T>(this ILogger logger, StatelessServiceContext context) where T : IService
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return Microsoft.Extensions.Logging.LoggerFactoryExtensions.CreateLogger<T>(
                new Microsoft.Extensions.Logging.LoggerFactory()
                .AddSerilog(logger.Enrich(context)));
        }
    }
}
