﻿#### appsettings.json
```json
  "ServiceRegisterConfig": {
    "Host": "http://10.222.12.28:8500/", // Consul地址
    "ServiceName": "zlw-report",
    "AddressBlackList":["127.0.0.1"] //服务地址黑名单，会过滤掉黑名单中的地址
    "AddressWhiteList":["127.0.0.1"] //服务地址白名单，会过滤掉非白名单的地址
  },
```

#### 注入服务
```csharp
public void ConfigureServices(IServiceCollection services){
	services.AddDistributedRequest(Configuration.GetSection("ServiceRegisterConfig"), typeof(Program).Assembly);
}
```

#### 映射路由
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapDistributedRequest();
    });
}
```

#### 任务定义
```csharp
using System.Threading;
using System.Threading.Tasks;
using DistributedRequest.AspNetCore.Interfaces;
using DistributedRequest.AspNetCore.Models;

public class DemoJobHandler : IJobRequestHandler<TRequest,TResponse>
{
    public DemoJobHandler()
    {
    }

    public Task<TResponse> Execute(TRequest request, BroadCastModel broadCast, CancellationToken cancellationToken)
    {
        throw new System.Exception();
    }
}
```

---

#### 程序内调用示例
```csharp
using DistributedRequest.AspNetCore.Models;

public class DemoController
{
    private readonly IDistributedRequest _distributedRequest;
    public DemoController(IDistributedRequest _distributedRequest) {
        this._distributedRequest = _distributedRequest;
    }

    public async void Test(){
        var rst = await _distributedRequest.PostJsonAsync(new RequestContext()));
    }
}
```