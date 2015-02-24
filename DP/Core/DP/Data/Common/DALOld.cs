using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using DP.Common;
using System.Data.Common;
using System.Web.UI;
using System.Web;
using System.ComponentModel;

namespace DP.Data.Common
{
    abstract public class DALOld<T>
    {
        #region 变量
        protected PropertyInfo[] _properties = null;
        protected EntityConverter<T> _entityConverter;
        protected bool _IncrementPrimaryKey = false;
        protected int _primaryColumnCount = 0;
        protected int _nonPrimaryColumnCount = 0;
        protected string _tableName = string.Empty;
        protected string _connectionString = string.Empty;
        protected bool _isDebugLog = false;

       
        #endregion

        #region 属性
        /// <summary>
        /// 数据库表名 如果为空，则调用 Entity 的表名。通常此处为空。
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// 数据库连接字符串 如果为空，则调用 Entity 的连接字符串。通常此处为空。
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        /// <summary>
        /// 是否记录日志
        /// Gets or sets a value indicating whether this instance is debug.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is debug; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugLog
        {
            get { return _isDebugLog; }
            set { _isDebugLog = value; }
        }
        #endregion

        #region 构造函数
        public DAL()
        {
            _properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
        #endregion


        #region 添加
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Insert(T obj);
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Insert(DbTransaction trans, T obj);

        /// <summary>
        /// 向数据库中插入一条新记录。
        /// Inserts the specified table name.
        /// </summary>
        /// <param name="tableName">表名    Name of the table.</param>
        /// <param name="primaryKeys">主键  The primary keys.</param>
        /// <param name="primaryKeyType">主键类型   Type of the primary key.</param>
        /// <param name="dicts">字段    The dicts.</param>
        /// <param name="primaryValues">主键返回值  The primary values.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Insert(string tableName, string[] primaryKeys, PrimaryKeyType primaryKeyType, Dictionary<string, string> dicts, out string primaryValues);
        #endregion

        #region 更新
        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update(T obj);

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update(DbTransaction trans, T obj);

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update(T obj, StringBuilder partitionName);

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update(DbTransaction trans, T obj, StringBuilder partitionName);


        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// </summary>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update(string Sets, string Conditions, StringBuilder partitionName);

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// Updates the specified sets.
        /// </summary>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <returns></returns>
        public abstract int Update(string Sets, string Conditions);

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// </summary>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="PrecedingNumber">The preceding number.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update(string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName);

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// Updates the specified sets.
        /// </summary>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="PrecedingNumber">The preceding number.</param>
        /// <returns></returns>
        public abstract int Update(string Sets, string Conditions, int PrecedingNumber);

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="PrecedingNumber">The preceding number.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName);

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// Updates the specified sets.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="PrecedingNumber">The preceding number.</param>
        /// <returns></returns>
        public abstract int Update(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber);
        
        /// <summary>
        /// 向数据表T更新一条记录。
        /// Updates the specified table name.
        /// </summary>
        /// <param name="tableName">表名    Name of the table.</param>
        /// <param name="primaryKeys">主键  The primary keys.</param>
        /// <param name="dicts">字段    The dicts.</param>
        /// <returns>影响的行数</returns>
        public abstract int Update(string tableName, string[] primaryKeys, Dictionary<string, string> dicts);
        #endregion

        #region 删除
        /// <summary>
        ///  删除数据表T中的一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract int Delete(T obj);

        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public abstract int Delete(DbTransaction trans, T obj);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <returns></returns>
        public abstract int Delete(string Conditions);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <returns></returns>
        public abstract int Delete(DbTransaction trans, string Conditions);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public abstract int Delete(string Field, string Value);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public abstract int Delete(DbTransaction trans, string Field, string Value);

