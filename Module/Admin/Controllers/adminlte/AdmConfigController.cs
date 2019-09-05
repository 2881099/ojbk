using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using FreeSql;
using ojbk.Entities;

namespace FreeSql.AdminLTE.Controllers
{
    [Route("/adminlte/[controller]")]
    public class AdmConfigController : Controller
    {
        IFreeSql fsql;
        public AdmConfigController(IFreeSql orm) {
            fsql = orm;
        }

        [HttpGet]
        async public Task<ActionResult> List([FromQuery] string key, [FromQuery] int limit = 20, [FromQuery] int page = 1)
        {
            var select = fsql.Select<AdmConfig>()
                .WhereIf(!string.IsNullOrEmpty(key), a => a.Value.Contains(key) || a.Remark.Contains(key) || a.Id.Contains(key));
            var items = await select.Count(out var count).Page(page, limit).ToListAsync();
            ViewBag.items = items;
            ViewBag.count = count;
            return View();
        }

        [HttpGet("add")]
        public ActionResult Edit() => View();

        [HttpGet("edit")]
        async public Task<ActionResult> Edit([FromQuery] string Id)
        {
            var item = await fsql.Select<AdmConfig>().Where(a => a.Id == Id).FirstAsync();
            if (item == null) return ApiResult.Failed.SetMessage("记录不存在");
            ViewBag.item = item;
            return View();
        }

        /***************************************** POST *****************************************/

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Add([FromForm] string Value, [FromForm] string Remark, [FromForm] string Id, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort)
        {
            var item = new AdmConfig();
            item.Value = Value;
            item.Remark = Remark;
            item.Id = Id;
            item.CreateTime = CreateTime;
            item.UpdateTime = UpdateTime;
            item.IsDeleted = IsDeleted;
            item.Sort = Sort;
            using (var ctx = fsql.CreateDbContext())
            {
                await ctx.AddAsync(item);
                await ctx.SaveChangesAsync();
            }
            return ApiResult<object>.Success.SetData(item);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Edit([FromForm] string Value, [FromForm] string Remark, [FromForm] string Id, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort)
        {
            var item = new AdmConfig();
            item.Id = Id;
            using (var ctx = fsql.CreateDbContext())
            {
                ctx.Attach(item);
                item.Value = Value;
                item.Remark = Remark;
                item.CreateTime = CreateTime;
                item.UpdateTime = UpdateTime;
                item.IsDeleted = IsDeleted;
                item.Sort = Sort;
                await ctx.UpdateAsync(item);
                var affrows = await ctx.SaveChangesAsync();
                if (affrows > 0) return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
            }
            return ApiResult.Failed;
        }

        [HttpPost("del")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Del([FromForm] string[] Id)
        {
            var items = Id?.Select((a, idx) => new AdmConfig { Id = Id[idx] });
            var affrows = await fsql.Delete<AdmConfig>().WhereDynamic(items).ExecuteAffrowsAsync();
            return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
        }
    }
}