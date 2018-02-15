using Microsoft.ServiceFabric.Services.Remoting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core.Enrichers;
using System;
using System.Fabric;

namespace Company.ServiceFabric.Logging.Serilog
{
    public static class Extensions
    {
        public static LoggerConfiguration WithAuditContext(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            return enrichmentConfiguration.With<AuditContextEnricher>();
        }

        public static LoggerConfiguration WithServiceContext(this LoggerEnrichmentConfiguration enrichmentConfiguration, ServiceContext context)
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
                new PropertyEnricher(nameof(context.ReplicaOrInstanceId), context.ReplicaOrInstanceId),
                new PropertyEnricher(nameof(context.PartitionId), context.PartitionId),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.ApplicationName), context.CodePackageActivationContext.ApplicationName),
                new PropertyEnricher(nameof(context.CodePackageActivationContext.ApplicationTypeName), context.CodePackageActivationContext.ApplicationTypeName),
                new PropertyEnricher(nameof(context.NodeContext.NodeName), context.NodeContext.NodeName),
                new PropertyEnricher(nameof(context.TraceId), context.TraceId),
            };
            return enrichmentConfiguration.With(propertyEnrichers);
        }

        public static Microsoft.Extensions.Logging.ILogger<T> ToGeneric<T>(this ILogger logger) where T : IService
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            return Microsoft.Extensions.Logging.LoggerFactoryExtensions.CreateLogger<T>(
                new Microsoft.Extensions.Logging.LoggerFactory().AddSerilog(logger));
        }
    }
}
