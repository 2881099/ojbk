using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    /// <summary>
    /// 菜单仓储
    /// </summary>
    partial class AdminMenu
    {
        [Navigate(ManyToMany = typeof(AdminRoleMenu))]
        public List<AdminRole> Roles { get; set; }

        /// <summary>
        /// 查询指定菜单的子菜单
        /// </summary>
        /// <returns></returns>
        public static Task<List<AdminMenu>> QueryChildren(Guid parentId) =>
            Select.Where(a => a.ParentId == parentId).OrderByDescending(a => a.Sort).ToListAsync();

        /// <summary>
        /// 判断该节点是否存在子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<bool> ExistsChild(Guid id) =>
            Select.Where(a => a.ParentId == id).AnyAsync();

        /// <summary>
        /// 在同级别下名称是重复
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<bool> ExistsNameByParentId(string name, Guid id, Guid parentId = default(Guid)) =>
            Select.Where(a => a.ParentId == parentId && a.Name == name).WhereIf(id != default(Guid), a => a.Id != id).AnyAsync();

        /// <summary>
        /// 根据模块编码判断是否有菜单
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public static Task<bool> ExistsWidthModule(string moduleCode) =>
            Select.Where(a => a.ModuleCode == moduleCode).AnyAsync();

        ///// <summary>
        ///// 获取指定账户的菜单列表
        ///// </summary>
        ///// <param name="accountId"></param>
        ///// <returns></returns>
        //public static Task<List<Menu>> GetByAccount(Guid accountId) =>
    }
}
