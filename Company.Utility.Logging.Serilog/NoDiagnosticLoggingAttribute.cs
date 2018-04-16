using System;

namespace Company.Utility.Logging.Serilog
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class NoDiagnosticLoggingAttribute
        : Attribute
    {
    }
}
