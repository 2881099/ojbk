using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    /// <summary>
    /// 账户
    /// </summary>
    public partial class Users : BaseEntity<Users, Guid>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Column(Unique = "uk_user_username")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column(Unique = "uk_user_phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Column(Unique = "uk_user_email")]
        public string Email { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// 注销时间
        /// </summary>
        public DateTime ClosedTime { get; set; }

        /// <summary>
        /// 注销人
        /// </summary>
        public Guid ClosedBy { get; set; }

        [Navigate(ManyToMany = typeof(AdminRoleUser))]
        public List<AdminRole> Roles { get; set; }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public Task<bool> UpdatePassword(string password)
        {
            this.Attach().PassWord = password;
            return this.Update();
        }

        /// <summary>
        /// 根据用户名查询账户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        async public static Task<Users> GetByUserName(string userName) =>
            (await Select.Where(a => a.UserName == userName).FirstAsync()).Attach();

        /// <summary>
        /// 修改登录信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public Task<bool> UpdateLoginInfo(string ip, AccountStatus? status = null)
        {
            this.Attach();
            this.LoginIP = ip;
            this.LoginTime = DateTime.Now;
            if (status != null) this.Status = status.Value;
            return this.Update();
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

    /// <summary>
    /// 账户状态
    /// </summary>
    public enum AccountStatus { 未激活, 正常, 禁用, 注销 }
}
