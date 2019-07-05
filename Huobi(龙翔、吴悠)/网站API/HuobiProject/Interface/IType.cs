using System.Collections.Generic;
using HuobiProject.Models;
using HuobiProject.Utils;
using System.Linq;
namespace HuobiProject.Interface
{
    /// <summary>
    /// 状态类型接口
    /// </summary>
    public interface IType : ICURD<TType>
    {
        /// <summary>
        /// 是否是文章子类型Id,并且有效
        /// </summary>
        /// <param name="typeId">子类型Id</param>
        /// <returns></returns>
        bool IsArticleType(int typeId);
        /// <summary>
        /// 查询语言文字
        /// </summary>
        /// <param name="type">子类型ID</param>
        /// <param name="lang">语言类型</param>
        /// <returns></returns>
        IQueryable QueryArticles(TType type, TLang lang);
        /// <summary>
        /// 根据Name值获取资源信息
        /// </summary>
        /// <param name="name">资源</param>
        /// <returns>
        /// Success：TType
        /// Failed：Null
        /// </returns>
        TType RetrieveResourceByName(string name);

        /// <summary>
        /// 根据语言Id获取文章主类型
        /// </summary>
        /// <param name="langId">语言id</param>
        /// <returns>文章主类型列表</returns>
        dynamic RetrieveArticleMainTypes(int langId);

        /// <summary>
        /// 根据主类型Id查询子类型
        /// 
        /// </summary>
        /// <param name="typeId">主类型Id</param>
        /// <returns>子类型列表</returns>
        dynamic RetrieveArticleTypes(int typeId);

        /// <summary>
        /// 根据语言Id获取所有文章子类型
        /// </summary>
        /// <param name="langId">语言Id</param>
        /// <returns></returns>
        IQueryable<TType> RetrieveArticleTypesByLangId(int langId);

        /// <summary>
        /// 获取工单类型列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TType> RetrieveTicketTypes();

        /// <summary>
        /// 获取工单问题分类列表
        /// </summary>
        /// <param name="typeId">工单类型Id</param>
        /// <returns></returns>
        IQueryable<TType> RetrieveTicketQuestionTypes(int typeId);

        /// <summary>
        /// 获取工单类型下问题列表
        /// </summary>
        /// <param name="typeId">工单问题类型id</param>
        /// <returns></returns>
        IQueryable<TType> RetrieveTicketQuestionList(int typeId);

        /// <summary>
        /// 获取工单主类型下级联问题信息
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        dynamic RetrieveTicketQuestionCascade(int typeId);

        /// <summary>
        /// 通过资源类型Id获取类型信息
        /// </summary>
        /// <param name="typeId">类型Id</param>
        /// <returns></returns>
        TType RetrieveResourceTypeByTypeId(int typeId);
    }
}