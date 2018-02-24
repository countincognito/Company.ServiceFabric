using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Company.Utility.Audit
{
    public class AuditableWrapper
    {
        private static readonly IProxyGenerator _ProxyGenerator = new ProxyGenerator();

        public static I Create<I>(I instance, ILogger<I> logger) where I : class
        {
            Debug.Assert(typeof(I).IsInterface);
            AuditContext.ClearCurrent();
            return _ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
                instance,
                new AsyncAuditableInterceptor().ToInterceptor(),
                new AsyncTimingInterceptor<I>(logger).ToInterceptor());
        }
    }
}
