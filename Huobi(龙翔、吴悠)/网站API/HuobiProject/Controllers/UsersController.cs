using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HuobiProject.Models;
using Newtonsoft.Json.Linq;
using HuobiProject.Utils;
using Microsoft.Extensions.Configuration;
using HuobiProject.Interface;
using Microsoft.AspNetCore.Http;

namespace HuobiProject.Controllers
{
    /// <summary>
    /// 用户操作API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUser userServer;
        private readonly IConfiguration m_config;//配置文件

        public UsersController(IConfiguration config, IUser userServer)
        {
            this.m_config = config;
            this.userServer = userServer;
        }

        /// <summary>
        /// 普通用户注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public ActionResult<Result> Register([FromBody] TUser user)
        {
            user.Password = "123456";
            userServer.VerifyRegister(user);

            userServer.Register(user);
            return new Result(200, "成功");
        }

        /// <summary>
        /// 管理员注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("register-super")]
        public ActionResult<Result> RegisterSuper([FromBody] TUser user)
        {
            user.Password = "123456";
            userServer.VerifyRegister(user);

            userServer.RegisterSuper(user);
            return new Result(200, "成功");
        }
        /// <summary>
        /// 普通用户登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public ActionResult<Result> Login([FromBody] TUser user)
        {
            return new Result(200, "成功", userServer.Login(user));
        }

        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("login-super")]
        public ActionResult<Result> LoginSuper([FromBody] TUser user)
        {
            return new Result(200, "成功", userServer.LoginSuper(user));
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("info")]
        public ActionResult<Result> RetrieveUserInfo()
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            var info = new
            {
                user.UserId,
                user.UserName,
                user.Account,
                user.Photo,
                user.CreateTime
            };
            return new Result(200, "成功", info);
        }
    }
}