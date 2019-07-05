using System;
using System.Linq;
using System.Text.RegularExpressions;
using HuobiProject.Models;
using HuobiProject.Interface;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using HuobiProject.Utils;
using NETCore.Encrypt;

namespace HuobiProject.Impl
{
    /// <summary>
    /// 用户接口实现
    /// </summary>
    public class UserImpl : IUser
    {
        private HuobiContext m_db;
        private IUser m_iUser;
        private Token m_token;

        public UserImpl(HuobiContext db, Token token)
        {
            this.m_db = db;
            this.m_iUser = this;
            this.m_token = token;
        }

        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="password">密码</param>
        /// <param name="salt">盐值</param>
        /// <returns>密文</returns>
        private string m_EncriptPassword(string id, string password, string salt)
        {
            return EncryptProvider.Sha256(
                    EncryptProvider.Sha256(password) +
                    EncryptProvider.Sha256(id) +
                    EncryptProvider.Sha256(salt)
                    );
        }

        bool ICURD<TUser>.Create(TUser t)
        {
            t.CreateTime = DateTime.Now;
            m_db.TUser.Add(t);
            return m_db.SaveChanges() > 0;
        }

        bool ICURD<TUser>.Update(TUser t)
        {
            TUser t_user = m_db.TUser.Find(t.UserId);
            if (t_user == null)
                return false;
            t_user.Account = t.Account;
            t_user.Password = t.Password;
            t_user.Salt = t.Salt;
            t_user.Super = t.Super;
            t_user.UserName = t.UserName;
            t_user.Photo = t.Photo;

            return m_db.SaveChanges() > 0;
        }

        TUser ICURD<TUser>.Retrieve(TUser t)
        {
            return m_db.TUser.Find(t.UserId);
        }

        bool ICURD<TUser>.Delete(TUser t)
        {
            TUser t_user = m_db.TUser.Find(t.UserId);
            if (t_user == null)
                return false;
            m_db.TUser.Remove(t_user);

            return m_db.SaveChanges() > 0;
        }


        /// <summary>
        /// 根据Account获取TUser
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>
        /// Success：TUser
        /// Failed：NULL
        /// </returns>
        TUser IUser.RetrieveByAccount(string account)
        {
            var t_user = m_db.TUser.Where(u => u.Account == account).FirstOrDefault();
            return t_user;
        }

        /// <summary>
        /// 普通用户登录
        /// </summary>
        /// <param name="user">TUser</param>
        /// <returns>Token</returns>
        string IUser.Login(TUser user)
        {
            TUser login_user = m_iUser.RetrieveByAccount(user.Account);
            if (login_user == null || login_user.Super != 0)
                throw new ResultException(400, "用户名或密码错误");
            user.Password = m_EncriptPassword(login_user.UserId, user.Password, login_user.Salt);

            if (login_user.Password != user.Password)
                throw new ResultException(400, "用户名或密码错误");

            return m_token.CreateToken(login_user);
        }

        /// <summary>
        /// 管理用户登录
        /// </summary>
        /// <param name="user">TUser</param>
        /// <returns>Token</returns>
        string IUser.LoginSuper(TUser user)
        {
            TUser login_user = m_iUser.RetrieveByAccount(user.Account);
            if (login_user == null || login_user.Super != 1)
                throw new ResultException(400, "用户名或密码错误");
            user.Password = m_EncriptPassword(login_user.UserId, user.Password, login_user.Salt);

            if (login_user.Password != user.Password)
                throw new ResultException(400, "用户名或密码错误");

            return m_token.CreateToken(login_user);
        }

        /// <summary>
        /// 普通用户注册
        /// </summary>
        /// <param name="user">TUser</param>
        void IUser.Register(TUser user)
        {
            //使用事务
            using (TransactionScope scope = new TransactionScope())
            {
                user.UserId = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) + "";
                user.Super = 0;//普通用户
                user.CreateTime = DateTime.Now;

                //生成salt
                user.Salt = Guid.NewGuid().ToString();

                //加密密码
                user.Password = m_EncriptPassword(user.UserId, user.Password, user.Salt);

                m_iUser.Create(user);

                scope.Complete();
            }
        }

        /// <summary>
        /// 管理用户注册
        /// </summary>
        /// <param name="user">TUser</param>
        void IUser.RegisterSuper(TUser user)
        {
            //使用事务
            using (TransactionScope scope = new TransactionScope())
            {
                user.UserId = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) + "";
                user.Super = 1;//管理用户
                user.CreateTime = DateTime.Now;

                //生成salt
                user.Salt = Guid.NewGuid().ToString();

                //加密密码
                user.Password = m_EncriptPassword(user.UserId, user.Password, user.Salt);

                m_iUser.Create(user);

                scope.Complete();
            }
        }

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
        bool IUser.VerifyRegister(TUser user)
        {
            //验证数据
            if (user.Account == null || user.Account == "")
                throw new ResultException("邮箱不能为空");

            string reg_email = @"^\w+(?<=[^ ])@\w+\.\w+$";

            if (!Regex.IsMatch(user.Account, reg_email))
                throw new ResultException("邮箱格式错误");


            TUser t_user = m_iUser.RetrieveByAccount(user.Account);
            if (t_user != null)
                throw new ResultException("邮箱已被注册");

            if (user.Account.Length > 50)
                throw new ResultException("最多50个字符");

            // if (user.Password == null || user.Password == "")
            //     throw new ResultException("密码不能为空");

            if (user.Password.Length > 18)
                throw new ResultException("密码最多18个字符");

            if (user.UserName == null || user.UserName == "")
                user.UserName = user.Account;

            return true;
        }
    }
}