using System;
using System.Collections.Generic;
using StackExchange.Redis;
using Newtonsoft.Json.Linq;

namespace HuobiProject.Interface
{
    /// <summary>
    /// Redis接口
    /// </summary>
    public interface IRedis
    {
        /// <summary>
        /// 设置字符串
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="expiry">存活时间</param>
        /// <returns></returns>
        bool SetString(string key, string val, TimeSpan? expiry = null);


        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string GetString(string key);

        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool KeyExists(string key);

        /// <summary>
        /// Hash是否存在
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool HashExists(RedisKey hash, RedisValue key);

        /// <summary>
        /// 键剩余时间
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        TimeSpan? KeyTimeToLive(string key);


        /// <summary>
        /// Hash设置
        /// </summary>
        /// <param name="hash">索引</param>
        /// <param name="key">Hash键</param>
        /// <param name="val">值</param>
        /// <param name="expiry">存活时间</param>
        /// <return></return>
        bool HashSet(RedisKey hash, RedisValue key, object val, TimeSpan? expiry = null);

        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <param name="hash">索引</param>
        /// <param name="key">Hash键</param>
        /// <returns></returns>
        T HashGet<T>(RedisKey hash, RedisValue key);

        /// <summary>
        /// 删除Hash键
        /// </summary>
        /// <param name="hash">索引</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool HashDelete(RedisKey hash, RedisValue key);
    }
}