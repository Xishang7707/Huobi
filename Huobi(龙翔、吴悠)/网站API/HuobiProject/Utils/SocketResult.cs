namespace HuobiProject.Utils
{
    /// <summary>
    /// WebSocket返回数据格式
    /// </summary>
    public class SocketResult
    {
        /// <summary>
        /// 目标
        /// </summary>
        /// <value></value>
        public string target { get; }

        /// <summary>
        /// 方法：[监听/操作]
        /// </summary>
        /// <value></value>
        public string method { get; }

        /// <summary>
        /// 数据
        /// </summary>
        /// <value></value>
        public object data { get; set; }

        public SocketResult() { }

        public SocketResult(string target, string method) : this(target, method, null) { }

        public SocketResult(string target, string method, object data)
        {
            this.target = target;
            this.method = method;
            this.data = data;
        }
    }
}