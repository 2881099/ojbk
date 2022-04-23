using FreeSql;
using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSql
{
    /// <summary>
    /// 简单的十级树状结构基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    [Table(DisableSyncStructure = true)]
    public abstract class BaseEntityTree<TEntity, TKey> : BaseEntity<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// 父级id
        /// </summary>
        public TKey ParentId
        {
            get => _ParentId;
            set
            {
                if (Equals(value, default) == false && Equals(value, Id))
                    throw new ArgumentException("ParentId 值不能与 Id 相同");
                _ParentId = value;
            }
        }
        public TEntity Parent { get; set; }
        private TKey _ParentId;

        /// <summary>
        /// 下级列表
        /// </summary>
        [Navigate("ParentId")]
        public List<TEntity> Childs { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        T UpdateIsDelete<T>(bool value, Func<IBaseRepository<TEntity>, List<TEntity>, T> func)
        {
            var childs = Select.WhereDynamic(this).AsTreeCte().ToList();
            childs.Add(this as TEntity);
            var repo = Orm.GetRepository<TEntity>();
            repo.UnitOfWork = _resolveUow?.Invoke();
            repo.Attach(childs);
            foreach (var item in childs)
                (item as BaseEntity).IsDeleted = false;
            return func(repo, childs);
        }
        public override bool Delete(bool physicalDelete = false) => UpdateIsDelete(true, (repo, chis) => repo.Update(chis)) > 0;
        async public override Task<bool> DeleteAsync(bool physicalDelete = false) => await UpdateIsDelete(true, (repo, chis) => repo.UpdateAsync(chis)) > 0;

        public override bool Restore() => UpdateIsDelete(false, (repo, chis) => repo.Update(chis)) > 0;
        async public override Task<bool> RestoreAsync() => await UpdateIsDelete(false, (repo, chis) => repo.UpdateAsync(chis)) > 0;
    }
}