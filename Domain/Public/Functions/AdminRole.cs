using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    partial class AdminRole
    {
        [Navigate(ManyToMany = typeof(AdminRoleMenu))]
        public List<AdminMenu> Menus { get; set; }

        [Navigate(ManyToMany = typeof(AdminRoleUser))]
        public List<User> Users { get; set; }

        public Task AddUser(User user)
        {
            var item = new AdminRoleUser { AdminRoleId = this.Id, UserId = user.Id }.Attach();
            item.IsDeleted = false;
            return item.Save();
        }
        public Task RemoveUser(User user) =>
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
}
