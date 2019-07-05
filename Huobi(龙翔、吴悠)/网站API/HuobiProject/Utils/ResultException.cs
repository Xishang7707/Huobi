using System;
namespace HuobiProject.Utils
{
    public class ResultException : Exception
    {
        public int Code { get; set; }//异常代码

        public ResultException() : this(400) { }
        public ResultException(int code) : this(code, "请求错误") { }
        public ResultException(string message) : this(400, message) { }
        public ResultException(int code, string message) : base(message) => this.Code = code;
    }
}