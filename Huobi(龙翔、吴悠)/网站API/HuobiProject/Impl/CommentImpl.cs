using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HuobiProject.Models;
using HuobiProject.Interface;
using HuobiProject.Utils;

namespace HuobiProject.Impl
{
    /// <summary>
    /// 评论接口实现
    /// </summary>
    public class CommentImpl : IComment
    {
        private HuobiContext m_db;
        private IComment m_comment;
        private ICommentLike m_commentLike;

        public CommentImpl(HuobiContext db, ICommentLike commentLike)
        {
            this.m_db = db;
            this.m_comment = this;
            this.m_commentLike = commentLike;
        }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TComment>.Create(TComment t)
        {
            Random rand = new Random((int)Tools.Ticks());
            string id = Tools.Ticks() + "" + rand.Next(1000, 10000);

            t.CommentId = id;
            t.CreateTime = DateTime.Now;

            m_db.TComment.Add(t);
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
        bool ICURD<TComment>.Update(TComment t)
        {
            TComment comment = m_comment.Retrieve(t);
            comment.Content = t.Content;

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
        TComment ICURD<TComment>.Retrieve(TComment t)
        {
            return m_db.TComment.Find(t.CommentId);
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：F
        /// </returns>
        bool ICURD<TComment>.Delete(TComment t)
        {
            TComment comment = m_comment.Retrieve(t);
            m_db.TComment.Remove(comment);
            return m_db.SaveChanges() > 0;
        }


        /// <summary>
        /// 获取文章所有评论
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <returns></returns>
        dynamic IComment.RetrieveAll(string articleLangId)
        {
            return m_db.TComment.Where(w => w.ArticleLangId == articleLangId)
                        .Include(i => i.User)
                        .Select(s => new
                        {
                            s.CommentId,
                            s.ArticleLangId,
                            s.Content,
                            s.CreateTime,
                            User = new
                            {
                                s.User.UserId,
                                s.User.UserName,
                                s.User.Photo
                            },
                            CommentLike = new
                            {
                                like = m_db.TCommentLike.Where(w => w.CommentId == s.CommentId && w.Like == 1).Count(),
                                dislike = m_db.TCommentLike.Where(w => w.CommentId == s.CommentId && w.Like == 0).Count()
                            }
                        })
                        .OrderByDescending(o => o.CreateTime);
        }

        /// <summary>
        /// 获取文章评论数量
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <returns></returns>
        int IComment.CommentCount(string articleLangId)
        {
            return m_db.TComment.Where(w => w.ArticleLangId == articleLangId).Count();
        }
    }
}