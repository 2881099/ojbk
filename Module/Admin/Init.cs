using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ojbk.Entities;

namespace Module.Admin
{
    public class Init : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime lifetime)
        {

        }

        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
        {

        }
    }
}
