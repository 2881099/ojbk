using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ojbk.Entities;
using System;
using System.Threading.Tasks;

namespace Module.Public.Controllers
{
    [Route("api/module/public/[controller]"), ApiExplorerSettings(GroupName = "公共")]
    public class LoginController : BaseController
    {

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost("login")]
        async public Task<ApiResult> Login([FromForm] string username, [FromForm] string password)
        {
            var user = await AuthUser.Where(a => a.Username == username).FirstAsync();
            if (user == null) return ApiResult.Failed.SetMessage("账户不存在");
            if (user.Status == AuthUserStatus.禁用) return ApiResult.Failed.SetMessage("该账户已禁用，请联系管理员");
            if (user.Password != Util.Md5(password)) return ApiResult.Failed.SetMessage("密码错误");

            user.LoginIp = base.Ip;
            user.LoginTime = DateTime.Now;
            await user.UpdateAsync();
            return ApiResult<object>.Success.SetData(new
            {
                token = base.GetUserToken(user)
            });
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="oldPassword">旧密码</param>
        /// <returns></returns>
        [HttpPost("update-password")]
        async public Task<ApiResult> UpdatePassword([FromForm] string password, [FromForm] string oldPassword)
        {
            if (string.IsNullOrEmpty(password)) return ApiResult.Failed.SetMessage("密码不能为空");

            var user = base.LoginUser;
            if (user.Password != Util.Md5(oldPassword)) return ApiResult.Failed.SetMessage("原密码错误");

            user.Password = password;
            await user.UpdateAsync();
            return ApiResult.Success;
        }
    }
}