        /// <summary>
        ///  删除数据表T中的一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete(T obj, StringBuilder partitionName);

        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete(DbTransaction trans, T obj, StringBuilder partitionName);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete(string Conditions, StringBuilder partitionName);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete(DbTransaction trans, string Conditions, StringBuilder partitionName);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete(string Field, string Value, StringBuilder partitionName);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete(DbTransaction trans, string Field, string Value, StringBuilder partitionName);
        #endregion

        #region 根据 指定字段 指定值，返回单个实体类
        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <returns></returns>
        public abstract T GetEntity(Dictionary<string, string> dict);

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>T 对象</returns>
        public abstract T GetEntity(string Field, string Value);

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract T GetEntity(Dictionary<string, string> dict, StringBuilder partitionName);

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>T 对象</returns>
        public abstract T GetEntity(string Field, string Value, StringBuilder partitionName);
        #endregion

        #region 返回实体类集合
        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList();
        /// </remarks>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList();
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions);
        /// <summary>
        /// 得到符合条件的，数据表Customers所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="Sort">排序字段</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions, string Sort);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ", 0, 10);
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始, 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions, int StartRecordIndex, int PageSize);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ", 0, 10, "ID");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="Sort">排序字段</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions, int StartRecordIndex, int PageSize, string Sort);
        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList();
        /// </remarks>
        /// <param name="partitionName">分区名</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions, StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表Customers所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="Sort">排序字段</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions, string Sort, StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ", 0, 10);
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始, 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions, int StartRecordIndex, int PageSize, StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ", 0, 10, "ID");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="Sort">排序字段</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>List＜T＞对象集</returns>
        public abstract List<T> GetList(string Conditions, int StartRecordIndex, int PageSize, string Sort, StringBuilder partitionName);
        #endregion

        #region 返回DataTable 对象
        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable();
        /// </remarks>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable();
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ", "ID");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="Sort">排序字段</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions, string Sort);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ", 0, 10);
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ", 0, 10, "ID");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="Sort">排序字段</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize, string Sort);

        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable();
        /// </remarks>
        /// <param name="partitionName">分区名</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions, StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ", "ID");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="Sort">排序字段</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions, string Sort, StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ", 0, 10);
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize, StringBuilder partitionName);
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ", 0, 10, "ID");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始 0 开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="Sort">排序字段</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize, string Sort, StringBuilder partitionName);
        #endregion

        #region 查询实体总数
        /// <summary>
        /// 查询实体数
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// int val = SqlServerDAL.GetCount();
        /// </remarks>
        /// <returns>实体的总数</returns>
        public abstract long GetCount();
        /// <summary>
        /// 查询符合条件的实体总数
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// int val = SqlServerDAL.GetCount(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>实体的总数</returns>
        public abstract long GetCount(string Conditions);
        /// <summary>
        /// 查询实体数
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// int val = SqlServerDAL.GetCount();
        /// </remarks>
        /// <param name="partitionName">分区名</param>
        /// <returns>实体的总数</returns>
        public abstract long GetCount(StringBuilder partitionName);
        /// <summary>
        /// 查询符合条件的实体总数
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// int val = SqlServerDAL.GetCount(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>实体的总数</returns>
        public abstract long GetCount(string Conditions, StringBuilder partitionName);
        #endregion

        #region 根据 指定字段 指定值，检测是否存在
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>数量</returns>
        public abstract int IsExist(Dictionary<string, string> dict);
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>数量</returns>
        public abstract int IsExist(string Field, string Value);
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns>数量</returns>
        public abstract int IsExist(Dictionary<string, string> dict, StringBuilder partitionName);
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>数量</returns>
        public abstract int IsExist(string Field, string Value, StringBuilder partitionName);
        #endregion

        #region 获取 指定字段的 最大值
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue(string Field);
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue(string Field, string Conditions);
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue(string Field, StringBuilder partitionName);
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue(string Field, string Conditions, StringBuilder partitionName);
        #endregion

        #region 获取 指定字段的 最小值
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue(string Field);
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue(string Field, string Conditions);
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue(string Field, StringBuilder partitionName);
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue(string Field, string Conditions, StringBuilder partitionName);
        #endregion

        #region 获取 指定字段的  唯一行
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable(string Fields);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable(string Fields, string Conditions);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable(string Fields, StringBuilder partitionName);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable(string Fields, string Conditions, StringBuilder partitionName);
        #endregion

        #region 获取 指定字段的  唯一行数
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract long GetDistinctCount(string Fields);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract long GetDistinctCount(string Fields, string Conditions);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public abstract long GetDistinctCount(string Fields, StringBuilder partitionName);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 结果集中的唯一行
        /// </returns>
        public abstract long GetDistinctCount(string Fields, string Conditions, StringBuilder partitionName);
        #endregion

        #region 检测表是否存在
        /// <summary>
        /// 根据表名 检测表是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistTable();
        /// <summary>
        /// 根据表名 检测表是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistTable(string TableName);
        /// <summary>
        /// 根据表名 查询符合条件的 表信息 （模糊查询） 
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public abstract DataTable GetTableList(string TableName);

        #endregion

        #region 检测视图是否存在
        /// <summary>
        /// 根据视图名 检测视图是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistView();
        /// <summary>
        /// 根据视图名 检测视图是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistView(string ViewName);
        /// <summary>
        /// 根据视图名 查询符合条件的 视图信息 （模糊查询） 
        /// </summary>
        /// <param name="ViewName">视图名</param>
        /// <returns></returns>
        public abstract DataTable GetViewList(string ViewName);

        #endregion

        #region 检测字段是否存在
        /// <summary>
        /// 根据字段名 检测字段是否存在 （精确查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistField(string FieldName);
        /// <summary>
        /// 根据字段名 检测字段是否存在 （精确查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名</param>
        /// <returns>
        /// 是/否
        /// </returns>
        public abstract bool IsExistField(string FieldName, string TableName);
        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <returns></returns>
        public abstract DataTable GetFieldList(string FieldName);
        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名 （精确查询）</param>
        /// <returns></returns>
        public abstract DataTable GetFieldList(string FieldName, string TableName);

        #endregion

        #region 执行SQL语句
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public abstract int ExecuteSQL(string SQLString);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public abstract int ExecuteSQL(string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public abstract int ExecuteSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public abstract int ExecuteSQL(DbTransaction trans, string SQLString);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public abstract int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public abstract int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public abstract object ExecuteScalarSQL(string SQLString);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public abstract object ExecuteScalarSQL(string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public abstract object ExecuteScalarSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public abstract object ExecuteScalarSQL(DbTransaction trans, string SQLString);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public abstract object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public abstract object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>DataTable</returns>
        public abstract DataTable ExecuteDataAdapterSQL(string SQLString);
        /// <summary>
        /// Executes the data adapter SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public abstract DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the data adapter SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public abstract DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters);

        #endregion

        #region 事务
        /// <summary>
        /// 创建一个事务
        /// </summary>
        /// <returns>返回一个事务</returns>
        public abstract DbTransaction BeginTransaction();

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        public abstract void TransactionCommit(DbTransaction trans);

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        public abstract void TransactionRollBack(DbTransaction trans);
        #endregion

        #region 格式化日期查询参数

        /// <summary>
        /// Gets the begin date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public abstract string GetBeginDateString(string fieldName, DateTime beginDate);
        /// <summary>
        /// Gets the end date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public abstract string GetEndDateString(string fieldName, DateTime endDate);


        /// <summary>
        /// Gets the begin date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public abstract string GetBeginDateString(string fieldName, string beginDate);
        /// <summary>
        /// Gets the end date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public abstract string GetEndDateString(string fieldName, string endDate);

        /// <summary>
        /// Gets the date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="date">The date.</param>
        /// <param name="operators">The operators.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public abstract string GetDateString(string fieldName, string date, string operators, int offset);
        /// <summary>
        /// Gets the date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="date">The date.</param>
        /// <param name="operators">The operators.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public abstract string GetDateString(string fieldName, DateTime date, string operators, int offset);

        #endregion

        #region 对象 数据 拷贝
        /// <summary>
        /// 对象 数据 拷贝
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="destination">目的对象</param>
        /// <returns>是否有异常</returns>
        public bool ObjectCopyTo(object source, object destination)
        {
            bool rel = true;
            if (destination == null)
            {
                throw new Exception("destination 未初始化！");
            }
            PropertyInfo[] _sourceProperties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] _destinationProperties = destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo sourcePropertie in _sourceProperties)
            {
                foreach (PropertyInfo destinationProperty in _destinationProperties)
                {
                    if (sourcePropertie.Name.ToLower().Equals(destinationProperty.Name.ToLower()))
                    {
                        try
                        {
                            destinationProperty.SetValue(destination, sourcePropertie.GetValue(source, null), null);
                            break;
                        }
                        catch
                        {
                            rel = false;
                        }
                    }
                }
            }
            return rel;
        }

        /// <summary>
        /// 对象 数据 拷贝
        /// </summary>
        /// <typeparam name="T2">目的对象 类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目的对象</param>
        /// <returns>是否有异常</returns>
        public bool ObjectCopyTo<S>(T source, ref S destination)
        {
            bool rel = true;
            if (destination == null)
            {
                destination = Activator.CreateInstance<S>();
            }
            PropertyInfo[] _destinationProperties = destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo sourcePropertie in _properties)
            {
                foreach (PropertyInfo destinationProperty in _destinationProperties)
                {
                    if (sourcePropertie.Name.ToLower().Equals(destinationProperty.Name.ToLower()))
                    {
                        try
                        {
                            destinationProperty.SetValue(destination, sourcePropertie.GetValue(source, null), null);
                            break;
                        }
                        catch
                        {
                            rel = false;
                        }
                    }
                }
            }
            return rel;
        }

        /// <summary>
        /// Objects the copy to.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        public bool DataRowCopyTo(DataRow dr, object destination)
        {
            if (destination == null)
            {
                throw new Exception("destination 未初始化！");
            }
            return DataRowCopyTo(dr, destination, destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public));
            
        }

        /// <summary>
        /// Objects the copy to.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationProperties">The destination properties.</param>
        /// <returns></returns>
        public bool DataRowCopyTo(DataRow dr, object destination, PropertyInfo[] destinationProperties)
        {
            bool rel = true;
            if (destination == null)
            {
                throw new Exception("destination 未初始化！");
            }
            PropertyInfo[] _destinationProperties = destinationProperties;
            if(_destinationProperties.Length == 0)
            {
                _destinationProperties = destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            }

            foreach (PropertyInfo destinationProperty in _destinationProperties)
            {
                try
                {
                    if (dr.Table.Columns.Contains(destinationProperty.Name.ToLower()))
                    {
                        destinationProperty.SetValue(destination, ReflectionHelper.ChangeType(dr[destinationProperty.Name.ToLower()], destinationProperty.PropertyType), null);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(String.Format("{0}", ex.Message.ToString()));
                }
            }
            return rel;
        }

        #endregion

        #region 保护方法
        /// <summary>
        /// 获取表名 如果 SqlServerDAL.TableName 属性指定了表名。则取 SqlServerDAL.TableName 否则取  _dbTableInfo.TableName
        /// </summary>
        /// <returns></returns>
        protected string GetTableName(DbTableInfo _dbTableInfo)
        {
            return String.IsNullOrEmpty(_tableName) ? _dbTableInfo.TableName : _tableName;
        }

        /// <summary>
        /// 获取连接字符串 如果 SqlServerDAL.ConnectionString 属性指定了表名。则取 SqlServerDAL.ConnectionString 否则取  _dbTableInfo.ConnectionString
        /// </summary>
        /// <returns></returns>
        protected string GetConnectionString(DbTableInfo _dbTableInfo)
        {
            return String.IsNullOrEmpty(_connectionString) ? _dbTableInfo.ConnectionString : _connectionString;
        }

        /// <summary>
        /// 判断 数据空 或 对象空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected bool IsNullOrDBNull(object obj)
        {
            return ((obj is DBNull) || string.IsNullOrEmpty(obj.ToString())) ? true : false;
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="sqlString">The SQL string.</param>
        /// <param name="dtBegin">The dt begin.</param>
        /// <param name="dtEnd">The dt end.</param>
        protected void WriteLog(string sqlString, DateTime dtBegin, DateTime dtEnd)
        {
            if (_isDebugLog)
            {
                string pageName = "";
                string url = "";
                Page page = HttpContext.Current.Handler as Page;
                if (page != null)
                {
                    url = page.Request.Url.ToString();
                    pageName = page.ToString();
                }
                LogHelper.WriteLog("SqlDebug", String.Format("PageName:{3}--Begin:{0}--End:{1}--Diff:{2}\r\nCmdText:{4}\r\nUrl:{5}", dtBegin.ToString("HH:mm:ss.fff"), dtEnd.ToString("HH:mm:ss.fff"), dtEnd.Subtract(dtBegin), pageName, sqlString, url));
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected object GetValue(Type type, string value)
        {
            object obj = DBNull.Value;
            switch (type.Name)
            {
                case "String":
                    {
                        obj = value;
                    }
                    break;
                case "Boolean":
                    {
                        obj = (value == "true") ? true : false;
                    }
                    break;
                case "Decimal":
                    {
                        obj = StringHelper.ToDecimal(value, 0);
                    }
                    break;
                case "Single":
                    {
                        obj = StringHelper.ToFloat(value, 0);
                    }
                    break;
                case "Double":
                    {
                        obj = StringHelper.ToDouble(value, 0);
                    }
                    break;
                case "Int16":
                    {
                        obj = StringHelper.ToShort(value, 0);
                    }
                    break;
                case "Int32":
                    {
                        obj = StringHelper.ToInt(value, 0);
                    }
                    break;
                case "Int64":
                    {
                        obj = StringHelper.ToLong(value, 0);
                    }
                    break;
                case "DateTime":
                    {
                        DateTime dt = DateTime.MinValue;
                        if (DateTimeHelper.ToDateTime(value, out dt))
                        {
                            obj = dt;
                        }
                        else
                        {
                            obj = DBNull.Value;
                        }
                    }
                    break;
                case "Nullable`1":
                    {
                        NullableConverter nullableConverter = new NullableConverter(type);
                        switch (nullableConverter.UnderlyingType.Name)
                        {
                            case "String":
                                {
                                    obj = value;
                                }
                                break;
                            case "Boolean":
                                {
                                    obj = (value == "true") ? true : false;
                                }
                                break;
                            case "Decimal":
                                {
                                    obj = StringHelper.ToDecimal(value, 0);
                                }
                                break;
                            case "Single":
                                {
                                    obj = StringHelper.ToFloat(value, 0);
                                }
                                break;
                            case "Double":
                                {
                                    obj = StringHelper.ToDouble(value, 0);
                                }
                                break;
                            case "Int16":
                                {
                                    obj = StringHelper.ToShort(value, 0);
                                }
                                break;
                            case "Int32":
                                {
                                    obj = StringHelper.ToInt(value, 0);
                                }
                                break;
                            case "Int64":
                                {
                                    obj = StringHelper.ToLong(value, 0);
                                }
                                break;
                            case "DateTime":
                                {
                                    DateTime dt = DateTime.MinValue;
                                    if (DateTimeHelper.ToDateTime(value, out dt))
                                    {
                                        obj = dt;
                                    }
                                    else
                                    {
                                        obj = DBNull.Value;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            return obj;
        }

        #endregion


    }
}
