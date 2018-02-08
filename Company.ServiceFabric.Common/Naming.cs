using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Diagnostics;

namespace Company.ServiceFabric.Common
{
    public static class Naming
    {
        public static string Microservice<I>() where I : IService
        {
            Debug.Assert(
                typeof(I).Namespace.Contains(Constant.Manager),
                $"Invalid microservice interface. Use only the {Constant.Manager} interface to access a microservice.");
            string[] namespaceSegments = typeof(I).Namespace.Split('.');
            if (namespaceSegments.Length < Constant.NamespaceMinimumSize)
            {
                throw new FormatException(@"Microservice namespace is an invalid format.");
            }
            return $"{namespaceSegments[0]}.{Constant.Microservice}.{namespaceSegments[2]}";
        }

        public static string Component<I>() where I : IService
        {
            string[] namespaceSegments = typeof(I).Namespace.Split('.');
            if (namespaceSegments.Length < Constant.NamespaceMinimumSize)
            {
                throw new FormatException(@"Component namespace is an invalid format.");
            }
            return namespaceSegments[2] + namespaceSegments[1];
        }

        public static string Listener<I>() where I : IService
        {
            return typeof(I).Name;
        }

        public static string ServiceType<T>() where T : IService
        {
            return typeof(T).FullName.Replace($".{Constant.Service}", string.Empty);
        }

        public static string ActorType<T>() where T : Actor
        {
            return typeof(T).FullName.Replace($".{Constant.Service}", string.Empty);
        }
    }
}
