// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： CTS_OPIDK_Entity.cs
// 项目名称：DP.Entity.Entity 
// 创建时间：2013/1/1
// 负责人：
// ===================================================================
using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.OracleClient.Mapping;
using DP.Data.SqlClient.Mapping;
using System.Configuration;


namespace DP.Entity.Entity.Entity
{
    /// <summary>
    ///CTS_OPIDK数据实体
    /// </summary>
    [Serializable]
    [OracleTable(TableName = "CTS_OPIDK", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    [SqlTable(TableName = "CTS_OPIDK", TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString")]
    public class CTS_OPIDK_Entity
    {
        #region 变量定义
        ///<summary>
        ///
        ///</summary>
        private string _gHID = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _lOGIN_NAME = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _rEAL_NAME = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _pYM = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _pASS = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private short? _rOLES;
        ///<summary>
        ///
        ///</summary>
        private int? _lEVELS;
        ///<summary>
        ///
        ///</summary>
        private short _uTYPE;
        ///<summary>
        ///
        ///</summary>
        private string _eXT = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _tEL = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _mOBILENO = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _eMAIL = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _aDDR = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _sEX = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _jOB = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _bIRTH = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _eMAIL_UID = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _eMAIL_PWD = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private DateTime? _lAST_PWD_DATE;
        ///<summary>
        ///
        ///</summary>
        private string _mEMO = String.Empty;
        #endregion

        #region 构造函数
        ///<summary>
        ///
        ///</summary>
        public CTS_OPIDK_Entity()
        {

        }
        #endregion

        #region 公共属性

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "GHID", IsPrimaryKey = true)]
        [SqlColumn(ColumnDescription = "", ColumnName = "GHID", IsPrimaryKey = true)]
        public string GHID
        {
            get { return _gHID; }
            set
            {
                _gHID = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "LOGIN_NAME")]
        [SqlColumn(ColumnDescription = "", ColumnName = "LOGIN_NAME")]
        public string LOGIN_NAME
        {
            get { return _lOGIN_NAME; }
            set
            {
                _lOGIN_NAME = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "REAL_NAME")]
        [SqlColumn(ColumnDescription = "", ColumnName = "REAL_NAME")]
        public string REAL_NAME
        {
            get { return _rEAL_NAME; }
            set
            {
                _rEAL_NAME = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "PYM")]
        [SqlColumn(ColumnDescription = "", ColumnName = "PYM")]
        public string PYM
        {
            get { return _pYM; }
            set
            {
                _pYM = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "PASS")]
        [SqlColumn(ColumnDescription = "", ColumnName = "PASS")]
        public string PASS
        {
            get { return _pASS; }
            set
            {
                _pASS = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "ROLES")]
        [SqlColumn(ColumnDescription = "", ColumnName = "ROLES")]
        public short? ROLES
        {
            get { return _rOLES; }
            set { _rOLES = value; }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "LEVELS")]
        [SqlColumn(ColumnDescription = "", ColumnName = "LEVELS")]
        public int? LEVELS
        {
            get { return _lEVELS; }
            set { _lEVELS = value; }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "UTYPE")]
        [SqlColumn(ColumnDescription = "", ColumnName = "UTYPE")]
        public short UTYPE
        {
            get { return _uTYPE; }
            set { _uTYPE = value; }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "EXT")]
        [SqlColumn(ColumnDescription = "", ColumnName = "EXT")]
        public string EXT
        {
            get { return _eXT; }
            set
            {
                _eXT = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "TEL")]
        [SqlColumn(ColumnDescription = "", ColumnName = "TEL")]
        public string TEL
        {
            get { return _tEL; }
            set
            {
                _tEL = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "MOBILENO")]
        [SqlColumn(ColumnDescription = "", ColumnName = "MOBILENO")]
        public string MOBILENO
        {
            get { return _mOBILENO; }
            set
            {
                _mOBILENO = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "EMAIL")]
        [SqlColumn(ColumnDescription = "", ColumnName = "EMAIL")]
        public string EMAIL
        {
            get { return _eMAIL; }
            set
            {
                _eMAIL = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "ADDR")]
        [SqlColumn(ColumnDescription = "", ColumnName = "ADDR")]
        public string ADDR
        {
            get { return _aDDR; }
            set
            {
                _aDDR = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "SEX")]
        [SqlColumn(ColumnDescription = "", ColumnName = "SEX")]
        public string SEX
        {
            get { return _sEX; }
            set
            {
                _sEX = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "JOB")]
        [SqlColumn(ColumnDescription = "", ColumnName = "JOB")]
        public string JOB
        {
            get { return _jOB; }
            set
            {
                _jOB = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "BIRTH")]
        [SqlColumn(ColumnDescription = "", ColumnName = "BIRTH")]
        public string BIRTH
        {
            get { return _bIRTH; }
            set
            {
                _bIRTH = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "EMAIL_UID")]
        [SqlColumn(ColumnDescription = "", ColumnName = "EMAIL_UID")]
        public string EMAIL_UID
        {
            get { return _eMAIL_UID; }
            set
            {
                _eMAIL_UID = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "EMAIL_PWD")]
        [SqlColumn(ColumnDescription = "", ColumnName = "EMAIL_PWD")]
        public string EMAIL_PWD
        {
            get { return _eMAIL_PWD; }
            set
            {
                _eMAIL_PWD = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "LAST_PWD_DATE")]
        [SqlColumn(ColumnDescription = "", ColumnName = "LAST_PWD_DATE")]
        public DateTime? LAST_PWD_DATE
        {
            get { return _lAST_PWD_DATE; }
            set { _lAST_PWD_DATE = value; }

        }

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(ColumnDescription = "", ColumnName = "MEMO")]
        [SqlColumn(ColumnDescription = "", ColumnName = "MEMO")]
        public string MEMO
        {
            get { return _mEMO; }
            set
            {
                _mEMO = value;
            }

        }


        #endregion

    }
}
