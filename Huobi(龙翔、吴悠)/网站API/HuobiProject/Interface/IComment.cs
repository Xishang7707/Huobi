using System.Linq;
using HuobiProject.Models;

namespace HuobiProject.Interface
{
    /// <summary>
    /// 评论接口
    /// </summary>
    public interface IComment : ICURD<TComment>
    {
        /// <summary>
        /// 获取文章所有评论
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <returns></returns>
        dynamic RetrieveAll(string articleLangId);

        /// <summary>
        /// 获取文章评论数量
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <returns></returns>
        int CommentCount(string articleLangId);

    }
}