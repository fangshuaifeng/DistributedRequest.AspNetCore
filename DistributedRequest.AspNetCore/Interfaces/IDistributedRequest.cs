using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedRequest.AspNetCore.Models;

namespace DistributedRequest.AspNetCore.Interfaces
{
    /// <summary>
    /// 分布式请求接口
    /// </summary>
    public interface IDistributedRequest
    {
        /// <summary>
        /// 分布式POST请求
        /// </summary>
        /// <param name="param">请求参数</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onlyLocal">仅请求本机，多做为本地调试用</param>
        /// <returns></returns>
        Task<List<ReturnT>> PostJsonAsync(RequestContext param, CancellationToken cancellationToken = default, bool onlyLocal = false);
        /// <summary>
        /// 分布式POST请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">请求参数</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onlyLocal">仅请求本机，多做为本地调试用</param>
        /// <returns></returns>
        Task<List<ReturnT<T>>> PostJsonAsync<T>(RequestContext param, CancellationToken cancellationToken = default, bool onlyLocal = false);
    }
}