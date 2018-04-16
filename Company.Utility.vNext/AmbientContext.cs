using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Company.Utility
{
    /// <summary>
    /// Provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    public static class AmbientContext
    {
        private static ConcurrentDictionary<Type, AsyncLocal<byte[]>> _State = new ConcurrentDictionary<Type, AsyncLocal<byte[]>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData<T>(T data) where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            _State.GetOrAdd(typeof(T), _ => new AsyncLocal<byte[]>()).Value = Serialize(data);
        }

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static T GetData<T>() where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);
            if (_State.TryGetValue(typeof(T), out AsyncLocal<byte[]> data))
            {
                if (data.Value == null)
                {
                    return null;
                }
                return DeSerialize<T>(data.Value);
            }
            return null;
        }

        public static void Clear<T>() where T : class
        {
            _State.TryRemove(typeof(T), out AsyncLocal<byte[]> data);
        }

        public static byte[] Serialize<T>(T obj) where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T DeSerialize<T>(byte[] array) where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            string json = Encoding.UTF8.GetString(array);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static bool IsDataContract(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(DataContractAttribute), false);
            return attributes.Any();
        }
    }
}
