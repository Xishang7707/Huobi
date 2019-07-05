using System;
using System.Transactions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using HuobiProject.Models;
using HuobiProject.Utils;
using HuobiProject.Interface;
using System.Linq;
using Newtonsoft.Json.Linq;
using NETCore.Encrypt;

namespace HuobiProject.Impl
{
    public class ArticleImpl : IArticle
    {
        private readonly String RedisDB = "DB:T_ArticleLang";
        private HuobiContext m_db;
        private IArticle m_iArticle;
        private IType m_iType;
        private IUser m_iUser;
        private IComment m_iComment;

        private IRedis m_redis;

        public ArticleImpl(HuobiContext db, IType type, IUser user, IComment comment, IRedis redis)
        {
            this.m_db = db;
            this.m_iArticle = this;
            this.m_iType = type;
            this.m_iUser = user;
            this.m_iComment = comment;
            this.m_redis = redis;
        }

        /// <summary>
        /// 增加数据
        /// 更新Redis数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TArticleLang>.Create(TArticleLang t)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Random rand = new Random((int)Tools.Ticks());
                string id = Tools.Ticks() + "" + rand.Next(1000, 10000);

                t.ArticleLangId = id;
                t.CreateTime = DateTime.Now;
                t.UpdateTime = t.CreateTime;
                m_db.TArticleLang.Add(t);

                m_db.SaveChanges();

                //存Redis
                //m_redis.HashSet(RedisDB, t.ArticleLangId, t);

