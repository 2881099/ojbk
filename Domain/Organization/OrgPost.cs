using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    /// <summary>
    /// 岗位
    /// </summary>
    public partial class OrgPost : BaseEntity<OrgPost, int>
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public OrgDepartment Department { get; set; }

        /// <summary>
        /// 岗位称呼
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 职责内容
        /// </summary>
        public string DutyContent { get; set; }
        /// <summary>
        /// 工作内容
        /// </summary>
        public string JobContent { get; set; }
    }

    #region ManyToMany OrgPerson
    partial class OrgPost
    {
        [Navigate(ManyToMany = typeof(OrgPostPerson))]
        public List<OrgPerson> Persons { get; set; }
        public class OrgPostPerson : BaseEntity<OrgPostPerson>
        {
            /// <summary>
            /// 岗位id
            /// </summary>
            public int PostId { get; set; }
            public OrgPost Post { get; set; }

            /// <summary>
            /// 员工id
            /// </summary>
            public int PersonId { get; set; }
            public OrgPerson Person { get; set; }
        }
    }
    partial class OrgPerson
    {
        [Navigate(ManyToMany = typeof(OrgPost.OrgPostPerson))]
        public List<OrgPost> Posts { get; set; }

        [Navigate("PersonId")]
        public List<OrgPost.OrgPostPerson> PostPersons { get; set; }
    }
    #endregion
}
