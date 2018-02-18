using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Company.Utility.Audit
{
    public class AuditableWrapper
    {
        private static readonly IProxyGenerator _ProxyGenerator = new ProxyGenerator();

        public static I Create<I, T>(T instance, ILogger<I> logger) where T : class, I where I : class
        {
            AuditContext.ClearCurrent();
            return _ProxyGenerator.CreateInterfaceProxyWithTargetInterface<I>(
                instance,
                new AsyncAuditableInterceptor().ToInterceptor(),
                new AsyncTimingInterceptor<I>(logger).ToInterceptor());
        }
    }
}
