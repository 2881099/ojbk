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
    public class AuthRoleController : Controller
    {
        IFreeSql fsql;
        public AuthRoleController(IFreeSql orm) {
            fsql = orm;
        }

        [HttpGet]
        async public Task<ActionResult> List([FromQuery] string key, [FromQuery] int[] mn_AdmRoutes_Id, [FromQuery] int[] mn_Users_Id, [FromQuery] int[] mn_OrgPosts_Id, [FromQuery] int limit = 20, [FromQuery] int page = 1)
        {
            var select = fsql.Select<AuthRole>()
                .WhereIf(!string.IsNullOrEmpty(key), a => a.Name.Contains(key) || a.Remark.Contains(key) || a.TenantId.Contains(key))
                .WhereIf(mn_AdmRoutes_Id?.Any() == true, a => a.AdmRoutes.AsSelect().Any(b => mn_AdmRoutes_Id.Contains(b.Id)))
                .WhereIf(mn_Users_Id?.Any() == true, a => a.Users.AsSelect().Any(b => mn_Users_Id.Contains(b.Id)))
                .WhereIf(mn_OrgPosts_Id?.Any() == true, a => a.OrgPosts.AsSelect().Any(b => mn_OrgPosts_Id.Contains(b.Id)));
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
            var item = await fsql.Select<AuthRole>().IncludeMany(a => a.AdmRoutes).IncludeMany(a => a.Users).IncludeMany(a => a.OrgPosts).Where(a => a.Id == Id).FirstAsync();
            if (item == null) return ApiResult.Failed.SetMessage("记录不存在");
            ViewBag.item = item;
            return View();
        }

        /***************************************** POST *****************************************/

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Add([FromForm] string Name, [FromForm] string Remark, [FromForm] string TenantId, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int[] mn_AdmRoutes_Id, [FromForm] int[] mn_Users_Id, [FromForm] int[] mn_OrgPosts_Id)
        {
            var item = new AuthRole();
            item.Name = Name;
            item.Remark = Remark;
            item.TenantId = TenantId;
            item.CreateTime = CreateTime;
            item.UpdateTime = UpdateTime;
            item.IsDeleted = IsDeleted;
            item.Sort = Sort;
            using (var ctx = fsql.CreateDbContext())
            {
                await ctx.AddAsync(item);
                //关联 AdmRoute
                var mn_AdmRoutes = mn_AdmRoutes_Id.Select((mn, idx) => new AuthRole.AuthRoleAdmRoute { AdmRouteId = mn, RoleId = item.Id }).ToArray();
                await ctx.AddRangeAsync(mn_AdmRoutes);
                //关联 AuthUser
                var mn_Users = mn_Users_Id.Select((mn, idx) => new AuthRole.AuthRoleUser { UserId = mn, RoleId = item.Id }).ToArray();
                await ctx.AddRangeAsync(mn_Users);
                //关联 OrgPost
                var mn_OrgPosts = mn_OrgPosts_Id.Select((mn, idx) => new AuthRole.AuthRolePost { OrgPostId = mn, RoleId = item.Id }).ToArray();
                await ctx.AddRangeAsync(mn_OrgPosts);
                await ctx.SaveChangesAsync();
            }
            return ApiResult<object>.Success.SetData(item);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Edit([FromForm] string Name, [FromForm] string Remark, [FromForm] string TenantId, [FromForm] int Id, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int[] mn_AdmRoutes_Id, [FromForm] int[] mn_Users_Id, [FromForm] int[] mn_OrgPosts_Id)
        {
            var item = new AuthRole();
            item.Id = Id;
            using (var ctx = fsql.CreateDbContext())
            {
                ctx.Attach(item);
                item.Name = Name;
                item.Remark = Remark;
                item.TenantId = TenantId;
                item.CreateTime = CreateTime;
                item.UpdateTime = UpdateTime;
                item.IsDeleted = IsDeleted;
                item.Sort = Sort;
                await ctx.UpdateAsync(item);
                //关联 AdmRoute
                if (mn_AdmRoutes_Id != null)
                {
                    var mn_AdmRoutes_Id_list = mn_AdmRoutes_Id.ToList();
                    var oldlist = ctx.Set<AuthRole.AuthRoleAdmRoute>().Where(a => a.RoleId == item.Id).ToList();
                    foreach (var olditem in oldlist)
                    {
                        var idx = mn_AdmRoutes_Id_list.FindIndex(a => a == olditem.AdmRouteId);
                        if (idx == -1) ctx.Remove(olditem);
                        else mn_AdmRoutes_Id_list.RemoveAt(idx);
                    }
                    var mn_AdmRoutes = mn_AdmRoutes_Id_list.Select((mn, idx) => new AuthRole.AuthRoleAdmRoute { AdmRouteId = mn, RoleId = item.Id }).ToArray();
                    await ctx.AddRangeAsync(mn_AdmRoutes);
                }
                //关联 AuthUser
                if (mn_Users_Id != null)
                {
                    var mn_Users_Id_list = mn_Users_Id.ToList();
                    var oldlist = ctx.Set<AuthRole.AuthRoleUser>().Where(a => a.RoleId == item.Id).ToList();
                    foreach (var olditem in oldlist)
                    {
                        var idx = mn_Users_Id_list.FindIndex(a => a == olditem.UserId);
                        if (idx == -1) ctx.Remove(olditem);
                        else mn_Users_Id_list.RemoveAt(idx);
                    }
                    var mn_Users = mn_Users_Id_list.Select((mn, idx) => new AuthRole.AuthRoleUser { UserId = mn, RoleId = item.Id }).ToArray();
                    await ctx.AddRangeAsync(mn_Users);
                }
                //关联 OrgPost
                if (mn_OrgPosts_Id != null)
                {
                    var mn_OrgPosts_Id_list = mn_OrgPosts_Id.ToList();
                    var oldlist = ctx.Set<AuthRole.AuthRolePost>().Where(a => a.RoleId == item.Id).ToList();
                    foreach (var olditem in oldlist)
                    {
                        var idx = mn_OrgPosts_Id_list.FindIndex(a => a == olditem.OrgPostId);
                        if (idx == -1) ctx.Remove(olditem);
                        else mn_OrgPosts_Id_list.RemoveAt(idx);
                    }
                    var mn_OrgPosts = mn_OrgPosts_Id_list.Select((mn, idx) => new AuthRole.AuthRolePost { OrgPostId = mn, RoleId = item.Id }).ToArray();
                    await ctx.AddRangeAsync(mn_OrgPosts);
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
            var items = Id?.Select((a, idx) => new AuthRole { Id = Id[idx] });
            var affrows = await fsql.Delete<AuthRole>().WhereDynamic(items).ExecuteAffrowsAsync();
            return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
        }
    }
}