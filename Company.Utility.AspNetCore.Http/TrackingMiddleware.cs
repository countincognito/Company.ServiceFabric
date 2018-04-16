using Company.Utility.Logging.Serilog;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Threading.Tasks;
using Zametek.Utility;

namespace Company.Utility.AspNetCore.Http
{
    public class TrackingMiddleware
    {
        public const string RemoteIpAddressName = nameof(ConnectionInfo.RemoteIpAddress);
        public const string TraceIdentifierName = nameof(HttpContext.TraceIdentifier);
        public const string UserIdName = @"UserId";
        private readonly RequestDelegate _next;
        private readonly Action _setupAction;

        public TrackingMiddleware(RequestDelegate next, Action setupAction)
        {
            _next = next;
            _setupAction = setupAction;
        }

        public Task Invoke(HttpContext httpContext)
        {
            _setupAction?.Invoke();
            TrackingContext.NewCurrentIfEmpty();

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
