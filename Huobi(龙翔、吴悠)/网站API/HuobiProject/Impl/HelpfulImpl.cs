using System;
using System.Linq;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;

namespace HuobiProject.Impl
{
    public class HelpfulImpl : IHelpful
    {
        private HuobiContext m_db;
        private IHelpful m_helpful;

        public HelpfulImpl(HuobiContext db)
        {
            this.m_db = db;
            this.m_helpful = this;
        }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<THelpful>.Create(THelpful t)
        {
            Random rand = new Random((int)Tools.Ticks());
            string id = Tools.Ticks() + "" + rand.Next(1000, 10000);

            t.HelpfulId = id;
            t.CreateTime = DateTime.Now;
            m_db.THelpful.Add(t);
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
        bool ICURD<THelpful>.Update(THelpful t)
        {
            THelpful hp = m_helpful.Retrieve(t);
            hp.Helpful = t.Helpful;
            hp.Ip = t.Ip;

            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：NULL
        /// </returns>
        THelpful ICURD<THelpful>.Retrieve(THelpful t)
        {
            return m_db.THelpful.Find(t.HelpfulId);
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：F
        /// </returns>
        bool ICURD<THelpful>.Delete(THelpful t)
        {
            THelpful hp = m_helpful.Retrieve(t);

            m_db.THelpful.Remove(hp);

            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据文章Id设置文章是否有帮助
        /// </summary>
        /// <param name="helpful">是否有帮助</param>
        void IHelpful.CommitHelpful(THelpful helpful)
        {
            m_helpful.Create(helpful);
        }

        /// <summary>
        /// 有帮助的人数
        /// </summary>
        /// <param name="articleLangId">文章Id</param>
        /// <returns></returns>
        int IHelpful.HelpfulCount(string articleLangId)
        {
            return m_db.THelpful.Where(w => w.ArticleLangId == articleLangId && w.Helpful == 1).Count();
        }

        /// <summary>
        /// 所有人数
        /// </summary>
        /// <param name="articleLangId">文章Id</param>
        /// <returns></returns>
        int IHelpful.HelpfulsCount(string articleLangId)
        {
            return m_db.THelpful.Where(w => w.ArticleLangId == articleLangId).Count();
        }

        /// <summary>
        /// 获取评价信息
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <param name="ip">ip地址</param>
        /// <returns></returns>
        THelpful IHelpful.RetrieveByIp(string articleLangId, string ip)
        {
            return m_db.THelpful.Where(w => w.ArticleLangId == articleLangId && w.Ip == ip).FirstOrDefault();
        }
    }
}