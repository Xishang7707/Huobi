using System;
using System.IO;
using System.Linq;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;
using NETCore.Encrypt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HuobiProject.Impl
{
    /// <summary>
    /// 资源接口实现
    /// </summary>
    public class ResourceImpl : IResource
    {
        private HuobiContext m_db;
        private IConfiguration m_config;
        private IResource m_resource;
        private IType m_type;
        public ResourceImpl(HuobiContext db, IConfiguration config, IType type)
        {
            m_db = db;
            m_config = config;
            m_resource = this;
            m_type = type;
        }


        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TResource>.Create(TResource t)
        {
            t.CreateTime = DateTime.Now;

            m_db.TResource.Add(t);
            return m_db.SaveChanges() > 0;
        }
        /// <summary>
        /// 根据ID更新数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TResource>.Update(TResource t)
        {
            TResource res = m_db.TResource.Find(t.ResourceId);

            res.TypeId = t.TypeId;
            res.Value = t.Value;

            return m_db.SaveChanges() > 0;
        }
        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：NULL
        /// </returns>
        TResource ICURD<TResource>.Retrieve(TResource t)
        {
            return m_db.TResource.Find(t.ResourceId);
        }
        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：TResource
        /// Failed：NULL
        /// </returns>
        bool ICURD<TResource>.Delete(TResource t)
        {
            TResource res = m_db.TResource.Find(t.ResourceId);
            m_db.TResource.Remove(res);
            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type">类型信息</param>
        /// <param name="file">文件数据</param>
        /// <returns>ID</returns>
        string IResource.Upload(TType type, IFormFile file, string contentType)
        {
            string ex_name = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            string filename = Guid.NewGuid().ToString();
            string filepath = m_config["Resource:Path:" + type.Name] + "/" + filename + ex_name;
            //if (File.Exists(filepath))
            using (FileStream fs_file = File.Create(filepath))
            {
                file.CopyTo(fs_file);
            }
            Random rand = new Random((int)Tools.Ticks());
            TResource res = new TResource();
            res.ResourceId = type.TypeId + "" + Tools.Ticks() + "-" + rand.Next(1000, 10000);
            res.Value = filename + ex_name;
            res.TypeId = type.TypeId;
            res.ContentType = contentType;
            m_resource.Create(res);

            return res.ResourceId;
        }

        /// <summary>
        /// 加密资源ID
        /// 有效期从配置文件读取
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>密文</returns>
        string IResource.EncryptResourceId(string id)
        {
            string key = m_config["DESKeys"];//密钥
            long exp = long.Parse(m_config["Resource:Expired"]);//过期时间

            var data = new
            {
                exp = Tools.Ticks(),
                id
            };

            string enc = EncryptProvider.DESEncrypt(JsonConvert.SerializeObject(data), key);
            return EncryptProvider.Base64Encrypt(enc);
        }

        /// <summary>
        /// 解密资源id
        /// </summary>
        /// <param name="enc_id">密文id</param>
        /// <returns>明文</returns>
        string IResource.DecryptResourceId(string enc_id)
        {
            string key = m_config["DESKeys"];//密钥
            string enc = EncryptProvider.Base64Decrypt(enc_id);
            return EncryptProvider.DESDecrypt(enc, key);
        }

        /// <summary>
        /// 验证资源id是否过期
        /// </summary>
        /// <param name="enc_id">密文id</param>
        /// <returns></returns>
        bool IResource.VerifyResourceId(string enc_id)
        {
            string dec = m_resource.DecryptResourceId(enc_id);
            long conf_exp = long.Parse(m_config["Resource:Expired"]);
            JObject json = JObject.Parse(dec);
            long exp = long.Parse(json["exp"].ToString());

            if (exp - Tools.Ticks() < conf_exp)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 通过id获取文件流
        /// </summary>
        /// <param name="id">资源id</param>
        /// <returns>文件流</returns>
        FileStream IResource.RetrieveResourceFile(string id)
        {
            TResource res = m_resource.Retrieve(new TResource()
            {
                ResourceId = id
            });
            TType type = m_type.Retrieve(new TType()
            {
                TypeId = res.TypeId
            });

            string filepath = m_config["Resource:Path:" + type.Name] + "/" + res.Value;
            if (!File.Exists(filepath))
                return null;

            return File.OpenRead(filepath);
        }
    }
}