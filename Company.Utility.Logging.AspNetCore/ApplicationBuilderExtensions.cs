using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

namespace Company.Utility.Logging.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTrackingMiddleware(
            this IApplicationBuilder builder,
            IDictionary<string, string> extraHeaders = null)
        {
            return builder.UseMiddleware<TrackingMiddleware>(extraHeaders);
        }
    }
}
