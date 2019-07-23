using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ojbk.Entities
{
    /// <summary>
    /// 账户
    /// </summary>
    public partial class Users : BaseEntity<Users, Guid>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Column(Unique = "uk_user_username")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column(Unique = "uk_user_phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Column(Unique = "uk_user_email")]
        public string Email { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// 注销时间
        /// </summary>
        public DateTime ClosedTime { get; set; }

        /// <summary>
        /// 注销人
        /// </summary>
        public Guid ClosedBy { get; set; }
    }

    /// <summary>
    /// 账户状态
    /// </summary>
    public enum AccountStatus { 未激活, 正常, 禁用, 注销 }
}
