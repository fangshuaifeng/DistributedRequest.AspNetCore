using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Consul;
using Newtonsoft.Json;
using DistributedRequest.AspNetCore.Models;
using System.Linq;
using DistributedRequest.AspNetCore.Extensions;
using DistributedRequest.AspNetCore.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace DistributedRequest.AspNetCore.Providers
{
    /// <summary>
    /// 分布式请求提供器
    /// </summary>
    internal class DistributedRequestProvider : IDistributedRequest
    {
        private readonly DistributedRequestOption _option;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServer _server;

        public DistributedRequestProvider(
            IOptions<DistributedRequestOption> namedOptionsAccessor
            , IHttpContextAccessor _httpContextAccessor
            , IHttpClientFactory _httpClientFactory
            , IServer _server
            )
        {
            this._server = _server;
            this._option = namedOptionsAccessor.Value;
            this._httpContextAccessor = _httpContextAccessor;
            this._httpClientFactory = _httpClientFactory;
        }

        #region 内部方法

        /// <summary>
        /// 获取当前服务所有服务器地址
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetServiceUrls(string serviceName)
        {
            using ConsulClient consulClient = new ConsulClient(c => { c.Address = new System.Uri(_option.Host); });
            var rst = await consulClient.Agent.Services();
            return rst?.Response.Where(w => w.Value.Service == serviceName && w.Value.Address != "127.0.0.1" && w.Value.Address != "localhost")
                                .Select(s => $"http://{s.Value.Address}:{s.Value.Port}").ToList() ?? new List<string>();
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        private AuthenticationHeaderValue GetCurrentToken()
        {
            AuthenticationHeaderValue.TryParse(_httpContextAccessor.HttpContext.Request.GetAuthToken(), out var _authentication);
            return _authentication;
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="reqHeaderSet"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<TResponse> SendAsync<TResponse>(string url, object param, CancellationToken cancellationToken, Action<HttpRequestHeaders> reqHeaderSet = null)
        {
            var httpClient = _httpClientFactory.CreateClient();
            StringContent param2 = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequestMessage.Content = param2;
            reqHeaderSet?.Invoke(httpRequestMessage.Headers);

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"this request failed. url is {httpResponseMessage.RequestMessage.RequestUri} , statusCode is {httpResponseMessage.StatusCode}");
            }

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        #endregion

        public async Task<List<TResopnse>> PostJsonAsync<TResopnse>(IJobRequest<TResopnse> request, CancellationToken cancellationToken = default, int? partitionCount = null, bool onlyLocal = false) where TResopnse : class
        {
            var token = GetCurrentToken();
            var ips = new List<string>();
            if (!onlyLocal)
            {
                ips.AddRange(await GetServiceUrls(_option.ServiceName));
            }
            else
            {
                var localAddress = _server.Features.Get<IServerAddressesFeature>().Addresses.First();
                ips.Add(localAddress);
            }

            if (partitionCount.HasValue && partitionCount < ips.Count && partitionCount > 0)
            {
                // 随机取
                ips = ips.OrderBy(o => Guid.NewGuid()).Take(partitionCount.Value).ToList();
            }
            if (ips.Count == 0) return new List<TResopnse>();

            var count = ips.Count;
            var strParams = request == null ? string.Empty : JsonConvert.SerializeObject(request);
            var tasks = ips.Select((server_address, idx) => SendAsync<TResopnse>($"{server_address}/{_option.BasePath}"
                                                                                , new InnerContext
                                                                                {
                                                                                    Parameter = strParams,
                                                                                    BroadCast = new BroadCastModel(idx, count),
                                                                                    TRequest = request.GetType().FullName,
                                                                                    TResponse = typeof(TResopnse).FullName
                                                                                }
                                                                                , cancellationToken
                                                                                , e => { e.Authorization = token; }));

            var reponses = await Task.WhenAll(tasks);
            return reponses.ToList();
        }
    }
}
