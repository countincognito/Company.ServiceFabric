using Serilog;
using System;

namespace Company.Utility.Logging.Serilog
{
    public static class LoggingExtensions
    {
        public static Microsoft.Extensions.Logging.ILogger<T> ToGeneric<T>(this ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            return Microsoft.Extensions.Logging.LoggerFactoryExtensions.CreateLogger<T>(
                new Microsoft.Extensions.Logging.LoggerFactory().AddSerilog(logger));
        }
    }
}
