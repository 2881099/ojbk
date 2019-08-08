using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ojbk.Entities;

namespace Module.Admin
{
    public class Init : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            var testsql = new AuthUser().SelectAdmRoute.ToSql();

            var testlist = new AuthUser().SelectAdmRoute.ToList();
        }

        public void ConfigureServices(IServiceCollection services, IHostingEnvironment env)
        {

        }
    }
}
