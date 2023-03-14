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

namespace DistributedRequest.AspNetCore.Extensions
{
    public static class DistributedRequestExtensions
    {
        /// <summary>
        /// 添加分布式请求
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDistributedRequest(this IServiceCollection services, IConfigurationSection configuration, params Assembly[] assemblies)
        {
            services.AddHttpClient();
            services.Configure<DistributedRequestOption>(configuration);
            services.AddSingleton<DistributedRequestServerHandler>();
            services.AddSingleton<DistributedRequestClientHandler>();
            services.AddScoped<IDistributedRequest, DistributedRequestProvider>();

            MutipleInjectService(services, assemblies);
        }

        /// <summary>
        /// 批量注入服务
        /// </summary>
        private static void MutipleInjectService(IServiceCollection services, Assembly[] assemblies)
        {
            //var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var allTypes = assemblies?.SelectMany(s => s.GetTypes()).ToList();
            var types = new GlobalTypeList();
            //批量注入Scoped
            allTypes.Where(x => x.IsClass && !x.IsAbstract && typeof(IJobBaseHandler).IsAssignableFrom(x)).ToList().ForEach(type =>
            {
                services.AddScoped(type);
                types.AddTypes(type.Name, type);
            });
            services.AddSingleton(types);
        }

        /// <summary>
        /// 映射分布式请求路由
        /// </summary>
        /// <param name="endpoints"></param>
        public static void MapDistributedRequest(this IEndpointRouteBuilder endpoints)
        {
            var basePath = endpoints.ServiceProvider.GetRequiredService<IOptions<DistributedRequestOption>>().Value.BasePath ?? "dr";
            endpoints.MapPost($"{basePath}-server", async context =>
            {
                var requestContext = JsonSerializer.DeserializeAsync<RequestContext>(context.Request.Body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }, context.RequestAborted).Result;
                var mediator = context.RequestServices.GetRequiredService<DistributedRequestServerHandler>();
                var rst = await mediator.HandlerAsync(requestContext, context.RequestAborted);
                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(rst), context.RequestAborted);
            }).WithDisplayName("DR-Server");

            endpoints.MapPost($"{basePath}-client", async context =>
            {
                var jobContext = JsonSerializer.DeserializeAsync<JobContext>(context.Request.Body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }, context.RequestAborted).Result;
                var mediator = context.RequestServices.GetRequiredService<DistributedRequestClientHandler>();
                var rst = await mediator.HandlerAsync(jobContext, context.RequestAborted);
                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(rst), context.RequestAborted);
            }).WithDisplayName("DR-Client");
        }
    }
}
