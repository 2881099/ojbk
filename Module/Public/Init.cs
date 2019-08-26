using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Module.Public
{
    public class Init : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {

        }

        public void ConfigureServices(IServiceCollection services, IHostingEnvironment env)
        {
            var blog = new Blog();
            blog.Author = "freesql";
            blog.Save();

            var get = Blog.Find(blog.Id);


            var repo = BaseEntity.Orm.GetRepository<Blog, int>();
            var blog2 = new Blog();
            blog2.Author = "freesql";
            repo.InsertOrUpdate(blog2);

            var get2 = repo.Get(blog2.Id);
            
        }


        class Blog : BaseEntity<Blog, int>
        {
            /// <summary>
            /// 作者
            /// </summary>
            public string Author { get; set; }
        }
    }
}
