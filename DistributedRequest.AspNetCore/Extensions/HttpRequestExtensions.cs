using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace DistributedRequest.AspNetCore.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// 从请求上下文header获取指定key的令牌
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        public static AuthenticationHeaderValue GetAuthToken(this HttpRequest httpRequest, string tokenKey = "Authorization")
        {
            if (httpRequest == null) return null;

            AuthenticationHeaderValue.TryParse(httpRequest.Headers[tokenKey].FirstOrDefault(), out var _authentication);
            return _authentication;
        }

        /// <summary>
        /// 从请求上下文header获取Cookie
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="cookieKey"></param>
        /// <returns></returns>
        public static string GetCookie(this HttpRequest httpRequest, string cookieKey = "Cookie")
        {
            if (httpRequest == null) return null;

            return httpRequest.Headers[cookieKey].FirstOrDefault();
        }
    }
}
