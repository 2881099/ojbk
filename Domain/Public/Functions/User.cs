using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    partial class User
    {
        [Navigate(ManyToMany = typeof(AdminRoleUser))]
        public List<AdminRole> Roles { get; set; }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">账户编号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static Task<bool> UpdatePassword(Guid id, string password)
        {
            var item = new User { Id = id }.Attach();
            item.PassWord = password;
            return item.Update();
        }

        /// <summary>
        /// 根据用户名查询账户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        async public static Task<User> GetByUserName(string userName) =>
            (await Select.Where(a => a.UserName == userName).FirstAsync()).Attach();

        /// <summary>
        /// 修改登录信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ip"></param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public static Task<bool> UpdateLoginInfo(Guid id, string ip, AccountStatus? status)
        {
            var item = new User { Id = id }.Attach();
            item.LoginIP = ip;
            item.LoginTime = DateTime.Now;
            if (status != null) item.Status = status.Value;
            return item.Update();
        }

        /// <summary>
        /// 用户名是否存在
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public static Task<bool> ExistsUserName(string userName, Guid? id = null) =>
            Select.Where(a => a.UserName == userName).WhereIf(id != null, a => a.Id == id).AnyAsync();

        /// <summary>
        /// 手机号是否存在
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public static Task<bool> ExistsPhone(string phone, Guid? id = null) =>
            Select.Where(a => a.Phone == phone).WhereIf(id != null, a => a.Id == id).AnyAsync();

        /// <summary>
        /// 邮箱是否存在
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public static Task<bool> ExistsEmail(string email, Guid? id = null) =>
            Select.Where(a => a.Email == email).WhereIf(id != null, a => a.Id == id).AnyAsync();
    }
}
