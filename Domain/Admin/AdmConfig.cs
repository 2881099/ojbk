using FreeSql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{


    //public record Record2(Guid id, string title) : BaseEntity<Record2, string>;

    /// <summary>
    /// 配置项
    /// </summary>
    public class AdmConfig : BaseEntity<AdmConfig, string>
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 验证键值是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task<bool> Exists(string key) =>
            Select.AnyAsync(a => a.Id == key);

        /// <summary>
        /// 数据项是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Task<bool> Exists(AdmConfig entity) =>
            Select.WhereDynamic(entity).AnyAsync();

        /// <summary>
        /// 根据前缀查询配置项列表
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static Task<List<AdmConfig>> QueryByPrefix(string prefix) =>
            Select.Where(a => a.Id.StartsWith(prefix)).ToListAsync();

        /// <summary>
        /// 根据Key获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task<AdmConfig> GetByKey(string key) =>
            FindAsync(key);
    }

}
