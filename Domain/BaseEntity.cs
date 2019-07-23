using FreeSql;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

[Table(DisableSyncStructure = true)]
public abstract class BaseEntity
{
    private static Lazy<IFreeSql> _ormLazy = new Lazy<IFreeSql>(() =>
    {
        var orm = new FreeSqlBuilder()
            .UseAutoSyncStructure(true)
            .UseNoneCommandParameter(true)
            .UseConnectionString(DataType.Sqlite, "data source=test.db;max pool size=5")
            .Build();
        orm.Aop.CurdBefore += (s, e) => Trace.WriteLine(e.Sql + "\r\n");
        return orm;
    });
    public static IFreeSql Orm => _ormLazy.Value;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
    /// <summary>
    /// 逻辑删除
    /// </summary>
    public bool IsDeleted { get; set; }
}

[Table(DisableSyncStructure = true)]
public abstract class BaseEntity<TEntity> : BaseEntity where TEntity : class
{
    public static ISelect<TEntity> Select => Orm.Select<TEntity>().WhereCascade(a => (a as BaseEntity<TEntity>).IsDeleted == false);
    public static ISelect<TEntity> Where(Expression<Func<TEntity, bool>> exp) => Select.Where(exp);
    public static ISelect<TEntity> WhereIf(bool condition, Expression<Func<TEntity, bool>> exp) => Select.WhereIf(condition, exp);

    [JsonIgnore]
    protected IBaseRepository<TEntity> Repository { get; set; }

    async Task<bool> UpdateIsDeleted(bool value)
    {
        if (this.Repository == null)
            return await Orm.Update<TEntity>(this as TEntity).Set(a => (a as BaseEntity<TEntity>).IsDeleted, this.IsDeleted = value).ExecuteAffrowsAsync() == 1;
        this.IsDeleted = value;
        return await this.Repository.UpdateAsync(this as TEntity) == 1;
    }
    /// <summary>
    /// 删除数据
    /// </summary>
    /// <returns></returns>
    public virtual Task<bool> Delete() => this.UpdateIsDeleted(true);
    /// <summary>
    /// 恢复删除的数据
    /// </summary>
    /// <returns></returns>
    public virtual Task<bool> Restore() => this.UpdateIsDeleted(false);

    /// <summary>
    /// 附加实体，在更新数据时，只更新变化的部分
    /// </summary>
    public TEntity Attach()
    {
        if (this.Repository == null) this.Repository = Orm.GetRepository<TEntity>();
        var item = this as TEntity;
        this.Repository.Attach(item);
        return item;
    }
    /// <summary>
    /// 更新数据
    /// </summary>
    /// <returns></returns>
    async public virtual Task<bool> Update()
    {
        this.UpdateTime = DateTime.Now;
        if (this.Repository == null)
            return await Orm.Update<TEntity>().SetSource(this as TEntity).ExecuteAffrowsAsync() == 1;
        return await this.Repository.UpdateAsync(this as TEntity) == 1;
    }
    /// <summary>
    /// 插入数据
    /// </summary>
    async public virtual Task Insert()
    {
        this.CreateTime = DateTime.Now;
        if (this.Repository == null) this.Repository = Orm.GetRepository<TEntity>();
        await this.Repository.InsertAsync(this as TEntity);
    }

    /// <summary>
    /// 更新或插入
    /// </summary>
    /// <returns></returns>
    async public virtual Task Save()
    {
        this.UpdateTime = DateTime.Now;
        if (this.Repository == null) this.Repository = Orm.GetRepository<TEntity>();
        await this.Repository.InsertOrUpdateAsync(this as TEntity);
    }
}

[Table(DisableSyncStructure = true)]
public abstract class BaseEntity<TEntity, TKey> : BaseEntity<TEntity> where TEntity : class
{
    static BaseEntity()
    {
        var tkeyType = typeof(TKey)?.NullableTypeOrThis();
        if (tkeyType == typeof(int) || tkeyType == typeof(long))
            Orm.CodeFirst.ConfigEntity(typeof(TEntity),
                t => t.Property("Id").IsIdentity(true));
    }

    /// <summary>
    /// 主键
    /// </summary>
    public virtual TKey Id { get; set; }

    /// <summary>
    /// 根据主键值获取数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    async public static Task<TEntity> Find(TKey id)
    {
        var item = await Select.WhereDynamic(id).FirstAsync();
        (item as BaseEntity<TEntity>)?.Attach();
        return item;
    }
}