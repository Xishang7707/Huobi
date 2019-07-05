using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HuobiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private ITicket ticketServer;
        private IType typeServer;
        private IUser userServer;

        public TicketsController(ITicket ticketServer, IType typeServer, IUser userServer)
        {
            this.ticketServer = ticketServer;
            this.typeServer = typeServer;
            this.userServer = userServer;
        }

        /// <summary>
        /// 工单提交-未登录
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [HttpPost("publish")]
        public ActionResult<Result> Publish([FromBody] dynamic ticket)
        {
            JObject data = JObject.Parse(ticket.ToString());

            //解析数据
            TTicket tkt = new TTicket
            {
                Email = data["Email"].ToString(),
                TypeId = int.Parse(data["TypeId"].ToString())
            };

            TTicketChat tktc = new TTicketChat
            {
                Content = data["Content"].ToString(),
                Annex = data["Annex"]?.ToString()
            };

            string reg_email = @"^\w+(?<=[^ ])@\w+\.\w+$";
            if (!Regex.IsMatch(tkt.Email, reg_email))
                throw new ResultException("邮箱格式错误");

            if (tkt.TypeId != 0 && tkt.TypeId != 400002 && (tkt.TypeId < 600000 || tkt.TypeId >= 700000))
                throw new ResultException("工单类型错误");

            //发布
            ticketServer.Publish(tkt, tktc);

            // ticketServer.Create(new TTicket()
            // {
            //     TypeId = ticket.TypeId == 0 ? 400002 : ticket.TypeId,
            //     Email = ticket.Email,
            //     Content = ticket.Content,
            //     Annex = ticket.Annex
            // });
            return new Result(200, "成功");
        }

        /// <summary>
        /// 工单提交-需要授权
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("publish-auth")]
        public ActionResult<Result> PublishAuth([FromBody] dynamic ticket)
        {
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            JObject data = JObject.Parse(ticket.ToString());
            //解析数据
            TTicket tkt = new TTicket
            {
                UserId = user.UserId,
                TypeId = int.Parse(data["TypeId"].ToString())
            };

            TTicketChat tktc = new TTicketChat
            {
                Content = data["Content"].ToString(),
                Annex = data["Annex"]?.ToString(),
                UserId = user.UserId
            };

            if (tkt.TypeId != 0 && tkt.TypeId != 400002 && (tkt.TypeId < 600000 || tkt.TypeId >= 700000))
                throw new ResultException("工单类型错误");

            //发布
            ticketServer.Publish(tkt, tktc);

            return new Result(200, "成功");
        }

        /// <summary>
        /// 回复工单
        /// </summary>
        /// <param name="tc">回复信息</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("reply")]
        public ActionResult<Result> Reply([FromBody] TTicketChat tc)
        {
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TTicket t = ticketServer.Retrieve(new TTicket { TicketId = tc.TicketId });

            if (t.UserId != user.UserId && user.Super == 0)
                throw new ResultException("无权操作");

            if (t == null)
                throw new ResultException("工单不存在");

            if (tc.Content == null || tc.Content == "")
                throw new ResultException("工单内容为空");

            if (t.UserId != user.UserId)
                t.StatusId = 700002;//等待回复

            ticketServer.AddReply(t, new TTicketChat
            {
                TicketId = t.TicketId,
                Content = tc.Content,
                Annex = tc.Annex,
                UserId = user.UserId
            });

            return new Result(200, "成功");
        }

        /// <summary>
        /// 获取工单主类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("types")]
        public ActionResult<Result> Types()
        {
            return new Result(200, "成功", typeServer.RetrieveTicketTypes());
        }

        /// <summary>
        /// 获取主工单下的问题类型
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpGet("types/{typeId}")]
        public ActionResult<Result> Types([FromRoute] int typeId)
        {
            TType tp = typeServer.Retrieve(new TType { TypeId = typeId });

            if (typeId < 400000 || typeId >= 500000 || tp == null)
                throw new ResultException("类型Id错误");

            return new Result(200, "成功", typeServer.RetrieveTicketQuestionCascade(typeId));
        }

        /// <summary>
        /// 根据问题id获取详情问题
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpGet("types/question/{typeId}")]
        public ActionResult<Result> TypesQuestion(int typeId)
        {
            TType tp = typeServer.Retrieve(new TType { TypeId = typeId });

            if (typeId < 400000 || typeId >= 600000 || tp == null)
                throw new ResultException("类型Id错误");

            return new Result(200, "成功", ticketServer.RetrieveByQuestionId(typeId));
        }

        /// <summary>
        /// 获取工单回复记录
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{ticketId}/{statusId}/{page?}/{count?}")]
        public ActionResult<Result> RetrieveChats(string ticketId, int statusId, int page, int count)
        {
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TTicket t = ticketServer.Retrieve(new TTicket { TicketId = ticketId });

            if (t.UserId != user.UserId && user.Super == 0)
                throw new ResultException("无法获取他人工单信息");

            dynamic tc_all = ticketServer.RetrieveAllByType(ticketId, statusId);

            return new Result(200, "成功", Pagination<dynamic>.Paging(tc_all, page, count));
        }

        /// <summary>
        /// 获取所有工单列表信息
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{statusId}/{page?}/{count?}")]
        public ActionResult<Result> RetrieveTickets(int statusId, int page, int count)
        {
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TType type = typeServer.Retrieve(new TType { TypeId = statusId });
            if (statusId < 700000 || statusId > 800000 || statusId != 700000 && type == null)
                throw new ResultException("状态错误");

            dynamic t_list = null;
            if (user.Super == 1)//管理员用户
                t_list = ticketServer.RetrieveTickets(statusId);
            else t_list = ticketServer.RetrieveTickets(user.UserId, statusId);

            return new Result(200, "成功", Pagination<dynamic>.Paging(t_list, page, count));
        }

        /// <summary>
        /// 完成工单
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("complete/{ticketId}")]
        public ActionResult<Result> Complete(string ticketId)
        {
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TTicket ticket = ticketServer.Retrieve(new TTicket { TicketId = ticketId });

            if (ticket == null)
                throw new ResultException("工单不存在");

            ticketServer.Update(new TTicket
            {
                TicketId = ticketId,
                StatusId = 700003
            });

            return new Result(200, "成功");
        }
    }
}