using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;
using System.Transactions;
using Newtonsoft.Json.Linq;

namespace HuobiProject.Impl
{
    public class TicketImpl : ITicket
    {
        private readonly String RedisDB = "DB:T_Ticket";
        private HuobiContext m_db;
        private ITicket m_iTicket;
        private IRedis m_redis;
        private IUser m_user;

        public TicketImpl(HuobiContext db, IRedis redis, IUser user)
        {
            this.m_db = db;
            this.m_iTicket = this;
            this.m_redis = redis;
            this.m_user = user;
        }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TTicket>.Create(TTicket t)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Random rand = new Random((int)Tools.Ticks());
                string id = Tools.Ticks() + "" + rand.Next(1000, 10000);

                t.TicketId = id;
                t.StatusId = 700001;

                t.CreateTime = DateTime.Now;
                t.ActiveTime = t.CreateTime;
                m_db.TTicket.Add(t);
                m_db.SaveChanges();

                m_redis.HashSet(RedisDB, t.TicketId, t);

                scope.Complete();
            }
            return true;
        }

        /// <summary>
        /// 根据ID更新数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TTicket>.Update(TTicket t)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                TTicket tkt = m_iTicket.Retrieve(t);
                tkt.StatusId = t.StatusId;
                tkt.ActiveTime = DateTime.Now;

                m_db.SaveChanges();

                //更新Redis
                m_redis.HashSet(RedisDB, tkt.TicketId, tkt);

                scope.Complete();
            }
            return true;
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：NULL
        /// </returns>
        TTicket ICURD<TTicket>.Retrieve(TTicket t)
        {
            //if (m_redis.HashExists(RedisDB, t.TicketId))
            //    return m_redis.HashGet<TTicket>(RedisDB, t.TicketId);

            return m_db.TTicket.Find(t.TicketId);
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：F
        /// </returns>
        bool ICURD<TTicket>.Delete(TTicket t)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                TTicket tk = m_iTicket.Retrieve(t);
                m_db.TTicket.Remove(tk);
                m_db.SaveChanges();

                m_redis.HashDelete(RedisDB, t.TicketId);

                scope.Complete();
            }
            return true;
        }

        /// <summary>
        /// 提交新工单
        /// </summary>
        /// <param name="t">工单信息</param>
        /// <param name="tc">内容信息</param>
        void ITicket.Publish(TTicket t, TTicketChat tc)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                m_iTicket.Create(t);

                m_iTicket.AddReply(t, new TTicketChat
                {
                    TicketId = t.TicketId,
                    Content = tc.Content,
                    UserId = tc.UserId,
                    Annex = tc.Annex
                });

                scope.Complete();
            }
        }

        /// <summary>
        /// 为工单添加回复
        /// </summary>
        /// <param name="t">工单信息</param>
        /// <param name="tc">回复信息</param>
        void ITicket.AddReply(TTicket t, TTicketChat tc)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Random rand = new Random((int)Tools.Ticks());
                string id = Tools.Ticks() + "" + rand.Next(1000, 10000);
                tc.TicketChatId = id;

                tc.CreateTime = DateTime.Now;
                m_db.TTicketChat.Add(tc);
                m_db.SaveChanges();

                m_redis.HashSet("DB:T_TicketChat", tc.TicketChatId, t);

                m_iTicket.Update(new TTicket
                {
                    TicketId = t.TicketId,
                    ActiveTime = t.CreateTime,
                    StatusId = t.StatusId
                });

                scope.Complete();
            }
        }

        /// <summary>
        /// 根据状态id获取所工单回复记录
        /// </summary>
        /// <param name="ticketId">工单Id</param>
        /// <param name="statusId">状态Id</param>
        /// <returns></returns>
        dynamic ITicket.RetrieveAllByType(string ticketId, int statusId)
        {
            TTicket t = m_iTicket.Retrieve(new TTicket { TicketId = ticketId });

            dynamic tc_all = m_db.TTicketChat
                .Include(i => i.User)
                .Where(w => w.TicketId == ticketId && (statusId == 700000 ? 1 == 1 : t.StatusId == statusId))
                .Select(s => new
                {
                    s.TicketChatId,
                    s.Content,
                    s.Annex,
                    s.CreateTime,
                    s.UserId,
                    //User = (s.User!=null?1:2),
                    // User = (s.User != null ? new
                    // {
                    //     s.User.UserId,
                    //     s.User.UserName,
                    //     s.User.Photo,
                    //     s.User.CreateTime
                    // } : null)
                })
                .OrderByDescending(o => o.CreateTime)
                .ToList();

            JArray list_arr = JArray.FromObject(tc_all);

            foreach (JObject item in list_arr)
            {
                string userId = item["UserId"].ToString();

                item.Remove("UserId");
                if (userId == null)
                    continue;

                TUser user = m_user.Retrieve(new TUser { UserId = userId });
                if (user == null)
                    continue;

                var user_fil = new
                {
                    user.UserId,
                    user.UserName,
                    user.Photo,
                    user.CreateTime
                };
                item.Add("user", JObject.FromObject(user_fil));
            }

            return list_arr;
        }

        /// <summary>
        /// 根据工单状态获取所有工单
        /// </summary>
        /// <param name="statusId">状态id</param>
        /// <returns></returns>
        dynamic ITicket.RetrieveTickets(int statusId)
        {
            dynamic t_all = m_db.TTicket
                .Where(w => (statusId == 700000 ? true : w.StatusId == statusId))
                .Select(s => new
                {
                    s.TicketId,
                    s.Email,
                    s.ActiveTime,
                    s.CreateTime,
                    Type = new
                    {
                        s.Type.TypeId,
                        s.Type.Name,
                        s.Type.Parent
                    },
                    Status = new
                    {
                        s.Status.TypeId,
                        s.Status.Name
                    },
                    s.UserId,
                    Content = s.TTicketChat.Where(w => s.TicketId == w.TicketId).LastOrDefault()
                })
                .OrderBy(o => o.Status.TypeId)
                .OrderByDescending(o => o.CreateTime)
                .ToList();

            JArray t_arr = JArray.FromObject(t_all);
            //获取用户信息
            foreach (JObject item in t_arr)
            {
                string user_id = item["UserId"].ToString();
                TUser user = m_user.Retrieve(new TUser { UserId = user_id });
                item.Remove("UserId");
                if (user == null)
                    continue;

                var t_user = new
                {
                    user.UserId,
                    user.UserName,
                    user.Photo,
                    user.CreateTime
                };

                item.Add("User", JObject.FromObject(t_user));
            }

            return t_arr;
        }

        /// <summary>
        /// 根据工单状态获取用户所有工单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="statusId">状态Id</param>
        /// <returns></returns>
        dynamic ITicket.RetrieveTickets(string userId, int statusId)
        {
            dynamic t_all = m_db.TTicket
                .Where(w => w.UserId == userId && (statusId == 700000 ? true : w.StatusId == statusId))
                .Select(s => new
                {
                    s.TicketId,
                    s.Email,
                    s.ActiveTime,
                    s.CreateTime,
                    Type = new
                    {
                        s.Type.TypeId,
                        s.Type.Name,
                        s.Type.Parent
                    },
                    Status = new
                    {
                        s.Status.TypeId,
                        s.Status.Name
                    },
                    s.UserId,
                    Content = s.TTicketChat.Where(w => s.TicketId == w.TicketId).LastOrDefault()
                })
            .OrderBy(o => o.Status.TypeId)
            .OrderByDescending(o => o.CreateTime)
            .ToList();

            JArray t_arr = JArray.FromObject(t_all);
            //获取用户信息
            foreach (JObject item in t_arr)
            {
                string user_id = item["UserId"].ToString();
                TUser user = m_user.Retrieve(new TUser { UserId = user_id });
                item.Remove("UserId");
                if (user == null)
                    continue;

                var t_user = new
                {
                    user.UserId,
                    user.UserName,
                    user.Photo,
                    user.CreateTime
                };

                item.Add("User", JObject.FromObject(t_user));
            }

            return t_all;
        }

        /// <summary>
        /// 通过问题id获取问题下详情列表
        /// </summary>
        /// <param name="typeId">类型id</param>
        /// <returns></returns>
        List<TType> ITicket.RetrieveByQuestionId(int typeId)
        {
            if (m_redis.HashExists("TicketQuestion", typeId))
                return m_redis.HashGet<List<TType>>("TicketQuestion", typeId);

            List<TType> list = m_db.TType
                .Where(w => w.ParentId == typeId)
                .ToList();

            m_redis.HashSet("TicketQuestion", typeId, list, TimeSpan.FromMinutes(30));

            return list;
        }
    }
}