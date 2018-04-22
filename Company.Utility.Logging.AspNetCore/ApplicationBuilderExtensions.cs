using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace Company.Utility.Logging.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTrackingMiddleware(
            this IApplicationBuilder builder,
            Func<IDictionary<string, string>> setupFunc = null)
        {
            return builder.UseMiddleware<TrackingMiddleware>(setupFunc);
        }
    }
}
