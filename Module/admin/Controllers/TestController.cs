﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Module.Admin.Controllers
{
    [Route("[controller]")]
    public class TestController : BaseController
    {
        public TestController(ILogger<TestController> logger) : base(logger) { }

        [HttpGet]
        public ApiResult List()
        {
            return ApiResult.Success;
        }
    }
}
