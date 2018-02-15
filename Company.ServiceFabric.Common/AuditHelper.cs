using Microsoft.ServiceFabric.Services.Remoting.V2;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Company.ServiceFabric.Common
{
    public class AuditHelper
    {
        private static byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            using (var ms = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, obj);
                var compressed = ms.ToArray();// Compress(ms.ToArray());
                return compressed;
            }
        }

        private static object DeSerialize(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                var decompressed = array;// Decompress(arrBytes);
                memoryStream.Write(decompressed, 0, decompressed.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(memoryStream);
            }
        }

        private static Guid NewInstanceId()
        {
            return Guid.NewGuid();
        }

        public static AuditContext Create()
        {
            return new AuditContext(NewInstanceId(), DateTime.UtcNow);
        }

        public static IServiceRemotingRequestMessage ProcessRequest(IServiceRemotingRequestMessage requestMessage)
        {
            if (requestMessage == null)
            {
                throw new ArgumentNullException(nameof(requestMessage));
            }

            IServiceRemotingRequestMessageHeader requestMessageHeader = requestMessage.GetHeader();

            if (requestMessageHeader == null)
            {
                return requestMessage;
            }

            // Retrieve the audit context from the ambient context, if it exists.
            AuditContext context = AuditContext.Current;

            // Retrieve the audit context from the message header, if it exists.
            if (requestMessageHeader.TryGetHeaderValue(AuditContext.Name, out byte[] byteArray))
            {
                // If an audit context exists in the message header, always use it to replace the ambient context.
                context = (AuditContext)DeSerialize(byteArray);
                AuditContext.Current = context;
            }
            else
            {
                // If no audit context exists in the message header, but does in the ambient context, copy it over.
                if (context != null)
                {
                    byteArray = Serialize(context);
                    requestMessageHeader.AddHeader(AuditContext.Name, byteArray);
                }
                else
                {
                    // If no audit context exists anywhere, then create one and copy it to the message header and ambient context.
                    context = Create();
                    byteArray = Serialize(context);
                    requestMessageHeader.AddHeader(AuditContext.Name, byteArray);
                    AuditContext.Current = context;
                }
            }

            return requestMessage;
        }

        public static IServiceRemotingResponseMessage ProcessResponse(IServiceRemotingResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            IServiceRemotingResponseMessageHeader responseMessageHeader = responseMessage.GetHeader();

            if (responseMessageHeader == null)
            {
                return responseMessage;
            }

            // Retrieve the audit context from the ambient context, if it exists.
            AuditContext context = AuditContext.Current;

            // Retrieve the audit context from the message header, if it exists.
            if (responseMessageHeader.TryGetHeaderValue(AuditContext.Name, out byte[] byteArray))
            {
                // If an audit context exists in the message header, always use it to replace the ambient context.
                context = (AuditContext)DeSerialize(byteArray);
                AuditContext.Current = context;
            }
            else
            {
                // If no audit context exists in the message header, but does in the ambient context, copy it over.
                if (context != null)
                {
                    byteArray = Serialize(context);
                    responseMessageHeader.AddHeader(AuditContext.Name, byteArray);
                }
                else
                {
                    // If no audit context exists anywhere, then create one and copy it to the message header and ambient context.
                    context = Create();
                    byteArray = Serialize(context);
                    responseMessageHeader.AddHeader(AuditContext.Name, byteArray);
                    AuditContext.Current = context;
                }
            }

            return responseMessage;
        }
    }
}
