using System;

namespace Company.Utility.Audit
{
    public class AuditHelper
    {
        private static Guid NewInstanceId()
        {
            return Guid.NewGuid();
        }

        private static AuditContext Create()
        {
            return new AuditContext(NewInstanceId(), DateTime.UtcNow);
        }

        public static void SetAuditContext()
        {
            AuditContext.Current = AuditContext.Current ?? Create();
        }
    }
}
