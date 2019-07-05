using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;

namespace HuobiProject.Controllers
{
    /// <summary>
    /// 关注API接口
    /// </summary>
    [Route("api/attention")]
    public class AttentionController : ControllerBase
    {
        private IUser userServer;
        private IType typeServer;
        private IAttention attentionServer;

        public AttentionController(IUser userServer, IType typeServer, IAttention attentionServer)
        {
            this.userServer = userServer;
            this.typeServer = typeServer;
            this.attentionServer = attentionServer;
        }

        /// <summary>
        /// 关注/取关文章类型板块
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{typeId}")]
        public ActionResult<Result> Attention([FromRoute] int typeId)
        {
            TType type = typeServer.Retrieve(new TType { TypeId = typeId });
            if (typeId < 300000 || typeId >= 400000 || type == null)
                throw new ResultException("类型错误");

            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TAttention at = attentionServer.RetriecveAttention(user.UserId, typeId);

            bool result = true;

            if (at == null)
                attentionServer.Create(new TAttention
                {
                    UserId = user.UserId,
                    TypeId = typeId
                });
            else
            {
                attentionServer.Delete(at);
                result = false;
            }

            return new Result(200, "成功", result);
        }


        /// <summary>
        /// 是否关注指定类型
        /// </summary>
        /// <param name="typeId">子类型Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{typeId}")]
        public ActionResult<Result> IsAttention([FromRoute] int typeId)
        {
            TType type = typeServer.Retrieve(new TType { TypeId = typeId });
            if (typeId < 300000 || typeId >= 400000 || type == null)
                throw new ResultException("类型错误");

            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            return new Result(200, "成功", attentionServer.RetriecveAttention(user.UserId, typeId) != null);
        }
    }
}