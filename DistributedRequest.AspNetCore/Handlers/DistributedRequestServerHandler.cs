using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;

namespace DistributedRequest.AspNetCore.Handlers
{
    internal class DistributedRequestServerHandler
    {
        private readonly IDistributedRequest _requestProvider;

        public DistributedRequestServerHandler(IDistributedRequest _requestProvider)
        {
            this._requestProvider = _requestProvider;
        }

        public async Task<List<ReturnT>> HandlerAsync(RequestContext param, CancellationToken cancellationToken)
        {
            return await _requestProvider.PostJsonAsync(param, cancellationToken);
        }
    }
}
