using System;

namespace Company.Utility.Logging.Serilog
{
    [Flags]
    public enum LogType
    {
        All = 0,
        Tracking = 1 << 0,
        Error = 1 << 1,
        Performance = 1 << 2,
        Diagnostic = 1 << 3,
    }
}
