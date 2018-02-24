using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Company.Utility.Audit
{
    public class AsyncTimingInterceptor<T>
         : AsyncTimingInterceptor
    {
        private readonly ILogger<T> _Logger;

        public AsyncTimingInterceptor(ILogger<T> logger)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void StartingTiming(IInvocation invocation)
        {
            _Logger.LogInformation($"{invocation.Method.Name}:StartingTiming");
        }

        protected override void CompletedTiming(IInvocation invocation, Stopwatch stopwatch)
        {
            _Logger.LogInformation($"{invocation.Method.Name}:CompletedTiming:{stopwatch.Elapsed:g}");
        }
    }
}
