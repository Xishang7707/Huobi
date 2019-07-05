using System;
using Microsoft.Extensions.Configuration;

namespace HuobiProject.Utils
{
    public class Tools
    {
        private IConfiguration config;

        public Tools(IConfiguration config) => this.config = config;

        /// <summary>
        /// 当前时间戳
        /// </summary>
        /// <returns></returns>
        public static long Ticks()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }


        /// <summary>
        /// 获取配置客户端过期时间
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetClientExpiry()
        {
            return TimeSpan.FromMinutes(int.Parse(config["Redis:Client-expiry"]));
        }
    }
}