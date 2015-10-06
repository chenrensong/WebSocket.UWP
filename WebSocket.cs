using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using WebSocket4UWP.ToolBox;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Networking.Sockets;

namespace WebSocket4UWP
{

    /**
     * 
     It's rewritten code of Eric Butler Android Web Socket <eric@codebutler.com>
     * 
     * - removed Android API (thread) - removed double arithmetic usage from Web
     * Socket header parser - added naive Web Server Socket
     * 
     * 
     * The MIT Licence
     * 
     * 
     * Copyright (c) 2009-2012 James Coglan Copyright (c) 2012 Eric Butler Copyright
     * (c) 2013 Igor Kolosov
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy
     * of this software and associated documentation files (the 'Software'), to deal
     * in the Software without restriction, including without limitation the rights
     * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
     * copies of the Software, and to permit persons to whom the Software is
     * furnished to do so, subject to the following conditions:
     * 
     * The above copyright notice and this permission notice shall be included in
     * all copies or substantial portions of the Software.
     * 
     * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
     * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
     * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
     * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
     * SOFTWARE.
     */
    public partial class WebSocket : IWebSocket, IDisposable
    {

        /// <summary>
        /// 强制为true，不能使用Server功能
        /// </summary>
        private const bool IS_CLIENT = true;
        /// <summary>
        /// 魔法数
        /// </summary>
        private const string MAGIC_NUM = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";


        private Uri _uri;
        private InternalSocket _socket;
        private List<Http.Header> _extraRequestHeaders = null;
        private List<Http.Header> _serverResponseHeaders = null;

        private Stream _stream = new MemoryStream();
        private bool _closed;
        private string _host; // set in server websocket to check in onConnect();
        private string _origin; // set in server websocket to check in onConnect();
        private bool _disposed;

        /// <summary>
        /// 连接建立
        /// </summary>
        public event EventHandler Opened;
        public event EventHandler<WebSocketMessageEventArgs> OnMessage;
        public event TypedEventHandler<IWebSocket, WebSocketClosedEventArgs> Closed;
        public event EventHandler<Exception> OnError;
        public event EventHandler<byte[]> OnPong;

        public WebSocketControl Control { get; private set; }

        public WebSocketInformation Information { get; private set; }

        public IOutputStream OutputStream
        {
            get
            {
                return this._socket.OutputStream;
            }
        }


        public WebSocket()
        {
            this.Control = new WebSocketControl();
            this.Information = new WebSocketInformation();
        }

        event TypedEventHandler<IWebSocket, Windows.Networking.Sockets.WebSocketClosedEventArgs> IWebSocket.Closed
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        private void ResetBuffer()
        {
            this.Control.MessageType = default(WebSocketMessageType);
            _stream.Seek(0, SeekOrigin.Begin);
        }

        private async Task ProcessIncomingFrame(FrameParser.Frame frame)
        {
            switch (frame.opCode)
            {
                case FrameParser.OP_CONTINUATION:
                    //if (this.Control.MessageType == default(WebSocketMessageType))
                    //{
                    //    throw new IOException("Unexpected CONTINUATION frame");
                    //}
                    _stream.Write(frame.payload, 0, frame.payload.Length);
                    if (frame.isFinal)
                    {
                        byte[] message = _stream.StreamToBytes();
                        if (this.OnMessage != null)
                        {
                            this.OnMessage(this, new WebSocketMessageEventArgs(this.Control.MessageType, message));
                        }

                        ResetBuffer();
                    }
                    break;

                case FrameParser.OP_TEXT:
                    if (frame.isFinal)
                    {
                        if (this.OnMessage != null)
                        {
                            this.OnMessage(this, new WebSocketMessageEventArgs(this.Control.MessageType, frame.payload));
                        }
                    }
                    else
                    {
                        if (_stream.Length != 0)
                        {
                            throw new IOException("no FIN frame");
                        }
                        this.Control.MessageType = WebSocketMessageType.Text;
                        _stream.Write(frame.payload, 0, frame.payload.Length);
                    }
                    break;

                case FrameParser.OP_BINARY:
                    if (frame.isFinal)
                    {
                        if (this.OnMessage != null)
                        {
                            this.OnMessage(this, new WebSocketMessageEventArgs(this.Control.MessageType, frame.payload));
                        }
                    }
                    else
                    {
                        if (_stream.Length != 0)
                        {
                            throw new IOException("no FIN frame");
                        }
                        this.Control.MessageType = WebSocketMessageType.Binary;
                        _stream.Write(frame.payload, 0, frame.payload.Length);
                    }
                    break;

                case FrameParser.OP_CLOSE:
                    int code = 0;
                    if (frame.payload.Length >= 2)
                    {
                        code = ((frame.payload[0] << 8) | (frame.payload[1] & 0xFF)) & 0xFFFF;
                    }
                    String reason = null;
                    if (frame.payload.Length > 2)
                    {
                        reason = frame.payload.CopyOfRange(2, frame.payload.Length).BytesToString();
                    }
                    if (this.Closed != null)
                    {
                        this.Closed(this, new WebSocketClosedEventArgs(code, reason));
                    }

                    Disconnect();
                    break;

                case FrameParser.OP_PING:
                    if (frame.payload.Length > 125)
                    {
                        throw new IOException("Ping payload too large");
                    }
                    byte[] data = FrameParser.BuildFrame(frame.payload, FrameParser.OP_PONG, -1, IS_CLIENT, true);
                    await SendFrame(data);
                    break;

                case FrameParser.OP_PONG:
                    if (this.OnPong != null)
                    {
                        this.OnPong(this, frame.payload);
                    }
                    break;
            }
        }

