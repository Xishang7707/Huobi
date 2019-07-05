using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;
using HuobiProject.Impl;
using Newtonsoft.Json.Linq;

namespace HuobiProject.Controllers
{
    [Route("api/Articles")]
    public class ArticlesController : ControllerBase
    {
        private IArticle articleServer;//文章接口
        private IUser userServer;//用户接口
        private IType typeServer;//类型接口
        private ILang langServer;//语言接口
        private IRedis redisServer;//Redis接口
        private IHelpful helpfulServer;//是否有帮助

        private IWebSocket socketServer;//WebSocket实例

        public ArticlesController(IArticle articleServer,
                                  IUser userServer,
                                  IType typeServer,
                                  ILang langServer,
                                  IRedis redisServer,
                                  IHelpful helpfulServer)
        {
            this.articleServer = articleServer;
            this.userServer = userServer;
            this.typeServer = typeServer;
            this.langServer = langServer;
            this.redisServer = redisServer;
            this.helpfulServer = helpfulServer;

            socketServer = new WebSocketImpl();
        }

        /// <summary>
        /// 发布文章
        /// --需要授权
        /// --仅管理者使用
        /// </summary>
        /// <param name="article">文章信息</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("Publish")]
        public ActionResult<Result> Publish([FromBody] TArticleLang article)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            if (user.Super != 1)
                throw new ResultException("无权操作");

            if (!typeServer.IsArticleType(article.TypeId))
                throw new ResultException("文章类型错误");
            articleServer.VerifyPublish(article);

            articleServer.Publish(article, user);

            socketServer.SendListenerAsync("Notification",
                new SocketResult("Notification", "on",
                    new Result(200, "通知", "新文章：" + article.Title)
                    )
                );
            return new Result(200, "发布成功");
        }

        /// <summary>
        /// 为已有文章添加其它语言
        /// </summary>
        /// <param name="articleId">文章主Id</param>
        /// <param name="article"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("Publish/{articleId}")]
        public ActionResult<Result> PublishAppend([FromRoute] string articleId, [FromBody] TArticleLang article)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            if (user.Super != 1)
                throw new ResultException("无权操作");

            if (!typeServer.IsArticleType(article.TypeId))
                throw new ResultException("文章类型错误");

            articleServer.VerifyPublish(article);

            TType type = typeServer.Retrieve(new TType() { TypeId = article.TypeId });

            if (articleServer.RetrieveByLang(articleId, type.LangId.Value) != null)
                throw new ResultException("已存在对应语言的文章");

            article.ArticleId = articleId;
            articleServer.PublishByLang(article, user);

