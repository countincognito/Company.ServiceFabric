using System;
using System.Collections.Generic;

namespace Company.Utility
{
    [Serializable]
    public class TrackingContext
    {
        private static readonly object m_Lock = new object();
        private readonly IDictionary<string, string> _ExtraHeaders;

        static TrackingContext()
        {
            FullName = typeof(TrackingContext).FullName;
        }

        public TrackingContext(
            Guid callChainId,
            DateTime originatorUtcTimestamp,
            IDictionary<string, string> extraHeaders)
        {
            _ExtraHeaders = extraHeaders ?? throw new ArgumentNullException(nameof(extraHeaders));
            CallChainId = callChainId;
            OriginatorUtcTimestamp = originatorUtcTimestamp;
            ExtraHeaders = new Dictionary<string, string>(_ExtraHeaders);
        }

        public static string FullName
        {
            get;
        }

        public static TrackingContext Current
        {
            get
            {
                lock (m_Lock)
                {
                    return AmbientContext.GetData<TrackingContext>();
                }
            }
            private set
            {
                lock (m_Lock)
                {
                    AmbientContext.SetData(value);
                }
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

        public static TrackingContext NewCurrentIfEmpty()
        {
            return NewCurrentIfEmpty(new Dictionary<string, string>());
        }

        public static TrackingContext NewCurrentIfEmpty(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }
            lock (m_Lock)
            {
                TrackingContext tc = Current;
                if (tc == null)
                {
                    return NewCurrent(headers);
                }
                return tc;
            }
        }

        public static byte[] Serialize(TrackingContext trackingContext)
        {
            if (trackingContext == null)
            {
                throw new ArgumentNullException(nameof(trackingContext));
            }
            return AmbientContext.Serialize(trackingContext);
        }

        public static TrackingContext DeSerialize(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            return AmbientContext.DeSerialize<TrackingContext>(array);
        }

        /// <summary>
        /// Dangerous! Use with caution.
        /// </summary>
        public static void ClearCurrent()
        {
            lock (m_Lock)
            {
                AmbientContext.Clear<TrackingContext>();
            }
        }

        /// <summary>
        /// Dangerous! Use with caution.
        /// </summary>
        public void SetAsCurrent()
        {
            lock (m_Lock)
            {
                Current = this;
            }
        }

        /// <summary>
        /// Dangerous! Use with caution.
        /// </summary>
        public static TrackingContext NewCurrent()
        {
            return NewCurrent(new Dictionary<string, string>());
        }

        /// <summary>
        /// Dangerous! Use with caution.
        /// </summary>
        public static TrackingContext NewCurrent(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }
            lock (m_Lock)
            {
                ClearCurrent();
                TrackingContext tc = Create(headers);
                Current = tc;
                return tc;
            }
        }

        private static Guid NewInstanceId()
        {
            return Guid.NewGuid();
        }

        private static TrackingContext Create(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }
            return new TrackingContext(NewInstanceId(), DateTime.UtcNow, headers);
        }
    }
}
