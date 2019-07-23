using FreeSql.DataAnnotations;
using System;

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
