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
    public class AdmRouteController : Controller
    {
        IFreeSql fsql;
        public AdmRouteController(IFreeSql orm) {
            fsql = orm;
        }

        [HttpGet]
        async public Task<ActionResult> List([FromQuery] string key, [FromQuery] int[] Parent_Id, [FromQuery] int[] mn_Roles_Id, [FromQuery] int limit = 20, [FromQuery] int page = 1)
        {
            var select = fsql.Select<AdmRoute>().Include(a => a.Parent)
                .WhereIf(!string.IsNullOrEmpty(key), a => a.Name.Contains(key) || a.Extdata.Contains(key) || a.Remark.Contains(key) || a.TenantId.Contains(key) || a.Parent.Name.Contains(key) || a.Parent.Extdata.Contains(key) || a.Parent.Remark.Contains(key) || a.Parent.TenantId.Contains(key))
                .WhereIf(Parent_Id?.Any() == true, a => Parent_Id.Contains(a.ParentId))
                .WhereIf(mn_Roles_Id?.Any() == true, a => a.Roles.AsSelect().Any(b => mn_Roles_Id.Contains(b.Id)));
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
            var item = await fsql.Select<AdmRoute>().IncludeMany(a => a.Roles).Where(a => a.Id == Id).FirstAsync();
            if (item == null) return ApiResult.Failed.SetMessage("记录不存在");
            ViewBag.item = item;
            return View();
        }

        /***************************************** POST *****************************************/

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Add([FromForm] int ParentId, [FromForm] string Name, [FromForm] string Extdata, [FromForm] string Remark, [FromForm] string TenantId, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int[] mn_Roles_Id)
        {
            var item = new AdmRoute();
            item.ParentId = ParentId;
            item.Name = Name;
            item.Extdata = Extdata;
            item.Remark = Remark;
            item.TenantId = TenantId;
            item.CreateTime = CreateTime;
            item.UpdateTime = UpdateTime;
            item.IsDeleted = IsDeleted;
            item.Sort = Sort;
            using (var ctx = fsql.CreateDbContext())
            {
                await ctx.AddAsync(item);
                //关联 AuthRole
                var mn_Roles = mn_Roles_Id.Select((mn, idx) => new AuthRole.AuthRoleAdmRoute { RoleId = mn, AdmRouteId = item.Id }).ToArray();
                await ctx.AddRangeAsync(mn_Roles);
                await ctx.SaveChangesAsync();
            }
            return ApiResult<object>.Success.SetData(item);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Edit([FromForm] int ParentId, [FromForm] string Name, [FromForm] string Extdata, [FromForm] string Remark, [FromForm] string TenantId, [FromForm] int Id, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int[] mn_Roles_Id)
        {
            var item = new AdmRoute();
            item.Id = Id;
            using (var ctx = fsql.CreateDbContext())
            {
                ctx.Attach(item);
                item.ParentId = ParentId;
                item.Name = Name;
                item.Extdata = Extdata;
                item.Remark = Remark;
                item.TenantId = TenantId;
                item.CreateTime = CreateTime;
                item.UpdateTime = UpdateTime;
                item.IsDeleted = IsDeleted;
                item.Sort = Sort;
                await ctx.UpdateAsync(item);
                //关联 AuthRole
                if (mn_Roles_Id != null)
                {
                    var mn_Roles_Id_list = mn_Roles_Id.ToList();
                    var oldlist = ctx.Set<AuthRole.AuthRoleAdmRoute>().Where(a => a.AdmRouteId == item.Id).ToList();
                    foreach (var olditem in oldlist)
                    {
                        var idx = mn_Roles_Id_list.FindIndex(a => a == olditem.RoleId);
                        if (idx == -1) ctx.Remove(olditem);
                        else mn_Roles_Id_list.RemoveAt(idx);
                    }
                    var mn_Roles = mn_Roles_Id_list.Select((mn, idx) => new AuthRole.AuthRoleAdmRoute { RoleId = mn, AdmRouteId = item.Id }).ToArray();
                    await ctx.AddRangeAsync(mn_Roles);
                }
                var affrows = await ctx.SaveChangesAsync();
                if (affrows > 0) return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
            }
            return ApiResult.Failed;
        }

        [HttpPost("del")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Del([FromForm] int[] Id)
        {
            var items = Id?.Select((a, idx) => new AdmRoute { Id = Id[idx] });
            var affrows = await fsql.Delete<AdmRoute>().WhereDynamic(items).ExecuteAffrowsAsync();
            return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
        }
    }
}