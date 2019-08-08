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
    }

    #region ManyToMany AdmRoute
    partial class AuthRole
    {
        [Navigate(ManyToMany = typeof(AdmRouteRole))]
        public List<AdmRoute> Routes { get; set; }
        public class AdmRouteRole : BaseEntity<AdmRouteRole>
        {
            public int RoleId { get; set; }
            public AuthRole Role { get; set; }

            public int AdmRouteId { get; set; }
            public AdmRoute AdmRoute { get; set; }
        }
    }
    partial class AdmRoute {[Navigate(ManyToMany = typeof(AuthRole.AdmRouteRole))] public List<AuthRole> Roles { get; set; } }
    #endregion

    #region ManyToMany AuthUser
    partial class AuthRole
    {
        [Navigate("RoleId")]
        public List<AuthRole.RoleUser> RoleUsers { get; set; }

        [Navigate(ManyToMany = typeof(AuthRole.RoleUser))]
        public List<AuthUser> Users { get; set; }
        public class RoleUser : BaseEntity<RoleUser>
        {
            public int RoleId { get; set; }
            public AuthRole Role { get; set; }

            public int UserId { get; set; }
            public AuthUser User { get; set; }
        }
    }
    partial class AuthUser {[Navigate(ManyToMany = typeof(AuthRole.RoleUser))] public List<AuthRole> Roles { get; set; } }
    #endregion

    #region ManyToMany OrgPost
    partial class AuthRole
    {
        [Navigate(ManyToMany = typeof(AuthRole.RolePost))]
        public List<OrgPost> Posts { get; set; }
        public class RolePost : BaseEntity<RolePost>
        {
            public int RoleId { get; set; }
            public AuthRole Role { get; set; }

            public int PostId { get; set; }
            public OrgPost Post { get; set; }
        }
    }
    partial class OrgPost {[Navigate(ManyToMany = typeof(AuthRole.RolePost))] public List<AuthRole> Roles { get; set; } }
    #endregion
}
