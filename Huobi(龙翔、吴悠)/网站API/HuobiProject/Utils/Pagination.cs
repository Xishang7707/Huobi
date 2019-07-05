using System.Linq;
using System.Collections.Generic;

namespace HuobiProject.Utils
{
    /// <summary>
    /// 分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pagination<T>
    {
        /// <summary>
        /// 页码
        /// </summary>
        /// <value></value>
        public int Page { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        /// <value></value>
        public int Total { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        /// <value></value>
        public int TotalPages { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        /// <value></value>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 构造分页数据
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="total">数据总量</param>
        /// <param name="totalPages">页码总量</param>
        /// <param name="data">数据</param>
        public Pagination(int page, int total, int totalPages, IEnumerable<T> data)
        {
            this.Page = page;
            this.Total = total;
            this.TotalPages = totalPages;
            this.Data = data;
        }

        /// <summary>
        /// 分页
        /// 如果页码或者数量为<=0则不进行分页，但保留分页数据格式
        /// </summary>
        /// <param name="list">数据源</param>
        /// <param name="page">页码</param>
        /// <param name="count">数量</param>
        /// <returns>分页数据</returns>
        public static Pagination<T> Paging(IQueryable<T> list, int page, int count)
        {
            // if (page <= 0)
            //     throw new ResultException("页码最小为1");
            // if (count <= 0)
            //     throw new ResultException("数据量最小为1");

            int dataCount = list.Count();

            if (page <= 0 || count <= 0) //不进行分页
                return new Pagination<T>(1, dataCount, 1, list);

            int totalPages = (dataCount % count > 0) ? dataCount / count + 1 : dataCount / count;
            int passCount = ((page - 1) * count);
            IEnumerable<T> list_enum = list.Skip(passCount).Take(count);

            return new Pagination<T>(page, dataCount, totalPages, list_enum);
        }

        public static Pagination<T> Paging(IEnumerable<T> list, int page, int count)
        {
            // if (page <= 0)
            //     throw new ResultException("页码最小为1");
            // if (count <= 0)
            //     throw new ResultException("数据量最小为1");

            int dataCount = list.Count();

            if (page <= 0 || count <= 0) //不进行分页
                return new Pagination<T>(1, dataCount, 1, list);

            int totalPages = (dataCount % count > 0) ? dataCount / count + 1 : dataCount / count;
            int passCount = ((page - 1) * count);
            IEnumerable<T> list_enum = list.Skip(passCount).Take(count);

            return new Pagination<T>(page, dataCount, totalPages, list_enum);
        }
    }
}