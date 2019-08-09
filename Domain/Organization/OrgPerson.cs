using FreeSql;
using System;

namespace ojbk.Entities
{
    /// <summary>
    /// 员工
    /// </summary>
    public partial class OrgPerson : BaseEntity<OrgPerson, int>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 身份证地址
        /// </summary>
        public string IdCardAddress { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FamilyAddress { get; set; }

        /// <summary>
        /// 是否离职
        /// </summary>
        public bool IsLeave { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime LeaveTime { get; set; }
    }
}
