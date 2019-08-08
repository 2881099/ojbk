using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ojbk.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Module.Admin.Controllers
{
    [Route("api/module/admin/[controller]")]
    public class OrganizationController : BaseController
    {
        public OrganizationController(ILogger<OrganizationController> logger) : base(logger) { }

        /// <summary>
        /// 获取组织机构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        async public Task<ApiResult> List()
        {
            var user = base.LoginUser;
            var posts = await user.SelectOrgPost.Include(a => a.Department).ToListAsync();
            var departs = posts.Select(a => (a.Department.Id, a.Department.Name, a.Department.ParentId, a.Department.Sort)).Distinct().Select(a => new
            {
                a.Id,
                a.Name,
                a.ParentId,
                a.Sort,
                posts = posts.Where(b => b.DepartmentId == a.Id).OrderBy(b => b.Sort).Select(b => new
                {
                    b.Id, b.Title
                }).ToArray()
            }).ToArray();
            return ApiResult<object>.Success.SetData(new
            {
                departs
            });
        }

        /// <summary>
        /// 添加或更新组织机构
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpPost("add-or-update")]
        async public Task<ApiResult> AddOrUpdate([FromForm] int? id, [FromForm] string name, [FromForm] int parentId, [FromForm] int sort)
        {
            var depart = new OrgDepartment
            {
                Id = id ?? 0,
                Name = name,
                ParentId = parentId,
                Sort = sort
            };
            await depart.SaveAsync();
            return ApiResult.Success;
        }

        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <param name="postId">一个或多个岗位id</param>
        /// <returns></returns>
        [HttpGet("persons")]
        async public Task<ApiResult> PersonList([FromQuery] int[] postId)
        {
            var user = base.LoginUser;
            var selectPosts = user.SelectOrgPerson;
            if (postId?.Any() == true) selectPosts.Where(a => a.PostPersons.AsSelect().Any(pp => postId.Contains(pp.PostId)));
            var posts = await selectPosts.IncludeMany(a => a.Posts, then => then.Include(p => p.Department)).ToListAsync();
            return ApiResult<object>.Success.SetData(new
            {
                persons = posts.Select(a => new
                {
                    a.Id,
                    a.CreateTime,
                    a.Email,
                    a.FamilyAddress,
                    a.FullName,
                    a.IdCard,
                    a.IdCardAddress,
                    a.IsLeave,
                    a.LeaveTime,
                    a.Mobile,
                    a.NickName,
                    a.Sort,
                    a.UpdateTime,
                    posts = a.Posts.Select(b => new
                    {
                        b.Id,
                        b.Title,
                        depart = new
                        {
                            b.Department.Id,
                            b.Department.Name,
                            b.Department.ParentId,
                            b.Department.Sort
                        }
                    })
                })
            });
        }
    }
}