        private async Task ConnectClientInternal()
        {
            if (_socket != null)
            {
                throw new Exception("connect() is already called");
            }

            await Task.Factory.StartNew(async () =>
                  {

                      try
                      {
                          int port = (_uri.Port != -1) ? _uri.Port : (_uri.Scheme.Equals("wss") ? 443 : 80);
                          string path = (_uri.AbsolutePath != null) ? _uri.AbsolutePath : "/";
                          if (_uri.Query != null)
                          {
                              path += "?" + _uri.Query;
                          }
                          string originScheme = _uri.Scheme.Equals("wss") ? "https" : "http";
                          Uri origin = new Uri(originScheme + "://" + _uri.Host);
                          // To fix: get Origin from extraHeaders if set there.
                          this._socket = new InternalSocket();
                          await this._socket.ConnectAsync(_uri.Host, port);
                          if (_uri.Scheme.Equals("wss"))
                          {
                              await this._socket.UpgradeToSslAsync(_uri.Host);
                          }
                          string key = CreateSecKey();
                          DataWriter writer = new DataWriter(_socket.OutputStream);
                          writer.WriteString("GET " + path + " HTTP/1.1\r\n");
                          writer.WriteString("Upgrade: websocket\r\n");
                          writer.WriteString("Connection: Upgrade\r\n");
                          writer.WriteString("Host: " + _uri.Host + "\r\n");
                          writer.WriteString("Origin: " + origin.ToString() + "\r\n");
                          writer.WriteString("Sec-WebSocket-Key: " + key + "\r\n");
                          writer.WriteString("Sec-WebSocket-Version: 13\r\n");

                          if (_extraRequestHeaders != null)
                          {
                              foreach (Http.Header header in _extraRequestHeaders)
                              {
                                  writer.WriteString(header.ToString() + "\r\n");
                              }
                          }
                          writer.WriteString("\r\n");
                          await writer.StoreAsync();  //异步发送数据
                          writer.DetachStream();  //分离
                          writer.Dispose();  //结束writer
                          DataReader reader = new DataReader(_socket.InputStream);
                          reader.ByteOrder = ByteOrder.LittleEndian;
                          //// Read HTTP response status line.
                          var startLine = await ReadLine(reader);
                          if (startLine == null)
                          {
                              throw new Exception("Received no reply from server.");
                          }
                          Http.StatusLine statusLine = new Http.StatusLine(startLine);
                          int statusCode = statusLine.getStatusCode();
                          if (statusCode != 101)
                          {
                              throw new Exception("wrong HTTP response code: " + statusCode);
                          }

                          // Read HTTP response headers.
                          String line;
                          while ((line = await ReadLine(reader)) != null && line.Length > 0)
                          {
                              Http.Header header = new Http.Header(line);
                              Debug.WriteLine(line);

                              if (header.HeaderName.Equals("Sec-WebSocket-Accept", StringComparison.OrdinalIgnoreCase))
                              {
                                  String receivedAccept = header.HeaderValue;
                                  String shouldBeAccept = CreateAccept(key);
                                  if (!receivedAccept.Equals(shouldBeAccept))
                                      throw new Exception("Wrong Sec-WebSocket-Accept: " + receivedAccept + " should be: " + shouldBeAccept);
                              }
                              if (_serverResponseHeaders == null)
                              {
                                  _serverResponseHeaders = new List<Http.Header>();
                              }
                              _serverResponseHeaders.Add(header);

                          }
                          if (this.Opened != null)
                          {
                              this.Opened(this, EventArgs.Empty);
                          }
                          //Upgrade: websocket
                          //Connection: Upgrade
                          //Sec-WebSocket-Accept: 1xY289lHcEMbLpEBgOYRBBL9N9c=
                          //Sec-WebSocket-Protocol: chat
                          //Content-Type: application/octet-stream
                          //Seq-Id: 667035124
                          // Read & process frame
                          while (true)
                          {
                              FrameParser.Frame frame = await FrameParser.ReadFrame(reader);

                              if (frame != null)
                              {
                                  await ProcessIncomingFrame(frame);
                              }

                          }
                      }
                      catch (IOException ex)
                      {
                          if (this.Closed != null)
                          {
                              this.Closed(this, new WebSocketClosedEventArgs(0, "EOF"));
                          }
                      }
                      catch (Exception ex)
                      {
                          if (this.Closed != null)
                          {
                              this.Closed(this, new WebSocketClosedEventArgs(0, "Exception"));
                          }
                      }
                      finally
                      {
                          Disconnect();
                      }

                  });
        }

