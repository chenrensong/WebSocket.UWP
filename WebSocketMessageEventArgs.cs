using System;
using System.Text;
using WebSocket4UWP.Internal;

namespace WebSocket4UWP
{
    public sealed class WebSocketMessageEventArgs
    {

        public WebSocketMessageEventArgs(WebSocketMessageType messageType, byte[] data)
        {
            this._messageType = messageType;
            this._data = data;
        }

        private byte[] _data = null;
        private string _text = null;
        private WebSocketMessageType _messageType;


        public string Text
        {
            get
            {
                if (_messageType == WebSocketMessageType.Binary)
                {
                    throw new InvalidOperationException("This request message type is binary, Can't convert to Text");
                }
                if (this._data != null && string.IsNullOrEmpty(this._text))
                {
                    this._text = Encoding.UTF8.GetString(_data, 0, _data.Length); 
                }
                return _text;
            }
        }

        public byte[] Data
        {
            get
            {
                return this._data;
            }
            private set
            {
                this._data = value;
            }
        }



    }
}
