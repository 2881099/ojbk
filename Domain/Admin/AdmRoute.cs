using FreeSql;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ojbk.Entities
{
    /// <summary>
    /// 后台路由，一级=菜单
    /// </summary>
    public partial class AdmRoute : BaseEntity<AdmRoute, int>
    {
        static AdmRoute()
        {
            #region 初始化菜单
            if (Orm.Select<AdmRoute>().Any() == false)
            {
                var adds = new[]
                {
                    new AdmRoute
                    {
                        Name = "文档",
                        Sort = 1,
                        Extdata = JsonConvert.SerializeObject(new { icon = "ios-book", href = "https://github.com/2881099/ojbk" }),
                    },
                    new AdmRoute
                    {
                        Name = "QQ群",
                        Sort = 2,
                        Extdata = JsonConvert.SerializeObject(new { icon = "_qq", path = "/join_qqgroup", name = "join_qqgroup", component = "@/view/join_qqgroup-page.vue" }),
                    },

                    new AdmRoute
                    {
                        Name = "系统管理",
                        Sort = 3,
                        Extdata = JsonConvert.SerializeObject(new { icon = "logo-buffer", path = "/sysadmin", name = "sysadmin" }),

                        Childs = new List<AdmRoute>( new []
                        {
                            new AdmRoute
                            {
                                Name = "用户管理",
                                Sort = 1,
                                Extdata = JsonConvert.SerializeObject(new { icon = "org_tree_page", path = "/authuser", name = "sysadmin_authuser", component = "@/view/sysadmin/authuser-page.vue" }),
                            },
                            new AdmRoute
                            {
                                Name = "角色管理",
                                Sort = 2,
                                Extdata = JsonConvert.SerializeObject(new { icon = "org_tree_page", path = "/authrole", name = "sysadmin_authrole", component = "@/view/sysadmin/authrole-page.vue" }),
                            },
                            new AdmRoute
                            {
                                Name = "菜单管理",
                                Sort = 3,
                                Extdata = JsonConvert.SerializeObject(new { icon = "org_tree_page", path = "/admroute", name = "sysadmin_admroute", component = "@/view/sysadmin/admroute-page.vue" }),
                            },
                            new AdmRoute
                            {
                                Name = "权限管理",
                                Sort = 4,
                                Extdata = JsonConvert.SerializeObject(new { icon = "org_tree_page", path = "/auth", name = "sysadmin_auth", component = "@/view/sysadmin/auth-page.vue" }),
                            },
                        })
                    },

                    new AdmRoute
                    {
                        Name = "组织机构",
                        Sort = 4,
                        Extdata = JsonConvert.SerializeObject(new { icon = "logo-buffer", path = "/organization", name = "organization" }),

                        Childs = new List<AdmRoute>( new []
                        {
                            new AdmRoute
                            {
                                Name = "部门管理",
                                Sort = 1,
                                Extdata = JsonConvert.SerializeObject(new { icon = "org_tree_page", path = "/department", name = "organization_department", component = "@/view/organization/department-page.vue" }),
                            },
                            new AdmRoute
                            {
                                Name = "岗位管理",
                                Sort = 2,
                                Extdata = JsonConvert.SerializeObject(new { icon = "org_tree_page", path = "/post", name = "organization_post", component = "@/view/organization/post-page.vue" }),
                            },
                            new AdmRoute
                            {
                                Name = "员工管理",
                                Sort = 3,
                                Extdata = JsonConvert.SerializeObject(new { icon = "org_tree_page", path = "/person", name = "organization_person", component = "@/view/organization/person-page.vue" }),
                            }
                        })
                    },

                };
                Orm.GetRepository<AdmRoute>().Insert(adds);
            }
            #endregion
        }

        [Navigate("ParentId")]
        public List<AdmRoute> Childs { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public int ParentId { get; set; }
        [Navigate("ParentId")]
        public AdmRoute Parent { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 前端数据
        /// </summary>
        public string Extdata { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string TenantId { get; set; }
    }
}
