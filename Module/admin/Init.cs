using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Module.admin
{
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
