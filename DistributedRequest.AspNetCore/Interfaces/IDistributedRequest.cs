using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        /// <typeparam name="TResopnse"></typeparam>
        /// <param name="request">请求参数</param>
        /// <param name="partitionCount">最大分片数/调用服务器个数</param>
        /// <param name="cancellationToken"></param>
        /// <param name="onlyLocal">仅请求本机，多做为本地调试用</param>
        /// <returns></returns>
        Task<List<TResopnse>> PostJsonAsync<TResopnse>(IJobRequest<TResopnse> request, CancellationToken cancellationToken = default, int? partitionCount = null, bool onlyLocal = false) where TResopnse : class;
    }
}