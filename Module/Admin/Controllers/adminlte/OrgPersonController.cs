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
    public class OrgPersonController : Controller
    {
        IFreeSql fsql;
        public OrgPersonController(IFreeSql orm) {
            fsql = orm;
        }

        [HttpGet]
        async public Task<ActionResult> List([FromQuery] string key, [FromQuery] int[] mn_Posts_Id, [FromQuery] int limit = 20, [FromQuery] int page = 1)
        {
            var select = fsql.Select<OrgPerson>()
                .WhereIf(!string.IsNullOrEmpty(key), a => a.FullName.Contains(key) || a.NickName.Contains(key) || a.IdCard.Contains(key) || a.IdCardAddress.Contains(key) || a.Mobile.Contains(key) || a.Email.Contains(key) || a.FamilyAddress.Contains(key))
                .WhereIf(mn_Posts_Id?.Any() == true, a => a.Posts.AsSelect().Any(b => mn_Posts_Id.Contains(b.Id)));
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
            var item = await fsql.Select<OrgPerson>().IncludeMany(a => a.Posts).Where(a => a.Id == Id).FirstAsync();
            if (item == null) return ApiResult.Failed.SetMessage("记录不存在");
            ViewBag.item = item;
            return View();
        }

        /***************************************** POST *****************************************/

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Add([FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] string FullName, [FromForm] string NickName, [FromForm] string IdCard, [FromForm] string IdCardAddress, [FromForm] string Mobile, [FromForm] string Email, [FromForm] string FamilyAddress, [FromForm] bool IsLeave, [FromForm] DateTime LeaveTime, [FromForm] int[] mn_Posts_Id)
        {
            var item = new OrgPerson();
            item.CreateTime = CreateTime;
            item.UpdateTime = UpdateTime;
            item.IsDeleted = IsDeleted;
            item.Sort = Sort;
            item.FullName = FullName;
            item.NickName = NickName;
            item.IdCard = IdCard;
            item.IdCardAddress = IdCardAddress;
            item.Mobile = Mobile;
            item.Email = Email;
            item.FamilyAddress = FamilyAddress;
            item.IsLeave = IsLeave;
            item.LeaveTime = LeaveTime;
            using (var ctx = fsql.CreateDbContext())
            {
                await ctx.AddAsync(item);
                //关联 OrgPost
                await ctx.SaveManyAsync(item, "Posts");
                await ctx.SaveChangesAsync();
            }
            return ApiResult<object>.Success.SetData(item);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Edit([FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int Id, [FromForm] string FullName, [FromForm] string NickName, [FromForm] string IdCard, [FromForm] string IdCardAddress, [FromForm] string Mobile, [FromForm] string Email, [FromForm] string FamilyAddress, [FromForm] bool IsLeave, [FromForm] DateTime LeaveTime, [FromForm] int[] mn_Posts_Id)
        {
            //var item = new OrgPerson();
            //item.Id = Id;
            using (var ctx = fsql.CreateDbContext())
            {
                //ctx.Attach(item);
                var item = await ctx.Set<OrgPerson>().Where(a => a.Id == Id).FirstAsync();
                item.CreateTime = CreateTime;
                item.UpdateTime = UpdateTime;
                item.IsDeleted = IsDeleted;
                item.Sort = Sort;
                item.FullName = FullName;
                item.NickName = NickName;
                item.IdCard = IdCard;
                item.IdCardAddress = IdCardAddress;
                item.Mobile = Mobile;
                item.Email = Email;
                item.FamilyAddress = FamilyAddress;
                item.IsLeave = IsLeave;
                item.LeaveTime = LeaveTime;
                await ctx.UpdateAsync(item);
                //关联 OrgPost
                await ctx.SaveManyAsync(item, "Posts");
                var affrows = await ctx.SaveChangesAsync();
                if (affrows > 0) return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
            }
            return ApiResult.Failed;
        }

        [HttpPost("del")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Del([FromForm] int[] Id)
        {
            var items = Id?.Select((a, idx) => new OrgPerson { Id = Id[idx] });
            var affrows = await fsql.Delete<OrgPerson>().WhereDynamic(items).ExecuteAffrowsAsync();
            return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
        }
    }
}