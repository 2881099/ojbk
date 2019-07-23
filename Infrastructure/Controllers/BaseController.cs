using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

[ServiceFilter(typeof(CustomExceptionFilter)), EnableCors("cors_all")]
public partial class BaseController : Controller
{
    public ILogger _logger;
    public ISession Session { get { return HttpContext.Session; } }
    public HttpRequest Req { get { return Request; } }
    public HttpResponse Res { get { return Response; } }

    public string Ip => this.Request.Headers["X-Real-IP"].FirstOrDefault() ?? this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
    public IConfiguration Configuration => (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));
    //public SysuserInfo LoginUser { get; private set; }
    public BaseController(ILogger logger) { _logger = logger; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        #region 参数验证
        if (context.ModelState.IsValid == false)
            foreach (var value in context.ModelState.Values)
                if (value.Errors.Any())
                {
                    context.Result = Json(APIReturn.参数格式不正确.SetMessage($"参数格式不正确：{value.Errors.First().ErrorMessage}"));
                    return;
                }
        #endregion
        base.OnActionExecuting(context);
    }
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
    }
}