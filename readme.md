# asp.net core ģ�黯��Ŀ�ṹ

ģ�黯��Ŀ�����̳ǣ����֣�CRM����Ƶ�ȵȵȣ�������������������� ojbk ��Ŀ����ӭ��Ҳ��룡

## Module

	����ҵ��ӿ�Լ���� Module ���ֲ��п�������������
    ���Ը��������ļ���appsettings������̬�ļ���wwwroot������������controller������ͼ��views���ȵ�

## WebHost

	WebHost �����ʱ�򣬻Ὣ Module/* ���������Ƶ���ǰĿ¼
	WebHost ֻ��������������ʱ���������Ӧ�� Module
	WebHost ���� npm ���밲װ node������Ŀ¼ִ�� npm install
	WebHost ���� gulp-cli����ִ��ȫ�ְ�װ npm install --global gulp-cli
	���в��裺
    1���� vs �һ� Module Ŀ¼ȫ�����룻
    2��cd WebHost && npm install && dotnet build && dotnet run

> ���ɽ������ʱ��Ӧ���ȱ��� Module �µ���Ŀ�����ִ�� WebHost + gulp-cli
    
> ������ģ���˳�򲻶���զ�죿�Ƴ� WebHost �󱣴�������������� WebHost���������̾Ͷ���

## Infrastructure

	Module ����ÿ����ģ�����������

## Domain

	BaseEntity ����ʵ��ķ�װ

# �����ص�

- �Զ�Ǩ��ʵ��ṹ��CodeFirst���������ݿ⣻

- ֱ�Ӳ���ʵ��ķ��������� CRUD ������

- ���û�����ʵ�����ͣ�ʡȥ�����������ֶε����ã���CreateTime��UpdateTime����

- ʵ�ֵ�������ѯ����ɾ���߼���

# ���� CRUD

```csharp
//���
var item = new UserGroup { GroupName = "��һ" };
item.Insert();

//����
item.GroupName = "���";
item.Update();

//��ӻ����
item.Save();

//��ɾ��
item.Delete();

//�ָ���ɾ��
item.Restore();

//����������ȡ����
var item = UserGroup.Find(1);

//��ѯ����
var items = UserGroup.Where(a => a.Id > 10).ToList();
```

ʵ������.Select ��һ����ѯ����ʹ�÷����� FreeSql.ISelect һ����

֧�ֶ���ѯʱ����ɾ�������ḽ����ÿ�����У�

> �йظ����ѯ��������ο����ϣ�https://github.com/2881099/FreeSql/wiki/%e6%9f%a5%e8%af%a2

ʾ����Ŀ��https://github.com/2881099/FreeSql/tree/master/Examples/base_entity