using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace TinyWebSocket
{
    /// <summary>
    /// WebSocket接口
    /// </summary>
    public interface IWebSocket : IDisposable
    {
        // 摘要: 
        //     获取 IWebSocket 对象上写入远程网络目标的输出流。
        //
        // 返回结果: 
        //     要写入远程目标的有序字节流。

        IOutputStream OutputStream { get; }

        // 摘要: 
        //     当 IWebSocket 对象作为关闭握手一部分收到结束帧时发生。
        event TypedEventHandler<IWebSocket, WebSocketClosedEventArgs> Closed;

        // 摘要: 
        //     关闭 IWebSocket。
        //
        // 参数: 
        //   code:
        //     指示关闭原因的状态代码。
        //
        //   reason:
        //     包含有关关闭的其他信息的可选 UTF-8 编码数据。
        Task Close(int code, string reason);
        //
        // 摘要: 
        //     启动 IWebSocket 对象连接到远程网络目标的异步操作。
        //
        // 参数: 
        //   uri:
        //     供服务器连接的绝对 Uri。
        //
        // 返回结果: 
        //     IWebSocket 对象的异步连接操作。
        IAsyncAction ConnectAsync(Uri uri);
        //
        // 摘要: 
        //     向由 IWebSocket 对象握手的 WebSocket 协议中使用的 HTTP 请求消息添加 HTTP 请求标题。
        //
        // 参数: 
        //   headerName:
        //     请求标头的名称。
        //
        //   headerValue:
        //     请求标头的值。
        void SetRequestHeader(string headerName, string headerValue);
    }
}
