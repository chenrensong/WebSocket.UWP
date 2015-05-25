using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace TinyWebSocket
{
    /// <summary>
    /// 内部Socket实现
    /// </summary>
    internal sealed class InternalSocket
    {
        private StreamSocket _streamSocket;

        public InternalSocket()
        {
            _streamSocket = new StreamSocket();
        }

        public StreamSocketControl Control
        {
            get
            {
                return _streamSocket.Control;
            }
        }

        public StreamSocketInformation Information
        {
            get
            {
                return _streamSocket.Information;
            }
        }

        public IInputStream InputStream
        {
            get
            {
                return _streamSocket.InputStream;
            }
        }

        public IOutputStream OutputStream
        {
            get
            {
                return _streamSocket.OutputStream;
            }
        }


        public IAsyncAction ConnectAsync(EndpointPair endpointPair)
        {
            return _streamSocket.ConnectAsync(endpointPair);
        }

        public IAsyncAction ConnectAsync(EndpointPair endpointPair, SocketProtectionLevel protectionLevel)
        {
            return _streamSocket.ConnectAsync(endpointPair, protectionLevel);
        }

        public IAsyncAction ConnectAsync(string host, int port)
        {
            var hostName = new HostName(host);
            return _streamSocket.ConnectAsync(hostName, port.ToString());
        }

        public IAsyncAction ConnectAsync(HostName remoteHostName, string remoteServiceName)
        {
            return _streamSocket.ConnectAsync(remoteHostName, remoteServiceName);
        }

        public IAsyncAction ConnectAsync(HostName remoteHostName, string remoteServiceName, SocketProtectionLevel protectionLevel)
        {
            return _streamSocket.ConnectAsync(remoteHostName, remoteServiceName, protectionLevel);
        }

        public IAsyncAction ConnectAsync(HostName remoteHostName, string remoteServiceName, SocketProtectionLevel protectionLevel, NetworkAdapter adapter)
        {
            return _streamSocket.ConnectAsync(remoteHostName, remoteServiceName, protectionLevel, adapter);
        }
        public void Dispose()
        {
            _streamSocket.Dispose();
        }

        public IAsyncAction UpgradeToSslAsync(string host)
        {
            var hostName = new HostName(host);
            return UpgradeToSslAsync(SocketProtectionLevel.Ssl, hostName);
        }
        public IAsyncAction UpgradeToSslAsync(SocketProtectionLevel protectionLevel, HostName validationHostName)
        {
            return _streamSocket.UpgradeToSslAsync(protectionLevel, validationHostName);
        }



    }
}
