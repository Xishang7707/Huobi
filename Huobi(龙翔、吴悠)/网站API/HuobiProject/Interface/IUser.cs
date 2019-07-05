using HuobiProject.Models;
using HuobiProject.Utils;

namespace HuobiProject.Interface
{
    /// <summary>
    /// 用户接口
    /// 逻辑处理(事务)
    /// </summary>
    public interface IUser : ICURD<TUser>
    {
        /// <summary>
        /// 根据Account获取TUser
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>
        /// Success：TUser
        /// Failed：NULL
        /// </returns>
        TUser RetrieveByAccount(string account);
        
        /// <summary>
        /// 普通用户登录
        /// </summary>
        /// <param name="user">TUser</param>
        /// <returns>Token</returns>
        string Login(TUser user);

        /// <summary>
        /// 管理用户登录
        /// </summary>
        /// <param name="user">TUser</param>
        /// <returns>Token</returns>
        string LoginSuper(TUser user);

        /// <summary>
        /// 普通用户注册
        /// </summary>
        /// <param name="user">TUser</param>
        void Register(TUser user);

        /// <summary>
        /// 管理用户注册
        /// </summary>
        /// <param name="user">TUser</param>
        void RegisterSuper(TUser user);

        /// <summary>
        /// 验证注册字段
        /// 
        /// 异常:
        ///     T:HuobiProject.Utils.ResultException:
        ///       用户注册信息验证失败，捕获错误可得到失败信息
        /// </summary>
        /// <param name="user">TUser</param>
        /// <returns>
        /// Success：True
        /// </returns>
        bool VerifyRegister(TUser user);
    }
}