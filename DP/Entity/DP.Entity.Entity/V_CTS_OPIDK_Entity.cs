// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： V_CTS_OPIDK_Entity.cs
// 项目名称：DP.Entity.Entity 
// 创建时间：2013/3/9
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
    ///V_CTS_OPIDK数据实体
    /// </summary>
    [Serializable]
    [OracleTable(TableName = "V_CTS_OPIDK", TableOrView = DP.Data.Common.TableType.View, ConnectionStringKey = "SQLConnString")]
    [SqlTable(TableName = "V_CTS_OPIDK", TableOrView = DP.Data.Common.TableType.View, ConnectionStringKey = "SQLConnString")]
    public class V_CTS_OPIDK_Entity
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
        ///<summary>
        ///
        ///</summary>
        private string _pBXUID = String.Empty;
        #endregion

        #region 构造函数
        ///<summary>
        ///
        ///</summary>
        public V_CTS_OPIDK_Entity()
        {

        }
        #endregion

        #region 公共属性

        ///<summary>
        ///
        ///</summary>		
        [OracleColumn(IsPrimaryKey = true)]
        [SqlColumn(IsPrimaryKey = true)]
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
        public short? ROLES
        {
            get { return _rOLES; }
            set { _rOLES = value; }

        }

        ///<summary>
        ///
        ///</summary>		
        public int? LEVELS
        {
            get { return _lEVELS; }
            set { _lEVELS = value; }

        }

        ///<summary>
        ///
        ///</summary>		
        public short UTYPE
        {
            get { return _uTYPE; }
            set { _uTYPE = value; }

        }

        ///<summary>
        ///
        ///</summary>		
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
        public DateTime? LAST_PWD_DATE
        {
            get { return _lAST_PWD_DATE; }
            set { _lAST_PWD_DATE = value; }

        }

        ///<summary>
        ///
        ///</summary>		
        public string MEMO
        {
            get { return _mEMO; }
            set
            {
                _mEMO = value;
            }

        }

        ///<summary>
        ///
        ///</summary>		
        public string PBXUID
        {
            get { return _pBXUID; }
            set
            {
                _pBXUID = value;
            }

        }


        #endregion

    }
}
