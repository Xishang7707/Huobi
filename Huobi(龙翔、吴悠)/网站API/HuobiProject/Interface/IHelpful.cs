using HuobiProject.Models;

namespace HuobiProject.Interface
{
    public interface IHelpful : ICURD<THelpful>
    {
        /// <summary>
        /// 根据文章Id设置文章是否有帮助
        /// </summary>
        /// <param name="helpful">是否有帮助</param>
        void CommitHelpful(THelpful helpful);

        /// <summary>
        /// 有帮助的人数
        /// </summary>
        /// <param name="articleLangId">文章Id</param>
        /// <returns></returns>
        int HelpfulCount(string articleLangId);

        /// <summary>
        /// 所有人数
        /// </summary>
        /// <param name="articleLangId">文章Id</param>
        /// <returns></returns>
        int HelpfulsCount(string articleLangId);

        /// <summary>
        /// 获取客户端评价信息
        /// </summary>
        /// <param name="articleLangId">文章id</param>
        /// <param name="ip">ip地址</param>
        /// <returns></returns>
        THelpful RetrieveByIp(string articleLangId, string ip);
    }
}