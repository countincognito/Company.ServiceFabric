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
        public const string LogTypeName = nameof(LogType);
        private readonly ILogger m_Logger;

        public AsyncPerformanceLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void StartingTiming(IInvocation invocation)
        {
            using (LogContext.PushProperty(LogTypeName, LogType.Performance))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} timing started");
            }
        }

        protected override void CompletedTiming(IInvocation invocation, Stopwatch state)
        {
            long elapsedMilliseconds = state.ElapsedMilliseconds;

            using (LogContext.PushProperty(LogTypeName, LogType.Performance))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} ElapsedMilliseconds: {{ElapsedMilliseconds}}", elapsedMilliseconds);
            }
        }

        private static string GetSourceMessage(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            return $"performance-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }
}
