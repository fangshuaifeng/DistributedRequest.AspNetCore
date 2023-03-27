using Consul;
using DistributedRequest.AspNetCore.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedRequest.AspNetCore.Interfaces.Services
{
    internal class ConsulServiceDiscovery : IServiceDiscovery
    {
        private readonly DistributedRequestOption _option;
        private readonly List<string> skipAddresses = new() { "127.0.0.1", "localhost" };

        public ConsulServiceDiscovery(IOptions<DistributedRequestOption> namedOptionsAccessor)
        {
            this._option = namedOptionsAccessor.Value;
        }

        public async Task<List<string>> GetServiceUrls(string serviceName)
        {
            serviceName ??= _option.ServiceName;
            using ConsulClient consulClient = new ConsulClient(c => { c.Address = new Uri(_option.Host); });
            var rst = await consulClient.Agent.Services();
            var list = rst?.Response.Where(w => w.Value.Service == serviceName && !skipAddresses.Contains(w.Value.Address));
            if (list != null)
            {
                // 白名单
                if (_option.AddressWhiteList != null && _option.AddressWhiteList.Count > 0)
                {
                    list = list.Where(w => _option.AddressWhiteList.Contains(w.Value.Address)).ToList();
                }
                // 黑名单
                if (_option.AddressBlackList != null && _option.AddressBlackList.Count > 0)
                {
                    list = list.Where(w => !_option.AddressWhiteList.Contains(w.Value.Address)).ToList();
                }
            }

            return list?.Select(s => $"http://{s.Value.Address}:{s.Value.Port}").ToList() ?? new List<string>();
        }
    }
}
