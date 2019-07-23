using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Module.cms
{

    /// <summary>
    /// 配置本 Module 依赖注入等，由 WebHost/Startup.cs 加载触发执行
    /// </summary>
    public class Init : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {

        }

        public void ConfigureServices(IServiceCollection services, IHostingEnvironment env)
        {

        }
    }
}
