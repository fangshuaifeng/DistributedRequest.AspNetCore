using DistributedRequest.AspNetCore.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedRequest.AspNetCore.Interfaces
{
    internal interface IClientHandler
    {
        Task<string> HandlerAsync(InnerContext jobContext, CancellationToken cancellationToken);
        Task<TResponse> HandlerAsync<TResponse>(InnerContext jobContext, CancellationToken cancellationToken);
        Task<object> HandlerAsync(InnerContext jobContext, Type type, CancellationToken cancellationToken);
    }
}
