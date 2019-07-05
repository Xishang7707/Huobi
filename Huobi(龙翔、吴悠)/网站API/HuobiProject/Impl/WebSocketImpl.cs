using System.Collections.Generic;
using HuobiProject.Interface;
using HuobiProject.Utils;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HuobiProject.Impl
{
    public class WebSocketImpl : IWebSocket
    {
        private static List<IWebSocket> m_socket_list = new List<IWebSocket>(); //所有客户端连接
        private static Dictionary<string, List<IWebSocket>> m_socket_listener = new Dictionary<string, List<IWebSocket>>();//客户端监听表

        private IWebSocket m_iSocket;

        private System.Net.WebSockets.WebSocket m_socket;
        System.Net.WebSockets.WebSocket IWebSocket.Socket { get { return m_socket; } }

        public WebSocketImpl() { }
        public WebSocketImpl(System.Net.WebSockets.WebSocket socket)
        {
            m_socket = socket;
            m_iSocket = this;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="endMessage">是否末尾数据</param>
        async Task IWebSocket.SendAsync(SocketResult data, bool? endMessage)
        {
            ArraySegment<byte> arg = new ArraySegment<byte>(
                UTF8Encoding.Default.GetBytes(
                    JsonConvert.SerializeObject(data)
                    )
                );
            await m_socket.SendAsync(arg, WebSocketMessageType.Text, endMessage.GetValueOrDefault(true), CancellationToken.None);
        }

        /// <summary>
        /// 向所有客户端发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="endMessage">是否末尾数据</param>
        async Task IWebSocket.SendAllAsync(SocketResult data, bool? endMessage)
        {
            foreach (var item in m_socket_list)
            {
                if (item.Socket.State == WebSocketState.Open)
                {
                    await item.SendAsync(data, endMessage.GetValueOrDefault(true));
                }
                else m_socket_list.Remove(item);
            }
        }

        /// <summary>
        /// 向所有监听客户端发送数据
        /// </summary>
        /// <param name="target">监听目标</param>
        /// <param name="data">数据</param>
        /// <param name="endMessage">是否末尾数据</param>
        /// <returns></returns>
        async Task IWebSocket.SendListenerAsync(string target, SocketResult data, bool? endMessage)
        {
            foreach (var client in m_socket_listener[target])
            {
                await client.SendAsync(data, endMessage);
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns></returns>
        async Task<SocketResult> IWebSocket.ReceiveAsync()
        {
            byte[] buffer = new byte[2048];

            var data = await m_socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            UTF8Encoding.Default.GetString(buffer);
            int count = 0;
            for (; count < buffer.Length; count++)
                if (buffer[count] == '\0')
                    break;
            string rcv_data = UTF8Encoding.Default.GetString(buffer, 0, count);
            //return JObject.Parse(rcv_data);
            JObject json = JObject.Parse(rcv_data);
            SocketResult res = new SocketResult(json["target"].ToString(), json["method"].ToString(), json["data"]);
            return res;
        }

        /// <summary>
        /// 新增客户端连接
        /// </summary>
        /// <param name="client"></param>
        void IWebSocket.AddClient(IWebSocket client)
        {
            if (m_socket_list.Contains(client))
                return;
            m_socket_list.Add(client);
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        /// <param name="client"></param>
        void IWebSocket.RemoveClient(IWebSocket client)
        {
            m_socket_list.Remove(client);
            m_iSocket.RemoveListenerAll(client);
            if (client.Socket.State == WebSocketState.Open)
                client.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="target">监听目标</param>
        /// <param name="client">客户端</param>
        void IWebSocket.AddListener(string target, IWebSocket client)
        {
            if (!m_socket_listener.ContainsKey(target))
                m_socket_listener.Add(target, new List<IWebSocket> { client });
            else if (!m_socket_listener[target].Contains(client))
                m_socket_listener[target].Add(client);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="target">监听目标</param>
        /// <param name="client">客户端</param>
        void IWebSocket.RemoveListener(string target, IWebSocket client)
        {
            if (!m_socket_listener.ContainsKey(target))
                return;

            m_socket_listener[target].Remove(client);
        }

        /// <summary>
        /// 移除客户端所有监听
        /// </summary>
        /// <param name="client">客户端</param>
        void IWebSocket.RemoveListenerAll(IWebSocket client)
        {
            foreach (var key in m_socket_listener.Keys)
            {
                m_socket_listener[key].Remove(client);
                if (m_socket_listener[key].Count == 0)
                    m_socket_listener.Remove(key);
            }
        }
    }
}