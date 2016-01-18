using System;
using System.Text;
using WebSocket4UWP.Internal;
using WebSocket4UWP.Security;

namespace WebSocket4UWP
{
    internal class WebSocketHelper
    {
        private static readonly Random _rnd = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string CreateClientKey()
        {
            //return Convert.ToBase64String(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString().Substring(0, 16))); ;
            var bytes = new byte[16];
            lock (_rnd)
                _rnd.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] CreateMaskingKey()
        {
            var bytes = new byte[4];
            lock (_rnd)
                _rnd.NextBytes(bytes);
            return bytes;
        }

        public static string CreateAccept(string key)
        {
            var challenge = Encoding.ASCII.GetBytes(key + Consts.ServerMagicGuid);
            var hash = Sha1Digest.ComputeHash(challenge);
            var calculatedAccept = Convert.ToBase64String(hash);
            return calculatedAccept;
        }


        public static Uri CreateWebSocketUri(Uri sourceUri)
        {
            if (sourceUri == null)
                throw new ArgumentException(Error.MustNotBeNullOrEmpty, "sourceUri");


            var uri = sourceUri;
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(Error.NotAnAbsoluteUri, "sourceUri");

            var scheme = uri.Scheme;
            if (scheme != "ws" && scheme != "wss")
                throw new ArgumentException(Error.InvalidScheme + scheme, "sourceUri");

            var fragment = uri.Fragment;
            if (fragment.Length > 0)
                throw new ArgumentException(Error.MustNotContainAFragment, "sourceUri");


            var port = uri.Port;
            if (port == 0)
            {
                port = scheme == "ws" ? 80 : 443;
                var url = String.Format("{0}://{1}:{2}{3}{4}", scheme, uri.Host, port, uri.LocalPath, uri.Query);
                uri = new Uri(url);
            }

            return uri;
        }


    }
}
