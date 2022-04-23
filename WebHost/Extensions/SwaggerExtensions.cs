using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;

namespace Swashbuckle.AspNetCore.Swagger
{
    //public class FormDataOperationFilter : IOperationFilter
    //{
    //	public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //	{
    //		if (context.ApiDescription.TryGetMethodInfo(out var method) == false) return;
    //		var actattrs = method.GetCustomAttributes(false);
    //		if (actattrs.OfType<HttpPostAttribute>().Any() ||
    //			actattrs.OfType<HttpPutAttribute>().Any())
    //			if (operation.Consumes.Count == 0)
    //				operation..Add("application/x-www-form-urlencoded");
    //	}
    //}

    public class EnumSchemaFilter : ISchemaFilter
	{
		public void Apply(OpenApiSchema model, SchemaFilterContext context)
		{
			if (context.Type.IsEnum)
			{
				model.Enum.Clear();
				Enum.GetNames(context.Type)
					.ToList()
					.ForEach(n => model.Enum.Add(new OpenApiString(n)));
			}
		}
	}
	public static class SwashbuckleSwaggerExtensions
	{
		public static IServiceCollection AddCustomizedSwaggerGen(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				foreach (var doc in _docs) options.SwaggerDoc(doc, new OpenApiInfo { Version = doc });
				options.DocInclusionPredicate((docName, apiDesc) =>
				{
					if (apiDesc.TryGetMethodInfo(out var method) == false) return false;
					var versions = method.DeclaringType.GetCustomAttributes(false)
						.OfType<ApiExplorerSettingsAttribute>()
						.Select(attr => attr.GroupName);
					if (docName == "未分类" && versions.Count() == 0) return true;
					return versions.Any(v => v == docName);
				});
				options.IgnoreObsoleteActions();
				//options.IgnoreObsoleteControllers(); // 类、方法标记 [Obsolete]，可以阻止【Swagger文档】生成
				options.EnableAnnotations();
				options.SchemaFilter<EnumSchemaFilter>();
				options.CustomSchemaIds(a => a.FullName);
				//options.OperationFilter<FormDataOperationFilter>();

				string root = Path.Combine(Directory.GetCurrentDirectory(), "Module");
				string xmlFile = string.Empty;
				string[] dirs = Directory.GetDirectories(root);
				foreach (var d in dirs)
				{
					xmlFile = Path.Combine(d, $"{new DirectoryInfo(d).Name}.xml");
					if (File.Exists(xmlFile))
						options.IncludeXmlComments(xmlFile); // 使用前需开启项目注释 xmldoc
				}
				var InfrastructureXml = Directory.GetFiles(Directory.GetCurrentDirectory(), "Infrastructure.xml", SearchOption.AllDirectories);
				if (InfrastructureXml.Any())
					options.IncludeXmlComments(InfrastructureXml[0]);
			});
			return services;
		}
		static readonly string[] _docs = new[] { "未分类", "公共", "cms", "后台管理" };
		public static IApplicationBuilder UseCustomizedSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
		{
			return app.UseSwagger().UseSwaggerUI(options =>
			{
				foreach (var doc in _docs) options.SwaggerEndpoint($"/swagger/{doc}/swagger.json", doc);
			});
		}
	}
}