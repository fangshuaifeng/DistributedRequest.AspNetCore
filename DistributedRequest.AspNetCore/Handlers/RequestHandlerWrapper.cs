using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedRequest.AspNetCore.Handlers
{
    public abstract class RequestHandlerBase
    {
        public abstract Task<object> Handle(object request, BroadCastModel broadCast, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }

    public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
        public abstract Task<TResponse> Handle(IJobRequest<TResponse> request, BroadCastModel broadCast, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }

    public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse> where TRequest : IJobRequest<TResponse>
    {
        public override Task<TResponse> Handle(IJobRequest<TResponse> request, BroadCastModel broadCast, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var hander = serviceProvider.GetRequiredService<IJobRequestHandler<TRequest, TResponse>>();
            return hander.Handle((TRequest)request, broadCast, cancellationToken);
        }

        public override async Task<object> Handle(object request, BroadCastModel broadCast, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            return await Handle((IJobRequest<TResponse>)request, broadCast, serviceProvider, cancellationToken);//.ConfigureAwait(false);
        }
    }
}
