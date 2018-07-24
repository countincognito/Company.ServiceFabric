using Serilog;
using Serilog.Configuration;
using Serilog.Core.Enrichers;
using System;
using System.Fabric;

namespace Company.ServiceFabric.Logging
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration FromServiceContext(this LoggerEnrichmentConfiguration enrichmentConfiguration, ServiceContext context)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var propertyEnrichers = new PropertyEnricher[]
            {
                new PropertyEnricher(nameof(context.ServiceName), context.ServiceName),
                new PropertyEnricher(nameof(context.ServiceTypeName), context.ServiceTypeName),
                new PropertyEnricher(nameof(context.ReplicaOrInstanceId), context.ReplicaOrInstanceId.ToString()),
                new PropertyEnricher(nameof(context.PartitionId), context.PartitionId.ToString()),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.ApplicationName), context.CodePackageActivationContext.ApplicationName),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.ApplicationTypeName), context.CodePackageActivationContext.ApplicationTypeName),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.CodePackageName), context.CodePackageActivationContext.CodePackageName),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.CodePackageVersion), context.CodePackageActivationContext.CodePackageVersion),
                new PropertyEnricher(nameof(context.NodeContext.NodeName), context.NodeContext.NodeName),
                new PropertyEnricher(nameof(context.TraceId), context.TraceId),
            };
            return enrichmentConfiguration.With(propertyEnrichers);
        }
    }
}
