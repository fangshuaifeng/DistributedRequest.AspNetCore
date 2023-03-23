namespace DistributedRequest.AspNetCore.Models
{
    internal class InnerContext
    {
        public InnerContext()
        {
        }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// 请求实体FullName
        /// </summary>
        public string TRequest { get; set; }
        /// <summary>
        /// 响应实体FullName
        /// </summary>
        public string TResponse { get; set; }
        /// <summary>
        /// 分片信息
        /// </summary>
        public BroadCastModel BroadCast { get; set; }
    }
}
