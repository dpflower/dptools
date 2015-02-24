using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Reflection;
using DP.Common;
using System.Web.UI;
using System.Web;
using System.ComponentModel;

namespace DP.Data.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public abstract class DAL
    {
        #region 变量
        /// <summary>
        /// 数据库表名 如果为空，则调用 Entity 的表名。通常此处为空。
        /// </summary>
        //protected string _tableName = string.Empty;
        /// <summary>
        /// 数据库连接字符串 如果为空，则调用 Entity 的连接字符串。通常此处为空。
        /// </summary>
        protected string _connectionString = string.Empty;
        /// <summary>
        /// 是否记录日志
        /// </summary>
        protected bool _isDebugLog = false;


        #endregion

        #region 属性
        /// <summary>
        /// 数据库表名 如果为空，则调用 Entity 的表名。通常此处为空。
        /// </summary>
        /// <value>The name of the table.</value>
        /// <remarks></remarks>
        //public string TableName
        //{
        //    get { return _tableName; }
        //    set { _tableName = value; }
        //}
        /// <summary>
        /// 数据库连接字符串 如果为空，则调用 Entity 的连接字符串。通常此处为空。
        /// </summary>
        /// <value>The connection string.</value>
        /// <remarks></remarks>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        /// <summary>
        /// 是否记录日志
        /// Gets or sets a value indicating whether this instance is debug.
        /// </summary>
        /// <value><c>true</c> if this instance is debug; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsDebugLog
        {
            get { return _isDebugLog; }
            set { _isDebugLog = value; }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化 <see cref="T:System.Object"/> 类的新实例。
        /// </summary>
        /// <remarks></remarks>
        public DAL()
        {
            
        }
        #endregion


        #region 添加
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract int Insert<T>(T obj);
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract int Insert<T>(T obj, string tableName);
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract int Insert<T>(DbTransaction trans, T obj);
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract int Insert<T>(DbTransaction trans, T obj, string tableName);
        #endregion

        #region 更新
        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update<T>(T obj);

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update<T>(DbTransaction trans, T obj);

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update<T>(T obj, string tableName);

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public abstract int Update<T>(DbTransaction trans, T obj, string tableName);
        
        /// <summary>
        /// 向数据表T更新符合条件的 指定字段值
        /// Updates the specified sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Sets">The sets.</param>
        /// <param name="conditions">The conditions.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract int Update(string connectionString, Dictionary<string, object> Sets, List<Condition> conditions, string tableName);
        
        /// <summary>
        /// 向数据表T更新符合条件的 指定字段值
        /// Updates the specified sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans">The trans.</param>
        /// <param name="Sets">The sets.</param>
        /// <param name="conditions">The conditions.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract int Update(DbTransaction trans, Dictionary<string, object> Sets, List<Condition> conditions, string tableName);

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// </summary>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="partitionName">分区名</param>
        ///// <returns>
        ///// 影响的行数
        ///// </returns>
        //public abstract int Update<T>(string Sets, string Conditions, StringBuilder partitionName);

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// Updates the specified sets.
        ///// </summary>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <returns></returns>
        //public abstract int Update<T>(string Sets, string Conditions);

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// </summary>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="PrecedingNumber">The preceding number.</param>
        ///// <param name="partitionName">分区名</param>
        ///// <returns>
        ///// 影响的行数
        ///// </returns>
        //public abstract int Update<T>(string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName);

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// Updates the specified sets.
        ///// </summary>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="PrecedingNumber">The preceding number.</param>
        ///// <returns></returns>
        //public abstract int Update<T>(string Sets, string Conditions, int PrecedingNumber);

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// </summary>
        ///// <param name="trans">The trans.</param>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="PrecedingNumber">The preceding number.</param>
        ///// <param name="partitionName">分区名</param>
        ///// <returns>
        ///// 影响的行数
        ///// </returns>
        //public abstract int Update<T>(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName);

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// Updates the specified sets.
        ///// </summary>
        ///// <param name="trans">The trans.</param>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="PrecedingNumber">The preceding number.</param>
        ///// <returns></returns>
        //public abstract int Update<T>(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber);

        /// <summary>
        /// 向数据表T更新一条记录。
        /// Updates the specified table name.
        /// </summary>
        /// <param name="tableName">表名    Name of the table.</param>
        /// <param name="primaryKeys">主键  The primary keys.</param>
        /// <param name="dicts">字段    The dicts.</param>
        /// <returns>影响的行数</returns>
        public abstract int Update(string connectionString, Dictionary<string, string> dicts, string[] primaryKeys, string tableName);
        #endregion

        #region 删除
        /// <summary>
        ///  删除数据表T中的一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract int Delete<T>(T obj);

        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public abstract int Delete<T>(DbTransaction trans, T obj);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <returns></returns>
        public abstract int Delete<T>(List<Condition> conditions);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <returns></returns>
        public abstract int Delete<T>(DbTransaction trans, List<Condition> conditions);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public abstract int Delete<T>(string Field, string Value);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public abstract int Delete<T>(DbTransaction trans, string Field, string Value);

        /// <summary>
        ///  删除数据表T中的一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete<T>(T obj, string tableName);

        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete<T>(DbTransaction trans, T obj, string tableName);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete<T>(List<Condition> conditions, string tableName);

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete<T>(DbTransaction trans, List<Condition> conditions, string tableName);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete<T>(string Field, string Value, string tableName);

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract int Delete<T>(DbTransaction trans, string Field, string Value, string tableName);
        #endregion

        #region 根据 指定字段 指定值，返回单个实体类
        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <returns></returns>
        public abstract T GetEntity<T>(List<Condition> conditions);

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>T 对象</returns>
        public abstract T GetEntity<T>(string Field, string Value);

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public abstract T GetEntity<T>(List<Condition> conditions, string tableName);

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>T 对象</returns>
        public abstract T GetEntity<T>(string Field, string Value, string tableName);
        #endregion

        #region 返回实体类集合
        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>List＜T＞对象集</returns>
        /// <remarks>示例:
        /// List＜T＞ list = SqlServerDAL.GetList();</remarks>
        public abstract List<T> GetList<T>(Query query);
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
        public abstract DataTable GetDataTable(Query query, string connectionString, string pagingKey);        
        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable();
        /// </remarks>
        /// <returns>DataSet集</returns>
        public abstract DataTable GetDataTable<T>(Query query);
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
        public abstract long GetCount<T>(Query query);
        public abstract long GetCount(Query query,string connectionString);
        #endregion

        #region 根据 指定字段 指定值，检测是否存在
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>数量</returns>
        public abstract int IsExist<T>(List<Condition> conditions);
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>数量</returns>
        public abstract int IsExist<T>(string Field, string Value);
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns>数量</returns>
        public abstract int IsExist<T>(List<Condition> conditions, string tableName);
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>数量</returns>
        public abstract int IsExist<T>(string Field, string Value, string tableName);
        #endregion


        #region 获取 指定字段的 最大值
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue<T>(string Field);
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue<T>(string Field, List<Condition> conditions);
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue<T>(string Field, string tableName);
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public abstract string GetMaxValue<T>(string Field, List<Condition> conditions, string tableName);
        #endregion

        #region 获取 指定字段的 最小值
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue<T>(string Field);
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue<T>(string Field, List<Condition> conditions);
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue<T>(string Field, string tableName);
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public abstract string GetMinValue<T>(string Field, List<Condition> conditions, string tableName);
        #endregion

        #region 获取 指定字段的  唯一行
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable<T>(string Fields);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable<T>(string Fields, List<Condition> conditions);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable<T>(string Fields, string tableName);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public abstract DataTable GetDistinctTable<T>(string Fields, List<Condition> conditions, string tableName);
        #endregion

        #region 获取 指定字段的  唯一行数
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract long GetDistinctCount<T>(string Fields);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public abstract long GetDistinctCount<T>(string Fields, List<Condition> conditions);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public abstract long GetDistinctCount<T>(string Fields, string tableName);
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 结果集中的唯一行
        /// </returns>
        public abstract long GetDistinctCount<T>(string Fields, List<Condition> conditions, string tableName);
        #endregion

        #region 检测表是否存在
        /// <summary>
        /// 根据表名 检测表是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistTable<T>();
        /// <summary>
        /// 根据表名 检测表是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistTable<T>(string TableName);
        /// <summary>
        /// 根据表名 查询符合条件的 表信息 （模糊查询） 
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public abstract DataTable GetTableList<T>(string TableName);

        #endregion

        #region 检测视图是否存在
        /// <summary>
        /// 根据视图名 检测视图是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistView<T>();
        /// <summary>
        /// 根据视图名 检测视图是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistView<T>(string ViewName);
        /// <summary>
        /// 根据视图名 查询符合条件的 视图信息 （模糊查询） 
        /// </summary>
        /// <param name="ViewName">视图名</param>
        /// <returns></returns>
        public abstract DataTable GetViewList<T>(string ViewName);

        #endregion

        #region 检测字段是否存在
        /// <summary>
        /// 根据字段名 检测字段是否存在 （精确查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <returns>是/否</returns>
        public abstract bool IsExistField<T>(string FieldName);
        /// <summary>
        /// 根据字段名 检测字段是否存在 （精确查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名</param>
        /// <returns>
        /// 是/否
        /// </returns>
        public abstract bool IsExistField<T>(string FieldName, string TableName);
        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <returns></returns>
        public abstract DataTable GetFieldList<T>(string FieldName);
        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名 （精确查询）</param>
        /// <returns></returns>
        public abstract DataTable GetFieldList<T>(string FieldName, string TableName);

        #endregion










        #region 执行SQL语句
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract int ExecuteSQL(string SQLString);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract int ExecuteSQL(string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract int ExecuteSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract int ExecuteSQL(DbTransaction trans, string SQLString);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract object ExecuteScalarSQL(string SQLString);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract object ExecuteScalarSQL(string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract object ExecuteScalarSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public abstract object ExecuteScalarSQL(DbTransaction trans, string SQLString);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>DataTable</returns>
        /// <remarks></remarks>
        public abstract DataTable ExecuteDataAdapterSQL(string SQLString);
        /// <summary>
        /// Executes the data adapter SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType);
        /// <summary>
        /// Executes the data adapter SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters);

        #endregion

        #region 事务
        /// <summary>
        /// 创建一个事务
        /// </summary>
        /// <returns>返回一个事务</returns>
        /// <remarks></remarks>
        public abstract DbTransaction BeginTransaction();

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        /// <remarks></remarks>
        public abstract void TransactionCommit(DbTransaction trans);

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        /// <remarks></remarks>
        public abstract void TransactionRollBack(DbTransaction trans);
        #endregion

        #region 格式化日期查询参数

        /// <summary>
        /// Gets the begin date string.
        /// </summary>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract string GetDateToDbSelectString(DateTime beginDate);

        #endregion

        #region 对象 数据 拷贝
        /// <summary>
        /// 对象 数据 拷贝
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="destination">目的对象</param>
        /// <returns>是否有异常</returns>
        /// <remarks></remarks>
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
        /// Objects the copy to.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// <remarks></remarks>
        public bool DataRowCopyTo(DataRow dr, object destination, PropertyInfo[] destinationProperties)
        {
            bool rel = true;
            if (destination == null)
            {
                throw new Exception("destination 未初始化！");
            }
            PropertyInfo[] _destinationProperties = destinationProperties;
            if (_destinationProperties.Length == 0)
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

        #region 公共
        /// <summary>
        /// Gets the conditions string.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected abstract string GetConditionsString(List<Condition> conditions, ref List<DbParameter> parameters);

        /// <summary>
        /// Gets the sets string.
        /// </summary>
        /// <param name="sets">The sets.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected abstract string GetSetsString(Dictionary<string, object> sets, ref List<DbParameter> parameters); 
        #endregion

        #region 保护方法
        /// <summary>
        /// 获取表名 如果 SqlServerDAL.TableName 属性指定了表名。则取 SqlServerDAL.TableName 否则取  _dbTableInfo.TableName
        /// </summary>
        /// <param name="_dbTableSchema">The _DB table schema.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected string GetTableName(DbTableSchema _dbTableSchema, string tableName)
        {
            return !String.IsNullOrEmpty(tableName) ? tableName: _dbTableSchema.TableName;
        }

        /// <summary>
        /// 获取连接字符串 如果 SqlServerDAL.ConnectionString 属性指定了表名。则取 SqlServerDAL.ConnectionString 否则取  _dbTableInfo.ConnectionString
        /// </summary>
        /// <param name="_dbTableSchema">The _DB table schema.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected string GetConnectionString(DbTableSchema _dbTableSchema)
        {
            if (_dbTableSchema == null )
            {
                return _connectionString;
            }
            if (String.IsNullOrEmpty(_dbTableSchema.ConnectionString))
            {
                return _connectionString;
            }
            return _dbTableSchema.ConnectionString;
            //return String.IsNullOrEmpty(_connectionString) ? _dbTableSchema.ConnectionString : _connectionString;
        }

        /// <summary>
        /// 获取是否存在自增字段。
        /// Determines whether [is has db generated] [the specified list].
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if [is has db generated] [the specified list]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        protected bool IsHasDbGenerated(List<DbColumnSchema> list)
        {
            return list.Exists(p => p.IsDbGenerated);
        }

        /// <summary>
        /// 获取主键字段数组。
        /// Gets the primary column.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected List<DbColumnSchema> GetPrimaryColumnList(List<DbColumnSchema> list)
        {
            return list.FindAll(p => p.IsPrimaryKey);
        }

        /// <summary>
        /// 判断 数据空 或 对象空
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns><c>true</c> if [is null or DB null] [the specified obj]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
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
        /// <remarks></remarks>
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
        /// <remarks></remarks>
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

        /// <summary>
        /// 
        /// Gets the order string.
        /// </summary>
        /// <param name="orders">The orders.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected string GetOrderString(List<SortOrder> orders)
        {
            StringBuilder order = new StringBuilder();
            foreach (SortOrder o in orders)
            {
                if (order.Length > 0)
                {
                    order.Append(",");
                }
                order.AppendFormat("{0} {1}", o.OrderName, o.OrderDirection.ToString());
            }
            if (order.Length > 0)
            {
                order.Insert(0, " Order by ");
            }
            return order.ToString();
        }

        /// <summary>
        /// Gets the select fields string.
        /// </summary>
        /// <param name="selectFields">The select fields.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected string GetSelectFieldsString(List<string> selectFields)
        {
            if (selectFields.Count == 0)
            {
                return "*";
            }
            StringBuilder select = new StringBuilder();
            foreach (string s in selectFields)
            {
                if (select.Length > 0)
                {
                    select.Append(",");
                }
                select.Append(s);
            }
            return select.ToString();
        }

        /// <summary>
        /// Gets the conditions string.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected string GetConditionsString(List<Condition> conditions)
        {            
            StringBuilder condition = new StringBuilder();
            foreach (Condition c in conditions)
            {
                if (condition.Length > 0)
                {
                    condition.Append(" and ");
                }
                string value = "";
                if (c.Value is DateTime)
                {
                    value = GetDateToDbSelectString((DateTime)c.Value);
                }
                else
                {
                    value = c.Value.ToString();
                }
                switch (c.QueryOperator)
                {
                    case QueryOperator.Equal:
                        {
                            condition.AppendFormat("{0} {1} '{2}'", c.Key, "=", value);
                        }
                        break;
                    case QueryOperator.Greater:
                        {
                            condition.AppendFormat("{0} {1} '{2}'", c.Key, ">", value);
                        }
                        break;
                    case QueryOperator.GreaterOrEqual:
                        {
                            condition.AppendFormat("{0} {1} '{2}'", c.Key, ">=", value);
                        }
                        break;
                    case QueryOperator.Less:
                        {
                            condition.AppendFormat("{0} {1} '{2}'", c.Key, "<", value);
                        }
                        break;
                    case QueryOperator.LessOrEqual:
                        {
                            condition.AppendFormat("{0} {1} '{2}'", c.Key, "<=", value);
                        }
                        break;
                    case QueryOperator.Like:
                        {
                            condition.AppendFormat("{0} like '{1}%'", c.Key, value);
                        }
                        break;
                    case QueryOperator.NotEqual:
                        {
                            condition.AppendFormat("{0} {1} '{2}'", c.Key, "<>", value);
                        }
                        break;
                }

            }
            return condition.ToString();
        }

    

        #endregion
    }
}
