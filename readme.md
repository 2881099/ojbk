# asp.net core 模块化项目结构

模块化项目，集商城，音乐，CRM，视频等等等，随意想做就做，这就是 ojbk 项目，欢迎大家参与！

## Module

	所有业务接口约定在 Module 划分并行开发，互不依赖
    可以隔离配置文件（appsettings）、静态文件（wwwroot）、控制器（controller）、视图（views）等等

## WebHost

	WebHost 编译的时候，会将 Module/* 编译结果复制到当前目录
	WebHost 只当做主引擎运行时按需加载相应的 Module
	WebHost 依赖 npm ，请安装 node，并在目录执行 npm install
	WebHost 依赖 gulp-cli，请执行全局安装 npm install --global gulp-cli
	运行步骤：
    1、打开 vs 右击 Module 目录全部编译；
    2、cd WebHost && npm install && dotnet build && dotnet run

> 生成解决方案时，应该先编译 Module 下的项目，最后执行 WebHost + gulp-cli
    
> 增加新模块后顺序不对了咋办？移除 WebHost 后保存解决方案，再添加 WebHost，编译流程就对了

## Infrastructure

	Module 里面每个子模块的依赖所需

## Domain

	BaseEntity 所有实体的封装

# 功能特点

- 自动迁移实体结构（CodeFirst），到数据库；

- 直接操作实体的方法，进行 CRUD 操作；

- 简化用户定义实体类型，省去主键、常用字段的配置（如CreateTime、UpdateTime）；

- 实现单表、多表查询的软删除逻辑；

# 极简单 CRUD

```csharp
//添加
var item = new UserGroup { GroupName = "组一" };
item.Insert();

//更新
item.GroupName = "组二";
item.Update();

//添加或更新
item.Save();

//软删除
item.Delete();

//恢复软删除
item.Restore();

//根据主键获取对象
var item = UserGroup.Find(1);

//查询数据
var items = UserGroup.Where(a => a.Id > 10).ToList();
```

实体类型.Select 是一个查询对象，使用方法和 FreeSql.ISelect 一样；

支持多表查询时，软删除条件会附加在每个表中；

> 有关更多查询方法，请参考资料：https://github.com/2881099/FreeSql/wiki/%e6%9f%a5%e8%af%a2

示范项目：https://github.com/2881099/FreeSql/tree/master/Examples/base_entity