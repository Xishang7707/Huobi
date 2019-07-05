using System.Linq;
using System.Collections.Generic;
using HuobiProject.Models;
using HuobiProject.Utils;

namespace HuobiProject.Interface
{
    /// <summary>
    /// 文章接口
    /// 逻辑处理(事务)
    /// </summary>
    public interface IArticle : ICURD<TArticleLang>
    {
        /// <summary>
        /// 创建文章主类型
        /// </summary>
        /// <returns>主类型Id</returns>
        string CreateArticle();

        /// <summary>
        /// 验证发布的文章
        /// </summary>
        /// <param name="article">文章信息</param>
        /// <returns></returns>
        bool VerifyPublish(TArticleLang article);

        /// <summary>
        /// 发布新文章
        /// </summary>
        /// <param name="article">文章信息</param>
        /// <param name="user">用户信息</param>
        void Publish(TArticleLang article, TUser user);

        /// <summary>
        /// 为已有文章添加新语言
        /// </summary>
        /// <param name="article">文章信息</param>
        /// <param name="user">用户信息</param>
        void PublishByLang(TArticleLang article, TUser user);

        /// <summary>
        /// 根据Id开启文章评论
        /// </summary>
        /// <param name="article">开启评论的文章</param>
        void OpenComment(TArticleLang article);

        /// <summary>
        /// 根据Id关闭文章评论
        /// </summary>
        /// <param name="article">关闭评论的文章</param>
        void CloseComment(TArticleLang article);

        /// <summary>
        /// 根据文章主Id获取对应语言的文章
        /// </summary>
        /// <param name="articleId">文章主Id</param>
        /// <param name="lang">语言信息</param>
        dynamic RetrieveByLang(string articleId, int langId);

        /// <summary>
        /// 根据关键字查询文章
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="langId">语言id</param>
        /// <returns></returns>
        dynamic SearchByKeyword(string keyword, int langId);

        /// <summary>
        /// 查询子类型所有文章.
        /// </summary>
        /// <param name="typeId">子类型Id</param>
        /// <returns></returns>
        List<TArticleLang> QueryArticles(int typeId);

        /// <summary>
        /// 根据文章主Id获取其所有语言文章
        /// </summary>
        /// <param name="articleId">文章主Id</param>
        /// <returns></returns>
        List<TArticleLang> RetrieveArticlesByArticleId(string articleId);

        /// <summary>
        /// 获取主分类下所有子类型和指定条数的文章
        /// </summary>
        /// <param name="langId">语言Id</param>
        /// <param name="mainArticleTypeId">文章主类型Id</param>
        /// <param name="count">数据量</param>
        /// <returns></returns>
        dynamic CategoriesArticles(int langId, int mainArticleTypeId, int count);

        /// <summary>
        /// 获取分类下所有文章
        /// </summary>
        /// <param name="typeId">类型Id</param>
        /// <returns>IQueryable<T></returns>
        dynamic RetrieveArticlesByTypeId(int typeId);

        /// <summary>
        /// 最近的活动
        /// </summary>
        /// <param name="langId">语言Id</param>
        /// <returns>列表</returns>
        dynamic RecentActivity(int langId);

        /// <summary>
        /// 推荐文章
        /// </summary>
        /// <param name="langId">语言id</param>
        /// <returns></returns>
        dynamic Recommended(int langId);

        /// <summary>
        /// 获取文章主信息
        /// </summary>
        /// <param name="article">文章</param>
        /// <returns></returns>
        TArticle RetrieveMainArticle(TArticle article);

        /// <summary>
        /// 删除主文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        bool DeleteMainArticle(TArticle article);

        /// <summary>
        /// 相关文章--随机
        /// </summary>
        /// <param name="articleLangId">当前文章id</param>
        /// <param name="count">数据量</param>
        /// <returns></returns>
        dynamic RelatedArticles(string articleLangId, int count);

        /// <summary>
        /// 根据语言id取所有文章信息
        /// </summary>
        /// <param name="langId">语言id</param>
        /// <returns></returns>
        dynamic RetrieveArticlesByLang(int langId);
    }
}