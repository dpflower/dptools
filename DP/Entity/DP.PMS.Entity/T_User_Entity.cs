// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： T_User_Entity.cs
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
    ///T_User数据实体
    /// </summary>
    [Serializable]
    [OracleTable(TableName = "T_User", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    [SqlTable(TableName = "T_User", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    public class T_User_Entity
    {
        #region 变量定义
        ///<summary>
        ///用户编号
        ///</summary>
        private long _userId;
        ///<summary>
        ///登录名
        ///</summary>
        private string _loginName = String.Empty;
        ///<summary>
        ///密码
        ///</summary>
        private string _password = String.Empty;
        ///<summary>
        ///姓名
        ///</summary>
        private string _realName = String.Empty;
        ///<summary>
        ///邮件
        ///</summary>
        private string _email = String.Empty;
        ///<summary>
        ///生日
        ///</summary>
        private DateTime? _birthDay;
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
        ///用户表
        ///</summary>
        public T_User_Entity()
        {

        }
        #endregion

        #region 公共属性

        ///<summary>
        ///用户编号
        ///</summary>		
        [OracleColumn(IsDbGenerated = true, IsPrimaryKey = true, ColumnDescription = "用户编号", ColumnName = "UserId")]
        [SqlColumn(IsPrimaryKey = true, IsDbGenerated = true, ColumnDescription = "用户编号", ColumnName = "UserId")]
        public long UserId
        {
            get { return _userId; }
            set { _userId = value; }

        }

        ///<summary>
        ///登录名
        ///</summary>		
        [OracleColumn(ColumnDescription = "登录名", ColumnName = "LoginName")]
        [SqlColumn(ColumnDescription = "登录名", ColumnName = "LoginName")]
        public string LoginName
        {
            get { return _loginName; }
            set
            {
                _loginName = value;
            }

        }

        ///<summary>
        ///密码
        ///</summary>		
        [OracleColumn(ColumnDescription = "密码", ColumnName = "Password")]
        [SqlColumn(ColumnDescription = "密码", ColumnName = "Password")]
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
            }

        }

        ///<summary>
        ///姓名
        ///</summary>		
        [OracleColumn(ColumnDescription = "姓名", ColumnName = "RealName")]
        [SqlColumn(ColumnDescription = "姓名", ColumnName = "RealName")]
        public string RealName
        {
            get { return _realName; }
            set
            {
                _realName = value;
            }

        }

        ///<summary>
        ///邮件
        ///</summary>		
        [OracleColumn(ColumnDescription = "邮件", ColumnName = "Email")]
        [SqlColumn(ColumnDescription = "邮件", ColumnName = "Email")]
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
            }

        }

        ///<summary>
        ///生日
        ///</summary>		
        [OracleColumn(ColumnDescription = "生日", ColumnName = "BirthDay")]
        [SqlColumn(ColumnDescription = "生日", ColumnName = "BirthDay")]
        public DateTime? BirthDay
        {
            get { return _birthDay; }
            set { _birthDay = value; }

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
