namespace DistributedRequest.AspNetCore.Models
{
    public class RequestContext
    {
        public RequestContext() { }

        /// <summary>
        /// 请求任务参数
        /// </summary>
        /// <param name="executorHandler">待执行任务</param>
        /// <param name="executorParams">参数</param>
        /// <param name="maxCount">最大分片数</param>
        public RequestContext(string executorHandler, string executorParams = null, string maxCount = null)
        {
            ExecutorHandler = executorHandler;
            ExecutorParams = executorParams;
            MaxCount = maxCount;
        }

        /// <summary>
        /// 待执行任务
        /// </summary>
        public string ExecutorHandler { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string ExecutorParams { get; set; }
        /// <summary>
        /// 集群请求最大服务器数，默认等于当前服务器数
        /// </summary>
        public string MaxCount { get; set; }

        internal int? _MaxCount
        {
            get
            {
                int.TryParse(MaxCount, out var _max);
                return _max;
            }
        }
    }
}