                scope.Complete();
            }
            return true;
        }

        /// <summary>
        /// 根据ID更新数据
        /// 更新Redis数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TArticleLang>.Update(TArticleLang t)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                TArticleLang lang = m_db.TArticleLang.Find(t.ArticleLangId);

                lang.Title = t.Title;
                lang.Content = t.Content;
                lang.Commentable = t.Commentable;
                lang.Flag = t.Flag;
                lang.UpdateTime = DateTime.Now;
                m_db.SaveChanges();

                //更新redis数据
                //m_redis.HashSet(RedisDB, t.ArticleLangId, t);

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
        TArticleLang ICURD<TArticleLang>.Retrieve(TArticleLang t)
        {
            //if (m_redis.HashExists(RedisDB, t.ArticleLangId))
            //    return m_redis.HashGet<TArticleLang>(RedisDB, t.ArticleLangId);

            TArticleLang tart = m_db.TArticleLang
                        .Include(i => i.User)
                        .Include(i => i.Type)
                        .Where(L => L.ArticleLangId == t.ArticleLangId).FirstOrDefault();

            //m_redis.HashSet(RedisDB, tart.ArticleLangId, tart);

            return tart;
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：NULL
        /// </returns>
        bool ICURD<TArticleLang>.Delete(TArticleLang t)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                TArticleLang lang = m_db.TArticleLang.Find(t.ArticleLangId);
                m_db.TArticleLang.Remove(lang);
                m_db.SaveChanges();

                //m_redis.HashDelete(RedisDB, lang.ArticleLangId);

                scope.Complete();
            }
            return true;
        }


        /// <summary>
        /// 创建文章主类型
        /// </summary>
        /// <returns>主类型Id</returns>
        string IArticle.CreateArticle()
        {
            Random rand = new Random((int)Tools.Ticks());
            string id = Tools.Ticks() + "" + rand.Next(1000, 10000);
            m_db.TArticle.Add(new TArticle()
            {
                ArticleId = id,
                CreateTime = DateTime.Now
            });

            m_db.SaveChanges();
            return id;
        }

        /// <summary>
        /// 验证发布的文章
        /// </summary>
        /// <param name="article">文章信息</param>
        /// <returns></returns>
        bool IArticle.VerifyPublish(TArticleLang article)
        {
            if (article.Title == null || article.Title.Length == 0)
                throw new ResultException("文章标题不能为空");
            if (article.Content == null || article.Content.Length == 0)
                throw new ResultException("文章内容不能为空");

            return true;
        }

        /// <summary>
        /// 发布新文章
        /// </summary>
        /// <param name="article">文章信息</param>
        /// <param name="user">用户信息</param>
        void IArticle.Publish(TArticleLang article, TUser user)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                string id = m_iArticle.CreateArticle();

                article.ArticleId = id;
                article.UserId = user.UserId;

                m_iArticle.Create(article);

                scope.Complete();
            }
        }

        /// <summary>
        /// 为已有文章添加新语言
        /// </summary>
        /// <param name="article">文章信息</param>
        /// <param name="user">用户信息</param>
        void IArticle.PublishByLang(TArticleLang article, TUser user)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                article.UserId = user.UserId;

                m_iArticle.Create(article);

                scope.Complete();
            }
        }

        /// <summary>
        /// 根据Id开启文章评论
        /// </summary>
        /// <param name="article">开启评论的文章</param>
        void IArticle.OpenComment(TArticleLang article)
        {
            article.Commentable = 1;

            m_iArticle.Update(article);
        }

        /// <summary>
        /// 根据Id关闭文章评论
        /// </summary>
        /// <param name="article">关闭评论的文章</param>
        void IArticle.CloseComment(TArticleLang article)
        {
            article.Commentable = 0;

            m_iArticle.Update(article);
        }

        /// <summary>
        /// 根据文章主Id获取对应语言的文章
        /// </summary>
        /// <param name="articleId">文章主Id</param>
        /// <param name="lang">语言信息</param>
        /// <returns></returns>
        dynamic IArticle.RetrieveByLang(string articleId, int langId)
        {
            //TArticleLang article_info = m_iArticle.Retrieve(article);
            IQueryable<TType> type_list = m_iType.RetrieveArticleTypesByLangId(langId);
            List<TArticleLang> article_list = m_iArticle.RetrieveArticlesByArticleId(articleId);

            return type_list.SelectMany(sm => article_list.Where(k => sm.TypeId == k.TypeId)
                                                        .Select(s => new
                                                        {
                                                            s.ArticleLangId,
                                                            s.Title,
                                                            s.Content,
                                                            s.Commentable,
                                                            s.Flag,
                                                            s.UpdateTime,
                                                            s.CreateTime,
                                                            Type = new
                                                            {
                                                                s.Type.TypeId,
                                                                s.Type.Name,
                                                                s.Type.Lang,
                                                                s.Type.Parent
                                                            },
                                                            s.ArticleId,
                                                            User = new
                                                            {
                                                                UserId = s.UserId,
                                                                UserName = s.User.UserName,
                                                                Photo = s.User.Photo
                                                            }
                                                        })).FirstOrDefault();

            //return type_list.Select(s => article_list.Select(k => k).Where(k => s.TypeId == k.TypeId)).ToList();
        }

        /// <summary>
        /// 根据关键字查询文章
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="langId">语言id</param>
        /// <returns></returns>
        dynamic IArticle.SearchByKeyword(string keyword, int langId)
        {
            string key_id_md5 = EncryptProvider.Md5((keyword + langId), MD5Length.L16);
            //检查Redis是否有记录
            if (m_redis.HashExists("SearchByKeyword", key_id_md5))
                return m_redis.HashGet<dynamic>("SearchByKeyword", key_id_md5);
            /*
                1.将相关数据查询
                2.匹配内容
                3.匹配度排序
             */
            IQueryable<TType> types = m_iType.RetrieveArticleTypesByLangId(langId);
            string partten = @"(?<=[？?！!：:。.\n]*)[^？?！!：:。.\n]*" + keyword + "[^？?！!：:。.\n]*.(?<=[？?！!：:。.\n]*)";
            //string partten = @"(?<=[。|？|！|：|.|?|!|:])*" + keyword + ".*?(?<=[。|？|！|：|.|?|!|:])";

            dynamic key_search = types.SelectMany(sm => m_db.TArticleLang
            .Include(i => i.User)
            .Include(i => i.Type)
            .Include(i => i.THelpful)
            .Where(
                    L =>
                    sm.TypeId == L.TypeId && (
                         L.Title.Contains(keyword) ||
                         L.Content.Contains(keyword)
                    )
                )
            .Select(s => new
            {
                s.ArticleLangId,
                s.Title,
                s.Commentable,
                s.Flag,
                s.ArticleId,
                s.UpdateTime,
                s.CreateTime,
                Type = new
                {
                    s.Type.TypeId,
                    s.Type.Name,
                    s.Type.Description,
                    s.Type.Parent
                },
                User = new
                {
                    s.User.UserId,
                    s.User.UserName,
                    s.User.Photo
                },
                Content = Regex.Match(s.Content, partten).Value,
                CommentCount = m_db.TComment.Where(w => w.ArticleLangId == s.ArticleLangId).Count(),
                Helpful = s.THelpful.Where(w => w.ArticleLangId == s.ArticleLangId && w.Helpful == 1).Count(),

            }))
            .OrderByDescending(ob => Regex.Matches(ob.Title, keyword).Count + Regex.Matches(ob.Content, keyword).Count)
            .ToList();

            // Regex.Matches(s.Content, keyword)
            //                                     .OrderByDescending(ob => Regex.Matches(ob.Value, keyword).Count)
            //                                     .FirstOrDefault().Value

            //记录搜索结果到Redis
            m_redis.HashSet("SearchByKeyword", key_id_md5, key_search, TimeSpan.FromMinutes(30));

            return JArray.FromObject(key_search);
        }

        /// <summary>
        /// 查询子类型所有文章.
        /// </summary>
        /// <param name="typeId">子类型Id</param>
        /// <returns></returns>
        List<TArticleLang> IArticle.QueryArticles(int typeId)
        {
            //if (m_redis.HashExists("ArticlesByTypeId", typeId))
            //    return m_redis.HashGet<List<TArticleLang>>("ArticlesByTypeId", typeId);

            List<TArticleLang> iq_tal = m_db.TArticleLang
                        .Include(i => i.User)
                        .Where(w => w.TypeId == typeId)
                        .OrderByDescending(o => o.Flag)
                        .OrderByDescending(o => o.CreateTime)
                        .ToList();
            m_redis.HashSet("ArticlesByTypeId", typeId, iq_tal, TimeSpan.FromMinutes(30));

            return iq_tal;
        }

        /// <summary>
        /// 根据文章主Id获取其所有语言文章
        /// </summary>
        /// <param name="articleId">文章主Id</param>
        /// <returns></returns>
        List<TArticleLang> IArticle.RetrieveArticlesByArticleId(string articleId)
        {
            //if (m_redis.HashExists("ArticleLangs", articleId))
            //    return m_redis.HashGet<List<TArticleLang>>("ArticleLangs", articleId);

            List<TArticleLang> iq_tal = m_db.TArticleLang.Include(i => i.User)
                                    .Where(w => w.ArticleId == articleId)
                                    .ToList();

            //m_redis.HashSet("ArticleLangs", articleId, iq_tal, TimeSpan.FromMinutes(30));
            return iq_tal;
        }

        /// <summary>
        /// 获取主分类下所有子类型和指定条数的文章
        /// </summary>
        /// <param name="langId">语言Id</param>
        /// <param name="mainArticleTypeId">文章主类型Id</param>
        /// <param name="count">数据量</param>
        /// <returns></returns>
        dynamic IArticle.CategoriesArticles(int langId, int mainArticleTypeId, int count)
        {
            var mainType = m_iType.Retrieve(new TType { TypeId = mainArticleTypeId });
            var articleTypes = m_db.TType //子类型信息
                .Where(w => w.TypeId > 300000 && w.TypeId < 400000 && w.ParentId == mainArticleTypeId)
                .Select(s => new
                {
                    s.TypeId,
                    s.Name
                }).ToList();

            JArray articleTypes_arr = new JArray();

            foreach (var item in articleTypes)
            {
                JObject json = JObject.FromObject(item);
                var articles = m_db.TArticleLang
                    .Where(w => w.TypeId == item.TypeId)
                    .Select(s => new
                    {
                        s.ArticleLangId,
                        s.Title,
                        s.Commentable,
                        s.Flag,
                        s.ArticleId,
                        s.UserId,
                        s.UpdateTime,
                        s.CreateTime
                    });

                json.Add("articles", JObject.FromObject(Pagination<dynamic>.Paging(articles, 1, 6)));

                articleTypes_arr.Add(json);
            }

            return new
            {
                mainType,//MainType 主类型信息
                articleTypes = articleTypes_arr
            };
        }

        /// <summary>
        /// 获取分类下所有文章
        /// </summary>
        /// <param name="typeId">类型Id</param>
        /// <returns>IQuaryable<T></returns>
        dynamic IArticle.RetrieveArticlesByTypeId(int typeId)
        {
            var list = m_db.TArticleLang
            .Where(w => w.TypeId > 300000 && w.TypeId < 400000 && w.TypeId == typeId)
            .Select(s => new
            {
                s.ArticleLangId,
                s.Title,
                s.Commentable,
                s.Flag,
                s.ArticleId,
                s.UserId,
                s.UpdateTime,
                s.CreateTime,
                Type = new TType
                {
                    TypeId = s.Type.TypeId,
                    Name = s.Type.Name,
                    Description = s.Type.Description,
                    Parent = s.Type.Parent
                }
            })
            .OrderByDescending(o => o.Flag)
            .OrderByDescending(o => o.CreateTime);

            return list;
        }

        /// <summary>
        /// 最近的活动
        /// 
        /// 被标记为重要的
        /// 按发布时间降序
        /// </summary>
        /// <param name="langId">语言Id</param>
        /// <returns>列表</returns>
        dynamic IArticle.RecentActivity(int langId)
        {
            List<TType> type_list = m_iType.RetrieveArticleTypesByLangId(langId).ToList();

            var data_list = type_list.SelectMany(t =>
                               m_iArticle.QueryArticles(t.TypeId)
                               .Where(w => w.Flag == 1)
                               .Select(sw => new
                               {
                                   sw.ArticleLangId,
                                   sw.Title,
                                   sw.Commentable,
                                   sw.Flag,
                                   sw.ArticleId,
                                   sw.UpdateTime,
                                   sw.CreateTime,
                                   User = new
                                   {
                                       UserId = sw.User.UserId,
                                       UserName = sw.User.UserName,
                                       Photo = sw.User.Photo
                                   },
                                   sw.Type,
                                   Comment = new
                                   {
                                       Count = m_db.TComment.Where(w => w.ArticleLangId == sw.ArticleLangId).Count(),
                                       LasterTime = m_db.TComment.Where(w => w.ArticleLangId == sw.ArticleLangId).LastOrDefault()?.CreateTime
                                   }
                               }))
                           .OrderByDescending(o => o.CreateTime)
                           .ToList();
            return data_list;
        }

        /// <summary>
        /// 推荐文章
        /// </summary>
        /// <param name="langId">语言id</param>
        /// <returns></returns>
        dynamic IArticle.Recommended(int langId)
        {
            return m_iType.RetrieveArticleTypesByLangId(langId)
                   .SelectMany(sm => m_db.TArticleLang
                   .Include(i => i.User)
                   .Where(w => w.Flag == 1 && w.TypeId == sm.TypeId))
                   .Select(s => new
                   {
                       s.ArticleLangId,
                       s.Title,
                       s.Commentable,
                       s.Flag,
                       s.ArticleId,
                       s.UpdateTime,
                       s.CreateTime,
                       User = new
                       {
                           s.User.UserId,
                           s.User.UserName,
                           s.User.Photo
                       },
                       s.Type
                   })
                   .OrderByDescending(obd => obd.CreateTime);
        }

        /// <summary>
        /// 获取文章主信息
        /// </summary>
        /// <param name="article">文章</param>
        /// <returns></returns>
        TArticle IArticle.RetrieveMainArticle(TArticle article)
        {
            return m_db.TArticle.Where(w => w.ArticleId == article.ArticleId).FirstOrDefault();
        }

        /// <summary>
        /// 删除主文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        bool IArticle.DeleteMainArticle(TArticle article)
        {
            TArticle art = m_iArticle.RetrieveMainArticle(article);
            m_db.TArticle.Remove(art);
            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 相关文章--随机
        /// </summary>
        /// <param name="articleLangId">当前文章id</param>
        /// <param name="count">数据量</param>
        /// <returns></returns>
        dynamic IArticle.RelatedArticles(string articleLangId, int count)
        {
            TArticleLang t = m_iArticle.Retrieve(new TArticleLang { ArticleLangId = articleLangId });

            var data_list = m_db.TArticleLang
                .Where(w => w.ArticleLangId != articleLangId && w.Type.LangId == t.Type.LangId)
                .OrderBy(o => articleLangId)
                .Take(count)
                .Select(s => new
                {
                    s.Title,
                    s.Type,
                    s.ArticleId,
                    s.ArticleLangId
                }).ToList();
            return data_list;
        }

        /// <summary>
        /// 根据语言id取所有文章信息
        /// </summary>
        /// <param name="langId">语言id</param>
        /// <returns></returns>
        dynamic IArticle.RetrieveArticlesByLang(int langId)
        {
            List<TType> types = m_iType.RetrieveArticleTypesByLangId(langId).ToList();

            var article_list = types
                .SelectMany(sm => m_db.TArticleLang.Include(i => i.User).Where(w => w.TypeId == sm.TypeId))
                .Select(s => new
                {
                    s.ArticleId,
                    s.ArticleLangId,
                    s.Title,
                    s.Type,
                    User = new
                    {
                        s.User.UserId,
                        s.User.UserName,
                        s.User.Photo
                    }
                })
                .ToList();



            return article_list;
        }
    }
}