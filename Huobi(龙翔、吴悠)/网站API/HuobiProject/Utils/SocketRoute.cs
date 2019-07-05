using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using HuobiProject.Interface;
using System.Net.WebSockets;
using HuobiProject.Impl;
using Newtonsoft.Json.Linq;

namespace HuobiProject.Utils
{
    /// <summary>
    /// Socket路由
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SocketRouteAttribute : Attribute
    {
        public string Path { get; set; }
        public SocketRouteAttribute(string path) => Path = path;
    }

    /// <summary>
    /// Socket路由分发
    /// </summary>
    public class SocketRouteHandler
    {
        public static async Task DistributeAsync(HttpContext context, Type t)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            IWebSocket i_webSocket = new WebSocketImpl(webSocket);
            object instance = Activator.CreateInstance(t, new object[] { i_webSocket });
            i_webSocket.AddClient(i_webSocket);

            //t.GetMethod("Connect")?.Invoke(instance, new Object[] { context, null });

            await Echo(context, webSocket, i_webSocket, instance, t);

            try
            {
                i_webSocket.RemoveClient(i_webSocket);
            }
            catch (Exception)
            {

            }

            // IWebSocket iwebsocket = new WebSocketImpl(webSocket);

            // JObject recv_data = iwebsocket.ReceiveAsync();

            // Assembly assembly = Assembly.Load(ass);
            // MethodInfo method = null;
            // try
            // {
            //     //assembly.GetTypes().Select(s=>s.GetMethods()).Where(w=>w.GetCustomAttribute(typeof(SocketRouteAttribute)) != null)
            //     method = assembly
            //     .GetTypes()
            //     .SelectMany(s => s.GetMethods())
            //     .First(f => f
            //         .GetCustomAttributes(typeof(SocketRouteAttribute), false)
            //         .Any(w => ((SocketRouteAttribute)w).Path == recv_data["target"].ToString()));

            //     if (!context.WebSockets.IsWebSocketRequest || method == null)
            //         context.Response.StatusCode = 404;

            //     object socketProsess = Activator.CreateInstance(tySocket, new object[] { iwebsocket });
            //     method.Invoke(socketProsess, new object[] { context.Request, recv_data });
            // }
            // catch (Exception)
            // {
            //     iwebsocket.SendAsync(new Result(400, "请求错误"));
            // }
        }

        private async static Task Echo(HttpContext context, System.Net.WebSockets.WebSocket webSocket, IWebSocket i_webSocket, object instance, Type t)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    SocketResult data = await i_webSocket.ReceiveAsync();

                    // string target = json["target"].ToString();
                    // string method = json["method"].ToString();

                    // SocketResult data = new SocketResult(target, method, json["data"]);

                    if (data.method == "on")//监听
                        i_webSocket.AddListener(data.target, i_webSocket);
                    else
                        t.GetMethod(data.method + "_" + data.target)?.Invoke(instance, new Object[] { context, data });
                }
                catch (Exception)
                {
                    //context.Response.StatusCode = 404;
                }
            }
        }
    }
}