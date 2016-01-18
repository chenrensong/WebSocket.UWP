using System;
using System.Text;
using WebSocket4UWP.Internal;
using WebSocket4UWP.Security;

namespace WebSocket4UWP
{
    public class WebSocketHelper
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



    }
}
