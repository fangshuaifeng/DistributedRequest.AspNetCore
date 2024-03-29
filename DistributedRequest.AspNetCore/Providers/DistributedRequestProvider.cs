﻿using DistributedRequest.AspNetCore.Extensions;
using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IClientHandler _clientHandler;
        private readonly IServiceDiscovery _serviceDiscovery;

        public DistributedRequestProvider(
            IOptions<DistributedRequestOption> namedOptionsAccessor
            , IHttpContextAccessor _httpContextAccessor
            , IHttpClientFactory _httpClientFactory
            , IClientHandler _clientHandler
            , IServiceDiscovery _serviceDiscovery
            )
        {
            this._clientHandler = _clientHandler;
            this._option = namedOptionsAccessor.Value;
            this._httpContextAccessor = _httpContextAccessor;
            this._httpClientFactory = _httpClientFactory;
            this._serviceDiscovery = _serviceDiscovery;
        }

        #region 内部方法

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
            var strParams = request == null ? string.Empty : JsonConvert.SerializeObject(request);
            var ips = new List<string>();
            if (onlyLocal || partitionCount == 1 || (ips = await _serviceDiscovery.GetServiceUrls()).Count == 0)
            {
                var oneRst = await _clientHandler.HandlerAsync<TResopnse>(new InnerContext
                {
                    Parameter = strParams,
                    BroadCast = new BroadCastModel(0, 1),
                    TRequest = request.GetType().FullName,
                    TResponse = typeof(TResopnse).FullName
                }, cancellationToken);
                return new List<TResopnse> { oneRst };
            }

            if (partitionCount.HasValue && partitionCount < ips.Count && partitionCount > 0)
            {
                ips = ips.OrderBy(o => Guid.NewGuid()).Take(partitionCount.Value).ToList(); // 随机策略
            }

            var count = ips.Count;
            var token = _httpContextAccessor.HttpContext?.Request.GetAuthToken();
            var cookie = _httpContextAccessor.HttpContext?.Request.GetCookie();

            var tasks = ips.Select((server_address, idx) => SendAsync<TResopnse>($"{server_address}/{_option.BasePath}"
                                                                                , new InnerContext
                                                                                {
                                                                                    Parameter = strParams,
                                                                                    BroadCast = new BroadCastModel(idx, count),
                                                                                    TRequest = request.GetType().FullName,
                                                                                    TResponse = typeof(TResopnse).FullName
                                                                                }
                                                                                , cancellationToken
                                                                                , e =>
                                                                                {
                                                                                    e.Authorization = token;
                                                                                    if (cookie != null) e.Add("Cookie", cookie);
                                                                                }));

            var reponses = await Task.WhenAll(tasks);
            return reponses.ToList();
        }
    }
}
