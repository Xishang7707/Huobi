using Microsoft.AspNetCore.Mvc;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;

namespace HuobiProject.Controllers
{
    [Route("api/lang")]
    public class LanguageController : ControllerBase
    {
        private ILang langServer;//语言接口

        public LanguageController(ILang langServer)
        {
            this.langServer = langServer;
        }

        /// <summary>
        /// 获取所有语言信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<Result> Get()
        {
            return new Result(200, "成功", langServer.RetrieveAll());
        }
    }
}