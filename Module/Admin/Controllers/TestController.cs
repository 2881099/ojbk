using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Module.Admin.Controllers
{
    [Route("api/module/admin/[controller]"), ApiExplorerSettings(GroupName = "后台管理")]
    public class TestController : BaseController
    {

        [HttpGet]
        public ApiResult List()
        {
            return ApiResult.Success;
        }
    }
}
