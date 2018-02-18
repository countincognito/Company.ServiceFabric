﻿using Serilog.Core;
using Serilog.Events;

namespace Company.Utility.Audit
{
    public class AuditContextEnricher
        : ILogEventEnricher
    {
        /// <summary>
        /// The property name added to enriched log events.
        /// </summary>
        public const string CallChainIdPropertyName = nameof(AuditContext.CallChainId);
        public const string OriginatorUtcTimestampPropertyName = nameof(AuditContext.OriginatorUtcTimestamp);

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            AuditContext context = AuditContext.Current;
            if (context != null)
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty(CallChainIdPropertyName, new ScalarValue(context.CallChainId)));
                logEvent.AddPropertyIfAbsent(new LogEventProperty(OriginatorUtcTimestampPropertyName, new ScalarValue(context.OriginatorUtcTimestamp)));
            }
        }
    }
}