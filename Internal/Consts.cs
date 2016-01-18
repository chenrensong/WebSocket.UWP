namespace WebSocket4UWP.Internal
{
    internal static class Consts
    {
        public static readonly char[] ExtensionParmeterValueNeedQuotesChars = { ' ', '\t', ',', ';', '=' };

        public const string HeaderOrigin = "Origin";
        public const string HeaderSecWebSocketAccept = "Sec-WebSocket-Accept";
        public const string HeaderSecWebSocketExtensions = "Sec-WebSocket-Extensions";
        public const string HeaderSecWebSocketKey = "Sec-WebSocket-Key";
        public const string HeaderSecWebSocketProtocol = "Sec-WebSocket-Protocol";
        public const string HeaderSecWebSocketVersion = "Sec-WebSocket-Version";

        public const string SchemeHttp = "http";
        public const string SchemeHttps = "https";
        public const string SchemeWs = "ws";
        public const string SchemeWss = "wss";

        /// <summary>
        /// 魔法数
        /// </summary>
        public const string ServerMagicGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        public static readonly string[] SupportedClientVersions = { "13" };

        public const int MaxDefaultFrameDataLength = 1016;
        public const int MaxAllowedFrameDataLength = 1024 * 256;
    }
}
