using Company.Utility.Audit;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using System;
using System.Diagnostics;
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

            // Retrieve the audit context from the message header, if it exists.
            if (requestMessageHeader.TryGetHeaderValue(AuditContext.Name, out byte[] byteArray))
            {
                // If an audit context exists in the message header, always use it to replace the ambient context.
                AuditContext.Current = (AuditContext)DeSerialize(byteArray);
            }
            else
            {
                // If no audit context exists then create one.
                AuditContext.NewCurrentIfEmpty();

                AuditContext context = AuditContext.Current;
                Debug.Assert(context != null);

                // Copy the audit context to the message header.
                byteArray = Serialize(context);
                requestMessageHeader.AddHeader(AuditContext.Name, byteArray);
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

            // Retrieve the audit context from the message header, if it exists.
            if (responseMessageHeader.TryGetHeaderValue(AuditContext.Name, out byte[] byteArray))
            {
                // If an audit context exists in the message header, always use it to replace the ambient context.
                AuditContext.Current = (AuditContext)DeSerialize(byteArray);
            }
            else
            {
                // If no audit context exists then create one.
                AuditContext.NewCurrentIfEmpty();

                AuditContext context = AuditContext.Current;
                Debug.Assert(context != null);

                // Copy the audit context to the message header.
                byteArray = Serialize(context);
                responseMessageHeader.AddHeader(AuditContext.Name, byteArray);
            }

            return responseMessage;
        }
    }
}
