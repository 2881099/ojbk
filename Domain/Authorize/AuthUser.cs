using FreeSql;
using FreeSql.DataAnnotations;
using System;

namespace ojbk.Entities
{
    /// <summary>
    /// 登陆账户
    /// </summary>
    public partial class AuthUser : BaseEntity<AuthUser, int>
    {
        static AuthUser()
        {
            if (AuthUser.Select.Any(a => a.IsAdmin) == false)
            {
                new AuthUser
                {
                    IsAdmin = true,
                    Password = Util.MD5("admin"),
                    Status = AuthUserStatus.正常,
                    Username = "admin"
                }.Insert();
            }
        }

        #region 方法
        /// <summary>
        /// 登陆用户，查询后台路由表
        /// </summary>
        public ISelect<AdmRoute> SelectAdmRoute => this.IsAdmin ? 
            AdmRoute.Select :
            AdmRoute.Select.Where(a => a.Roles.AsSelect().Any(role => role.RoleUsers.AsSelect().Any(ur => ur.UserId == this.Id)));

        /// <summary>
        /// 登陆用户，查询岗位表
        /// </summary>
        public ISelect<OrgPost> SelectOrgPost => this.IsAdmin ?
            OrgPost.Select :
            OrgPost.Select.Where(a => a.Roles.AsSelect().Any(role => role.RoleUsers.AsSelect().Any(ur => ur.UserId == this.Id)));

        /// <summary>
        /// 登陆用户，查询员工表
        /// </summary>
        public ISelect<OrgPerson> SelectOrgPerson => this.IsAdmin ?
            OrgPerson.Select :
            OrgPerson.Select.Where(a => a.Posts.AsSelect().Any(post => post.Roles.AsSelect().Any(role => role.RoleUsers.AsSelect().Any(ur => ur.UserId == this.Id))));
        #endregion

        /// <summary>
        /// 员工id
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// 员工
        /// </summary>
        [Navigate("PersonId")]
        public OrgPerson Person { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column(Unique = "uk_authuser_username")]
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LoginIp { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public AuthUserStatus Status { get; set; }
        /// <summary>
        /// 是否为系统管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }

    /// <summary>
    /// 账户状态
    /// </summary>
    public enum AuthUserStatus { 正常, 禁用 }
}
