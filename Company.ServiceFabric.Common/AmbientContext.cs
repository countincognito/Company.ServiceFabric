using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Company.ServiceFabric.Common
{
    /// <summary>
    /// Provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    public static class AmbientContext
    {
        private static ConcurrentDictionary<Type, AsyncLocal<object>> _State = new ConcurrentDictionary<Type, AsyncLocal<object>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData<T>(T data) where T : class
        {
            _State.GetOrAdd(typeof(T), _ => new AsyncLocal<object>()).Value = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static T GetData<T>() where T : class
        {
            return _State.TryGetValue(typeof(T), out AsyncLocal<object> data) ? (T)data.Value : null;
        }
    }
}
