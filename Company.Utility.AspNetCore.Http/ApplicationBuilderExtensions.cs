using Microsoft.AspNetCore.Builder;
using System;

namespace Company.Utility.AspNetCore.Http
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTrackingMiddleware(
            this IApplicationBuilder builder,
            Action setupAction = null)
        {
            return builder.UseMiddleware<TrackingMiddleware>(setupAction);
        }
    }
}
