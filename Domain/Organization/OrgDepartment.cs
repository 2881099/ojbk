using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ojbk.Entities
{
    /// <summary>
    /// 部门
    /// <para></para>
    /// 部门 -> 岗位 -> 员工
    /// </summary>
    public partial class OrgDepartment : BaseEntityTree<OrgDepartment, int>
    {
        
    }
}
