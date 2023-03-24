using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DistributedRequest.AspNetCore.Handlers;
using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;
using DistributedRequest.AspNetCore.Providers;
using System.Data;
using System.Collections.Generic;
using System;

namespace DistributedRequest.AspNetCore.Extensions
{
    public static class DistributedRequestExtensions
    {
        /// <summary>
        /// 添加分布式请求
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="assemblies"></param>
        public static void AddDistributedRequest(this IServiceCollection services, IConfigurationSection configuration, params Assembly[] assemblies)
        {
            services.AddHttpClient();
            services.Configure<DistributedRequestOption>(configuration);
            services.AddSingleton<IClientHandler, DistributedRequestClientHandler>();
            services.AddSingleton<IDistributedRequest, DistributedRequestProvider>();

            MutipleInjectService(services, assemblies);
        }

        private static List<TypeInfo> GetCustomeTypes(List<TypeInfo> allTypes, Type openGenericType)
        {
            return (from x in allTypes
                    from z in x.GetInterfaces()
                    let y = x.BaseType
                    where (y != null && y.IsGenericType && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition()))
                          || (z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                    select x).ToList();
        }

        /// <summary>
        /// 批量注入服务
        /// </summary>
        private static void MutipleInjectService(IServiceCollection services, Assembly[] assemblies)
        {
            var allTypes = assemblies?.SelectMany(s => s.DefinedTypes).Where(x => x.IsClass && !x.IsAbstract).ToList();
            var types = new GlobalTypeList();

            ////批量注入任务
            foreach (var type in GetCustomeTypes(allTypes, typeof(IJobRequestHandler<,>)))
            {
                foreach (var item in type.ImplementedInterfaces)
                {
                    services.AddScoped(item, type);
                }
                //types.AddTypes(Enums.TypeEnum.Job, type);
            }
            // 批量注入参和出参类
            foreach (var type in GetCustomeTypes(allTypes, typeof(IJobRequest<>)))
            {
                var defTypes = type.GetInterfaces().Where(w => w.GetGenericTypeDefinition() == typeof(IJobRequest<>)).SelectMany(i => i.GetGenericArguments()).ToArray();
                types.AddTypes(Enums.TypeEnum.Response, defTypes);
                types.AddTypes(Enums.TypeEnum.Request, type);
            }

            services.AddSingleton(types);
        }

        /// <summary>
        /// 映射分布式请求路由
        /// </summary>
        /// <param name="endpoints"></param>
        public static void MapDistributedRequest(this IEndpointRouteBuilder endpoints)
        {
            var basePath = endpoints.ServiceProvider.GetRequiredService<IOptions<DistributedRequestOption>>().Value.BasePath ?? "dr-client";
            endpoints.MapPost($"{basePath}", async context =>
            {
                var jobContext = JsonSerializer.DeserializeAsync<InnerContext>(context.Request.Body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }, context.RequestAborted).Result;
                var mediator = context.RequestServices.GetRequiredService<IClientHandler>();
                var rst = await mediator.HandlerAsync(jobContext, context.RequestAborted);
                await context.Response.WriteAsync(rst, context.RequestAborted);
            }).WithDisplayName("DR-Client");
        }
    }
}
