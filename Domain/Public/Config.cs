
namespace ojbk.Entities
{
    /// <summary>
    /// 配置项
    /// </summary>
    public partial class Config : BaseEntity<Config, string>
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
