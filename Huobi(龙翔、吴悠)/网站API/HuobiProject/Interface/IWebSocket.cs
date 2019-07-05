using System;
using System.Threading;
using System.Threading.Tasks;
using HuobiProject.Utils;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;

namespace HuobiProject.Interface
{
    public interface IWebSocket
    {
        System.Net.WebSockets.WebSocket Socket { get; }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="endMessage">是否末尾数据</param>
        Task SendAsync(SocketResult data, bool? endMessage = true);

        /// <summary>
        /// 向所有客户端发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="endMessage">是否末尾数据</param>
        Task SendAllAsync(SocketResult data, bool? endMessage = true);

        /// <summary>
        /// 向所有监听客户端发送数据
        /// </summary>
        /// <param name="target">监听目标</param>
        /// <param name="data">数据</param>
        /// <param name="endMessage">是否末尾数据</param>
        /// <returns></returns>
        Task SendListenerAsync(string target, SocketResult data, bool? endMessage = true);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns></returns>
        Task<SocketResult> ReceiveAsync();

        /// <summary>
        /// 新增客户端连接
        /// </summary>
        /// <param name="client"></param>
        void AddClient(IWebSocket client);

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        /// <param name="client"></param>
        void RemoveClient(IWebSocket client);

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="target">监听目标</param>
        /// <param name="client">客户端</param>
        void AddListener(string target, IWebSocket client);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="target">监听目标</param>
        /// <param name="client">客户端</param>
        void RemoveListener(string target, IWebSocket client);

        /// <summary>
        /// 移除客户端所有监听
        /// </summary>
        /// <param name="client">客户端</param>
        void RemoveListenerAll(IWebSocket client);
    }
}