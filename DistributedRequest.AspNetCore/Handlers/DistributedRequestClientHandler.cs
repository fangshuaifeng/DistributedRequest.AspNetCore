using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedRequest.AspNetCore.Handlers
{
    internal class DistributedRequestClientHandler : IClientHandler
    {
        private readonly ServiceTypeList _typeList;
        private readonly IServiceProvider _serviceProvider;
        public DistributedRequestClientHandler(ServiceTypeList _typeList, IServiceProvider _serviceProvider)
        {
            this._typeList = _typeList;
            this._serviceProvider = _serviceProvider;
        }

        public async Task<string> HandlerAsync(InnerContext jobContext, CancellationToken cancellationToken)
        {
            var typeResponse = _typeList.GetType(Enums.TypeEnum.Response, jobContext.TResponse);
            if (typeResponse == null) throw new NotImplementedException("This response does not exist.");

            return JsonConvert.SerializeObject(await HandlerAsync(jobContext, typeResponse, cancellationToken));
        }

        public async Task<TResponse> HandlerAsync<TResponse>(InnerContext jobContext, CancellationToken cancellationToken)
        {
            return (TResponse)await HandlerAsync(jobContext, typeof(TResponse), cancellationToken);
        }

        public async Task<object> HandlerAsync(InnerContext jobContext, Type type, CancellationToken cancellationToken)
        {
            var typeRequest = _typeList.GetType(Enums.TypeEnum.Request, jobContext.TRequest);
            if (typeRequest == null) throw new NotImplementedException("This request does not exist.");

            var command = JsonConvert.DeserializeObject(jobContext.Parameter, typeRequest);
            if (command == null) throw new NotImplementedException("This request parameters cannot be deserialized to object.");

            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(typeRequest, type);
            var wrapper = (RequestHandlerBase)Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for type {jobContext.TRequest}");
            return await wrapper.Handle(command, jobContext.BroadCast, _serviceProvider, cancellationToken);
        }
    }
}
