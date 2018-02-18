using Serilog;
using Serilog.Configuration;
using System;

namespace Company.Utility.Audit
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithAuditContext(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            return enrichmentConfiguration.With<AuditContextEnricher>();
        }
    }
}
