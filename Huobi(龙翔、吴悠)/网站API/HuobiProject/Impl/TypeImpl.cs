using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using HuobiProject.Interface;
using HuobiProject.Models;
using HuobiProject.Utils;

namespace HuobiProject.Impl
{
    /// <summary>
    /// 状态类型实现
    /// </summary>
    public class TypeImpl : IType
    {
        private HuobiContext m_db;
        private IType m_type;

        public TypeImpl(HuobiContext db)
        {
            m_db = db;
            m_type = this;
        }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// True：成功
        /// False：失败
        /// </returns>
        bool ICURD<TType>.Create(TType t)
        {
            m_db.TType.Add(t);
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
        bool ICURD<TType>.Update(TType t)
        {
            TType tp = m_type.Retrieve(t);

            tp.Name = t.Name;
            tp.Description = t.Description;
            tp.ParentId = t.ParentId;
            tp.LangId = t.LangId;

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
        TType ICURD<TType>.Retrieve(TType t)
        {
            return m_db.TType.Find(t.TypeId);
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="t">HuobiProject.Models</param>
        /// <returns>
        /// Success：T
        /// Failed：NULL
        /// </returns>
        bool ICURD<TType>.Delete(TType t)
        {
            TType tp = m_type.Retrieve(t);
            m_db.TType.Remove(tp);
            return m_db.SaveChanges() > 0;
        }

        /// <summary>
        /// 是否是文章子类型Id,并且有效
        /// </summary>
        /// <param name="typeId">子类型Id</param>
        /// <returns></returns>
        bool IType.IsArticleType(int typeId)
        {
            TType t = m_type.Retrieve(new TType()
            {
                TypeId = typeId
            });
            if (t == null || typeId <= 300000 || typeId > 400000)
                return false;
            return true;
        }

        /// <summary>
        /// 查询语言文字
        /// </summary>
        /// <param name="type">子类型ID</param>
        /// <param name="lang">语言类型</param>
        /// <returns></returns>
        IQueryable IType.QueryArticles(TType type, TLang lang) => throw new NotImplementedException();

        /// <summary>
        /// 根据Name值获取资源信息
        /// </summary>
        /// <param name="name">资源</param>
        /// <returns>
        /// Success：TType
        /// Failed：Null
        /// </returns>
        TType IType.RetrieveResourceByName(string name)
        {
            //资源范围[100000-200000]
            return m_db.TType
            .Where(w => w.Name.ToLower() == name.ToLower() && (w.TypeId > 100000 && w.TypeId < 200000))
            .FirstOrDefault();
        }

        /// <summary>
        /// 根据语言Id获取文章主类型
        /// </summary>
        /// <param name="langId">语言id</param>
        /// <returns>文章主类型列表</returns>
        dynamic IType.RetrieveArticleMainTypes(int langId)
        {
            return m_db.TType
            .Where(w => w.TypeId > 200000 && w.TypeId < 300000 && w.LangId == langId)
            .Select(s => new
            {
                s.TypeId,
                s.Name,
                s.Description,
                s.Lang
            });
        }

        /// <summary>
        /// 根据主类型Id查询子类型
        /// 
        /// </summary>
        /// <param name="typeId">主类型Id</param>
        /// <returns>子类型列表</returns>
        dynamic IType.RetrieveArticleTypes(int typeId)
        {
            return m_db.TType
            .Where(w => w.TypeId > 300000 && w.TypeId < 400000 && w.ParentId == typeId)
            .Select(s => new
            {
                s.TypeId,
                s.Name,
                s.Description,
                Parent = new
                {
                    s.Parent.TypeId,
                    s.Parent.Name
                },
                s.Lang
            });
        }

        /// <summary>
        /// 根据语言Id获取所有文章子类型
        /// </summary>
        /// <param name="langId">语言Id</param>
        /// <returns></returns>
        IQueryable<TType> IType.RetrieveArticleTypesByLangId(int langId)
        {
            return m_db.TType
            .Where(w => w.TypeId > 300000 && w.TypeId < 400000 && w.LangId == langId);
        }

        /// <summary>
        /// 获取工单类型列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TType> IType.RetrieveTicketTypes()
        {
            return m_db.TType.Where(w => w.TypeId > 400000 && w.TypeId < 500000);
        }

        /// <summary>
        /// 获取工单问题分类列表
        /// </summary>
        /// <param name="typeId">工单类型Id</param>
        /// <returns></returns>
        IQueryable<TType> IType.RetrieveTicketQuestionTypes(int typeId)
        {
            return m_db.TType.Where(w => w.TypeId > 500000 && w.TypeId < 600000 && w.ParentId == typeId);
        }

        /// <summary>
        /// 获取工单类型下问题列表
        /// </summary>
        /// <param name="typeId">工单问题类型id</param>
        /// <returns></returns>
        IQueryable<TType> IType.RetrieveTicketQuestionList(int typeId)
        {
            return m_db.TType.Where(w => w.TypeId > 600000 && w.TypeId < 700000 && w.ParentId == typeId);
        }

        /// <summary>
        /// 获取工单主类型下级联问题信息
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        dynamic IType.RetrieveTicketQuestionCascade(int typeId)
        {
            return m_db.TType
                        .Include(i => i.InverseParent)
                        .Where(w => w.ParentId == typeId)
                        .Select(s => new
                        {
                            s.TypeId,
                            s.Name,
                            s.Description,
                            s.ParentId,
                            s.LangId,
                            Childrens = s.InverseParent
                        });
        }

        /// <summary>
        /// 通过资源类型Id获取类型信息
        /// </summary>
        /// <param name="typeId">类型Id</param>
        /// <returns></returns>
        TType IType.RetrieveResourceTypeByTypeId(int typeId)
        {
            return m_db.TType.Where(w => w.TypeId > 100000 && w.TypeId < 200000 && w.TypeId == typeId).SingleOrDefault();
        }
    }
}