        private void Disconnect()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Dispose();
                    _socket = null;
                    _closed = true;
                }
                catch (Exception ex)
                {
                    if (this.OnError != null)
                    {
                        this.OnError(this, ex);
                    }
                }
            }
        }

        public async Task Send(string str)
        {
            await SendFragment(str, true, true);
        }

        public async Task Send(byte[] data)
        {
            await SendFragment(data, true, true);
        }

        public async Task SendFragment(String str, bool isFirst, bool isLast)
        {
            if (_closed)
            {
                return;
            }
            byte[] data = str.StringToBytes();
            byte[] frame = FrameParser.BuildFrame(data, isFirst ? FrameParser.OP_TEXT : FrameParser.OP_CONTINUATION, -1, IS_CLIENT, isLast);
            await SendFrame(frame);
        }

        public async Task SendFragment(byte[] data, bool isFirst, bool isLast)
        {
            if (_closed)
            {
                return;
            }
            byte[] frame = FrameParser.BuildFrame(data, isFirst ? FrameParser.OP_BINARY : FrameParser.OP_CONTINUATION, -1, IS_CLIENT, isLast);
            await SendFrame(frame);
        }

        public async Task Ping(byte[] data = null)
        {
            if (_closed)
            {
                return;
            }
            byte[] frame = FrameParser.BuildFrame(data, FrameParser.OP_PING, -1, IS_CLIENT, true);
            await SendFrame(frame);
        }


        private async Task SendFrame(byte[] frame)
        {
            await Task.Factory.StartNew(async () =>
                  {
                      try
                      {
                          var outputStream = _socket.OutputStream;
                          DataWriter writer = new DataWriter(outputStream);
                          writer.WriteBytes(frame);
                          await writer.StoreAsync();  //异步发送数据
                          writer.DetachStream();  //分离
                          writer.Dispose();  //结束writer
                      }
                      catch (IOException ex)
                      {
                          if (this.OnError != null)
                          {
                              this.OnError(this, ex);
                          }
                      }

                  });
        }



        private async Task<String> ReadLine(DataReader reader)
        {
            StringBuilder stringBuilder = new StringBuilder();
            await reader.LoadAsync(sizeof(byte));

            int readChar = reader.ReadByte();
            if (readChar == -1)
            {
                return null;
            }
            while (readChar != '\n')
            {
                if (readChar != '\r')
                {
                    stringBuilder.Append((char)readChar);
                }
                await reader.LoadAsync(sizeof(byte));
                readChar = reader.ReadByte();
                if (readChar == -1)
                {
                    return null;
                }
            }
            return stringBuilder.ToString();
        }


        private static string GenerateHashedString(byte[] b)
        {
            var algorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var buffer = b.BytesToBuffer();
            var hashedData = algorithm.HashData(buffer);
            return Convert.ToBase64String(hashedData.BufferToBytes());
        }

        private static String CreateSecKey()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(
                Guid.NewGuid().ToString().Substring(0, 16))); ;
        }


        private static String CreateAccept(String key)
        {
            String str = key + MAGIC_NUM;
            byte[] inData = null;
            try
            {
                inData = Encoding.ASCII.GetBytes(str);
            }
            catch (Exception ex)
            {
                throw new Exception("ASCII encoding is not supported");
            }
            return GenerateHashedString(inData);


        }





        public IAsyncAction ConnectAsync(Uri uri)
        {
            this._uri = uri;
            return AsyncInfo.Run(
              (token) => // CancellationToken token
                  Task.Run(
                     async () =>
                     {
                         await ConnectClientInternal();
                         token.ThrowIfCancellationRequested();
                     },
                      token));
        }


        /// <summary>
        /// 获取返回头
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public string GetResponseHeader(string headerName)
        {
            if (this._serverResponseHeaders == null || this._serverResponseHeaders.Count == 0)
            {
                return null;
            }
            foreach (var item in this._serverResponseHeaders)
            {
                if (item.HeaderName != null && item.HeaderName.Equals(headerName))
                {
                    return item.HeaderValue;
                }
            }
            return null;

        }


        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        public void SetRequestHeader(string headerName, string headerValue)
        {
            if (this._extraRequestHeaders == null)
            {
                this._extraRequestHeaders = new List<Http.Header>();
            }
            this._extraRequestHeaders.Add(new Http.Header(headerName, headerValue));
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebSocket()
        {
            Dispose(false);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (this._socket != null)
                {
                    this._socket.Dispose();
                    this._socket = null;
                }
            }
            _disposed = true;
        }

        public async void Close(ushort code, string reason)
        {
            if (_closed)
            {
                return;
            }
            byte[] data = null;
            if (reason != null && reason.Length > 0)
            {
                try
                {
                    data = Encoding.UTF8.GetBytes(reason);
                }
                catch (Exception e)
                {
                    data = new byte[0];
                }
            }
            else
            {
                data = new byte[0];
            }
            byte[] frame = FrameParser.BuildFrame(data, FrameParser.OP_CLOSE, code, IS_CLIENT, true);
            await SendFrame(frame);
            Disconnect();
        }
    }
}
