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
    [Route("/adminlte/[controller]"), ApiExplorerSettings(GroupName = "后台管理")]
    public class OrgPostController : Controller
    {
        IFreeSql fsql;
        public OrgPostController(IFreeSql orm) {
            fsql = orm;
        }

        [HttpGet]
        async public Task<ActionResult> List([FromQuery] string key, [FromQuery] int[] Department_Id, [FromQuery] int[] mn_Roles_Id, [FromQuery] int[] mn_Persons_Id, [FromQuery] int limit = 20, [FromQuery] int page = 1)
        {
            var select = fsql.Select<OrgPost>().Include(a => a.Department)
                .WhereIf(!string.IsNullOrEmpty(key), a => a.Title.Contains(key) || a.DutyContent.Contains(key) || a.JobContent.Contains(key) || a.Department.Name.Contains(key))
                .WhereIf(Department_Id?.Any() == true, a => Department_Id.Contains(a.DepartmentId))
                .WhereIf(mn_Roles_Id?.Any() == true, a => a.Roles.AsSelect().Any(b => mn_Roles_Id.Contains(b.Id)))
                .WhereIf(mn_Persons_Id?.Any() == true, a => a.Persons.AsSelect().Any(b => mn_Persons_Id.Contains(b.Id)));
            var items = await select.Count(out var count).Page(page, limit).ToListAsync();
            ViewBag.items = items;
            ViewBag.count = count;
            return View();
        }

        [HttpGet("add")]
        public ActionResult Edit() => View();

        [HttpGet("edit")]
        async public Task<ActionResult> Edit([FromQuery] int Id)
        {
            var item = await fsql.Select<OrgPost>().IncludeMany(a => a.Roles).IncludeMany(a => a.Persons).Where(a => a.Id == Id).FirstAsync();
            if (item == null) return ApiResult.Failed.SetMessage("记录不存在");
            ViewBag.item = item;
            return View();
        }

        /***************************************** POST *****************************************/

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Add([FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int DepartmentId, [FromForm] string Title, [FromForm] string DutyContent, [FromForm] string JobContent, [FromForm] int[] mn_Roles_Id, [FromForm] int[] mn_Persons_Id)
        {
            var item = new OrgPost();
            item.CreateTime = CreateTime;
            item.UpdateTime = UpdateTime;
            item.IsDeleted = IsDeleted;
            item.Sort = Sort;
            item.DepartmentId = DepartmentId;
            item.Title = Title;
            item.DutyContent = DutyContent;
            item.JobContent = JobContent;
            using (var ctx = fsql.CreateDbContext())
            {
                await ctx.AddAsync(item);
                //关联 AuthRole
                await ctx.SaveManyAsync(item, "Roles");
                //关联 OrgPerson
                await ctx.SaveManyAsync(item, "Persons");
                await ctx.SaveChangesAsync();
            }
            return ApiResult<object>.Success.SetData(item);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Edit([FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int Id, [FromForm] int DepartmentId, [FromForm] string Title, [FromForm] string DutyContent, [FromForm] string JobContent, [FromForm] int[] mn_Roles_Id, [FromForm] int[] mn_Persons_Id)
        {
            //var item = new OrgPost();
            //item.Id = Id;
            using (var ctx = fsql.CreateDbContext())
            {
                //ctx.Attach(item);
                var item = await ctx.Set<OrgPost>().Where(a => a.Id == Id).FirstAsync();
                item.CreateTime = CreateTime;
                item.UpdateTime = UpdateTime;
                item.IsDeleted = IsDeleted;
                item.Sort = Sort;
                item.DepartmentId = DepartmentId;
                item.Title = Title;
                item.DutyContent = DutyContent;
                item.JobContent = JobContent;
                await ctx.UpdateAsync(item);
                //关联 AuthRole
                await ctx.SaveManyAsync(item, "Roles");
                //关联 OrgPerson
                await ctx.SaveManyAsync(item, "Persons");
                var affrows = await ctx.SaveChangesAsync();
                if (affrows > 0) return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
            }
            return ApiResult.Failed;
        }

        [HttpPost("del")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Del([FromForm] int[] Id)
        {
            var items = Id?.Select((a, idx) => new OrgPost { Id = Id[idx] });
            var affrows = await fsql.Delete<OrgPost>().WhereDynamic(items).ExecuteAffrowsAsync();
            return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
        }
    }
}