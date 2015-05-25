using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketNet.ToolBox;

namespace WebSocketNet
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
                    this._text = this._data.BytesToString();
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
