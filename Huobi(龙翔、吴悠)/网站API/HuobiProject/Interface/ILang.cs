using System.Linq;
using System.Collections.Generic;
using HuobiProject.Models;

namespace HuobiProject.Interface
{
    public interface ILang : ICURD<TLang>
    {
        /// <summary>
        /// 根据语言代码值获取语言信息
        /// </summary>
        /// <param name="val">语言代码</param>
        /// <returns></returns>
        TLang RetrieveByValue(string val);

        /// <summary>
        /// 获取所有语言信息
        /// </summary>
        /// <returns></returns>
        IQueryable<TLang> RetrieveAll();
    }
}