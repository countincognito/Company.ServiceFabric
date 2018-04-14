using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using Serilog.Exceptions.Core;
using System;
using System.Threading.Tasks;

namespace Company.Utility.Logging.Serilog
{
    public class AsyncErrorLoggingInterceptor
        : AsyncInterceptorBase
    {
        private readonly ILogger m_Logger;
        private readonly IDestructuringOptions m_DestructuringOptions;

        public AsyncErrorLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_DestructuringOptions = new DestructuringOptionsBuilder().WithDefaultDestructurers();
        }

        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            try
            {
                await proceed(invocation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogException(invocation, ex);
                throw;
            }
        }

        protected override async Task<T> InterceptAsync<T>(IInvocation invocation, Func<IInvocation, Task<T>> proceed)
        {
            try
            {
                return await proceed(invocation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogException(invocation, ex);
                throw;
            }
        }

        private void LogException(IInvocation invocation, Exception ex)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            using (LogContext.PushProperty(nameof(LogType), LogType.Error))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            using (LogContext.Push(new ExceptionEnricher(m_DestructuringOptions)))
            {
                m_Logger.Error(ex, $"{GetSourceMessage(invocation)}");
            }
        }

        private static string GetSourceMessage(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            return $"Error-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }
}
