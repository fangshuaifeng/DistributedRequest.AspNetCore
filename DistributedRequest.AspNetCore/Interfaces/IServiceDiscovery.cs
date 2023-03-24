using System.Collections.Generic;
using System.Threading.Tasks;

namespace DistributedRequest.AspNetCore.Interfaces
{
    /// <summary>
    /// 服务发现
    /// </summary>
    internal interface IServiceDiscovery
    {
        /// <summary>
        /// 通过服务名称获取所有服务地址
        /// </summary>
        /// <param name="serviceName">默认取配置文件中的服务名称</param>
        /// <returns></returns>
        Task<List<string>> GetServiceUrls(string serviceName = null);
    }
}
