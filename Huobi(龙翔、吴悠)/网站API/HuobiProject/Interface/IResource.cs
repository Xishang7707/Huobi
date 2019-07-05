using System;
using System.IO;
using HuobiProject.Models;
using Microsoft.AspNetCore.Http;

namespace HuobiProject.Interface
{
    /// <summary>
    /// 资源接口
    /// </summary>
    public interface IResource : ICURD<TResource>
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type">类型信息</param>
        /// <param name="file">文件数据</param>
        /// <param name="contentType">传输格式</param>
        /// <returns>ID</returns>
        string Upload(TType type, IFormFile file, string contentType);

        /// <summary>
        /// 加密资源ID
        /// 有效期从配置文件读取
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>密文</returns>
        string EncryptResourceId(string id);

        /// <summary>
        /// 解密资源id
        /// </summary>
        /// <param name="enc_id">密文id</param>
        /// <returns>明文</returns>
        string DecryptResourceId(string enc_id);

        /// <summary>
        /// 验证资源id是否过期
        /// </summary>
        /// <param name="enc_id">密文id</param>
        /// <returns></returns>
        bool VerifyResourceId(string enc_id);

        /// <summary>
        /// 通过id获取文件流
        /// </summary>
        /// <param name="id">资源id</param>
        /// <returns>文件流</returns>
        FileStream RetrieveResourceFile(string id);
    }
}