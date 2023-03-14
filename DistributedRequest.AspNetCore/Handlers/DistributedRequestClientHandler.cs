using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;

namespace DistributedRequest.AspNetCore.Handlers
{
    internal class DistributedRequestClientHandler
    {
        private readonly GlobalTypeList _globalTypeList;
        private readonly IServiceProvider _serviceProvider;
        public DistributedRequestClientHandler(GlobalTypeList _globalTypeList, IServiceProvider _serviceProvider)
        {
            this._globalTypeList = _globalTypeList;
            this._serviceProvider = _serviceProvider;
        }

        public async Task<ReturnT> HandlerAsync(JobContext jobContext, CancellationToken cancellationToken)
        {
            var requestContext = JsonConvert.DeserializeObject<RequestContext>(jobContext.Parameter ?? string.Empty);
            var currentType = _globalTypeList.GetType(requestContext?.ExecutorHandler ?? string.Empty);
            if (currentType == null) throw new NotImplementedException("This executorHandler does not exist.");

            IJobBaseHandler handler = (IJobBaseHandler)ActivatorUtilities.CreateInstance(_serviceProvider, currentType);
            return await handler.Execute(new JobContext(requestContext.ExecutorParams, jobContext.BroadCast), cancellationToken);
        }
    }
}
