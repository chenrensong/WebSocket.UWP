using System;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace WebSocket4UWP
{
    /// <summary>
    /// WebSocket接口
    /// </summary>
    public interface IWebSocket : IDisposable
    {
        /// <summary>
        /// 获取 IWebSocket 对象上写入远程网络目标的输出流。
        /// </summary>
        ///<returns>要写入远程目标的有序字节流。</returns>
        IOutputStream OutputStream { get; }


        /// <summary>
        /// 当 IWebSocket 对象作为关闭握手一部分收到结束帧时发生。
        /// </summary>
        event TypedEventHandler<IWebSocket, WebSocketClosedEventArgs> Closed;

        /// <summary>
        ///  关闭 IWebSocket。
        /// </summary>
        /// <param name="code">指示关闭原因的状态代码。</param>
        /// <param name="reason">包含有关关闭的其他信息的可选 UTF-8 编码数据。</param>
        void Close(ushort code, string reason);
 
        /// <summary>
        /// 启动 IWebSocket 对象连接到远程网络目标的异步操作。
        /// </summary>
        /// <param name="uri">供服务器连接的绝对 Uri。</param>
        /// <returns>IWebSocket 对象的异步连接操作。</returns>
        IAsyncAction ConnectAsync(Uri uri);

        /// <summary>
        ///    向由 IWebSocket 对象握手的 WebSocket 协议中使用的 HTTP 请求消息添加 HTTP 请求标题。
        /// </summary>
        /// <param name="headerName">请求标头的名称。</param>
        /// <param name="headerValue">请求标头的值。</param>
        void SetRequestHeader(string headerName, string headerValue);
    }
}