            return new Result(200, "发布成功");
        }

        /// <summary>
        /// 根据语言值和文章主id获取文章信息(文章根据语言切换)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{articleId}/{langValue?}")]
        public ActionResult<Result> GetArticle([FromRoute] string articleId, [FromRoute] string langValue)
        {
            TLang lang = langServer.RetrieveByValue(langValue ?? "zh-CN");
            lang = lang ?? langServer.RetrieveByValue("zh-CN");

            dynamic article = articleServer.RetrieveByLang(articleId, lang.LangId);

            return new Result(200, "成功", article);
        }

        /// <summary>
        /// 根据语言类型获取文章主题
        /// </summary>
        /// <param name="langValue">语言值</param>
        /// <returns></returns>
        [HttpGet("subjects/{langValue}")]
        public ActionResult<Result> GetMainTypes([FromRoute] string langValue)
        {
            TLang lang = langServer.RetrieveByValue(langValue);

            if (lang == null)
                throw new ResultException("语言类型错误");

            return new Result(200, "成功", typeServer.RetrieveArticleMainTypes(lang.LangId));
        }

        /// <summary>
        /// 根据主类型Id查询子类型
        /// </summary>
        /// <param name="typeId">主类型Id</param>
        /// <returns></returns>
        [HttpGet("types/{typeId}")]
        public ActionResult<Result> GetArticleTypes([FromRoute] int typeId)
        {
            return new Result(200, "成功", typeServer.RetrieveArticleTypes(typeId));
        }

        /// <summary>
        /// 获取子类型下文章列表
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("list/{typeId}/{page?}/{count?}")]
        public ActionResult<Result> GetArticleList([FromRoute] int typeId, [FromRoute] int page, [FromRoute] int count)
        {
            return new Result(200, "成功", Pagination<dynamic>.Paging(articleServer.RetrieveArticlesByTypeId(typeId), page, count));
        }

        /// <summary>
        /// 分页搜索
        /// 页码或数据量<=0不分页
        /// </summary>
        /// <returns></returns>
        [HttpPost("search")]
        public ActionResult<Result> Search_old([FromBody] dynamic in_data)
        {
            JObject json = JObject.Parse(in_data.ToString());
            TLang lang = langServer.RetrieveByValue(json["langValue"]?.ToString());
            if (lang == null)
                throw new ResultException("语言代号错误");
            string client_id = HttpContext.Request.Headers["client_id"];

            //object cache_data = redisServer.HashGet("Client:" + client_id + ":search", json["keyword"].ToString() + lang.LangId);
            IQueryable<TArticleLang> data_query = articleServer.SearchByKeyword(json["keyword"].ToString(), lang.LangId);
            Pagination<TArticleLang> pagination = Pagination<TArticleLang>.Paging(
                                                                    data_query,
                                                                    int.Parse(json["page"]?.ToString() ?? "0"),
                                                                    int.Parse(json["count"]?.ToString() ?? "0")
                                                                );
            return new Result(200, "成功", pagination);
        }

        /// <summary>
        /// 分页搜索
        /// 页码或数据量<=0不分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("search/{langValue}/{keyword}/{page?}/{count?}")]
        public ActionResult<Result> Search_new([FromRoute] string langValue, [FromRoute] string keyword, [FromRoute] int page, [FromRoute] int count)
        {
            //JObject json = JObject.Parse(in_data.ToString());
            TLang lang = langServer.RetrieveByValue(langValue);
            if (lang == null)
                throw new ResultException("语言代号错误");
            string client_id = HttpContext.Request.Headers["client_id"];

            //object cache_data = redisServer.HashGet("Client:" + client_id + ":search", json["keyword"].ToString() + lang.LangId);
            dynamic data_query = articleServer.SearchByKeyword(keyword, lang.LangId);
            Pagination<dynamic> pagination = Pagination<dynamic>.Paging(
                                                                    data_query,
                                                                    page,
                                                                    count
                                                                );
            return new Result(200, "成功", pagination);
        }

        /// <summary>
        /// 更新文章信息
        /// 需要授权
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("update")]
        public ActionResult<Result> Update([FromBody] TArticleLang article)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            if (user.Super != 1)
                throw new ResultException("无权操作");

            TArticleLang tlang = articleServer.Retrieve(new TArticleLang() { ArticleLangId = article.ArticleLangId });

            if (user.UserId != tlang.UserId)
                throw new ResultException("无法修改他人的文章");

            if (tlang == null)
                throw new ResultException("文章不存在");

            articleServer.VerifyPublish(article);

            articleServer.Update(article);
            return new Result(200, "成功");
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="articleLangId">文章本身Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("delete/{articleLangId}")]
        public ActionResult<Result> Delete([FromRoute] string articleLangId)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            if (user.Super != 1)
                throw new ResultException("无权操作");

            TArticleLang lang = articleServer.Retrieve(new TArticleLang { ArticleLangId = articleLangId });

            if (lang == null)
                throw new ResultException("文章不存在");

            if (lang.UserId != user.UserId)
                throw new ResultException("无法删除他人的文章");

            TArticle p_article = articleServer.RetrieveMainArticle(new TArticle { ArticleId = lang.ArticleId });

            //如果这篇文章下只有一篇语言文章则连父级也删除
            if (articleServer.RetrieveArticlesByArticleId(p_article.ArticleId).Count() == 1)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    articleServer.Delete(lang);
                    articleServer.DeleteMainArticle(p_article);

                    scope.Complete();
                }
            }
            else articleServer.Delete(lang);

            return new Result(200, "成功");
        }

        /// <summary>
        /// 最近活动
        /// </summary>
        /// <param name="langValue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("RecentActivity/{langValue}/{page?}/{count?}")]
        public ActionResult<Result> RecentActivity(string langValue, int page, int count)
        {
            TLang lang = langServer.RetrieveByValue(langValue);
            if (lang == null)
                throw new ResultException("语言代号错误");

            return new Result(200, "成功", Pagination<dynamic>.Paging(articleServer.RecentActivity(lang.LangId), page, count));
        }

        /// <summary>
        /// 开启评论
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("OpenComment/{articleLangId}")]
        public ActionResult<Result> OpenComment([FromRoute] string articleLangId)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TArticleLang tlang = articleServer.Retrieve(new TArticleLang() { ArticleLangId = articleLangId });

            if (tlang == null)
                throw new ResultException("文章不存在");

            if (user.UserId != tlang.UserId)
                throw new ResultException("无法操作他人的文章");

            articleServer.OpenComment(tlang);

            return new Result(200, "成功");
        }

        /// <summary>
        /// 关闭评论
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("CloseComment/{articleLangId}")]
        public ActionResult<Result> CloseComment([FromRoute] string articleLangId)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TArticleLang tlang = articleServer.Retrieve(new TArticleLang() { ArticleLangId = articleLangId });

            if (tlang == null)
                throw new ResultException("文章不存在");

            if (user.UserId != tlang.UserId)
                throw new ResultException("无法操作他人的文章");

            articleServer.CloseComment(tlang);

            return new Result(200, "成功");
        }



        /// <summary>
        /// 文章是否有帮助
        /// </summary>
        /// <param name="helpful"></param>
        /// <returns></returns>
        [HttpGet("helpful/{articleLangId}/{helpful}")]
        public ActionResult<Result> Helpful([FromRoute] string articleLangId, [FromRoute] short helpful)
        {
            THelpful help = new THelpful()
            {
                Ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString(),
                ArticleLangId = articleLangId,
                Helpful = helpful
            };

            if (helpfulServer.RetrieveByIp(articleLangId, help.Ip) != null)
                throw new ResultException("不能重复评价此文章");

            helpfulServer.CommitHelpful(help);
            return new Result(200, "成功");
        }


        /// <summary>
        /// 文章帮助信息
        /// </summary>
        /// <param name="articleLangId"></param>
        /// <returns></returns>
        [HttpGet("helpful/{articleLangId}")]
        public ActionResult<Result> HelpfulInfo([FromRoute] string articleLangId)
        {
            var data = new
            {
                sumCount = helpfulServer.HelpfulsCount(articleLangId),
                helpfulCount = helpfulServer.HelpfulCount(articleLangId)
            };

            return new Result(200, "成功", data);
        }

        /// <summary>
        /// 客户端评价过此篇文章信息
        /// </summary>
        /// <param name="articleLangId"></param>
        /// <returns></returns>
        [HttpPost("helpful/{articleLangId}")]
        public ActionResult<Result> ClientHelpfulInfo([FromRoute] string articleLangId)
        {
            THelpful th = helpfulServer.RetrieveByIp(articleLangId, HttpContext.Request.Headers["X-Forwarded-For"].ToString());

            return new Result(200, "成功", th == null ? 0 : (th.Helpful == 0 ? -1 : 1));
        }

        /// <summary>
        /// 推荐文章
        /// </summary>
        /// <param name="langValue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("Recommended/{langValue}/{count?}")]
        public ActionResult<Result> Recommended([FromRoute] string langValue, [FromRoute] int? count)
        {
            TLang lang = langServer.RetrieveByValue(langValue);
            if (lang == null)
                throw new ResultException("语言代号错误");

            dynamic art_data = articleServer.Recommended(lang.LangId);

            if (count.HasValue)
                return new Result(200, "成功", ((IEnumerable<dynamic>)art_data).Take(count.Value));
            else return new Result(200, "成功", art_data);
        }

        [HttpGet("categories/{langValue}/{articleMainType}/{count?}")]
        public ActionResult<Result> CategoriesArticles([FromRoute] string langValue, [FromRoute] int articleMainType, [FromRoute] int count)
        {
            TLang lang = langServer.RetrieveByValue(langValue);
            if (lang == null)
                throw new ResultException("语言代号错误");

            // var data = new
            // {
            //     MainType = typeServer.Retrieve(new TType { TypeId = articleMainType }),
            //     ArticleTypes = 
            // };

            return new Result(200, "成功", articleServer.CategoriesArticles(lang.LangId, articleMainType, count));
        }

        /// <summary>
        /// 相关文章
        /// </summary>
        /// <param name="articleLangId">当前文章</param>
        /// <param name="count">数据量</param>
        /// <returns></returns>
        [HttpGet("Related/{articleLangId}/{count}")]
        public ActionResult<Result> RelatedArticles(string articleLangId, int count)
        {
            return new Result(200, "成功", articleServer.RelatedArticles(articleLangId, count));
        }

        /// <summary>
        /// 获取相应语言下所有文章信息
        /// </summary>
        /// <param name="LangValue"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("LangArticles/{langValue}/{page?}/{count?}")]
        public ActionResult<Result> RetrieveArticlesByLang(string LangValue, int page, int count)
        {
            TLang lang = langServer.RetrieveByValue(LangValue);
            if (lang == null)
                throw new ResultException("语言代号错误");

            return new Result(200, "成功", Pagination<dynamic>.Paging(articleServer.RetrieveArticlesByLang(lang.LangId), page, count));
        }
    }
}