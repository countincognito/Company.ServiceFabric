using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Company.Utility.Logging.Serilog
{
    public class AsyncDiagnosticLoggingInterceptor
        : AsyncInterceptorBase
    {
        public const string LogTypeName = nameof(LogType);
        public const string ArgumentsName = nameof(IInvocation.Arguments);
        public const string FilteredParameterSubstitute = @"__FILTERED__";
        private readonly ILogger m_Logger;

        public AsyncDiagnosticLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            bool mustLog = MustLog(invocation);

            if (mustLog)
            {
                LogBeforeInvocation(invocation);
            }

            await proceed(invocation).ConfigureAwait(false);
        }

        protected override async Task<T> InterceptAsync<T>(IInvocation invocation, Func<IInvocation, Task<T>> proceed)
        {
            bool mustLog = MustLog(invocation);

            if (mustLog)
            {
                LogBeforeInvocation(invocation);
            }

            return await proceed(invocation).ConfigureAwait(false);
        }

        private static bool MustLog(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            Debug.Assert(methodInfo != null);

            // Check for NoDiagnosticLogging Method scope.
            bool methodHasNoDiagnosticAttribute = methodInfo.GetCustomAttribute(typeof(NoDiagnosticLoggingAttribute)) != null;
            return !methodHasNoDiagnosticAttribute;
        }

        private void LogBeforeInvocation(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            Debug.Assert(methodInfo != null);

            // Check for NoDiagnosticLogging Parameter scope.
            List<object> filteredParameters = FilterParameters(invocation, methodInfo);

            using (LogContext.PushProperty(LogTypeName, LogType.Diagnostic))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            using (LogContext.PushProperty(ArgumentsName, filteredParameters, destructureObjects: true))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)}");
            }
        }

        private static List<object> FilterParameters(IInvocation invocation, MethodInfo methodInfo)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            Debug.Assert(parameterInfos != null);

            object[] parameters = invocation.Arguments;
            Debug.Assert(parameters != null);

            Debug.Assert(parameterInfos.Length == parameters.Length);

            var filteredParameters = new List<object>();

            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                ParameterInfo parameterInfo = parameterInfos[parameterIndex];
                bool parameterHasNoDiagnosticAttribute = parameterInfo.GetCustomAttribute(typeof(NoDiagnosticLoggingAttribute)) != null;

                if (parameterHasNoDiagnosticAttribute)
                {
                    filteredParameters.Add(FilteredParameterSubstitute);
                }

                object parameter = parameters[parameterIndex];
                filteredParameters.Add(parameter);
            }

            return filteredParameters;
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
