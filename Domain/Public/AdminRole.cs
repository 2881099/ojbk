using System;

namespace ojbk.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    public partial class AdminRole : BaseEntity<AdminRole, Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 角色菜单
    /// </summary>
    public partial class AdminRoleMenu : BaseEntity<AdminRoleMenu>
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public Guid AdminRoleId { get; set; }
        public AdminRole AdminRole { get; set; }

        /// <summary>
        /// 菜单编号
        /// </summary>
        public Guid AdminMenuId { get; set; }
        public AdminMenu AdminMenu { get; set; }
    }

    /// <summary>
    /// 角色账户
    /// </summary>
    public partial class AdminRoleUser : BaseEntity<AdminRoleUser>
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public Guid AdminRoleId { get; set; }
        public AdminRole AdminRole { get; set; }

        /// <summary>
        /// 账户编号
        /// </summary>
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
