using Serilog;
using Serilog.Configuration;
using System;

namespace Company.Utility.Logging.Serilog
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration FromTrackingContext(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            return enrichmentConfiguration
                .With<TrackingContextEnricher>()
                .Enrich.WithMachineName();
        }

        public static LoggerConfiguration FromLoggingProxy(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            return enrichmentConfiguration.FromLogContext()
                .Enrich.FromTrackingContext();
        }
    }
}
