using Castle.DynamicProxy;
using Serilog;
using System.Diagnostics;

namespace Company.Utility.Logging.Serilog
{
    public class LoggingProxy
    {
        private static readonly IProxyGenerator _ProxyGenerator = new ProxyGenerator();

        public static I Create<I>(I instance, ILogger logger) where I : class
        {
            Debug.Assert(typeof(I).IsInterface);

            TrackingContext.ClearCurrent();

            var asyncTrackingInterceptor = new AsyncTrackingInterceptor();
            var asyncPerfLoggingInterceptor = new AsyncPerformanceLoggingInterceptor(logger);

            return _ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
                instance,
                asyncTrackingInterceptor.ToInterceptor(),
                asyncPerfLoggingInterceptor.ToInterceptor());
        }
    }
}
