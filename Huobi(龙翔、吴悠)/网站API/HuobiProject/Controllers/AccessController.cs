using System;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt;
using HuobiProject.Utils;
using HuobiProject.Interface;

namespace HuobiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private IRedis redisServer;
        private Tools tools;

        public AccessController(IRedis redisServer, Tools tools)
        {
            this.redisServer = redisServer;
            this.tools = tools;
        }

        /// <summary>
        /// 获取浏览器访问id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string AccessToken()
        {
            Random rand = new Random((int)Tools.Ticks());
            string client_id_str = Guid.NewGuid().ToString() + rand.Next(1000, 10000) + Tools.Ticks();
            string client_id = EncryptProvider.Md5(client_id_str);
            //redisServer.HashSet("Client:" + client_id, "client_id", client_id, tools.GetClientExpiry());
            // redisServer.HashSet("Client:" + client_id, "value_test", "123", tools.GetClientExpiry());
            // redisServer.HashGet("Client:" + client_id, "client_id"));
            return client_id;
        }
    }
}