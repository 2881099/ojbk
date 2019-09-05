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
        async public Task<ApiResult> _Add([FromForm] string FullName, [FromForm] string NickName, [FromForm] string IdCard, [FromForm] string IdCardAddress, [FromForm] string Mobile, [FromForm] string Email, [FromForm] string FamilyAddress, [FromForm] bool IsLeave, [FromForm] DateTime LeaveTime, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int[] mn_Posts_Id)
        {
            var item = new OrgPerson();
            item.FullName = FullName;
            item.NickName = NickName;
            item.IdCard = IdCard;
            item.IdCardAddress = IdCardAddress;
            item.Mobile = Mobile;
            item.Email = Email;
            item.FamilyAddress = FamilyAddress;
            item.IsLeave = IsLeave;
            item.LeaveTime = LeaveTime;
            item.CreateTime = CreateTime;
            item.UpdateTime = UpdateTime;
            item.IsDeleted = IsDeleted;
            item.Sort = Sort;
            using (var ctx = fsql.CreateDbContext())
            {
                await ctx.AddAsync(item);
                //关联 OrgPost
                var mn_Posts = mn_Posts_Id.Select((mn, idx) => new OrgPost.OrgPostPerson { PostId = mn, PersonId = item.Id }).ToArray();
                await ctx.AddRangeAsync(mn_Posts);
                await ctx.SaveChangesAsync();
            }
            return ApiResult<object>.Success.SetData(item);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        async public Task<ApiResult> _Edit([FromForm] string FullName, [FromForm] string NickName, [FromForm] string IdCard, [FromForm] string IdCardAddress, [FromForm] string Mobile, [FromForm] string Email, [FromForm] string FamilyAddress, [FromForm] bool IsLeave, [FromForm] DateTime LeaveTime, [FromForm] int Id, [FromForm] DateTime CreateTime, [FromForm] DateTime UpdateTime, [FromForm] bool IsDeleted, [FromForm] int Sort, [FromForm] int[] mn_Posts_Id)
        {
            var item = new OrgPerson();
            item.Id = Id;
            using (var ctx = fsql.CreateDbContext())
            {
                ctx.Attach(item);
                item.FullName = FullName;
                item.NickName = NickName;
                item.IdCard = IdCard;
                item.IdCardAddress = IdCardAddress;
                item.Mobile = Mobile;
                item.Email = Email;
                item.FamilyAddress = FamilyAddress;
                item.IsLeave = IsLeave;
                item.LeaveTime = LeaveTime;
                item.CreateTime = CreateTime;
                item.UpdateTime = UpdateTime;
                item.IsDeleted = IsDeleted;
                item.Sort = Sort;
                await ctx.UpdateAsync(item);
                //关联 OrgPost
                if (mn_Posts_Id != null)
                {
                    var mn_Posts_Id_list = mn_Posts_Id.ToList();
                    var oldlist = ctx.Set<OrgPost.OrgPostPerson>().Where(a => a.PersonId == item.Id).ToList();
                    foreach (var olditem in oldlist)
                    {
                        var idx = mn_Posts_Id_list.FindIndex(a => a == olditem.PostId);
                        if (idx == -1) ctx.Remove(olditem);
                        else mn_Posts_Id_list.RemoveAt(idx);
                    }
                    var mn_Posts = mn_Posts_Id_list.Select((mn, idx) => new OrgPost.OrgPostPerson { PostId = mn, PersonId = item.Id }).ToArray();
                    await ctx.AddRangeAsync(mn_Posts);
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
            var items = Id?.Select((a, idx) => new OrgPerson { Id = Id[idx] });
            var affrows = await fsql.Delete<OrgPerson>().WhereDynamic(items).ExecuteAffrowsAsync();
            return ApiResult.Success.SetMessage($"更新成功，影响行数：{affrows}");
        }
    }
}