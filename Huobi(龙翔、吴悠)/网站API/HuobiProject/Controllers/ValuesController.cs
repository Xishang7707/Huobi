using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using HuobiProject.Models;
using Microsoft.Extensions.Configuration;
using HuobiProject.Utils;
using Microsoft.AspNetCore.Authorization;
using HuobiProject.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using NETCore.Encrypt;

namespace HuobiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IDistributedCache m_cache;
        private Token m_token;
        private IConfiguration m_config;
        private IRedis m_redis;

        private IArticle articleServer;
        public ValuesController(IDistributedCache cache, Token token, IConfiguration config, IRedis redis, IArticle articleServer)
        {
            this.m_cache = cache;
            this.m_token = token;
            this.m_config = config;
            m_redis = redis;
            this.articleServer = articleServer;

        }
        [HttpGet]
        public ActionResult<Result> Get()
        {
            Test();
            //articleServer.Retrieve(new TArticleLang { ArticleLangId = "15620722664684384" })
            return new Result(200, "OK", articleServer.Retrieve(new TArticleLang { ArticleLangId = "15620722664684384" }));
        }

        [Authorize]
        private void Test()
        {
            System.Console.WriteLine("1231231");
        }

        [HttpGet("{id}")]
        public ActionResult<Result> Get2([FromRoute] string id)
        {

            return new Result(200, "OK");
        }
        // private readonly IConfiguration m_config;//配置文件

        // public ValuesController(IConfiguration config, IUser u)
        // {
        //     this.m_config = config;
        // }

        // [HttpGet("{lang}")]
        // public ActionResult<TLang> Sel(string lang = "zh-CN")
        // {
        //     var data = m_dbContext.TLang.Where(l => l.Value == lang).FirstOrDefault();
        //     return data;
        // }

        // [HttpGet("{lang}/{id}/{name}")]
        // public ActionResult<IEnumerable<TType>> Add(int id, string name, string lang = "zh-CN")
        // {
        //     TType t = new TType();
        //     t.TypeId = id;
        //     t.Name = name;
        //     m_dbContext.TType.Add(t);
        //     m_dbContext.SaveChanges();
        //     System.Console.WriteLine(name);
        //     return m_dbContext.TType.ToArray();
        // }
        // [HttpGet("token")]
        // public ActionResult<string> token()
        // {
        //     TUser user = new TUser();
        //     user.UserId = 100;
        //     return "";//new Token(Configuration).CreateToken(user);
        // }
        // [Authorize]
        // [HttpGet("token2")]
        // public ActionResult<string> token2()
        // {

        //     return "";
        // }
        // // GET api/values
        // [HttpGet]
        // public ActionResult<IEnumerable<TType>> Get()
        // {
        //     return m_dbContext.TType.ToArray();
        // }

        // // // GET api/values/5
        // // [HttpGet("{id}")]
        // // public ActionResult<string> Get(int id)
        // // {
        // //     return "value";
        // // }

        // // POST api/values
        // [HttpPost]
        // public void Post([FromBody] string value)
        // {
        // }

        // // PUT api/values/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }

        // // DELETE api/values/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
