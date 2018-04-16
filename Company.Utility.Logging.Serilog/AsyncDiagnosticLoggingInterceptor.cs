using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Company.Utility.Logging.Serilog
{
    public class AsyncDiagnosticLoggingInterceptor
        : AsyncInterceptorBase
    {
        public const string LogTypeName = nameof(LogType);
        public const string ArgumentsName = nameof(IInvocation.Arguments);
        private readonly ILogger m_Logger;

        public AsyncDiagnosticLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            LogInvocation(invocation);
            await proceed(invocation).ConfigureAwait(false);
        }

        protected override async Task<T> InterceptAsync<T>(IInvocation invocation, Func<IInvocation, Task<T>> proceed)
        {
            LogInvocation(invocation);
            return await proceed(invocation).ConfigureAwait(false);
        }

        private void LogInvocation(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            bool hasNoDiagnosticAttribute = invocation.MethodInvocationTarget.GetCustomAttribute(typeof(NoDiagnosticLoggingAttribute)) != null;
            if (hasNoDiagnosticAttribute)
            {
                return;
            }

            using (LogContext.PushProperty(LogTypeName, LogType.Diagnostic))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            using (LogContext.PushProperty(ArgumentsName, invocation.Arguments, destructureObjects: true))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)}");
            }
        }

        private static string GetSourceMessage(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            return $"diagnostic-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }
}
