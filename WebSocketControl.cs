using System.Collections.Generic;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;

namespace WebSocket4UWP
{
    public class WebSocketControl : IWebSocketControl
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public WebSocketMessageType MessageType { get; set; }

        public uint OutboundBufferSizeInBytes
        {
            get;
            set;
        }

        public PasswordCredential ProxyCredential
        {
            get;
            set;
        }

        public PasswordCredential ServerCredential
        {
            get;
            set;
        }

        public IList<string> SupportedProtocols
        {
            get;
            private set;
        }
    }
}
