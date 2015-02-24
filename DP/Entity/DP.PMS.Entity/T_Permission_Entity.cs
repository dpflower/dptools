// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： T_Permission_Entity.cs
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
    ///T_Permission数据实体
    /// </summary>
    [Serializable]
    [OracleTable(TableName = "T_Permission", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    [SqlTable(TableName = "T_Permission", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    public class T_Permission_Entity
    {
        #region 变量定义
        ///<summary>
        ///自增编号
        ///</summary>
        private long _permissionID;
        ///<summary>
        ///角色编号
        ///</summary>
        private long _roleID;
        ///<summary>
        ///资源编号
        ///</summary>
        private long _resourceID;
        ///<summary>
        ///创建时间
        ///</summary>
        private DateTime? _createTime;
        #endregion

        #region 构造函数
        ///<summary>
        ///
        ///</summary>
        public T_Permission_Entity()
        {

        }
        #endregion

        #region 公共属性

        ///<summary>
        ///自增编号
        ///</summary>		
        [OracleColumn(IsDbGenerated = true, IsPrimaryKey = true, ColumnDescription = "自增编号", ColumnName = "PermissionID")]
        [SqlColumn(IsPrimaryKey = true, IsDbGenerated = true, ColumnDescription = "自增编号", ColumnName = "PermissionID")]
        public long PermissionID
        {
            get { return _permissionID; }
            set { _permissionID = value; }

        }

        ///<summary>
        ///角色编号
        ///</summary>		
        [OracleColumn(ColumnDescription = "角色编号", ColumnName = "RoleID")]
        [SqlColumn(ColumnDescription = "角色编号", ColumnName = "RoleID")]
        public long RoleID
        {
            get { return _roleID; }
            set { _roleID = value; }

        }

        ///<summary>
        ///资源编号
        ///</summary>		
        [OracleColumn(ColumnDescription = "资源编号", ColumnName = "ResourceID")]
        [SqlColumn(ColumnDescription = "资源编号", ColumnName = "ResourceID")]
        public long ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }

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


        #endregion

    }
}
