﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;

namespace TinyWebSocket
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
