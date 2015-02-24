// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： T_Role_Entity.cs
// 项目名称：DP.PMS 
// 创建时间：2013/3/9
// 负责人：
// ===================================================================
using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.OracleClient.Mapping;
using DP.Data.SqlClient.Mapping;
using System.Configuration;


namespace DP.PMS.Entity
{
    /// <summary>
    ///T_Role数据实体
    /// </summary>
    [Serializable]
    [OracleTable(TableName = "T_Role", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    [SqlTable(TableName = "T_Role", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    public class T_Role_Entity
    {
        #region 变量定义
        ///<summary>
        ///角色编号
        ///</summary>
        private long _roleID;
        ///<summary>
        ///角色名称
        ///</summary>
        private string _roleName = String.Empty;
        ///<summary>
        ///描述
        ///</summary>
        private string _roleDescription = String.Empty;
        ///<summary>
        ///父节点
        ///</summary>
        private long? _parentID;
        ///<summary>
        ///排序
        ///</summary>
        private short? _sort;
        ///<summary>
        ///创建时间
        ///</summary>
        private DateTime? _createTime;
        ///<summary>
        ///修改时间
        ///</summary>
        private DateTime? _updateTime;
        #endregion

        #region 构造函数
        ///<summary>
        ///
        ///</summary>
        public T_Role_Entity()
        {

        }
        #endregion

        #region 公共属性

        ///<summary>
        ///角色编号
        ///</summary>		
        [OracleColumn(IsDbGenerated = true, IsPrimaryKey = true, ColumnDescription = "角色编号", ColumnName = "RoleID")]
        [SqlColumn(IsPrimaryKey = true, IsDbGenerated = true, ColumnDescription = "角色编号", ColumnName = "RoleID")]
        public long RoleID
        {
            get { return _roleID; }
            set { _roleID = value; }

        }

        ///<summary>
        ///角色名称
        ///</summary>		
        [OracleColumn(ColumnDescription = "角色名称", ColumnName = "RoleName")]
        [SqlColumn(ColumnDescription = "角色名称", ColumnName = "RoleName")]
        public string RoleName
        {
            get { return _roleName; }
            set
            {
                _roleName = value;
            }

        }

        ///<summary>
        ///描述
        ///</summary>		
        [OracleColumn(ColumnDescription = "描述", ColumnName = "RoleDescription")]
        [SqlColumn(ColumnDescription = "描述", ColumnName = "RoleDescription")]
        public string RoleDescription
        {
            get { return _roleDescription; }
            set
            {
                _roleDescription = value;
            }

        }

        ///<summary>
        ///父节点
        ///</summary>		
        [OracleColumn(ColumnDescription = "父节点", ColumnName = "ParentID")]
        [SqlColumn(ColumnDescription = "父节点", ColumnName = "ParentID")]
        public long? ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }

        }

        ///<summary>
        ///排序
        ///</summary>		
        [OracleColumn(ColumnDescription = "排序", ColumnName = "Sort")]
        [SqlColumn(ColumnDescription = "排序", ColumnName = "Sort")]
        public short? Sort
        {
            get { return _sort; }
            set { _sort = value; }

        }

        ///<summary>
        ///创建时间
        ///</summary>		
        [OracleColumn(ColumnDescription = "创建时间", ColumnName = "CreateTime")]
        [SqlColumn(ColumnDescription = "创建时间", ColumnName = "CreateTime")]
        public DateTime? CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }

        }

        ///<summary>
        ///修改时间
        ///</summary>		
        [OracleColumn(ColumnDescription = "修改时间", ColumnName = "UpdateTime")]
        [SqlColumn(ColumnDescription = "修改时间", ColumnName = "UpdateTime")]
        public DateTime? UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }

        }


        #endregion

    }
}
