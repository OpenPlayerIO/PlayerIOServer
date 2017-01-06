using Nancy;
using ProtoBuf;

namespace OpenPlayerIO.PlayerIOServer.WebServer
{
    using System.Collections.Generic;
    using Extensions;
    using Helpers;

    public class ChannelResponse
    {
        /// <param name="token"> The PlayerIO API Authentication Token. </param>
        /// <param name="instance"> The Player.IO Channel Message. </param>
        public Response Get(object instance, string token = null, bool checkHeader = true)
        {
            var response = new Response();

            response.Contents = s => {
                if (token != null) {
                    s.WriteBytes(PlayerTokenHelper.MaskPlayerTokenBytes(token, checkHeader));
                }

                Serializer.Serialize(s, instance);
            };

            return response;
        }
    }
}