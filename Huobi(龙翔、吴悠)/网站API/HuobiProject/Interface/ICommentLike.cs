using HuobiProject.Models;

namespace HuobiProject.Interface
{
    /// <summary>
    /// 点赞接口
    /// </summary>
    public interface ICommentLike : ICURD<TCommentLike>
    {
        /// <summary>
        /// 通过用户id和评论id获取点赞信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="commentId">评论Id</param>
        /// <returns></returns>
        TCommentLike RetrieveByUserAndComment(string userId, string commentId);
    }
}