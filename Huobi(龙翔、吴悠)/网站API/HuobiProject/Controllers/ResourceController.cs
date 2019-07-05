using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using HuobiProject.Interface;
using HuobiProject.Utils;
using NETCore.Encrypt;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using HuobiProject.Models;

namespace HuobiProject.Controllers
{
    /// <summary>
    /// 资源API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private IConfiguration config;
        private IResource resourceServer;
        private IUser userServer;
        private IType typeServer;
        public ResourceController(IConfiguration config, IResource resourceServer, IUser userServer, IType typeServer)
        {
            this.config = config;
            this.resourceServer = resourceServer;
            this.userServer = userServer;
            this.typeServer = typeServer;
        }

        /// <summary>
        /// 图片上传
        /// 需要授权-管理用户
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("upload")]
        public ActionResult<Result> UploadFile([FromForm] IFormCollection form)
        {
            IFormFile file = form.Files["upload"];

            if (file == null)
                throw new ResultException("文件为空");

            string resourceType_str = config["Resource:Type:" + file.ContentType] ?? config["Resource:Type:Others"];
            int resourceType = int.Parse(resourceType_str);

            TType type_info = typeServer.RetrieveResourceTypeByTypeId(resourceType);

            if (type_info == null)
                throw new ResultException("资源类型错误");

            //如果是其他文件则以文件流形式传输
            string contentType = null;
            if (type_info.TypeId == int.Parse(config["Resource:Type:Others"]))
                contentType = "application/octet-stream";
            else contentType = file.ContentType;

            string file_id = resourceServer.Upload(type_info, file, contentType);

            return new Result(200, "成功", file_id);
        }

        /// <summary>
        /// 编辑器图片上传
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("image/upload-editor-huobi")]
        public void UploadEditorFile([FromForm] IFormCollection form)
        {
            IFormFile file = form.Files["upload"];
            if (file == null)
                throw new ResultException("文件为空");

            string resourceType_str = config["Resource:Type:" + file.ContentType] ?? config["Resource:Type:Others"];
            int resourceType = int.Parse(resourceType_str);

            TType type_file = typeServer.RetrieveResourceTypeByTypeId(resourceType);
            TType type_info = typeServer.RetrieveResourceByName("Images");

            if (type_info == null || type_info.TypeId != type_file.TypeId)
                throw new ResultException("资源类型错误");

            string file_id = resourceServer.Upload(type_info, file, file.ContentType);

            string re_url = HttpContext.Request.Headers["origin"] + HttpContext.Request.Query["callUrl"] + HttpContext.Request.QueryString.Value + "&imageid=" + file_id;
            HttpContext.Response.Redirect(re_url);
        }

        /// <summary>
        /// 通过Id获取资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<FileStream> Get(string id)
        {
            // if (!resourceServer.VerifyResourceId(id))
            //     throw new ResultException(404, "找不到资源");
            // string res_info = resourceServer.DecryptResourceId(id);
            TResource res_info = resourceServer.Retrieve(new TResource()
            {
                ResourceId = id
            });
            if (res_info == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            Response.ContentType = res_info.ContentType;
            return resourceServer.RetrieveResourceFile(id);
        }
    }
}