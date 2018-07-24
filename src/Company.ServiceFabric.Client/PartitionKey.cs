using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Text;

namespace Company.ServiceFabric.Proxy
{
    public static class PartitionKey
    {
        public static ServicePartitionKey Generate(string input)
        {
            return new ServicePartitionKey(Convert.ToInt64(HashFNV1aToLong(Encoding.ASCII.GetBytes(input))));
        }

        private static long HashFNV1aToLong(byte[] bytes)
        {
            const ulong fnv64Offset = 14695981039346656037;
            const ulong fnv64Prime = 0x100000001b3;
            ulong hash = fnv64Offset;
            for (var i = 0; i < bytes.Length; i++)
            {
                hash = hash ^ bytes[i];
                hash *= fnv64Prime;
            }
            unchecked
            {
                return (long)hash;
            }
        }
    }
}
