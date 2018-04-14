using Castle.DynamicProxy;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;

namespace Company.Utility.Logging.Serilog
{
    public class LoggingProxy
    {
        private static readonly IProxyGenerator _ProxyGenerator = new ProxyGenerator();

        public static I Create<I>(
            I instance,
            ILogger logger,
            LogType logType = LogType.Tracking | LogType.Usage | LogType.Error | LogType.Performance) where I : class
        {
            Debug.Assert(typeof(I).IsInterface);

            var interceptors = new List<IInterceptor>();
            
            if (logType.HasFlag(LogType.Tracking))
            {
                interceptors.Add(new AsyncTrackingInterceptor().ToInterceptor());
            }

            if (logType.HasFlag(LogType.Usage))
            {
            }

            if (logType.HasFlag(LogType.Error))
            {
                interceptors.Add(new AsyncErrorLoggingInterceptor(logger).ToInterceptor());
            }

            if (logType.HasFlag(LogType.Performance))
            {
                interceptors.Add(new AsyncPerformanceLoggingInterceptor(logger).ToInterceptor());
            }

            if (logType.HasFlag(LogType.Diagnostic))
            {
            }

            return _ProxyGenerator.CreateInterfaceProxyWithTargetInterface(instance, interceptors.ToArray());
        }
    }
}
