using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <exception cref="ArgumentException"></exception>
        public static string GetAuthToken(this HttpRequest httpRequest, string tokenKey = "Authorization")
        {
            if (httpRequest == null)
            {
                throw new ArgumentException();
            }

            return httpRequest.Headers[tokenKey].FirstOrDefault();
        }
    }
}
