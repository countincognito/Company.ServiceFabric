using System;

namespace Company.ServiceFabric.Common
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
    }
}
