using Company.ServiceFabric.Common;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;

namespace Company.ServiceFabric.Client
{
    public class Proxy
    {
        private static I ForService<I>(Uri serviceAddress, ServicePartitionKey partitionKey = null) where I : class, IService
        {
            Debug.Assert(typeof(I).IsInterface);
            return ServiceProxy.Create<I>(serviceAddress, partitionKey: partitionKey, listenerName: Naming.Listener<I>());
        }

        private static I ForActor<I>(string instanceName, Actor caller) where I : class, IActor
        {
            Debug.Assert(typeof(I).IsInterface);
            return ActorProxy.Create<I>(new ActorId(instanceName), Addressing.ActorAddress<I>(caller));
        }

        public static I ForMicroservice<I>(ServicePartitionKey partitionKey = null) where I : class, IService
        {
            Debug.Assert(
                typeof(I).Namespace.Contains(Constant.Manager),
                $"Invalid microservice call. Use only the {Constant.Manager} interface to access a microservice.");
            return ForService<I>(Addressing.Microservice<I>(), partitionKey);
        }

        public static I ForComponent<I>(StatelessService caller, ServicePartitionKey partitionKey = null) where I : class, IService
        {
            Debug.Assert(caller != null, "Invalid component call. Must supply stateless service caller.");
            return ForService<I>(Addressing.Component<I>(caller), partitionKey);
        }

        public static I ForAccessor<I>() where I : class, IActor
        {
            return ForAccessor<I>(ActorId.CreateRandom().ToString());
        }

        public static I ForAccessor<I>(string instanceName) where I : class, IActor
        {
            Debug.Assert(
                typeof(I).Namespace.Contains(Constant.Accessor),
                $"Invalid resource call. Use only the {Constant.Accessor} interface to access a resource.");
            return ForActor<I>(instanceName, null);
        }

        public static I ForNode<I>(string instanceName, Actor caller) where I : class, IActor
        {
            if (caller == null)
            {
                throw new ArgumentNullException(nameof(caller), "Invalid node call. Must supply stateful actor caller.");
            }
            return ForActor<I>(instanceName, caller);
        }
    }
}
