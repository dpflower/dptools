using DP.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Common;
using DP.Data.Common;
using System.Collections.Generic;
using System.Data;
using DP.Entity.Entity.Entity;

namespace TestProject
{
    
    
    /// <summary>
    ///这是 SqlServerDALTest 的测试类，旨在
    ///包含所有 SqlServerDALTest 单元测试
    ///</summary>
    [TestClass()]
    public class SqlServerDALTest
    {


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


        /// <summary>
        ///Delete 的测试
        ///</summary>
        public void DeleteTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            CTS_OPIDK_Entity obj = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            obj.GHID = "1111";            
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.Delete<CTS_OPIDK_Entity>(obj);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void DeleteTest()
        {
            DeleteTestHelper<CTS_OPIDK_Entity>();
        }

        

        /// <summary>
        ///GetCount 的测试
        ///</summary>
        public void GetCountTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            Query query = new Query(); // TODO: 初始化为适当的值
            query.Conditions.Add(new Condition("GHID", "8600"));
            long expected = 0; // TODO: 初始化为适当的值
            long actual;
            actual = target.GetCount<CTS_OPIDK_Entity>(query);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetCountTest()
        {
            GetCountTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetDataTable 的测试
        ///</summary>
        public void GetDataTableTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            Query query = new Query(); // TODO: 初始化为适当的值
            query.Conditions.Add(new Condition("GHID", "8600"));
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetDataTable<CTS_OPIDK_Entity>(query);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetDataTableTest()
        {
            GetDataTableTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetDistinctCount 的测试
        ///</summary>
        public void GetDistinctCountTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Fields = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            long expected = 0; // TODO: 初始化为适当的值
            long actual;
            actual = target.GetDistinctCount<T>(Fields, conditions);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetDistinctCountTest()
        {
            GetDistinctCountTestHelper<CTS_OPIDK_Entity>();
        }


        /// <summary>
        ///GetEntity 的测试
        ///</summary>
        public void GetEntityTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            CTS_OPIDK_Entity expected = new CTS_OPIDK_Entity(); // TODO: 初始化为适当的值
            expected.GHID = "8600";
            CTS_OPIDK_Entity actual;
            actual = target.GetEntity<CTS_OPIDK_Entity>(conditions);
            Assert.AreEqual(expected.GHID, actual.GHID);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetEntityTest()
        {
            GetEntityTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetFieldList 的测试
        ///</summary>
        public void GetFieldListTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string FieldName = "GHID"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetFieldList<CTS_OPIDK_Entity>(FieldName);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetFieldListTest()
        {
            GetFieldListTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetList 的测试
        ///</summary>
        public void GetListTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            Query query = new Query(); // TODO: 初始化为适当的值
            query.Conditions.Add(new Condition("GHID", "8600"));
            List<CTS_OPIDK_Entity> expected = null; // TODO: 初始化为适当的值
            List<CTS_OPIDK_Entity> actual;
            actual = target.GetList<CTS_OPIDK_Entity>(query);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetListTest()
        {
            GetListTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetMaxValue 的测试
        ///</summary>
        public void GetMaxValueTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMaxValue<CTS_OPIDK_Entity>(Field, conditions);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetMaxValueTest()
        {
            GetMaxValueTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetMinValue 的测试
        ///</summary>
        public void GetMinValueTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string Field = "GHID"; // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            string expected = "8600"; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetMinValue<CTS_OPIDK_Entity>(Field, conditions);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetMinValueTest()
        {
            GetMinValueTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetTableList 的测试
        ///</summary>
        public void GetTableListTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string TableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetTableList<CTS_OPIDK_Entity>(TableName);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetTableListTest()
        {
            GetTableListTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///GetViewList 的测试
        ///</summary>
        public void GetViewListTestHelper<T>()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //string ViewName = string.Empty; // TODO: 初始化为适当的值
            //DataTable expected = null; // TODO: 初始化为适当的值
            //DataTable actual;
            //actual = target.GetViewList<T>(ViewName);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void GetViewListTest()
        {
            GetViewListTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///Insert 的测试
        ///</summary>
        public void InsertTestHelper<T>()
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
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void InsertTest()
        {
            InsertTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///IsExist 的测试
        ///</summary>
        public void IsExistTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            conditions.Add(new Condition("GHID", "8600"));
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.IsExist<CTS_OPIDK_Entity>(conditions);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void IsExistTest()
        {
            IsExistTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///IsExistField 的测试
        ///</summary>
        public void IsExistFieldTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string FieldName = "GHID"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistField<CTS_OPIDK_Entity>(FieldName);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void IsExistFieldTest()
        {
            IsExistFieldTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///IsExistField 的测试
        ///</summary>
        public void IsExistFieldTest1Helper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string FieldName = "GHID"; // TODO: 初始化为适当的值
            string TableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistField<CTS_OPIDK_Entity>(FieldName, TableName);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void IsExistFieldTest1()
        {
            IsExistFieldTest1Helper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///IsExistTable 的测试
        ///</summary>
        public void IsExistTableTestHelper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistTable<CTS_OPIDK_Entity>();
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void IsExistTableTest()
        {
            IsExistTableTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///IsExistTable 的测试
        ///</summary>
        public void IsExistTableTest1Helper<T>()
        {
            SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            string TableName = "CTS_OPIDK"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.IsExistTable<CTS_OPIDK_Entity>(TableName);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void IsExistTableTest1()
        {
            IsExistTableTest1Helper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///IsExistView 的测试
        ///</summary>
        public void IsExistViewTestHelper<T>()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //bool expected = false; // TODO: 初始化为适当的值
            //bool actual;
            //actual = target.IsExistView<T>();
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void IsExistViewTest()
        {
            IsExistViewTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///IsExistView 的测试
        ///</summary>
        public void IsExistViewTest1Helper<T>()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //string ViewName = string.Empty; // TODO: 初始化为适当的值
            //bool expected = false; // TODO: 初始化为适当的值
            //bool actual;
            //actual = target.IsExistView<T>(ViewName);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void IsExistViewTest1()
        {
            IsExistViewTest1Helper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///Update 的测试
        ///</summary>
        public void UpdateTestHelper<T>()
        {
            //SqlServerDAL target = new SqlServerDAL(); // TODO: 初始化为适当的值
            //Dictionary<string, object> Sets = new Dictionary<string,object>(); // TODO: 初始化为适当的值
            //Sets.Add("LAST_PWD_DATE", DateTime.Now);
            //List<Condition> conditions = new List<Condition>(); // TODO: 初始化为适当的值
            //conditions.Add(new Condition("GHID", "8600"));
            //int expected = 0; // TODO: 初始化为适当的值
            //int actual;
            //actual = target.Update<CTS_OPIDK_Entity>(Sets, conditions);
            //Assert.AreNotEqual(expected, actual);
            ////Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void UpdateTest()
        {
            UpdateTestHelper<CTS_OPIDK_Entity>();
        }

        /// <summary>
        ///Update 的测试
        ///</summary>
        public void UpdateTest1Helper<T>()
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
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod()]
        public void UpdateTest1()
        {
            UpdateTest1Helper<CTS_OPIDK_Entity>();
        }

        
    }
}
