using System;
using System.Linq;
using HuobiProject.Models;
using HuobiProject.Interface;
using HuobiProject.Utils;

namespace HuobiProject.Impl
{
    /// <summary>
    /// 关注接口实现
    /// </summary>
    public class AttentionImpl : IAttention
    {
        private HuobiContext m_db;
        private IAttention m_iAttention;

        public AttentionImpl(HuobiContext db)
        {
            this.m_db = db;
            this.m_iAttention = this;
        }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TAttention>.Create(TAttention t)
        {
            Random rand = new Random((int)Tools.Ticks());
            string id = Tools.Ticks() + "" + rand.Next(1000, 10000);

            t.AttentionId = id;
            t.CreateTime = DateTime.Now;
            m_db.TAttention.Add(t);
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
        bool ICURD<TAttention>.Update(TAttention t) => throw new NotImplementedException();

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：NULL
        /// </returns>
        TAttention ICURD<TAttention>.Retrieve(TAttention t)
        {
            return m_db.TAttention.Find(t.AttentionId);
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：F
        /// </returns>
        bool ICURD<TAttention>.Delete(TAttention t)
        {
            TAttention tat = m_iAttention.Retrieve(t);
            m_db.TAttention.Remove(tat);
            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 获取用户关注的类型信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="typeId">文章子类型Id</param>
        /// <returns></returns>
        TAttention IAttention.RetriecveAttention(string userId, int typeId)
        {
            return m_db.TAttention.Where(w => w.UserId == userId && w.TypeId == typeId).FirstOrDefault();
        }
    }
}