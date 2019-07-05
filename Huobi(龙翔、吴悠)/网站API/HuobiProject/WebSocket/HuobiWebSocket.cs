using HuobiProject.Interface;
using HuobiProject.Utils;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HuobiProject.WebSocket
{
    public class HuobiWebSocket
    {
        private IWebSocket m_socket;
        public HuobiWebSocket(IWebSocket socket) => m_socket = socket;

        public void Connect(HttpContext context, SocketResult data)
        {
            m_socket.SendAsync(data);
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public void on_Notification(HttpContext context, SocketResult data)
        {
            System.Console.WriteLine(data.ToString());
            data.data = new Result(200, "OK", JsonConvert.SerializeObject(data.data));
            m_socket.SendListenerAsync(data.target, data);
        }
    }
}