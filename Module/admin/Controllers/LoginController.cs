using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ojbk.Entities;
using System;
using System.Threading.Tasks;

namespace Module.admin.Controllers
{
    [Route("api/module/admin/[controller]")]
    public class LoginController : BaseController
    {
        public LoginController(ILogger<LoginController> logger) : base(logger) { }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost("verify-code")]
        async public Task<APIReturn> VerifyCode()
        {
            var verifyCodeModel = new
            {
                Id = Guid.NewGuid().ToString("N"),
                Base64String = Util.DrawVerifyCodeBase64String(out string code, 6)
            };

            //把验证码放到内存缓存中，有效期10分钟
            await RedisHelper.SetAsync($"VerifyCode{verifyCodeModel.Id}", code, 10 * 60);
            return APIReturn<object>.Success.SetData(verifyCodeModel);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        async public Task<APIReturn> Login([FromForm] string username, [FromForm] string password, [FromForm] string verifyCodeId, [FromForm] string verifyCode)
        {
            if (string.IsNullOrEmpty(verifyCodeId) ||
                string.IsNullOrEmpty(verifyCode)) return APIReturn.Failed.SetMessage("验证码参数错误");

            var verifyCodeCache = await RedisHelper.GetAsync($"VerifyCode{verifyCodeId}");
            if (verifyCode != verifyCodeCache) return APIReturn.Failed.SetMessage("验证码有误");

            var user = await Users.GetByUserName(username);
            if (user == null) return APIReturn.Failed.SetMessage("账户不存在");
            if (user.Status == AccountStatus.注销) return APIReturn.Failed.SetMessage("该账户已注销，请联系管理员");
            if (user.Status == AccountStatus.禁用) return APIReturn.Failed.SetMessage("该账户已禁用，请联系管理员");
            if (user.PassWord != Util.MD5(password)) return APIReturn.Failed.SetMessage("密码错误");

            await RedisHelper.DelAsync($"VerifyCode{verifyCodeId}");
            await user.UpdateLoginInfo(base.Ip);
            return APIReturn<object>.Success.SetData(new
            {
                token = base.GetUserToken(user)
            });
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost("update-password")]
        async public Task<APIReturn> UpdatePassword([FromForm] string password, [FromForm] string oldPassword, [FromForm] string verifyCodeId, [FromForm] string verifyCode)
        {
            if (string.IsNullOrEmpty(password)) return APIReturn.Failed.SetMessage("密码不能为空");
            if (string.IsNullOrEmpty(verifyCodeId) ||
                string.IsNullOrEmpty(verifyCode)) return APIReturn.Failed.SetMessage("验证码参数错误");

            var verifyCodeCache = await RedisHelper.GetAsync($"VerifyCode{verifyCodeId}");
            if (verifyCode != verifyCodeCache) return APIReturn.Failed.SetMessage("验证码有误");

            var user = base.LoginUser;
            if (user.PassWord != Util.MD5(oldPassword)) return APIReturn.Failed.SetMessage("原密码错误");

            await user.UpdatePassword(password);
            return APIReturn.Success;
        }
    }
}
