using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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


        [Navigate(ManyToMany = typeof(AdminRoleMenu))]
        public List<AdminMenu> Menus { get; set; }

        [Navigate(ManyToMany = typeof(AdminRoleUser))]
        public List<Users> Users { get; set; }

        public Task AddUser(Users user)
        {
            var item = new AdminRoleUser { AdminRoleId = this.Id, UserId = user.Id }.Attach();
            item.IsDeleted = false;
            return item.Save();
        }
        public Task RemoveUser(Users user) =>
            new AdminRoleUser { AdminRoleId = this.Id, UserId = user.Id }.Delete();

        public Task AddMenu(AdminMenu menu)
        {
            var item = new AdminRoleMenu { AdminRoleId = this.Id, AdminMenuId = menu.Id }.Attach();
            item.IsDeleted = false;
            return item.Save();
        }
        public Task RemoveMenu(AdminMenu menu) =>
            new AdminRoleMenu { AdminRoleId = this.Id, AdminMenuId = menu.Id }.Delete();

        /// <summary>
        /// 判断角色是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<bool> Exists(string name, Guid? id = null) =>
            Select.Where(a => a.Name == name).WhereIf(id != null, a => a.Id == id).AnyAsync();
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
        public Users User { get; set; }
    }
}
