// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： T_Operation_Entity.cs
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
    ///T_Operation数据实体
    /// </summary>
    [Serializable]
    [OracleTable(TableName = "T_Operation", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    [SqlTable(TableName = "T_Operation", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    public class T_Operation_Entity
    {
        #region 变量定义
        ///<summary>
        ///操作类型编号
        ///</summary>
        private long _operationID;
        ///<summary>
        ///操作类型名称
        ///</summary>
        private string _operationName = String.Empty;
        ///<summary>
        ///操作类型描述
        ///</summary>
        private string _operationDescription = String.Empty;
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
        public T_Operation_Entity()
        {

        }
        #endregion

        #region 公共属性

        ///<summary>
        ///操作类型编号
        ///</summary>		
        [OracleColumn(IsDbGenerated = true, IsPrimaryKey = true, ColumnDescription = "操作类型编号", ColumnName = "OperationID")]
        [SqlColumn(IsPrimaryKey = true, IsDbGenerated = true, ColumnDescription = "操作类型编号", ColumnName = "OperationID")]
        public long OperationID
        {
            get { return _operationID; }
            set { _operationID = value; }

        }

        ///<summary>
        ///操作类型名称
        ///</summary>		
        [OracleColumn(ColumnDescription = "操作类型名称", ColumnName = "OperationName")]
        [SqlColumn(ColumnDescription = "操作类型名称", ColumnName = "OperationName")]
        public string OperationName
        {
            get { return _operationName; }
            set
            {
                _operationName = value;
            }

        }

        ///<summary>
        ///操作类型描述
        ///</summary>		
        [OracleColumn(ColumnDescription = "操作类型描述", ColumnName = "OperationDescription")]
        [SqlColumn(ColumnDescription = "操作类型描述", ColumnName = "OperationDescription")]
        public string OperationDescription
        {
            get { return _operationDescription; }
            set
            {
                _operationDescription = value;
            }

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
