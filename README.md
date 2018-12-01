# WebSocket.UWP介绍

WebSocket.UWP是基于Windows.Networking.Sockets.StreamSocket的WebSocket客户端实现，能良好的运行在 Windows Universal Platform。

该项目初衷是为了让 贴吧UWP 支持WebSocket,目前开放出来供大家研究，使用方法如下：

```
      _socket = new WebSocket();
      _socket.SetRequestHeader("Sec-WebSocket-Extensions", "xxx");
      _socket.Opened += socket_Opened;
      _socket.Closed += socket_Closed;
      _socket.OnPong += socket_OnPong;
      _socket.OnMessage += socket_OnMessage;
      _socket.OnError += socket_OnError;
      await _socket.ConnectAsync(new Uri("ws://chenrensong.com/"));
```

# 支持平台
- Windows Universal Platform。 

# Nuget
``` c#
PM> Install-Package WebSocket.Net
