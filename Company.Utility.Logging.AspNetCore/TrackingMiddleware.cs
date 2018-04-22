using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zametek.Utility;

namespace Company.Utility.Logging.AspNetCore
{
    public class TrackingMiddleware
    {
        public const string RemoteIpAddressName = nameof(ConnectionInfo.RemoteIpAddress);
        public const string TraceIdentifierName = nameof(HttpContext.TraceIdentifier);
        public const string UserIdName = @"UserId";
        private readonly RequestDelegate _next;
        private readonly IDictionary<string, string> _extraHeaders;

        public TrackingMiddleware(RequestDelegate next, IDictionary<string, string> extraHeaders)
        {
            _next = next;
            _extraHeaders = extraHeaders ?? new Dictionary<string, string>();
        }

        public Task Invoke(HttpContext httpContext)
        {
            TrackingContext.NewCurrentIfEmpty(_extraHeaders);

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
