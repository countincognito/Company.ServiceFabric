using System;

namespace Company.Utility.Logging.Serilog
{
    [Flags]
    public enum LogType
    {
        Tracking = 1 << 0,
        Usage = 1 << 1,
        Error = 1 << 2,
        Performance = 1 << 3,
        Diagnostic = 1 << 4,
    }
}
