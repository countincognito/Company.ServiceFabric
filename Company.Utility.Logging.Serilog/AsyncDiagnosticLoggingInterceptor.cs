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
        : ProcessingAsyncInterceptor<object>
    {
        public const string LogTypeName = nameof(LogType);
        public const string ArgumentsName = nameof(IInvocation.Arguments);
        public const string ReturnValueName = nameof(IInvocation.ReturnValue);
        public const string VoidSubstitute = @"__VOID__";
        public const string FilteredParameterSubstitute = @"__FILTERED__";
        private readonly ILogger m_Logger;

        public AsyncDiagnosticLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override object StartingInvocation(IInvocation invocation)
        {
            bool mustLog = MustLog(invocation);

            if (mustLog)
            {
                LogBeforeInvocation(invocation);
            }

            return null;
        }

        protected override void CompletedInvocation(IInvocation invocation, object state, object returnValue)
        {
            bool mustLog = MustLog(invocation);

            if (mustLog)
            {
                LogAfterInvocation(invocation, returnValue);
            }
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

        private void LogAfterInvocation(IInvocation invocation, object returnValue)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            Debug.Assert(methodInfo != null);

            // Check for NoDiagnosticLogging ReturnValue scope.
            object filteredReturnValue = FilterReturnValue(methodInfo, returnValue);

            using (LogContext.PushProperty(LogTypeName, LogType.Diagnostic))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            using (LogContext.PushProperty(ReturnValueName, filteredReturnValue, destructureObjects: true))
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

        private static object FilterReturnValue(MethodInfo methodInfo, object returnValue)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            ParameterInfo parameterInfo = methodInfo.ReturnParameter;
            Debug.Assert(parameterInfo != null);

            bool returnValueHasNoDiagnosticAttribute = parameterInfo.GetCustomAttribute(typeof(NoDiagnosticLoggingAttribute)) != null;

            if (returnValueHasNoDiagnosticAttribute)
            {
                return FilteredParameterSubstitute;
            }

            if (parameterInfo.ParameterType == typeof(void)
                || parameterInfo.ParameterType == typeof(Task))
            {
                return VoidSubstitute;
            }

            return returnValue;
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
