using System;

namespace Company.Utility.Audit
{
    [Serializable]
    public class AuditContext
    {
        static AuditContext()
        {
            Name = typeof(AuditContext).FullName;
        }

        public AuditContext(
            Guid callChainId,
            DateTime originatorUtcTimestamp)
        {
            CallChainId = callChainId;
            OriginatorUtcTimestamp = originatorUtcTimestamp;
        }

        public static string Name
        {
            get;
        }

        public static AuditContext Current
        {
            get
            {
                return AmbientContext.GetData<AuditContext>();
            }
            set
            {
                AmbientContext.SetData(value);
            }
        }

        public Guid CallChainId
        {
            get;
        }

        public DateTime OriginatorUtcTimestamp
        {
            get;
        }

        public static void NewCurrent()
        {
            ClearCurrent();
            Current = Create();
        }

        public static void NewCurrentIfEmpty()
        {
            AuditContext context = Current;
            if (context == null)
            {
                NewCurrent();
            }
        }

        public static void ClearCurrent()
        {
            AmbientContext.Clear<AuditContext>();
        }

        private static Guid NewInstanceId()
        {
            return Guid.NewGuid();
        }

        private static AuditContext Create()
        {
            return new AuditContext(NewInstanceId(), DateTime.UtcNow);
        }
    }
}
