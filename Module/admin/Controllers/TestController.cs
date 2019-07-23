using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Module.admin.Controllers
{
    [Route("[controller]")]
    public class TestController : BaseController
    {
        public TestController(ILogger<TestController> logger) : base(logger) { }

        [HttpGet]
        public APIReturn List()
        {
            return APIReturn.Success;
        }
    }
}
