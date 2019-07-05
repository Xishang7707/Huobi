using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;

namespace HuobiProject.Controllers
{
    /// <summary>
    /// 评论API接口
    /// </summary>
    [Route("api/Comments")]
    public class CommentsController : ControllerBase
    {
        private IComment commentServer;
        private IUser userServer;

        private ICommentLike commentLikeServer;

        public CommentsController(IComment commentServer, IUser userServer, ICommentLike commentLikeServer)
        {
            this.commentServer = commentServer;
            this.userServer = userServer;
            this.commentLikeServer = commentLikeServer;
        }

        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("publish")]
        public ActionResult<Result> Publish([FromBody] TComment comment)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            comment.UserId = user.UserId;

            commentServer.Create(comment);

            return new Result(200, "成功");
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("delete/{commentId}")]
        public ActionResult<Result> Delete([FromRoute] string commentId)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TComment comment = commentServer.Retrieve(new TComment()
            {
                CommentId = commentId
            });

            if (comment.UserId != user.UserId)
                throw new ResultException("不能删除他人的评论");

            commentServer.Delete(new TComment()
            {
                CommentId = commentId
            });
            return new Result(200, "成功");
        }

        /// <summary>
        /// 修改评论
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("update")]
        public ActionResult<Result> Delete([FromBody] TComment comment)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TComment com = commentServer.Retrieve(new TComment()
            {
                CommentId = comment.CommentId
            });

            if (com.UserId != user.UserId)
                throw new ResultException("不能修改他人的评论");

            commentServer.Update(comment);
            return new Result(200, "成功");
        }

        /// <summary>
        /// 查询评论
        /// --页码或数据量<=0不分页
        /// --但提供分页数据格式
        /// </summary>
        /// <returns></returns>
        [HttpGet("retrieves/{articleLangId}/{page?}/{count?}")]
        public ActionResult<Result> Retrieve([FromRoute] string articleLangId, [FromRoute] int page, [FromRoute] int count)
        {
            return new Result(200, "成功", Pagination<dynamic>.Paging(commentServer.RetrieveAll(articleLangId), page, count));
        }


        /// <summary>
        /// 获取文章评论数量
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <returns></returns>
        [HttpGet("count/{articleLangId}")]
        public ActionResult<Result> Count([FromRoute] string articleLangId)
        {
            return new Result(200, "成功", commentServer.CommentCount(articleLangId));
        }

        /// <summary>
        /// 点赞/取消赞
        /// 踩/取消踩
        /// 改赞为踩
        /// 改踩为赞
        /// </summary>
        /// <param name="commentId">评论Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("like/{commentId}/{islike}")]
        public ActionResult<Result> Like([FromRoute] string commentId, [FromRoute] short islike)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TComment cmt = commentServer.Retrieve(new TComment { CommentId = commentId });
            TCommentLike tml = commentLikeServer.RetrieveByUserAndComment(user.UserId, commentId);

            if (cmt.UserId == user.UserId)
                throw new ResultException($"不能{(islike == 0 ? "踩" : "赞")}自己的评论");

            if (tml == null)//点赞
                commentLikeServer.Create(new TCommentLike
                {
                    UserId = user.UserId,
                    CommentId = commentId,
                    Like = islike
                });
            else if (tml.Like == 1 && islike == 0)//赞改为踩
            {
                tml.Like = 0;
                commentLikeServer.Update(tml);
            }
            else if (tml.Like == 0 && islike == 1)//踩改为赞
            {
                tml.Like = 1;
                commentLikeServer.Update(tml);
            }
            else//取消赞/踩
            {
                commentLikeServer.Delete(tml);
            }

            return new Result(200, "成功");
        }

        /// <summary>
        /// 是否赞/踩
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("like/{commentId}")]
        public ActionResult<Result> Like([FromRoute] string commentId)
        {
            //获取用户信息
            string user_id = Token.GetUserId(HttpContext.Request.Headers["Authorization"].ToString().Substring(7));
            TUser user = userServer.Retrieve(new TUser() { UserId = user_id });

            TCommentLike tml = commentLikeServer.RetrieveByUserAndComment(user.UserId, commentId);

            if (tml == null)
                return new Result(200, "成功", null);
            return new Result(200, "成功", tml.Like);
        }
    }
}