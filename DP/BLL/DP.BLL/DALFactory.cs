// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： DALFactory.cs
// 项目名称：DP 
// 创建时间：2013/3/9
// 负责人：
// ===================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using DP.Web;
using DP.Common;
using DP.Data.OracleClient;
using DP.Data.Common;
using System.Configuration;
using DP.Data.SqlClient;


namespace DP.BLL
{
    /// <summary>
    /// 数据工厂类 DALFactory.
    /// </summary>
    public class DALFactory
    {
        public static DAL CreateDal()
        {
            return CreateDal("");
        }

        public static DAL CreateDal(string connectionString)
        {
            string DataSourceType = "SqlServer";
            try
            {
                DataSourceType = ConfigurationManager.AppSettings["DataSourceType"].ToString();
            }
            catch (Exception ex)
            {
            }
            DAL dal = null;
            switch (DataSourceType)
            {
                case "Oracle":
                    {
                        //dal = new OracleDAL();
                        //dal.ConnectionString = connectionString;
                    }
                    break;
                case "SqlServer":
                    {
                        dal = new SqlServerDAL();
                        dal.ConnectionString = connectionString;
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
            return dal;

        }
    }
}
