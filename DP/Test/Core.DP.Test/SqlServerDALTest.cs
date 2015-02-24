using DP.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Common;
using DP.Data.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DP.Entity.Entity.Entity;
using VoiceMail.Entity;

namespace Core.DP.Test
{
    
    
    /// <summary>
    ///这是 SqlServerDALTest 的测试类，旨在
    ///包含所有 SqlServerDALTest 单元测试
    ///</summary>
    [TestClass()]
    public class SqlServerDALTest
    {

        //string SQLConnString = "Data Source=.;Initial Catalog=CallThink_sheitc;User ID=sa;Password=1qaz2wsx;";
        string SQLConnString = "Data Source=(local);Initial Catalog=VoiceMail;User ID=sa;Password=1qaz2wsx;";
        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext  
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void GetListTest_User()
        {
            SqlServerDAL target = new SqlServerDAL(SQLConnString); // TODO: 初始化为适当的值

            var actual = target.GetEntity<UserEntity>("LoginName", "admin");
            //Assert.AreNotEqual(expected, actual);
        }


     

        [TestMethod()]
        public void DeleteTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "1111";
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(trans, obj, tableName);
            trans.Commit();
            Assert.AreEqual(expected, actual);
        }

     

        [TestMethod()]
        public void DeleteTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "1111"));
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(conditions);
            Assert.AreEqual(expected, actual);
        }

  
        [TestMethod()]
        public void DeleteTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "1111"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(conditions, tableName);
            Assert.AreEqual(expected, actual);
        }

  
        [TestMethod()]
        public void DeleteTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "1111";
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(obj, tableName);
            Assert.AreEqual(expected, actual);
        }

    

        [TestMethod()]
        public void DeleteTest4()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "1111";
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(obj);
            Assert.AreEqual(expected, actual);
        }

        

        [TestMethod()]
        public void DeleteTest5()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "1111";
            int expected = 0; // TODO: 初始化为适当的值
            int actual = 0;

            try
            {
                actual = target.Delete<CTS_OPIDK_Entity>(trans, obj);
                target.TransactionCommit(trans);
            }
            catch (Exception ex)
            {
                target.TransactionRollBack(trans);
            }
            Assert.AreEqual(expected, actual);
        }

        

        [TestMethod()]
        public void DeleteTest6()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "1111"; // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(Field, Value, tableName);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void DeleteTest7()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "1111"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(trans, Field, Value);
            trans.Commit();
            Assert.AreEqual(expected, actual);
        }

      

        [TestMethod()]
        public void DeleteTest8()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "1111"; // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(trans, Field, Value, tableName);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void DeleteTest9()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "1111"));
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(trans, conditions);
            trans.Commit();
            Assert.AreEqual(expected, actual);
        }

       

        [TestMethod()]
        public void DeleteTest10()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "1111"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(trans, conditions, tableName);
            trans.Commit();
            Assert.AreEqual(expected, actual);
        }

        
        [TestMethod()]
        public void DeleteTest11()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "1111"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(Field, Value);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///ExecuteDataAdapterSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteDataAdapterSQLTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.ExecuteDataAdapterSQL(SQLString);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteDataAdapterSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteDataAdapterSQLTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.ExecuteDataAdapterSQL(SQLString, commandType);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteDataAdapterSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteDataAdapterSQLTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            DbParameter[] commandParameters = null; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.ExecuteDataAdapterSQL(SQLString, commandType, commandParameters);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteDataAdapterSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteDataAdapterSQLTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            SqlParameter[] commandParameters = null; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.ExecuteDataAdapterSQL(SQLString, commandType, commandParameters);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual = 0;

            try
            {
                actual = target.ExecuteSQL(trans, SQLString, commandType);
                target.TransactionCommit(trans);
            }
            catch (Exception ex)
            {
                target.TransactionRollBack(trans);
            }
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            SqlParameter[] commandParameters = null; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual = 0;
            try
            {
                actual = target.ExecuteSQL(trans, SQLString, commandType, commandParameters);
                target.TransactionCommit(trans);
            }
            catch (Exception ex)
            {
                target.TransactionRollBack(trans);
            }
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.ExecuteSQL(SQLString, commandType);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.ExecuteSQL(SQLString);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest4()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            DbParameter[] commandParameters = null; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.ExecuteSQL(trans, SQLString, commandType, commandParameters);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest5()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.ExecuteSQL(trans, SQLString);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest6()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            SqlParameter[] commandParameters = null; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.ExecuteSQL(SQLString, commandType, commandParameters);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteSQLTest7()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK "; // TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            DbParameter[] commandParameters = null; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.ExecuteSQL(SQLString, commandType, commandParameters);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK (nolock) ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(SQLString, commandType);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK  (nolock) ";// TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(SQLString);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK  (nolock) ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            DbParameter[] commandParameters = null; // TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(trans, SQLString, commandType, commandParameters);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK  (nolock) ";// TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(trans, SQLString);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest4()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK  (nolock) ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(trans, SQLString, commandType);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest5()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK  (nolock) ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            DbParameter[] commandParameters = null; // TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(SQLString, commandType, commandParameters);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest6()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string SQLString = "select count(*) from CTS_OPIDK  (nolock) ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            SqlParameter[] commandParameters = null; // TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(SQLString, commandType, commandParameters);
            Assert.AreNotEqual(expected, actual);
            
        }

        /// <summary>
        ///ExecuteScalarSQL 的测试
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarSQLTest7()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            string SQLString = "select count(*) from CTS_OPIDK  (nolock) ";// TODO: 初始化为适当的值
            CommandType commandType = new CommandType(); // TODO: 初始化为适当的值
            SqlParameter[] commandParameters = null; // TODO: 初始化为适当的值
            object expected = null; // TODO: 初始化为适当的值
            object actual;
            actual = target.ExecuteScalarSQL(trans, SQLString, commandType, commandParameters);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
            
        }

       

        [TestMethod()]
        public void GetCountTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            Query query = new Query(); // TODO: 初始化为适当的值
            query.Conditions.Add(new Condition("GHID", "8600"));
            long expected = 0; // TODO: 初始化为适当的值
            long actual;
            actual = target.GetCount<CTS_OPIDK_Entity>(query);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void GetDataTableTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            Query query = new Query(); // TODO: 初始化为适当的值
            query.TableName = "CTS_OPIDK";
            query.Conditions.Add(new Condition("GHID", "8600"));
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetDataTable(query, SQLConnString, "");
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///GetDateToDbSelectString 的测试
        ///</summary>
        [TestMethod()]
        public void GetDateToDbSelectStringTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            DateTime date = new DateTime(); // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetDateToDbSelectString(date);
            Assert.AreNotEqual(expected, actual);
            
        }

      

        [TestMethod()]
        public void GetDistinctCountTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Fields = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            long expected = 0; // TODO: 初始化为适当的值
            long actual;
            actual = target.GetDistinctCount<CTS_OPIDK_Entity>(Fields, conditions);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetDistinctCountTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Fields = "GHID"; // TODO: 初始化为适当的值
            long expected = 0; // TODO: 初始化为适当的值
            long actual;
            actual = target.GetDistinctCount<CTS_OPIDK_Entity>(Fields);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void GetDistinctCountTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Fields = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            long expected = 0; // TODO: 初始化为适当的值
            long actual;
            actual = target.GetDistinctCount<CTS_OPIDK_Entity>(Fields, conditions, tableName);
            Assert.AreNotEqual(expected, actual);
        }

        

        [TestMethod()]
        public void GetDistinctCountTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Fields = "GHID"; // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            long expected = 0; // TODO: 初始化为适当的值
            long actual;
            actual = target.GetDistinctCount<CTS_OPIDK_Entity>(Fields, tableName);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void GetDistinctTableTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string Fields = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetDistinctTable<CTS_OPIDK_Entity>(Fields, conditions);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void GetDistinctTableTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Fields = "GHID"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetDistinctTable<CTS_OPIDK_Entity>(Fields);
            Assert.AreNotEqual(expected, actual);
        }

     

        [TestMethod()]
        public void GetDistinctTableTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            string Fields = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetDistinctTable<CTS_OPIDK_Entity>(Fields, conditions, tableName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetDistinctTableTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Fields = "GHID"; // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetDistinctTable<CTS_OPIDK_Entity>(Fields, tableName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetEntityTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600")); 
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            CTS_OPIDK_Entity expected = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            expected.GHID = "8600";
            CTS_OPIDK_Entity actual;
            actual = target.GetEntity<CTS_OPIDK_Entity>(conditions, tableName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetEntityTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "8600"; // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            CTS_OPIDK_Entity expected = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            expected.GHID = "8600";
            CTS_OPIDK_Entity actual;
            actual = target.GetEntity<CTS_OPIDK_Entity>(Field, Value, tableName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetEntityTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            CTS_OPIDK_Entity expected = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            expected.GHID = "8600";
            CTS_OPIDK_Entity actual;
            actual = target.GetEntity<CTS_OPIDK_Entity>(conditions);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetEntityTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "8600"; // TODO: 初始化为适当的值
            CTS_OPIDK_Entity expected = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            expected.GHID = "8600";
            CTS_OPIDK_Entity actual;
            actual = target.GetEntity<CTS_OPIDK_Entity>(Field, Value);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetFieldListTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string FieldName = "GHID"; // TODO: 初始化为适当的值
            string TableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetFieldList<CTS_OPIDK_Entity>(FieldName, TableName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetFieldListTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string FieldName = "GHID"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetFieldList<CTS_OPIDK_Entity>(FieldName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetListTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            Query query = new Query(); // TODO: 初始化为适当的值
            query.Conditions.Add(new Condition("GHID", "8600"));
            query.Conditions.Add(new Condition("ROLES", 1, QueryOperator.In));
            query.Conditions.Add(new Condition("LEVELS", new int[] { 1, 7, 6, 5, 3, 2 }));
            query.Conditions.Add(new Condition("MOBILENO", "132", QueryOperator.Like));
            query.ConditionString = " and JOB like @JOB+'%'";
            query.DbParameters.Add(new SqlParameter("@JOB", "工程"));
            List<CTS_OPIDK_Entity> expected = null; // TODO: 初始化为适当的值
            List<CTS_OPIDK_Entity> actual;
            actual = target.GetList<CTS_OPIDK_Entity>(query);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetMaxValueTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMaxValue<CTS_OPIDK_Entity>(Field, tableName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void GetMaxValueTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMaxValue<CTS_OPIDK_Entity>(Field, conditions, tableName);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void GetMaxValueTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMaxValue<CTS_OPIDK_Entity>(Field);
            Assert.AreNotEqual(expected, actual);
        }

        

        [TestMethod()]
        public void GetMaxValueTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMaxValue<CTS_OPIDK_Entity>(Field, conditions);
            Assert.AreEqual(expected, actual);
        }

      

        [TestMethod()]
        public void GetMinValueTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMinValue<CTS_OPIDK_Entity>(Field, conditions);
            Assert.AreEqual(expected, actual);
        }

        

        [TestMethod()]
        public void GetMinValueTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMinValue<CTS_OPIDK_Entity>(Field, tableName);
            Assert.AreNotEqual(expected, actual);
        }

      
        [TestMethod()]
        public void GetMinValueTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMinValue<CTS_OPIDK_Entity>(Field);
            Assert.AreNotEqual(expected, actual);
        }

        

        [TestMethod()]
        public void GetMinValueTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMinValue<CTS_OPIDK_Entity>(Field, conditions, tableName);
            Assert.AreEqual(expected, actual);
        }
                

      

        [TestMethod()]
        public void GetTableListTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string TableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetTableList<CTS_OPIDK_Entity>(TableName);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void GetViewListTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string ViewName = string.Empty; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetViewList<CTS_OPIDK_Entity>(ViewName);
            Assert.AreNotEqual(expected, actual);
            
        }

      

        [TestMethod()]
        public void InsertTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = DateTime.Now.ToString("HHmmssfff");
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            int expected = 1; // TODO: 初始化为适当的值
            int actual;
            actual = target.Insert<CTS_OPIDK_Entity>(obj);
            Assert.AreEqual(expected, actual);
        }

      

        [TestMethod()]
        public void InsertTest1()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //target.ConnectionString = SQLConnString;
            //string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            //string[] primaryKeys = new string[] {"GHID"}; // TODO: 初始化为适当的值
            //PrimaryKeyType primaryKeyType = PrimaryKeyType.Other; // TODO: 初始化为适当的值
            
            //Dictionary<string, string> dicts = new Dictionary<string,string>(); // TODO: 初始化为适当的值
            //dicts.Add("GHID", "8600");
            //dicts.Add("LAST_PWD_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //string primaryValues = string.Empty; // TODO: 初始化为适当的值
            //string primaryValuesExpected = string.Empty; // TODO: 初始化为适当的值
            //int expected = 0; // TODO: 初始化为适当的值
            //int actual;
            //actual = target.Insert<CTS_OPIDK_Entity>(tableName, primaryKeys, primaryKeyType, dicts, out primaryValues);
            //Assert.AreNotEqual(primaryValuesExpected, primaryValues);
            //Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void InsertTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = DateTime.Now.ToString("HHmmssfff");
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 1; // TODO: 初始化为适当的值
            int actual;
            actual = target.Insert<CTS_OPIDK_Entity>(trans, obj, tableName);
            trans.Commit();
            Assert.AreEqual(expected, actual);
            
        }

      

        [TestMethod()]
        public void InsertTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = DateTime.Now.ToString("HHmmssfff");
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 1; // TODO: 初始化为适当的值
            int actual;
            actual = target.Insert<CTS_OPIDK_Entity>(obj, tableName);
            Assert.AreEqual(expected, actual);
        }

        

        [TestMethod()]
        public void InsertTest4()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = DateTime.Now.ToString("HHmmssfff");
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            int expected = 1; // TODO: 初始化为适当的值
            int actual;
            actual = target.Insert<CTS_OPIDK_Entity>(trans, obj);
            trans.Commit();
            Assert.AreEqual(expected, actual);
        }

       

        [TestMethod()]
        public void IsExistTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "8600"; // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.IsExist<CTS_OPIDK_Entity>(Field, Value, tableName);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void IsExistTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            string Value = "8600"; // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.IsExist<CTS_OPIDK_Entity>(Field, Value);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void IsExistTest2()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.IsExist<CTS_OPIDK_Entity>(conditions);
            Assert.AreNotEqual(expected, actual);
        }

        

        [TestMethod()]
        public void IsExistTest3()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.IsExist<CTS_OPIDK_Entity>(conditions, tableName);
            Assert.AreNotEqual(expected, actual);
        }

      

        [TestMethod()]
        public void IsExistFieldTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string FieldName = "GHID"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistField<CTS_OPIDK_Entity>(FieldName);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void IsExistFieldTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string FieldName = "GHID"; // TODO: 初始化为适当的值
            string TableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistField<CTS_OPIDK_Entity>(FieldName, TableName);
            Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void IsExistTableTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistTable<CTS_OPIDK_Entity>();
            Assert.AreNotEqual(expected, actual);
        }

        

        [TestMethod()]
        public void IsExistTableTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string TableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistTable<CTS_OPIDK_Entity>(TableName);
            Assert.AreNotEqual(expected, actual);
        }

        
        [TestMethod()]
        public void IsExistViewTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistView<CTS_OPIDK_Entity>();
            Assert.AreEqual(expected, actual);
        }

      

        [TestMethod()]
        public void IsExistViewTest1()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string ViewName = "V_CTS_OPIDK"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistView<CTS_OPIDK_Entity>(ViewName);
            Assert.AreNotEqual(expected, actual);
        }

        

        

        [TestMethod()]
        public void UpdateTest()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            string[] primaryKeys = null; // TODO: 初始化为适当的值
            Dictionary<string, string> dicts = null; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual = 0;
            //actual = target.Update<CTS_OPIDK_Entity>(tableName, primaryKeys, dicts);
            Assert.AreEqual(expected, actual);
        }

       

        [TestMethod()]
        public void UpdateTest1()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //Dictionary<string, object> Sets = new Dictionary<string, object>(); // TODO: 初始化为适当的值
            //Sets.Add("LAST_PWD_DATE", DateTime.Now);
            //List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            //conditions.Add(new Condition("GHID", "8600"));
            //string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            //int expected = 0; // TODO: 初始化为适当的值
            //int actual;
            //actual = target.Update<CTS_OPIDK_Entity>(Sets, conditions, tableName);
            //Assert.AreNotEqual(expected, actual);
        }

        

        [TestMethod()]
        public void UpdateTest2()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //Dictionary<string, object> Sets = new Dictionary<string, object>(); // TODO: 初始化为适当的值
            //Sets.Add("LAST_PWD_DATE", DateTime.Now);
            //List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            //conditions.Add(new Condition("GHID", "8600"));
            //int expected = 0; // TODO: 初始化为适当的值
            //int actual;
            //actual = target.Update<CTS_OPIDK_Entity>(Sets, conditions);
            //Assert.AreNotEqual(expected, actual);
        }

       

        [TestMethod()]
        public void UpdateTest3()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //target.ConnectionString = SQLConnString;
            //DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            //Dictionary<string, object> Sets = new Dictionary<string, object>(); // TODO: 初始化为适当的值
            //Sets.Add("LAST_PWD_DATE", DateTime.Now);
            //List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            //conditions.Add(new Condition("GHID", "8600"));
            //string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            //int expected = 0; // TODO: 初始化为适当的值
            //int actual;
            //actual = target.Update<CTS_OPIDK_Entity>(trans, Sets, conditions, tableName);
            //trans.Commit();
            //Assert.AreNotEqual(expected, actual);
        }

     

        [TestMethod()]
        public void UpdateTest4()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //target.ConnectionString = SQLConnString;
            //DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            //Dictionary<string, object> Sets = new Dictionary<string, object>(); // TODO: 初始化为适当的值
            //Sets.Add("LAST_PWD_DATE", DateTime.Now);
            //List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            //conditions.Add(new Condition("GHID", "8600"));
            //int expected = 0; // TODO: 初始化为适当的值
            //int actual;
            //actual = target.Update<CTS_OPIDK_Entity>(trans, Sets, conditions);
            //trans.Commit();
            //Assert.AreNotEqual(expected, actual);
        }

      

        [TestMethod()]
        public void UpdateTest5()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "8601";
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            obj.LOGIN_NAME = "坐席01";
            obj.REAL_NAME = "坐席01";
            obj.ROLES = 5;
            obj.LEVELS = 7;
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Update<CTS_OPIDK_Entity>(trans, obj);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
        }

        

        [TestMethod()]
        public void UpdateTest6()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "8601";
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            obj.LOGIN_NAME = "坐席01";
            obj.REAL_NAME = "坐席01";
            obj.ROLES = 5;
            obj.LEVELS = 7;
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Update<CTS_OPIDK_Entity>(obj);
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void UpdateTest7()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            target.ConnectionString = SQLConnString;
            DbTransaction trans = target.BeginTransaction(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "8601";
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            obj.LOGIN_NAME = "坐席01";
            obj.REAL_NAME = "坐席01";
            obj.ROLES = 5;
            obj.LEVELS = 7;
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Update<CTS_OPIDK_Entity>(trans, obj, tableName);
            trans.Commit();
            Assert.AreNotEqual(expected, actual);
        }


        [TestMethod()]
        public void UpdateTest8()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "8601";
            obj.UTYPE = 1;
            obj.LAST_PWD_DATE = DateTime.Now;
            obj.LOGIN_NAME = "坐席01";
            obj.REAL_NAME = "坐席01";
            obj.ROLES = 5;
            obj.LEVELS = 7;
            string tableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Update<CTS_OPIDK_Entity>(obj, tableName);
            Assert.AreNotEqual(expected, actual);
        }
    }
}
