using System;
using System.Linq;
using System.Collections.Generic;
using HuobiProject.Models;
using HuobiProject.Interface;

namespace HuobiProject.Impl
{
    public class LangImpl : ILang
    {
        private HuobiContext m_db;
        private ILang m_lang;

        public LangImpl(HuobiContext db) { m_db = db; m_lang = this; }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TLang>.Create(TLang t)
        {
            m_db.TLang.Add(t);
            return m_db.SaveChanges() > 0;
        }
        /// <summary>
        /// 根据ID更新数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TLang>.Update(TLang t)
        {
            TLang lang = m_lang.Retrieve(t);
            lang.Name = t.Name;
            lang.Value = t.Value;
            lang.Description = t.Description;

            return m_db.SaveChanges() > 0;
        }
        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：NULL
        /// </returns>
        TLang ICURD<TLang>.Retrieve(TLang t)
        {
            return m_db.TLang.Find(t.LangId);
        }
        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：F
        /// </returns>
        bool ICURD<TLang>.Delete(TLang t)
        {
            TLang lang = m_lang.Retrieve(t);
            m_db.TLang.Remove(lang);
            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据语言代码值获取语言信息
        /// </summary>
        /// <param name="val">语言代码</param>
        /// <returns></returns>
        TLang ILang.RetrieveByValue(string val)
        {
            return m_db.TLang.Where(w => w.Value == val).FirstOrDefault();
        }

        /// <summary>
        /// 获取所有语言信息
        /// </summary>
        /// <returns></returns>
        IQueryable<TLang> ILang.RetrieveAll()
        {
            return m_db.TLang.AsQueryable();
        }
    }
}