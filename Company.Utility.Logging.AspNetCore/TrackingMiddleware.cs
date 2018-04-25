using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Zametek.Utility;
using Zametek.Utility.Logging;

namespace Company.Utility.Logging.AspNetCore
{
    public class TrackingMiddleware
    {
        public const string RemoteIpAddressName = nameof(ConnectionInfo.RemoteIpAddress);
        public const string TraceIdentifierName = nameof(HttpContext.TraceIdentifier);
        public const string UserIdName = @"UserId";
        private readonly RequestDelegate _next;
        private readonly Func<IDictionary<string, string>> _setupFunc;

        public TrackingMiddleware(RequestDelegate next, Func<IDictionary<string, string>> setupFunc)
        {
            _next = next;
            _setupFunc = setupFunc;
        }

        public Task Invoke(HttpContext httpContext)
        {
            IDictionary<string, string> extraHeaders = _setupFunc?.Invoke() ?? new Dictionary<string, string>();
            Debug.Assert(extraHeaders != null);
            TrackingContext.NewCurrentIfEmpty(extraHeaders);

            using (LogContext.Push(new TrackingContextEnricher()))
            using (LogContext.PushProperty(RemoteIpAddressName, httpContext.Connection.RemoteIpAddress))
            using (LogContext.PushProperty(TraceIdentifierName, httpContext.TraceIdentifier))
            using (LogContext.PushProperty(UserIdName, httpContext.User?.Identity?.Name))
            {
                return _next.Invoke(httpContext);
            }
        }
    }
}
