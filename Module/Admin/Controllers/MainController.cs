using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ojbk.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Module.Admin.Controllers
{
    [Route("api/module/admin/[controller]"), ApiExplorerSettings(GroupName = "后台管理")]
    public class MainController : BaseController
    {

        /// <summary>
        /// 后台管理首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        async public Task<ApiResult> Home()
        {
            var user = base.LoginUser;
            var menus = await user.SelectAdmRoute.OrderBy(a => a.Sort).ToListAsync();
            var roles = await AuthRole.Select.Where(a => a.RoleUsers.AsSelect().Any(ru => ru.UserId == user.Id)).ToListAsync();
            var person = await OrgPerson.Select.Where(a => a.Id == user.PersonId)
                .IncludeMany(a => a.Posts, then => then.Include(post => post.Department)).FirstAsync();

            return ApiResult<object>.Success.SetData(new
            {
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Status,
                    user.LoginTime,
                    user.LoginIp,
                    user.IsAdmin,
                    user.CreateTime,
                    person = new
                    {
                        person.Id,
                        person.CreateTime,
                        person.Email,
                        person.FullName,
                        person.NickName,
                        departments = person.Posts.Select(p => p.DepartmentId).Distinct().Select(dept => new
                        {
                            posts = person.Posts.Where(p2 => p2.DepartmentId == dept).Select(p => new
                            {
                                p.Id,
                                p.Title
                            })
                        })
                    },
                    roles = (user.IsAdmin ?
                        new[] { new AuthRole { Id = 0, Name = "系统管理员", Remark = "系统管理员" } }.Concat(roles) :
                        roles).Select(role => new
                        {
                            role.Id,
                            role.Name,
                            role.Remark
                        })
                },
                menus = menus.Where(a => a.ParentId == 0).Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Extdata,
                    m.ParentId,
                    m.Remark,
                    m.Sort
                })
            });
        }
    }
}
