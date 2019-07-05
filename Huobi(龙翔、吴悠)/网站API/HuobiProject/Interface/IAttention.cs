using HuobiProject.Models;

namespace HuobiProject.Interface
{
    /// <summary>
    /// 关注接口
    /// </summary>
    public interface IAttention : ICURD<TAttention>
    {
        /// <summary>
        /// 获取用户关注的类型信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="typeId">文章子类型Id</param>
        /// <returns></returns>
        TAttention RetriecveAttention(string userId, int typeId);
    }
}