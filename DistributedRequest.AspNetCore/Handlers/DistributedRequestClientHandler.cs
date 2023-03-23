using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedRequest.AspNetCore.Handlers
{
    internal class DistributedRequestClientHandler
    {
        private readonly GlobalTypeList _typeList;
        private readonly IServiceProvider _serviceProvider;
        public DistributedRequestClientHandler(GlobalTypeList _typeList, IServiceProvider _serviceProvider)
        {
            this._typeList = _typeList;
            this._serviceProvider = _serviceProvider;
        }

        public async Task<string> HandlerAsync(InnerContext jobContext, CancellationToken cancellationToken)
        {
            var typeRequest = _typeList.GetType(Enums.TypeEnum.Request, jobContext.TRequest);
            var typeResponse = _typeList.GetType(Enums.TypeEnum.Response, jobContext.TResponse);
            if (typeRequest == null || typeRequest == null) throw new NotImplementedException("This request does not exist.");

            var command = JsonConvert.DeserializeObject(jobContext.Parameter, typeRequest);
            if (command == null) throw new NotImplementedException("This request does not exist.");

            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(typeRequest, typeResponse);
            var wrapper = (RequestHandlerBase)Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for type {jobContext.TRequest}");

            var rst = await wrapper.Handle(command, jobContext.BroadCast, _serviceProvider, cancellationToken);
            return JsonConvert.SerializeObject(rst);
        }
    }
}
