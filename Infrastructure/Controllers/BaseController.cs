﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ojbk.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[ServiceFilter(typeof(CustomExceptionFilter)), EnableCors("cors_all")]
public partial class BaseController : Controller
{
    public ISession Session { get { return HttpContext.Session; } }
    public HttpRequest Req { get { return Request; } }
    public HttpResponse Res { get { return Response; } }

    public string Ip => this.Request.Headers["X-Real-IP"].FirstOrDefault() ?? this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
    public IConfiguration Configuration => (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));

    public AuthUser LoginUser { get; private set; }

    public static AsyncLocal<ControllerContext> CurrentControllerContext = new AsyncLocal<ControllerContext>();
    async public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        CurrentControllerContext.Value = this.ControllerContext;

        string token = Request.Headers["token"].FirstOrDefault() ?? Request.Query["token"].FirstOrDefault();
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                this.LoginUser = await this.GetUserByToken(token);
            }
            catch
            {
                context.Result = ApiResult.Failed.SetMessage("登陆TOKEN失效_请重新登陆");
                return;
            }
        }
        if (this.ValidModelState(context) == false) return;
        await base.OnActionExecutionAsync(context, next);
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (this.ValidModelState(context) == false) return;
        base.OnActionExecuting(context);
    }


    protected string GetUserToken(AuthUser user)
    {
        string text = JsonConvert.SerializeObject(Tuple.Create(user.Id, Guid.NewGuid(), user.LoginTime.GetTime()));
        return Util.AesEncrypt(text, Encoding.UTF8.GetBytes(Configuration["login_aes:key"]), Encoding.UTF8.GetBytes(Configuration["login_aes:iv"]));
    }
    async protected Task<AuthUser> GetUserByToken(string token)
    {
        var data = Util.AesDecrypt(token, Encoding.UTF8.GetBytes(Configuration["login_aes:key"]), Encoding.UTF8.GetBytes(Configuration["login_aes:iv"])); //解密
        var at = JsonConvert.DeserializeObject<(int UserId, Guid RandomId, long LoginTime)>(data);
        var user = await AuthUser.FindAsync(at.UserId);
        if (user.Status == AuthUserStatus.禁用) return null;
        //if (user?.LoginTime.GetTime() != at.LoginTime) user = null;
        //验证 token 内的登陆时间，与实际的登陆时间，不相等的话等于 token 失效
        return user;
    }

    bool ValidModelState(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid == false)
            foreach (var value in context.ModelState.Values)
                if (value.Errors.Any())
                {
                    context.Result = Json(ApiResult.Failed.SetMessage($"参数格式不正确：{value.Errors.First().ErrorMessage}"));
                    return false;
                }
        return true;
    }
}