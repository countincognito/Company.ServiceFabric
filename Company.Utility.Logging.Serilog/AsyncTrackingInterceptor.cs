using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;
using Zametek.Utility;

namespace Company.Utility.Logging.Serilog
{
    public class AsyncTrackingInterceptor
        : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            TrackingContext.NewCurrentIfEmpty();
            await proceed(invocation).ConfigureAwait(false);
        }

        protected override async Task<T> InterceptAsync<T>(IInvocation invocation, Func<IInvocation, Task<T>> proceed)
        {
            TrackingContext.NewCurrentIfEmpty();
            return await proceed(invocation).ConfigureAwait(false);
        }
    }
}
