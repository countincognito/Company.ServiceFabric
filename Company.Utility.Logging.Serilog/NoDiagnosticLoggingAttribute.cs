using System;

namespace Company.Utility.Logging.Serilog
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class NoDiagnosticLoggingAttribute
        : Attribute
    {
    }
}
