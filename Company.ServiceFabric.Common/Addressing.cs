using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;

namespace Company.ServiceFabric.Common
{
    public static class Addressing
    {
        public static Uri Microservice<I>() where I : IService
        {
            Debug.Assert(typeof(I).IsInterface);
            Debug.Assert(
                typeof(I).Namespace.Contains($"{Constant.Manager}"),
                $"Invalid microservice interface. Use only the {Constant.Manager} interface to access a microservice.");
            string[] namespaceSegments = typeof(I).Namespace.Split('.');
            return new Uri($"{Constant.FabricScheme}/{Naming.Microservice<I>()}/{Naming.Component<I>()}");
        }

        public static Uri Component<I>(StatelessService caller) where I : IService
        {
            Debug.Assert(typeof(I).IsInterface);
            string[] namespaceSegments = caller?.Context?.ServiceName?.Segments;
            if (namespaceSegments == null)
            {
                throw new ArgumentNullException(nameof(namespaceSegments));
            }
            if (namespaceSegments.Length < Constant.NamespaceMinimumSize)
            {
                throw new FormatException(@"Component namespace is an invalid format.");
            }
            return new Uri($"{Constant.FabricScheme}/{namespaceSegments[1]}{Naming.Component<I>()}");
        }

        public static Uri ActorAddress<I>() where I : IActor
        {
            return ActorAddress<I>(null);
        }

        public static Uri ActorAddress<I>(Actor caller) where I : IActor
        {
            Debug.Assert(typeof(I).IsInterface);
            string[] namespaceSegments = null;
            Uri serviceUri = null;
            if (caller == null)
            {
                namespaceSegments = typeof(I).Namespace.Split('.');
                if (namespaceSegments.Length < Constant.NamespaceMinimumSize)
                {
                    throw new FormatException(@"Actor namespace is an invalid format.");
                }
                serviceUri = new Uri($"{Constant.FabricScheme}/{namespaceSegments[0]}.{Constant.Resource}.{namespaceSegments[2]}/{typeof(I).Name.TrimStart('I')}");
            }
            else
            {
                namespaceSegments = caller.ServiceUri.Segments;
                if (namespaceSegments.Length < Constant.NamespaceMinimumSize)
                {
                    throw new FormatException(@"Actor namespace is an invalid format.");
                }
                serviceUri = new Uri($"{Constant.FabricScheme}/{namespaceSegments[1]}{typeof(I).Name.TrimStart('I')}");
            }
            return serviceUri;
        }
    }
}
