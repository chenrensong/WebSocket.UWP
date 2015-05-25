using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace WebSocketNet
{
    public class WebSocketInformation : IWebSocketInformation
    {
        public BandwidthStatistics BandwidthStatistics
        {
            get;
            private set;
        }

        public HostName LocalAddress
        {
            get;
            private set;
        }

        public string Protocol
        {
            get;
            private set;
        }
    }
}
