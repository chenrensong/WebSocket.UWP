using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace WebSocket4UWP
{
    public partial class WebSocket
    {
        private void ConnectServer()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    DataReader stream = new DataReader(this._socket.InputStream);

                    // Read HTTP request line.
                    string startLine = await this.ReadLine(stream);
                    if (startLine == null)
                    {
                        throw new Exception("Cannot read HTTP request start line");
                    }

                    Http.RequestLine requestLine = new Http.RequestLine(startLine);
                    this._uri = new Uri(requestLine.RequestURI); // can be checked in
                    // onConnect()

                    // Read HTTP response headers
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    string line;
                    while ((line = await this.ReadLine(stream)) != null && line.Length > 0)
                    {
                        Http.Header header = new Http.Header(line);
                        map.Add(header.HeaderName.ToLower(), header.HeaderValue);
                    }

                    string value = map["sec-websocket-version"];
                    if (!"13".Equals(value))
                        throw new IOException("wrong Sec-WebSocket-Version");

                    string key = map["sec-websocket-key"];
                    if (key == null)
                        throw new IOException("missed Sec-WebSocket-Key");
                    string accept = CreateAccept(key);

                    string upgrade = map["upgrade"];
                    if (upgrade == null || !upgrade.Equals("websocket", StringComparison.OrdinalIgnoreCase))
                        throw new IOException("wrong Upgrade");

                    string connection = map["connection"];
                    if (connection == null || !connection.Equals("upgrade", StringComparison.OrdinalIgnoreCase))
                        throw new IOException("wrong Connection");

                    // Host and Origin can be checked later in onConnect() callback.
                    this._host = map["host"];
                    if (this._host == null)
                        throw new IOException("Missed 'Host' header");

                    this._origin = map["origin"];
                    if (this._origin == null)
                        throw new IOException("Missed 'Origin' header");

                    // Some naive protocol selection.
                    string protocols = map["sec-websocket-protocol"];
                    string selectedProtocol = null;
                    if (protocols != null && protocols.Contains("chat"))
                        selectedProtocol = "chat";

                    DataWriter writer = new DataWriter(this._socket.OutputStream);
                    writer.WriteString("HTTP/1.1 101 Switching Protocols\r\n");
                    writer.WriteString("Upgrade: websocket\r\n");
                    writer.WriteString("Connection: Upgrade\r\n");
                    writer.WriteString("Sec-WebSocket-Accept: " + accept + "\r\n");
                    if (selectedProtocol != null)
                        writer.WriteString("Sec-WebSocket-Protocol: " + selectedProtocol + "\r\n");
                    writer.WriteString("\r\n");
                    await writer.FlushAsync();

                    if (this.Opened != null)
                    {
                        this.Opened(this, EventArgs.Empty);
                    }

                    // Read & process frame
                    for (; ; )
                    {
                        FrameParser.Frame frame = await FrameParser.ReadFrame(stream);
                        await this.ProcessIncomingFrame(frame);
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
                    this.Disconnect();
                }
            });
        }

    }
}
