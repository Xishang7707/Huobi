using System;
using System.Linq;
using HuobiProject.Models;
using HuobiProject.Interface;
using HuobiProject.Utils;

namespace HuobiProject.Impl
{
    /// <summary>
    /// 点赞实现
    /// </summary>
    public class CommentLikeImpl : ICommentLike
    {
        private HuobiContext m_db;
        private ICommentLike m_iCommentLike;

        public CommentLikeImpl(HuobiContext db)
        {
            this.m_db = db;
            this.m_iCommentLike = this;
        }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TCommentLike>.Create(TCommentLike t)
        {
            Random rand = new Random((int)Tools.Ticks());
            string id = Tools.Ticks() + "" + rand.Next(1000, 10000);
            t.CommentLikeId = id;
            t.CreateTime = DateTime.Now;

            m_db.TCommentLike.Add(t);
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
        bool ICURD<TCommentLike>.Update(TCommentLike t)
        {
            TCommentLike tml = m_iCommentLike.Retrieve(t);
            tml.Like = t.Like;
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
        TCommentLike ICURD<TCommentLike>.Retrieve(TCommentLike t)
        {
            return m_db.TCommentLike.Find(t.CommentLikeId);
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：F
        /// </returns>
        bool ICURD<TCommentLike>.Delete(TCommentLike t)
        {
            TCommentLike tml = m_iCommentLike.Retrieve(t);
            m_db.TCommentLike.Remove(tml);
            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 通过用户id和评论id获取点赞信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="commentId">评论Id</param>
        /// <returns></returns>
        TCommentLike ICommentLike.RetrieveByUserAndComment(string userId, string commentId)
        {
            return m_db.TCommentLike.Where(w => w.UserId == userId && w.CommentId == commentId).SingleOrDefault();
        }
    }
}