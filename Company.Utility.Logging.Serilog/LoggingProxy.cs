using Castle.DynamicProxy;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Company.Utility.Logging.Serilog
{
    public class LoggingProxy
    {
        private static readonly IProxyGenerator _ProxyGenerator = new ProxyGenerator();

        public static I Create<I>(
            I instance,
            ILogger logger,
            LogType logType = LogType.Tracking | LogType.Usage | LogType.Error | LogType.Performance | LogType.Diagnostic) where I : class
        {
            Debug.Assert(typeof(I).IsInterface);

            bool useAll = logType == LogType.All;
            var interceptors = new List<IInterceptor>();

            if (useAll || logType.HasFlag(LogType.Tracking))
            {
                interceptors.Add(new AsyncTrackingInterceptor().ToInterceptor());
            }

            if (useAll || logType.HasFlag(LogType.Usage))
            {
            }

            if (useAll || logType.HasFlag(LogType.Error))
            {
                interceptors.Add(new AsyncErrorLoggingInterceptor(logger).ToInterceptor());
            }

            if (useAll || logType.HasFlag(LogType.Performance))
            {
                interceptors.Add(new AsyncPerformanceLoggingInterceptor(logger).ToInterceptor());
            }

            if (useAll || logType.HasFlag(LogType.Diagnostic))
            {
                // Check for NoDiagnosticLogging Class scope.
                bool classHasNoDiagnosticAttribute = instance.GetType().GetCustomAttributes(typeof(NoDiagnosticLoggingAttribute), false).Any();

                if (!classHasNoDiagnosticAttribute)
                {
                    interceptors.Add(new AsyncDiagnosticLoggingInterceptor(logger).ToInterceptor());
                }
            }

            return _ProxyGenerator.CreateInterfaceProxyWithTargetInterface(instance, interceptors.ToArray());
        }
    }
}
