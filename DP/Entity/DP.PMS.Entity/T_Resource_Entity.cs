// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： T_Resource_Entity.cs
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
    ///T_Resource数据实体
    /// </summary>
    [Serializable]
    [OracleTable(TableName = "T_Resource", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    [SqlTable(TableName = "T_Resource", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    public class T_Resource_Entity
    {
        #region 变量定义
        ///<summary>
        ///资源编号
        ///</summary>
        private long _resourceID;
        ///<summary>
        ///资源代码
        ///</summary>
        private string _resourceCode = String.Empty;
        ///<summary>
        ///资源名称
        ///</summary>
        private string _resourceName = String.Empty;
        ///<summary>
        ///资源类型
        ///</summary>
        private byte _resourceType;
        ///<summary>
        ///资源描述
        ///</summary>
        private string _resourceDescription = String.Empty;
        ///<summary>
        ///父节点编码
        ///</summary>
        private long _parentID;
        ///<summary>
        ///唯一标识
        ///</summary>
        private string _resourceURI = String.Empty;
        ///<summary>
        ///Target
        ///</summary>
        private string _resourceTarget = String.Empty;
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
        public T_Resource_Entity()
        {

        }
        #endregion

        #region 公共属性

        ///<summary>
        ///资源编号
        ///</summary>		
        [OracleColumn(IsDbGenerated = true, IsPrimaryKey = true, ColumnDescription = "资源编号", ColumnName = "ResourceID")]
        [SqlColumn(IsPrimaryKey = true, IsDbGenerated = true, ColumnDescription = "资源编号", ColumnName = "ResourceID")]
        public long ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }

        }

        ///<summary>
        ///资源代码
        ///</summary>		
        [OracleColumn(ColumnDescription = "资源代码", ColumnName = "ResourceCode")]
        [SqlColumn(ColumnDescription = "资源代码", ColumnName = "ResourceCode")]
        public string ResourceCode
        {
            get { return _resourceCode; }
            set
            {
                _resourceCode = value;
            }

        }

        ///<summary>
        ///资源名称
        ///</summary>		
        [OracleColumn(ColumnDescription = "资源名称", ColumnName = "ResourceName")]
        [SqlColumn(ColumnDescription = "资源名称", ColumnName = "ResourceName")]
        public string ResourceName
        {
            get { return _resourceName; }
            set
            {
                _resourceName = value;
            }

        }

        ///<summary>
        ///资源类型
        ///</summary>		
        [OracleColumn(ColumnDescription = "资源类型", ColumnName = "ResourceType")]
        [SqlColumn(ColumnDescription = "资源类型", ColumnName = "ResourceType")]
        public byte ResourceType
        {
            get { return _resourceType; }
            set { _resourceType = value; }

        }

        ///<summary>
        ///资源描述
        ///</summary>		
        [OracleColumn(ColumnDescription = "资源描述", ColumnName = "ResourceDescription")]
        [SqlColumn(ColumnDescription = "资源描述", ColumnName = "ResourceDescription")]
        public string ResourceDescription
        {
            get { return _resourceDescription; }
            set
            {
                _resourceDescription = value;
            }

        }

        ///<summary>
        ///父节点编码
        ///</summary>		
        [OracleColumn(ColumnDescription = "父节点编码", ColumnName = "ParentID")]
        [SqlColumn(ColumnDescription = "父节点编码", ColumnName = "ParentID")]
        public long ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }

        }

        ///<summary>
        ///唯一标识
        ///</summary>		
        [OracleColumn(ColumnDescription = "唯一标识", ColumnName = "ResourceURI")]
        [SqlColumn(ColumnDescription = "唯一标识", ColumnName = "ResourceURI")]
        public string ResourceURI
        {
            get { return _resourceURI; }
            set
            {
                _resourceURI = value;
            }

        }

        ///<summary>
        ///Target
        ///</summary>		
        [OracleColumn(ColumnDescription = "Target", ColumnName = "ResourceTarget")]
        [SqlColumn(ColumnDescription = "Target", ColumnName = "ResourceTarget")]
        public string ResourceTarget
        {
            get { return _resourceTarget; }
            set
            {
                _resourceTarget = value;
            }

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
