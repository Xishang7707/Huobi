using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using HuobiProject.Interface;
using StackExchange.Redis;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace HuobiProject.Impl
{
    /// <summary>
    /// Redis操作实现
    /// </summary>
    public class RedisImpl : HuobiProject.Interface.IRedis
    {
        private IConnectionMultiplexer m_iConnectionMultiplexer;
        private string m_instanceName;
        private IDatabase m_db;
        public RedisImpl(ConfigurationOptions config, string instanceName = "")
        {
            this.m_iConnectionMultiplexer = ConnectionMultiplexer.Connect(config);
            this.m_instanceName = instanceName;
            this.m_db = m_iConnectionMultiplexer.GetDatabase(config.DefaultDatabase.Value);
        }

        /// <summary>
        /// 设置字符串
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        bool HuobiProject.Interface.IRedis.SetString(string key, string val, TimeSpan? expiry)
        {
            return m_db.StringSet(m_instanceName + key, val, expiry);
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string HuobiProject.Interface.IRedis.GetString(string key)
        {
            return m_db.StringGet(m_instanceName + key);
        }


        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool HuobiProject.Interface.IRedis.KeyExists(string key)
        {
            return m_db.KeyExists(m_instanceName + key);
        }

        /// <summary>
        /// Hash是否存在
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool HuobiProject.Interface.IRedis.HashExists(RedisKey hash, RedisValue key)
        {
            return m_db.HashExists(m_instanceName + hash.ToString(), key);
        }

        /// <summary>
        /// 键剩余时间
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        TimeSpan? HuobiProject.Interface.IRedis.KeyTimeToLive(string key)
        {
            return m_db.KeyTimeToLive(m_instanceName + key);
        }

        /// <summary>
        /// Hash设置
        /// </summary>
        /// <param name="hash">索引</param>
        /// <param name="key">Hash键</param>
        /// <param name="val">值</param>
        /// <param name="expiry">存活时间</param>
        /// <return></return>
        bool HuobiProject.Interface.IRedis.HashSet(RedisKey hash, RedisValue key, object val, TimeSpan? expiry)
        {
            bool result = m_db.HashSet(m_instanceName + hash.ToString(), key, JsonConvert.SerializeObject(val));

            m_db.KeyExpire(m_instanceName + hash.ToString(), expiry);

            return result;
        }

        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <param name="hash">索引</param>
        /// <param name="key">Hash键</param>
        /// <returns></returns>
        T HuobiProject.Interface.IRedis.HashGet<T>(RedisKey hash, RedisValue key)
        {
            return JsonConvert.DeserializeObject<T>(m_db.HashGet(m_instanceName + hash.ToString(), key));
        }

        /// <summary>
        /// 删除Hash键
        /// </summary>
        /// <param name="hash">索引</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool HuobiProject.Interface.IRedis.HashDelete(RedisKey hash, RedisValue key)
        {
            return m_db.HashDelete(m_instanceName + hash.ToString(), key);
        }
    }
}