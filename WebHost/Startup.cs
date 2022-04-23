using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WebHost
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var orm = new FreeSqlBuilder()
                .UseAutoSyncStructure(true)
                .UseNoneCommandParameter(true)
                .UseConnectionString(DataType.Sqlite, "data source=test.db;max pool size=5")
                //.UseConnectionString(FreeSql.DataType.MySql, "Data Source=127.0.0.1;Port=3306;User ID=root;Password=root;Initial Catalog=cccddd;Charset=utf8;SslMode=none;Max pool size=2")
                //.UseConnectionString(FreeSql.DataType.PostgreSQL, "Host=192.168.164.10;Port=5432;Username=postgres;Password=123456;Database=tedb;Pooling=true;Maximum Pool Size=2")
                //.UseConnectionString(FreeSql.DataType.SqlServer, "Data Source=.;Integrated Security=True;Initial Catalog=freesqlTest;Pooling=true;Max Pool Size=2")
                //.UseConnectionString(FreeSql.DataType.Oracle, "user id=user1;password=123456;data source=//127.0.0.1:1521/XE;Pooling=true;Max Pool Size=2")
                .Build();
            orm.Aop.CurdBefore += (s, e) =>
            {
                Trace.WriteLine("[" + BaseController.CurrentControllerContext.Value?.ActionDescriptor.ControllerName + "/" +
                    BaseController.CurrentControllerContext.Value?.ActionDescriptor.ActionName + "]: " + e.Sql + "\r\n");
            };
            BaseEntity.Initialization(orm, null);

            var builder = new ConfigurationBuilder()
                .LoadInstalledModules(Modules, env)
                .AddCustomizedJsonFile(Modules, env, "/var/webconfig/xxx/");

            this.Configuration = builder.AddEnvironmentVariables().Build();
            this.Env = env;

            Newtonsoft.Json.JsonConvert.DefaultSettings = () =>
            {
                var st = new Newtonsoft.Json.JsonSerializerSettings();
                st.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                st.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                st.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
                return st;
            };
            //去掉以下注释可开启 RedisHelper 静态类
            //var csredis = new CSRedis.CSRedisClient(Configuration["ConnectionStrings:redis1"]); //单redis节点模式
            //RedisHelper.Initialization(csredis);
        }

        public static List<ModuleInfo> Modules = new List<ModuleInfo>();
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //下面这行代码依赖redis-server，注释后系统将以memory作为缓存存储的介质
            //services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));
            services.AddSingleton<IFreeSql>(BaseEntity.Orm);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IWebHostEnvironment>(Env);
            services.AddScoped<CustomExceptionFilter>();

            services.AddSession(a =>
            {
                a.IdleTimeout = TimeSpan.FromMinutes(30);
                a.Cookie.Name = "Session_bbs";
            });
            services.AddCors(options => options.AddPolicy("cors_all", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            services.AddCustomizedMvc(Modules);
            Modules.ForEach(module => module.Initializer?.ConfigureServices(services, Env));

            if (Env.IsDevelopment())
                services.AddCustomizedSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.GetEncoding("GB2312");
            Console.InputEncoding = Encoding.GetEncoding("GB2312");

            NLog.LogManager.LoadConfiguration("nlog.config");

            if (Env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSession();
            app.UseRouting();
            app.UseCors("cors_all");
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCustomizedStaticFiles(Modules);
            Modules.ForEach(module => module.Initializer?.Configure(app, env, loggerFactory, lifetime));

            if (env.IsDevelopment())
                app.UseCustomizedSwagger(env);
        }
    }
}
