using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using System;
using System.Diagnostics;

namespace Company.Utility.Logging.Serilog
{
    public class AsyncPerformanceLoggingInterceptor
        : AsyncTimingInterceptor
    {
        private readonly ILogger m_Logger;

        public AsyncPerformanceLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void StartingTiming(IInvocation invocation)
        {
            using (LogContext.PushProperty(nameof(LogType), LogType.Performance))
            using (LogContext.PushProperties(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetPerformanceMessage(invocation)} timing started");
            }
        }

        protected override void CompletedTiming(IInvocation invocation, Stopwatch state)
        {
            long elapsedMilliseconds = state.ElapsedMilliseconds;

            using (LogContext.PushProperty(nameof(LogType), LogType.Performance))
            using (LogContext.PushProperties(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetPerformanceMessage(invocation)} ElapsedMilliseconds: {{ElapsedMilliseconds}}", elapsedMilliseconds);
            }
        }

        private static string GetPerformanceMessage(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            return $"Perf-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }
}
