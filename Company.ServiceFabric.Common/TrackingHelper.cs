using Company.Utility;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using System;
using System.Diagnostics;

namespace Company.ServiceFabric.Common
{
    public class TrackingHelper
    {
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

            // Retrieve the tracking context from the message header, if it exists.
            if (requestMessageHeader.TryGetHeaderValue(TrackingContext.FullName, out byte[] byteArray))
            {
                // If an tracking context exists in the message header, always use it to replace the ambient context.
                TrackingContext tc = TrackingContext.DeSerialize(byteArray);
                tc.SetAsCurrent();
            }
            else
            {
                // If no tracking context exists then create one.
                TrackingContext.NewCurrentIfEmpty();

                Debug.Assert(TrackingContext.Current != null);

                // Copy the tracking context to the message header.
                byteArray = TrackingContext.Serialize(TrackingContext.Current);
                requestMessageHeader.AddHeader(TrackingContext.FullName, byteArray);
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

            // Retrieve the tracking context from the message header, if it exists.
            if (responseMessageHeader.TryGetHeaderValue(TrackingContext.FullName, out byte[] byteArray))
            {
                // If an tracking context exists in the message header, always use it to replace the ambient context.
                TrackingContext tc = TrackingContext.DeSerialize(byteArray);
                tc.SetAsCurrent();
            }
            else
            {
                // If no tracking context exists then create one.
                TrackingContext.NewCurrentIfEmpty();

                Debug.Assert(TrackingContext.Current != null);

                // Copy the tracking context to the message header.
                byteArray = TrackingContext.Serialize(TrackingContext.Current);
                responseMessageHeader.AddHeader(TrackingContext.FullName, byteArray);
            }

            return responseMessage;
        }
    }
}
