using System.Collections.Generic;

namespace DistributedRequest.AspNetCore.Models
{
    public class DistributedRequestOption
    {
        /// <summary>
        /// Consul地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 当前服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 路由地址，默认 = dr-client
        /// </summary>
        public string BasePath { get; set; } = "dr-client";
        /// <summary>
        /// 服务地址黑名单
        /// </summary>
        public List<string> AddressBlackList { get; set; } = new List<string>();
    }
}
