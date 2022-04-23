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
    public class AuthUserController : Controller
    {
        IFreeSql fsql;
        public AuthUserController(IFreeSql orm) {
            fsql = orm;
        }

        [HttpGet]
        async public Task<ActionResult> List([FromQuery] string key, [FromQuery] int[] Person_Id, [FromQuery] int[] mn_Roles_Id, [FromQuery] int limit = 20, [FromQuery] int page = 1)
        {
            var select = fsql.Select<AuthUser>().Include(a => a.Person)
                .WhereIf(!string.IsNullOrEmpty(key), a => a.Username.Contains(key) || a.Password.Contains(key) || a.LoginIp.Contains(key) || a.Person.FullName.Contains(key) || a.Person.NickName.Contains(key) || a.Person.IdCard.Contains(key) || a.Person.IdCardAddress.Contains(key) || a.Person.Mobile.Contains(key) || a.Person.Email.Contains(key) || a.Person.FamilyAddress.Contains(key))
                .WhereIf(Person_Id?.Any() == true, a => Person_Id.Contains(a.PersonId))
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
            var item = await fsql.Select<AuthUser>().IncludeMany(a => a.Roles).Where(a => a.Id == Id).FirstAsync();
            if (item == null) return ApiResult.Failed.SetMessage("记录不存在");
            ViewBag.item = item;
            return View();
        }

        /***************************************** POST *****************************************/

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Add([FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int PersonId, [FromForm] string Username, [FromForm] string Password, [FromForm] DateTime LoginTime, [FromForm] string LoginIp, [FromForm] AuthUserStatus Status, [FromForm] bool IsAdmin, [FromForm] int[] mn_Roles_Id)
        {
            var item = new AuthUser();
            item.CreateTime = CreateTime;
            item.UpdateTime = UpdateTime;
            item.IsDeleted = IsDeleted;
            item.Sort = Sort;
            item.PersonId = PersonId;
            item.Username = Username;
            item.Password = Password;
            item.LoginTime = LoginTime;
            item.LoginIp = LoginIp;
            item.Status = Status;
            item.IsAdmin = IsAdmin;
            using (var ctx = fsql.CreateDbContext())
            {
                await ctx.AddAsync(item);
                //关联 AuthRole
                await ctx.SaveManyAsync(item, "Roles");
                await ctx.SaveChangesAsync();
            }
            return ApiResult<object>.Success.SetData(item);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Edit([FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int Id, [FromForm] int PersonId, [FromForm] string Username, [FromForm] string Password, [FromForm] DateTime LoginTime, [FromForm] string LoginIp, [FromForm] AuthUserStatus Status, [FromForm] bool IsAdmin, [FromForm] int[] mn_Roles_Id)
        {
            //var item = new AuthUser();
            //item.Id = Id;
            using (var ctx = fsql.CreateDbContext())
            {
                //ctx.Attach(item);
                var item = await ctx.Set<AuthUser>().Where(a => a.Id == Id).FirstAsync();
                item.CreateTime = CreateTime;
                item.UpdateTime = UpdateTime;
                item.IsDeleted = IsDeleted;
                item.Sort = Sort;
                item.PersonId = PersonId;
                item.Username = Username;
                item.Password = Password;
                item.LoginTime = LoginTime;
                item.LoginIp = LoginIp;
                item.Status = Status;
                item.IsAdmin = IsAdmin;
                await ctx.UpdateAsync(item);
                //关联 AuthRole
                await ctx.SaveManyAsync(item, "Roles");
                var affrows = await ctx.SaveChangesAsync();
                if (affrows > 0) return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
            }
            return ApiResult.Failed;
        }

        [HttpPost("del")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Del([FromForm] int[] Id)
        {
            var items = Id?.Select((a, idx) => new AuthUser { Id = Id[idx] });
            var affrows = await fsql.Delete<AuthUser>().WhereDynamic(items).ExecuteAffrowsAsync();
            return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
        }
    }
}