using FreeSql;
using FreeSql.DataAnnotations;
using System.Collections.Generic;

namespace ojbk.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    public partial class AuthRole : BaseEntity<AuthRole, int>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string TenantId { get; set; }
    }

    #region ManyToMany AdmRoute
    partial class AuthRole
    {
        [Navigate(ManyToMany = typeof(AuthRoleAdmRoute))]
        public List<AdmRoute> AdmRoutes { get; set; }
        public class AuthRoleAdmRoute : BaseEntity<AuthRoleAdmRoute>
        {
            public int RoleId { get; set; }
            public AuthRole Role { get; set; }

            public int AdmRouteId { get; set; }
            public AdmRoute AdmRoute { get; set; }
        }
    }
    partial class AdmRoute {[Navigate(ManyToMany = typeof(AuthRole.AuthRoleAdmRoute))] public List<AuthRole> Roles { get; set; } }
    #endregion

    #region ManyToMany AuthUser
    partial class AuthRole
    {
        [Navigate("RoleId")]
        public List<AuthRole.AuthRoleUser> RoleUsers { get; set; }

        [Navigate(ManyToMany = typeof(AuthRole.AuthRoleUser))]
        public List<AuthUser> Users { get; set; }
        public class AuthRoleUser : BaseEntity<AuthRoleUser>
        {
            public int RoleId { get; set; }
            public AuthRole Role { get; set; }

            public int UserId { get; set; }
            public AuthUser User { get; set; }
        }
    }
    partial class AuthUser {[Navigate(ManyToMany = typeof(AuthRole.AuthRoleUser))] public List<AuthRole> Roles { get; set; } }
    #endregion

    #region ManyToMany OrgPost
    partial class AuthRole
    {
        [Navigate(ManyToMany = typeof(AuthRole.AuthRolePost))]
        public List<OrgPost> OrgPosts { get; set; }
        public class AuthRolePost : BaseEntity<AuthRolePost>
        {
            public int RoleId { get; set; }
            public AuthRole Role { get; set; }

            public int OrgPostId { get; set; }
            public OrgPost OrgPost { get; set; }
        }
    }
    partial class OrgPost {[Navigate(ManyToMany = typeof(AuthRole.AuthRolePost))] public List<AuthRole> Roles { get; set; } }
    #endregion
}
