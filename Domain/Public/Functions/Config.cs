using System.Collections.Generic;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    partial class Config
    {
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
        public static Task<bool> Exists(Config entity) =>
            Select.WhereDynamic(entity).AnyAsync();

        /// <summary>
        /// 根据前缀查询配置项列表
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static Task<List<Config>> QueryByPrefix(string prefix) =>
            Select.Where(a => a.Id.StartsWith(prefix)).ToListAsync();

        /// <summary>
        /// 根据Key获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task<Config> GetByKey(string key) =>
            Find(key);
    }
}
