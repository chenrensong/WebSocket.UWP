namespace WebSocket4UWP
{
    public sealed class WebSocketClosedEventArgs
    {
        internal WebSocketClosedEventArgs(int code, string reason)
        {
            this.Code = code;
            this.Reason = reason;
        }

        public int Code { get; private set; }
        public string Reason { get; private set; }
    }
}
