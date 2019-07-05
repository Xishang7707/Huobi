using System.Collections.Generic;
using HuobiProject.Models;

namespace HuobiProject.Interface
{
    /// <summary>
    /// 工单接口
    /// </summary>
    public interface ITicket : ICURD<TTicket>
    {
        /// <summary>
        /// 提交新工单
        /// </summary>
        /// <param name="t">工单信息</param>
        /// <param name="tc">内容信息</param>
        void Publish(TTicket t, TTicketChat tc);

        /// <summary>
        /// 为工单添加回复
        /// </summary>
        /// <param name="t">工单信息</param>
        /// <param name="tc">回复信息</param>
        void AddReply(TTicket t, TTicketChat tc);

        /// <summary>
        /// 根据状态id获取所工单回复记录
        /// </summary>
        /// <param name="ticketId">工单Id</param>
        /// <param name="statusId">状态Id</param>
        /// <returns></returns>
        dynamic RetrieveAllByType(string ticketId, int statusId);

        /// <summary>
        /// 根据工单状态获取所有工单
        /// </summary>
        /// <param name="statusId">状态Id</param>
        /// <returns></returns>
        dynamic RetrieveTickets(int statusId);

        /// <summary>
        /// 根据工单状态获取用户所有工单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="statusId">状态Id</param>
        /// <returns></returns>
        dynamic RetrieveTickets(string userId, int statusId);

        /// <summary>
        /// 通过问题id获取问题下详情列表
        /// </summary>
        /// <param name="typeId">类型id</param>
        /// <returns></returns>
        List<TType> RetrieveByQuestionId(int typeId);
    }
}