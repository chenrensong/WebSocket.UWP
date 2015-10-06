using Windows.Networking;
using Windows.Networking.Sockets;

namespace WebSocket4UWP
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
