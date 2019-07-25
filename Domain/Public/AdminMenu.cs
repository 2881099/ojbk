using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    /// <summary>
    /// 菜单
    /// </summary>
    public partial class AdminMenu : BaseEntity<AdminMenu, Guid>
    {
        /// <summary>
        /// 所属模块
        /// </summary>
        [Column(Unique = "uk_adminmenu_modulecode")]
        public string ModuleCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public AdminMenuType Type { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// 路由参数
        /// </summary>
        public string RouteParams { get; set; }

        /// <summary>
        /// 路由参数
        /// </summary>
        public string RouteQuery { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 图标颜色
        /// </summary>
        public string IconColor { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 打开方式
        /// </summary>
        public AdminMenuTarget Target { get; set; }

        /// <summary>
        /// 对话框宽度
        /// </summary>
        public string DialogWidth { get; set; }

        /// <summary>
        /// 对话框高度
        /// </summary>
        public string DialogHeight { get; set; }

        /// <summary>
        /// 对话框可全屏
        /// </summary>
        public bool DialogFullscreen { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


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

    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum AdminMenuType { 节点, 路由, 链接,
        /// <summary>
        /// 按钮类型，不显示在后台管理的左侧菜单
        /// </summary>
        按钮
    }

    /// <summary>
    /// 菜单打开方式(只针对链接菜单)
    /// </summary>
    public enum AdminMenuTarget { 新窗口, 当前窗口, 对话框, 容器内, 内容区 }

}
