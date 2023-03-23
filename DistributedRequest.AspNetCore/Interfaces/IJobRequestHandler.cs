using System.Threading.Tasks;
using System.Threading;
using DistributedRequest.AspNetCore.Models;

namespace DistributedRequest.AspNetCore.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IJobRequestHandler<in TRequest, TResponse> where TRequest : IJobRequest<TResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="broadCast">分片信息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResponse> Handle(TRequest request, BroadCastModel broadCast, CancellationToken cancellationToken);
    }
}
