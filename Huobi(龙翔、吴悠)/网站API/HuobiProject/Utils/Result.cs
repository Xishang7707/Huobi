using Newtonsoft.Json;

namespace HuobiProject.Utils
{
    /// <summary>
    /// 返回结果类
    /// </summary>
    public class Result
    {
        public int code { get; set; }
        public string status { get; set; }
        public object data { get; set; }

        public Result(int code, string status) : this(code, status, null)
        {

        }

        public Result(int code, string status, object data)
        {
            this.code = code;
            this.status = status;
            this.data = data;
        }
    }
}