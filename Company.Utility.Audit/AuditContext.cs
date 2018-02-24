using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Company.Utility.Audit
{
    public class AuditContext
    {
        private readonly IDictionary<string, string> _ExtraHeaders;

        static AuditContext()
        {
            Name = typeof(AuditContext).FullName;
        }

        public AuditContext(
            Guid callChainId,
            DateTime originatorUtcTimestamp,
            IDictionary<string, string> extraHeaders)
        {
            CallChainId = callChainId;
            OriginatorUtcTimestamp = originatorUtcTimestamp;
            _ExtraHeaders = extraHeaders ?? throw new ArgumentNullException(nameof(extraHeaders));
            ExtraHeaders = new ReadOnlyDictionary<string, string>(_ExtraHeaders);
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

        public IReadOnlyDictionary<string, string> ExtraHeaders
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

        public static byte[] Serialize(AuditContext auditContext)
        {
            if (auditContext == null)
            {
                throw new ArgumentNullException(nameof(auditContext));
            }
            return AmbientContext.Serialize(auditContext);
        }

        public static AuditContext DeSerialize(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            return AmbientContext.DeSerialize<AuditContext>(array);
        }

        public void RemoveExtraHeaders(IList<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            foreach (string key in keys)
            {
                _ExtraHeaders.Remove(key);
            }
            SetAsCurrent();
        }

        public void AddExtraHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                _ExtraHeaders.Add(kvp.Key, kvp.Value);
            }
            SetAsCurrent();
        }

        public void RemoveExtraHeader(string key)
        {
            RemoveExtraHeaders(new[] { key });
        }

        public void AddExtraHeader(string key, string value)
        {
            AddExtraHeaders(new Dictionary<string, string>() { { key, value } });
        }

        private void SetAsCurrent()
        {
            ClearCurrent();
            Current = this;
        }

        private static Guid NewInstanceId()
        {
            return Guid.NewGuid();
        }

        private static AuditContext Create()
        {
            return new AuditContext(NewInstanceId(), DateTime.UtcNow, new Dictionary<string, string>());
        }
    }
}
