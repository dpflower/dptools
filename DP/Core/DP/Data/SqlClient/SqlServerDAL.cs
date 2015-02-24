using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Data.Common;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.ComponentModel;
using DP.Common;
using DP.Data.Common.Emit;

namespace DP.Data.SqlClient
{
    public class SqlServerDAL : DAL
    {
        #region 变量
        SqlHelper _SqlHelper;
        //0:TableName
        //1:StartRecordIndex
        //2:PageSize
        //3:Where
        //4:Order
        //5:Selects
        //6:PageKey
        readonly string sqlSelectPaging2000 = "select top {2} {5} from [{0}]  with  (nolock)  where {6} NOT IN ((Select Top {1} {6} from {0} where 1=1 {3}  {4} )) ";
        //0:TableName
        //1:StartRecordIndex
        //2:PageSize
        //3:Where
        //4:Order
        //5:Selects
        readonly string sqlSelectPaging2005 = "select {5} from (select row_number() over( {4}) as rownumber , * from [{0}]   with  (nolock)  where 1=1 {3} ) as tb where rownumber >= {1} and rownumber < {1}+{2} {3} ";
        //0:TableName
        //1:Where
        //2:Order
        //3:Selects
        readonly string sqlSelectAll = "select {3} from [{0}]    with  (nolock) where 1=1 {1} {2} ";
        //0:TableName
        //1:Where
        readonly string sqlSelectCount = "select count(1)     from [{0}] with  (nolock) where 1=1 {1} ";
        /// <summary>
        /// 
        /// </summary>
        SqlServerVerson _sqlVerson = SqlServerVerson.Sql2005;

        #endregion    

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public SqlServerVerson SqlVerson
        {
            get { return _sqlVerson; }
            set { _sqlVerson = value; }
        }
        
        #endregion

        public SqlServerDAL()
            : this("")
        {
            
        }
        public SqlServerDAL(string connectionString)
        {
            _connectionString = connectionString;
            _SqlHelper = new SqlHelper();
        }


        #region 添加
        
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <param name="T">T实体</param>
        /// <returns>影响的行数</returns>
        public override int Insert<T>(T obj)
        {
            return Insert<T>(obj, "");
        }

        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <param name="T">T实体</param>
        /// <returns>影响的行数</returns>
        public override int Insert<T>(T obj, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            bool IsHasDbGenerated = base.IsHasDbGenerated(dbColumnSchemaList);

            int rel = 0;
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();

            #region 设置参数及Insert语句
            PropertyInfo propertyKey = null;
            SqlParameter param = null;
            List<SqlParameter> paramList = new List<SqlParameter>();
            foreach (SqlDbColumnSchema column in dbColumnSchemaList)
            {
                if (column.IsPrimaryKey && IsHasDbGenerated)
                {
                    if (column.IsDbGenerated)
                    {
                        propertyKey = column.Property;
                    }
                    continue;
                }
                fields.Append(column.ColumnName).Append(",");
                values.Append("@").Append(column.ColumnName).Append(",");
                param = new SqlParameter();
                param.SqlDbType = GetSqlDbType(column.Type);
                param.ParameterName = "@" + column.ColumnName;
                if (column.Property.GetValue(obj, null) != null)
                {
                    param.Value = column.Property.GetValue(obj, null);

                }
                else
                {
                    param.Value = DBNull.Value;
                }
                paramList.Add(param);
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);
            #endregion
            

            #region 跟据是否是自增主键进行Insert
            if (IsHasDbGenerated)
            {
                sqlInsert.AppendFormat("SET NOCOUNT ON;Insert Into [{0}] ({1}) values ({2}); select @@identity ", GetTableName(dbTableSchema, tableName), fields, values);

                object relobj = _SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlInsert.ToString(), paramList.ToArray());
                long temp = System.Convert.ToInt64(relobj);
                if (temp > 0)
                {
                    if (propertyKey != null)
                    {
                        switch (GetSqlDbType(propertyKey.PropertyType))
                        {
                            case SqlDbType.BigInt:
                                {
                                    propertyKey.SetValue(obj, System.Convert.ToInt64(relobj), null);
                                }
                                break;
                            case SqlDbType.Int:
                                {
                                    propertyKey.SetValue(obj, System.Convert.ToInt32(relobj), null);
                                }
                                break;
                            case SqlDbType.SmallInt:
                                {
                                    propertyKey.SetValue(obj, System.Convert.ToInt16(relobj), null);
                                }
                                break;
                        };
                    }
                    rel = 1;
                }
                else
                {
                    rel = 0;
                }
            }
            else
            {
                sqlInsert.AppendFormat("Insert Into [{0}] ({1}) values ({2}); ", GetTableName(dbTableSchema, tableName), fields, values);
                rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlInsert.ToString(), paramList.ToArray());
            }
            #endregion
            base.WriteLog(sqlInsert.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
                
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public override int Insert<T>(DbTransaction trans, T obj)
        {
            return Insert<T>(trans, obj, "");
        }

        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>影响的行数</returns>
        /// <remarks></remarks>
        public override int Insert<T>(DbTransaction trans, T obj, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            bool IsHasDbGenerated = base.IsHasDbGenerated(dbColumnSchemaList);

            int rel = 0;
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();

            #region 设置参数及Insert语句
            PropertyInfo propertyKey = null;
            SqlParameter param = null;
            List<SqlParameter> paramList = new List<SqlParameter>();
            foreach (SqlDbColumnSchema column in dbColumnSchemaList)
            {
                if (column.IsPrimaryKey && IsHasDbGenerated)
                {
                    if (column.IsDbGenerated)
                    {
                        propertyKey = column.Property;
                    }
                    continue;
                }
                fields.Append(column.ColumnName).Append(",");
                values.Append("@").Append(column.ColumnName).Append(",");
                param = new SqlParameter();
                param.SqlDbType = GetSqlDbType(column.Type);
                param.ParameterName = "@" + column.ColumnName;
                if (column.Property.GetValue(obj, null) != null)
                {
                    param.Value = column.Property.GetValue(obj, null);

                }
                else
                {
                    param.Value = DBNull.Value;
                }
                paramList.Add(param);
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);
            #endregion
            

            #region 跟据是否是自增主键进行Insert
            if (IsHasDbGenerated)
            {
                sqlInsert.AppendFormat("SET NOCOUNT ON;Insert Into [{0}] ({1}) values ({2}); select @@identity ", GetTableName(dbTableSchema, tableName), fields, values);

                object relobj = _SqlHelper.ExecuteScalar((SqlTransaction)trans, CommandType.Text, sqlInsert.ToString(), paramList.ToArray());
                long temp = System.Convert.ToInt64(relobj);
                if (temp > 0)
                {
                    if (propertyKey != null)
                    {
                        switch (GetSqlDbType(propertyKey.PropertyType))
                        {
                            case SqlDbType.BigInt:
                                {
                                    propertyKey.SetValue(obj, System.Convert.ToInt64(relobj), null);
                                }
                                break;
                            case SqlDbType.Int:
                                {
                                    propertyKey.SetValue(obj, System.Convert.ToInt32(relobj), null);
                                }
                                break;
                            case SqlDbType.SmallInt:
                                {
                                    propertyKey.SetValue(obj, System.Convert.ToInt16(relobj), null);
                                }
                                break;
                        };
                    }
                    rel = 1;
                }
                else
                {
                    rel = 0;
                }
            }
            else
            {
                sqlInsert.AppendFormat("Insert Into [{0}] ({1}) values ({2}); ", GetTableName(dbTableSchema, tableName), fields, values);
                rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlInsert.ToString(), paramList.ToArray());
            }
            #endregion
            base.WriteLog(sqlInsert.ToString(), dtBegin, DateTime.Now);
            return rel;

        }

        #region MyRegion
        ///// <summary>
        ///// 未完成方法
        ///// 向数据库中插入一条新记录。
        ///// Inserts the specified table name.
        ///// </summary>
        ///// <param name="tableName">表名    Name of the table.</param>
        ///// <param name="primaryKeys">主键  The primary keys.</param>
        ///// <param name="primaryKeyType">主键类型   Type of the primary key.</param>
        ///// <param name="dicts">字段    The dicts.</param>
        ///// <param name="primaryValues">主键返回值  The primary values.</param>
        ///// <returns>
        ///// 影响的行数
        ///// </returns>
        //private override int Insert<T>(string tableName, string[] primaryKeys, PrimaryKeyType primaryKeyType, Dictionary<string, string> dicts, out string primaryValues)
        //{
        //    DateTime dtBegin = DateTime.Now;
        //    primaryValues = string.Empty;

        //    Type type = typeof(T);
        //    DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
        //    List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
        //    bool IsHasDbGenerated = base.IsHasDbGenerated(dbColumnSchemaList);

        //    int rel = 0;
        //    StringBuilder fields = new StringBuilder();
        //    StringBuilder values = new StringBuilder();
        //    StringBuilder sqlInsert = new StringBuilder();
        //    DataTable dt = GetEmptyTable(dbTableSchema, tableName); 

        //    List<SqlParameter> parms = new List<SqlParameter>();
        //    SqlParameter parm;
        //    foreach (DataColumn column in dt.Columns)
        //    {
        //        if (StringHelper.IsExist(column.ColumnName, primaryKeys))
        //        {
        //            switch (primaryKeyType)
        //            {
        //                case PrimaryKeyType.Auto:
        //                case PrimaryKeyType.Sequence:
        //                    {

        //                    }
        //                    break;
        //                case PrimaryKeyType.Guid:
        //                    {
        //                        if (dicts.ContainsKey(column.ColumnName.ToLower()))
        //                        {
        //                            fields.Append(column.ColumnName).Append(",");
        //                            values.Append("@").Append(column.ColumnName).Append(",");
        //                            parm = new SqlParameter();
        //                            parm.SqlDbType = GetSqlDbType(column.DataType);
        //                            parm.ParameterName = "@" + column.ColumnName;
        //                            parm.Value = Guid.NewGuid().ToString();
        //                            parms.Add(parm);
        //                            if (!String.IsNullOrEmpty(primaryValues))
        //                            {
        //                                primaryValues += ",";
        //                            }
        //                            primaryValues += parm.Value.ToString();
        //                        }
        //                    }
        //                    break;
        //                case PrimaryKeyType.Other:
        //                    {
        //                        if (dicts.ContainsKey(column.ColumnName.ToLower()))
        //                        {
        //                            fields.Append(column.ColumnName).Append(",");
        //                            values.Append("@").Append(column.ColumnName).Append(",");
        //                            parm = new SqlParameter();
        //                            parm.SqlDbType = GetSqlDbType(column.DataType);
        //                            parm.ParameterName = "@" + column.ColumnName;
        //                            parm.Value = GetValue(column.DataType, dicts[column.ColumnName.ToLower()].ToString());
        //                            parms.Add(parm);
        //                            if (!String.IsNullOrEmpty(primaryValues))
        //                            {
        //                                primaryValues += ",";
        //                            }
        //                            primaryValues += parm.Value.ToString();
        //                        }
        //                    }
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            if (dicts.ContainsKey(column.ColumnName.ToLower()))
        //            {
        //                fields.Append(column.ColumnName).Append(",");
        //                values.Append("@").Append(column.ColumnName).Append(",");
        //                parm = new SqlParameter();
        //                parm.SqlDbType = GetSqlDbType(column.DataType);
        //                parm.ParameterName = "@" + column.ColumnName;
        //                parm.Value = GetValue(column.DataType, dicts[column.ColumnName.ToLower()].ToString());
        //                parms.Add(parm);
        //            }
        //        }
        //    }
        //    fields.Remove(fields.Length - 1, 1);
        //    values.Remove(values.Length - 1, 1);

        //    switch (primaryKeyType)
        //    {
        //        case PrimaryKeyType.Auto:
        //        case PrimaryKeyType.Sequence:
        //            {
        //                sqlInsert.AppendFormat("SET NOCOUNT ON;Insert Into {0} ({1}) values ({2}); select @@identity ", tableName, fields, values);
        //                object relobj = _SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlInsert.ToString(), parms.ToArray());
        //                long temp = System.Convert.ToInt64(relobj);
        //                if (temp > 0)
        //                {
        //                    primaryValues = temp.ToString();
        //                    rel = 1;
        //                }
        //                else
        //                {
        //                    rel = 0;
        //                }
        //            }
        //            break;
        //        case PrimaryKeyType.Guid:
        //        case PrimaryKeyType.Other:
        //            {
        //                sqlInsert.AppendFormat("Insert Into {0} ({1}) values ({2}); ", tableName, fields, values);
        //                rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlInsert.ToString(), parms.ToArray());
        //            }
        //            break;
        //    }

        //    base.WriteLog(sqlInsert.ToString(), dtBegin, DateTime.Now);
        //    return rel;
        //} 
        #endregion
        #endregion

        #region 更新
        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="T">T</param>
        /// <returns>影响的行数</returns>
        public override int Update<T>(T obj)
        {
            return Update<T>(obj, "");
        }

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public override int Update<T>(DbTransaction trans, T obj)
        {
            return Update<T>(trans, obj, "");
        }

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public override int Update<T>(T obj, string tableName)
        {
            DateTime dtBegin = DateTime.Now; 
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type); 
            List<DbColumnSchema> dbColumnSchemaPkList = GetPrimaryColumnList(dbColumnSchemaList);
            if (dbColumnSchemaPkList.Count == 0)
            {
                throw new Exception("实体类没有主键！");
            }
            int rel = 0;

            #region 设置参数及Update语句
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = null;
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();

            foreach (SqlDbColumnSchema column in dbColumnSchemaList)
            {
                if (column.IsPrimaryKey)
                {
                    wheres.AppendFormat(" and {0} = @{0} ", column.ColumnName);
                    //wheres.Append(" and ").Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(" ");
                    if (column.Property.GetValue(obj, null) == null)
                    {
                        throw new Exception("更新主键不能为空！");
                    }
                }
                else
                {
                    sets.AppendFormat("{0} = @{0},", column.ColumnName);
                    //sets.Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(",");
                }
                parm = new SqlParameter();
                parm.SqlDbType = GetSqlDbType(column.Type);
                parm.ParameterName = "@" + column.ColumnName;
                if (column.Property.GetValue(obj, null) != null)
                {
                    parm.Value = column.Property.GetValue(obj, null);
                }
                else
                {
                    parm.Value = DBNull.Value;
                }
                parms.Add(parm);
            }
            sets.Remove(sets.Length - 1, 1);
            #endregion

            sqlUpdate.AppendFormat("Update [{0}] set {1} where 1=1 ", GetTableName(dbTableSchema, tableName), sets);
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlUpdate.Append(" and ").Append(partitionName);
            //}
            sqlUpdate.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlUpdate.ToString(), parms.ToArray());

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public override int Update<T>(DbTransaction trans, T obj, string tableName)
        {
            DateTime dtBegin = DateTime.Now; 
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaPkList = GetPrimaryColumnList(dbColumnSchemaList);
            if (dbColumnSchemaPkList.Count == 0)
            {
                throw new Exception("实体类没有主键！");
            }
            int rel = 0;

            #region 设置参数及Update语句
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = null;
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();

            foreach (SqlDbColumnSchema column in dbColumnSchemaList)
            {
                if (column.IsPrimaryKey)
                {
                    wheres.AppendFormat(" and {0} = @{0} ", column.ColumnName);
                    //wheres.Append(" and ").Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(" ");
                    if (column.Property.GetValue(obj, null) == null)
                    {
                        throw new Exception("更新主键不能为空！");
                    }
                }
                else
                {
                    sets.AppendFormat("{0} = @{0},", column.ColumnName);
                    //sets.Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(",");
                }
                parm = new SqlParameter();
                parm.SqlDbType = GetSqlDbType(column.Type);
                parm.ParameterName = "@" + column.ColumnName;
                if (column.Property.GetValue(obj, null) != null)
                {
                    parm.Value = column.Property.GetValue(obj, null);
                }
                else
                {
                    parm.Value = DBNull.Value;
                }
                parms.Add(parm);
            }
            sets.Remove(sets.Length - 1, 1);
            #endregion

            sqlUpdate.AppendFormat("Update [{0}] set {1} where 1=1 ", GetTableName(dbTableSchema, tableName), sets);
            //sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo)).Append(" set ").Append(sets).Append(" where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlUpdate.Append(" and ").Append(partitionName);
            //}
            sqlUpdate.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlUpdate.ToString(), parms.ToArray());

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        
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
        public override int Update(string connectionString, Dictionary<string, object> sets, List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            if (sets.Count == 0)
            {
                throw new Exception("需要提供需更新字段！");
            } 
            if (String.IsNullOrEmpty(tableName))
            {
                throw new Exception("需要提供要更新的表！");
            }
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new Exception("需要提供连接字符串！");
            }
            List<DbParameter> inputParameters = new List<DbParameter>();
            List<DbParameter> parameters = new List<DbParameter>();
            //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            string _setsString = GetSetsString(sets, ref inputParameters);
            int rel = 0;
            StringBuilder sqlUpdate = new StringBuilder();
            sqlUpdate.AppendFormat("Update [{0}] set {1} where 1=1 {2} ", tableName, _setsString, _conditionsString);
            //sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            //sqlUpdate.Append(" set ").Append(Sets).Append(" where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlUpdate.Append(" and ").Append(partitionName);
            //}
            //sqlUpdate.Append(_conditionsString);
            parameters.AddRange(inputParameters);
            rel = _SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, sqlUpdate.ToString(), parameters.ToArray());

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);

            return rel;
        }
        
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
        public override int Update(DbTransaction trans, Dictionary<string, object> Sets, List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now; 
            if (Sets.Count == 0)
            {
                throw new Exception("需要提供需更新字段！");
            }
            if (String.IsNullOrEmpty(tableName))
            {
                throw new Exception("需要提供要更新的表！");
            }
            List<DbParameter> inputParameters = new List<DbParameter>();
            List<DbParameter> parameters = new List<DbParameter>();
            //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            string _setsString = GetSetsString(Sets, ref inputParameters);
            int rel = 0;
            StringBuilder sqlUpdate = new StringBuilder();
            sqlUpdate.AppendFormat("Update [{0}] set {1} where 1=1 {2} ", tableName, _setsString, _conditionsString);
            //sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            //sqlUpdate.Append(" set ").Append(Sets).Append(" where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlUpdate.Append(" and ").Append(partitionName);
            //}
            //sqlUpdate.Append(_conditionsString);
            parameters.AddRange(inputParameters);
            rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlUpdate.ToString(), parameters.ToArray());

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);

            return rel;
        }

        #region 不需要
        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// </summary>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="partitionName">分区名</param>
        ///// <returns>
        ///// 影响的行数
        ///// </returns>
        //public override int Update<T>(string Sets, string Conditions, StringBuilder partitionName)
        //{
        //    DateTime dtBegin = DateTime.Now; 
        //    Type type = typeof(T);
        //    DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
        //    //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);string tableName = GetTableName(dbTableSchema, query.TableName);
        //    int rel = 0;
        //    StringBuilder sqlUpdate = new StringBuilder();
        //    sqlUpdate.AppendFormat("Update {0} set {1} where 1=1 ", GetTableName(dbTableSchema), Sets);
        //    //sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
        //    //sqlUpdate.Append(" set ").Append(Sets).Append(" where 1=1 ");
        //    if (!IsNullOrEmpty(partitionName))
        //    {
        //        sqlUpdate.Append(" and ").Append(partitionName);
        //    }
        //    sqlUpdate.Append(Conditions);
        //    rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlUpdate.ToString(), null);

        //    base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);

        //    return rel;
        //}

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// Updates the specified sets.
        ///// </summary>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <returns></returns>
        //public override int Update<T>(string Sets, string Conditions)
        //{
        //    return Update<T>(Sets, Conditions, new StringBuilder());
        //}

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
        //public override int Update<T>(string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName)
        //{
        //    DateTime dtBegin = DateTime.Now;
        //    Type type = typeof(T);
        //    DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
        //    List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
        //    int rel = 0;
        //    int i = 0;
        //    string SortOrder = GetSortOrder(dbColumnSchemaList, "");
        //    StringBuilder sqlUpdate = new StringBuilder();
        //    sqlUpdate.AppendFormat("Update {0}  set {1}  where {2}  IN ((Select Top {3} {2}  from  {1}  where 1=1 ", GetTableName(dbTableSchema), Sets, GetPageKey(dbTableSchema, dbColumnSchemaList), PrecedingNumber); 
        //    //sqlUpdate.Append("Update ").Append(GetTableName(dbTableSchema));
        //    //sqlUpdate.Append(" set ").Append(Sets).Append(" where ").Append(GetPageKey(dbTableSchema, dbColumnSchemaList)).Append(" IN ((Select Top ").Append(PrecedingNumber).Append(" ").Append(GetPageKey(dbTableSchema, dbColumnSchemaList)).Append(" from ").Append(GetTableName(dbTableSchema)).Append(" where 1=1 ");
        //    if (!IsNullOrEmpty(partitionName))
        //    {
        //        sqlUpdate.Append(" and ").Append(partitionName);
        //    }
        //    sqlUpdate.AppendFormat(" {0}  Order by {1} )) ", Conditions, SortOrder);
        //    //sqlUpdate.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
        //    if (!IsNullOrEmpty(partitionName))
        //    {
        //        sqlUpdate.Append(" and ").Append(partitionName);
        //    }
        //    sqlUpdate.Append(Conditions);
        //    rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlUpdate.ToString(), null);

        //    base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);

        //    return rel;
        //}

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// Updates the specified sets.
        ///// </summary>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="PrecedingNumber">The preceding number.</param>
        ///// <returns></returns>
        //public override int Update<T>(string Sets, string Conditions, int PrecedingNumber)
        //{
        //    return Update<T>(Sets, Conditions, PrecedingNumber, new StringBuilder());
        //}

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
        //public override int Update<T>(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName)
        //{
        //    DateTime dtBegin = DateTime.Now;
        //    Type type = typeof(T);
        //    DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
        //    List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
        //    int rel = 0;
        //    int i = 0;
        //    string SortOrder = GetSortOrder(dbColumnSchemaList, "");
        //    StringBuilder sqlUpdate = new StringBuilder();
        //    sqlUpdate.AppendFormat("Update {0}  set {1}  where {2}  IN ((Select Top {3} {2}  from  {1}  where 1=1 ", GetTableName(dbTableSchema), Sets, GetPageKey(dbTableSchema, dbColumnSchemaList), PrecedingNumber); 
        //    //sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
        //    //sqlUpdate.Append(" set ").Append(Sets).Append(" where ").Append(GetPageKey()).Append(" IN ((Select Top ").Append(PrecedingNumber).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
        //    if (!IsNullOrEmpty(partitionName))
        //    {
        //        sqlUpdate.Append(" and ").Append(partitionName);
        //    }
        //    sqlUpdate.AppendFormat(" {0}  Order by {1} )) ", Conditions, SortOrder);
        //    //sqlUpdate.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
        //    if (!IsNullOrEmpty(partitionName))
        //    {
        //        sqlUpdate.Append(" and ").Append(partitionName);
        //    }
        //    sqlUpdate.Append(Conditions);
        //    rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlUpdate.ToString(), null);


        //    base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);
        //    return rel;
        //}

        ///// <summary>
        ///// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        ///// Updates the specified sets.
        ///// </summary>
        ///// <param name="trans">The trans.</param>
        ///// <param name="Sets">The sets.</param>
        ///// <param name="Conditions">The conditions.</param>
        ///// <param name="PrecedingNumber">The preceding number.</param>
        ///// <returns></returns>
        //public override int Update<T>(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber)
        //{
        //    return Update<T>(trans, Sets, Conditions, PrecedingNumber, new StringBuilder());
        //}

        #endregion

        /// <summary>
        /// 向数据表T更新一条记录。
        /// Updates the specified table name.
        /// </summary>
        /// <param name="tableName">表名    Name of the table.</param>
        /// <param name="primaryKeys">主键  The primary keys.</param>
        /// <param name="dicts">字段    The dicts.</param>
        /// <returns>影响的行数</returns>
        public override int Update(string connectionString, Dictionary<string, string> dicts, string[] primaryKeys, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            DataTable dt = GetEmptyTable(connectionString, tableName);
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm;
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();

            if (primaryKeys.Length == 0)
            {
                throw new Exception("更新主键不能为空！");
            }
            foreach (string s in primaryKeys)
            {
                if (!dicts.ContainsKey(s.ToLower()))
                {
                    throw new Exception("更新主键不能为空！");
                }
            }


            foreach (DataColumn column in dt.Columns)
            {
                if (dicts.ContainsKey(column.ColumnName.ToLower()))
                {
                    if (StringHelper.IsExist(column.ColumnName, primaryKeys))
                    {
                        wheres.Append(" and ").Append(column.ColumnName).Append(" = ").Append("@").Append(column.ColumnName).Append(" ");
                        if (!dicts.ContainsKey(column.ColumnName.ToLower()))
                        {
                            throw new Exception("更新主键不能为空！");
                        }
                        if (String.IsNullOrEmpty(dicts[column.ColumnName.ToLower()].ToString()))
                        {
                            throw new Exception("更新主键不能为空！");
                        }
                    }
                    else
                    {
                        sets.Append(column.ColumnName).Append(" = ").Append("@").Append(column.ColumnName).Append(",");
                    }
                    parm = new SqlParameter();
                    parm.SqlDbType = GetSqlDbType(column.DataType);
                    parm.ParameterName = "@" + column.ColumnName;
                    parm.Value = GetValue(column.DataType, dicts[column.ColumnName.ToLower()].ToString());
                    parms.Add(parm);
                }
            }
            sets.Remove(sets.Length - 1, 1);

            sqlUpdate.Append("Update [").Append(tableName).Append("] set ").Append(sets).Append(" where 1=1 ");
            sqlUpdate.Append(wheres);

            rel = _SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, sqlUpdate.ToString(), parms.ToArray());

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        #endregion

        #region 删除
        /// <summary>
        ///  删除数据表T中的一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int Delete<T>(T obj)
        {
            return Delete<T>(obj, "");
        }
        /// <summary>
        ///  删除数据表T中的一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete<T>(T obj, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaPkList = GetPrimaryColumnList(dbColumnSchemaList);
            if (dbColumnSchemaPkList.Count == 0)
            {
                throw new Exception("对象没有主键！");
            }
            int rel = 0;

            #region 设置参数及Delete语句
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlDelete = new StringBuilder();


            foreach (DbColumnSchema column in dbColumnSchemaPkList)
            {
                if (column.IsPrimaryKey)
                {
                    wheres.AppendFormat(" and {0} = @{0} ", column.ColumnName);
                    //wheres.Append(" and ").Append(column.ColumnName).Append(" = ").Append("@").Append(column.ColumnName).Append(" ");
                    parm = new SqlParameter();
                    parm.SqlDbType = GetSqlDbType(column.Type);
                    parm.ParameterName = "@" + column.ColumnName;
                    if (column.Property.GetValue(obj, null) != null)
                    {
                        parm.Value = column.Property.GetValue(obj, null);
                    }
                    else
                    {
                        throw new Exception("删除对像主键不能为空！");
                    }
                    parms.Add(parm);
                }
            }
            #endregion

            sqlDelete.Append("Delete from [").Append(GetTableName(dbTableSchema, tableName)).Append("] where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlDelete.Append(" and ").Append(partitionName);
            //}
            sqlDelete.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlDelete.ToString(), parms.ToArray());

            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override int Delete<T>(DbTransaction trans, T obj)
        {
            return this.Delete<T>(trans, obj, "");
        }
        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete<T>(DbTransaction trans, T obj, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaPkList = GetPrimaryColumnList(dbColumnSchemaList);
            if (dbColumnSchemaPkList.Count == 0)
            {
                throw new Exception("对象没有主键！");
            }
            int rel = 0;

            #region 设置参数及Delete语句
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlDelete = new StringBuilder();

            foreach (DbColumnSchema column in dbColumnSchemaList)
            {
                if (column.IsPrimaryKey)
                {
                    wheres.AppendFormat(" and {0} = @{0} ", column.ColumnName);
                    //wheres.Append(" and ").Append(column.ColumnName).Append(" = ").Append("@").Append(column.ColumnName).Append(" ");
                    parm = new SqlParameter();
                    parm.SqlDbType = GetSqlDbType(column.Type);
                    parm.ParameterName = "@" + column.ColumnName;
                    if (column.Property.GetValue(obj, null) != null)
                    {
                        parm.Value = column.Property.GetValue(obj, null);
                    }
                    else
                    {
                        throw new Exception("删除对像主键不能为空！");
                    }
                    parms.Add(parm);
                }
            }
            #endregion

            sqlDelete.Append("Delete from [").Append(GetTableName(dbTableSchema, tableName)).Append("] where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlDelete.Append(" and ").Append(partitionName);
            //}
            sqlDelete.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlDelete.ToString(), parms.ToArray());

            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <returns></returns>
        public override int Delete<T>(List<Condition> conditions)
        {
            return Delete<T>(conditions, "");

        }
        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete<T>(List<Condition> conditions, string tableName)
        {
            int rel = 0;
            if (conditions == null)
            {
                return rel;
            }
            if (conditions.Count == 0)
            {
                return rel;
            }

            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);

            StringBuilder sqlDelete = new StringBuilder();
            sqlDelete.Append("Delete from [").Append(GetTableName(dbTableSchema, tableName)).Append("] where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlDelete.Append(" and ").Append(partitionName);
            //}
            sqlDelete.Append(_conditionsString);
            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlDelete.ToString(), parameters.ToArray());
            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);

            return rel;
        }

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <returns></returns>
        public override int Delete<T>(DbTransaction trans, List<Condition> conditions)
        {
            return Delete<T>(trans, conditions, "");
        }
        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete<T>(DbTransaction trans, List<Condition> conditions, string tableName)
        {
            int rel = 0;
            if (conditions == null)
            {
                return rel;
            }
            if (conditions.Count == 0)
            {
                return rel;
            }

            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);

            DateTime dtBegin = DateTime.Now;
            StringBuilder sqlDelete = new StringBuilder();
            sqlDelete.Append("Delete from [").Append(GetTableName(dbTableSchema, tableName)).Append("] where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlDelete.Append(" and ").Append(partitionName);
            //}
            sqlDelete.Append(_conditionsString);
            rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlDelete.ToString(), parameters.ToArray());
            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);

            return rel;
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override int Delete<T>(string Field, string Value)
        {
            return Delete<T>(Field, Value, "");
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete<T>(string Field, string Value, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            conditions.Add(new Condition(Field, Value));
            return Delete<T>(conditions, tableName);
            //Type type = typeof(T);
            //DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            //int rel = 0;
            //if (!String.IsNullOrEmpty(Field.Trim()))
            //{
            //    DateTime dtBegin = DateTime.Now;
            //    StringBuilder wheres = new StringBuilder();
            //    StringBuilder sqlDelete = new StringBuilder();


            //    wheres.Append(" and ").Append(Field.Trim()).Append(" = ").Append("'").Append(Value.Trim()).Append("' ");

            //    sqlDelete.Append("Delete from ").Append(GetTableName(dbTableSchema)).Append(" where 1=1 ");
            //    if (!IsNullOrEmpty(partitionName))
            //    {
            //        sqlDelete.Append(" and ").Append(partitionName);
            //    }
            //    sqlDelete.Append(wheres);
            //    rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(dbTableSchema), CommandType.Text, sqlDelete.ToString(), null);
            //    base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            //}
            //return rel;
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public override int Delete<T>(DbTransaction trans, string Field, string Value)
        {
            return Delete<T>(trans, Field, Value, "");
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete<T>(DbTransaction trans, string Field, string Value, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            conditions.Add(new Condition(Field, Value));
            return Delete<T>(trans, conditions, tableName);
            //Type type = typeof(T);
            //DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            //int rel = 0;
            //if (!String.IsNullOrEmpty(Field.Trim()))
            //{
            //    DateTime dtBegin = DateTime.Now;
            //    StringBuilder wheres = new StringBuilder();
            //    StringBuilder sqlDelete = new StringBuilder();


            //    wheres.Append(" and ").Append(Field.Trim()).Append(" = ").Append("'").Append(Value.Trim()).Append("' ");

            //    sqlDelete.Append("Delete from ").Append(GetTableName(dbTableSchema)).Append(" where 1=1 ");
            //    if (!IsNullOrEmpty(partitionName))
            //    {
            //        sqlDelete.Append(" and ").Append(partitionName);
            //    }
            //    sqlDelete.Append(wheres);
            //    rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlDelete.ToString(), null);
            //    base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            //}
            //return rel;
        }





        #endregion

        #region 根据 指定字段 指定值，返回单个实体类
        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <returns></returns>
        public override T GetEntity<T>(List<Condition> conditions)
        {
            return GetEntity<T>(conditions, "");
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>T 对象</returns>
        public override T GetEntity<T>(string Field, string Value)
        {
            return GetEntity<T>(Field, Value, "");
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override T GetEntity<T>(List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            T _obj = Activator.CreateInstance<T>();

            //SqlParameter[] parms = null;
            //StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();
            //string[] keys = new string[dict.Count];
            //dict.Keys.CopyTo(keys, 0);
            //for (int i = 0; i < keys.Length; i++)
            //{
            //    wheres.AppendFormat(" and {0} = '{1}' ", keys[i], dict[keys[i]].ToString());
            //    //wheres.Append(" and ").Append(keys[i]).Append(" = ").Append(" '").Append(dict[keys[i]].ToString()).Append("' ");
            //}
            sqlSelect.AppendFormat("select * from [{0}]  with  (nolock)   where 1=1 ", GetTableName(dbTableSchema, tableName));
            //sqlSelect.Append("select * from ").Append(GetTableName(dbTableSchema)).Append(" where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString);
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray()))
            {
                if (_populateMode == PopulateMode.Emit)
                {
                    _obj = PopulateFromDrByEmitFirstOrDefault<T>(dr);
                }
                else
                {
                    _obj = PopulateFromDrFirstOrDefault<T>(dr);
                }
            }
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return _obj;
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>T 对象</returns>
        public override T GetEntity<T>(string Field, string Value, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            conditions.Add(new Condition(Field, Value));
            return GetEntity<T>(conditions, tableName);
            //DateTime dtBegin = DateTime.Now;
            //Type type = typeof(T);
            //DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            //T _obj = Activator.CreateInstance<T>();

            //SqlParameter[] parms = null;
            //StringBuilder wheres = new StringBuilder();
            //StringBuilder sqlSelect = new StringBuilder();
            //wheres.AppendFormat(" and {0} = '{1}' ", Field, Value);
            ////wheres.Append(" and ").Append(Field).Append(" = '").Append(Value).Append("' ");
            //sqlSelect.AppendFormat("select * from {0} where 1=1 ", GetTableName(dbTableSchema));
            ////sqlSelect.Append("select * from ").Append(GetTableName(dbTableSchema)).Append(" where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            //sqlSelect.Append(wheres.ToString());
            //using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parms))
            //{
            //    //_obj = _entityConverter.FirstOrDefault(dr); 
            //    _obj = PopulateFromDrFirstOrDefault<T>(dr);
            //}
            //base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            //return _obj;
        }
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
        public override List<T> GetList<T>(Query query)
        {
            DateTime dtBegin = DateTime.Now;
            List<DbParameter> parameters = new List<DbParameter>();
            Type type = typeof(T);
            SqlDbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type) as SqlDbTableSchema;
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            List<T> list = new List<T>();
            string tableName = GetTableName(dbTableSchema, query.TableName);
            string SortOrder = GetSortOrder(dbColumnSchemaList, base.GetOrderString(query.Orders));
            string SelectFieldsString = GetSelectFieldsString(query.SelectFields);
            string ConditionsString = GetConditionsString(query.Conditions, ref parameters);
            ConditionsString += query.ConditionString;
            parameters.AddRange(query.DbParameters);
            if (!String.IsNullOrEmpty(query.PartitionName))
            {
                ConditionsString = String.Format(" and {0} {1} ", query.PartitionName, ConditionsString);
            }
            StringBuilder sqlSelect = new StringBuilder();
            if (query.IsPaging)
            {
                switch (dbTableSchema.SqlVerson)
                {
                    case SqlServerVerson.Sql2000:
                        {
                            //0:TableName
                            //1:StartRecordIndex
                            //2:PageSize
                            //3:Where
                            //4:Order
                            //5:Selects
                            //6:PageKey
                            sqlSelect.AppendFormat(sqlSelectPaging2000, tableName, query.StartRecordIndex, query.PageSize, ConditionsString, SortOrder, SelectFieldsString, GetPageKey(dbTableSchema, dbColumnSchemaList));
                        }
                        break;
                    case SqlServerVerson.Sql2005:
                    case SqlServerVerson.Sql2008:
                        {
                            //0:TableName
                            //1:StartRecordIndex
                            //2:PageSize
                            //3:Where
                            //4:Order
                            //5:Selects
                            sqlSelect.AppendFormat(sqlSelectPaging2005, tableName, query.StartRecordIndex, query.PageSize, ConditionsString, SortOrder, SelectFieldsString);                        
                        }
                        break;
                }
            }
            else
            {
                //0:TableName
                //1:Where
                //2:Order
                //3:Selects
                sqlSelect.AppendFormat(sqlSelectAll, tableName, ConditionsString, SortOrder, SelectFieldsString);
            }

            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray()))
            {
                //
                if (_populateMode == PopulateMode.Emit)
                {
                    list = PopulateFromDrByEmit<T>(dr);
                }
                else
                {
                    list = PopulateFromDr<T>(dr);
                }
            }

            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return list;
        }
        #endregion

        #region 返回DataTable 对象
        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <param name="pagingKey">SqlServer2000必须写</param>
        /// <returns></returns>
        public override DataTable GetDataTable(Query query, string connectionString, string pagingKey)
        {
            DateTime dtBegin = DateTime.Now;
            if (String.IsNullOrEmpty(query.TableName))
            {
                throw new Exception("查询条件 表名不能为空！");
            }
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new Exception("查询条件 连接字符串不能为空！");
            }
            List<DbParameter> parameters = new List<DbParameter>();
            string tableName = query.TableName;
            string SortOrder = base.GetOrderString(query.Orders);
            string SelectFieldsString = GetSelectFieldsString(query.SelectFields);
            string ConditionsString = GetConditionsString(query.Conditions, ref parameters);
            ConditionsString += query.ConditionString;
            parameters.AddRange(query.DbParameters);
            if (!String.IsNullOrEmpty(query.PartitionName))
            {
                ConditionsString = String.Format(" and {0} {1} ", query.PartitionName, ConditionsString);
            }
            StringBuilder sqlSelect = new StringBuilder();
            if (query.IsPaging)
            {
                switch (SqlVerson)
                {
                    case SqlServerVerson.Sql2000:
                        {
                            if (String.IsNullOrEmpty(pagingKey))
                            {
                                throw new Exception("SqlServer 2000 分页 Key 不能为空！");
                            }
                            //0:TableName
                            //1:StartRecordIndex
                            //2:PageSize
                            //3:Where
                            //4:Order
                            //5:Selects
                            //6:PageKey                             
                            sqlSelect.AppendFormat(sqlSelectPaging2000, tableName, query.StartRecordIndex, query.PageSize, ConditionsString, SortOrder, SelectFieldsString, pagingKey);
                        }
                        break;
                    case SqlServerVerson.Sql2005:
                    case SqlServerVerson.Sql2008:
                        {
                            //0:TableName
                            //1:StartRecordIndex
                            //2:PageSize
                            //3:Where
                            //4:Order
                            //5:Selects
                            sqlSelect.AppendFormat(sqlSelectPaging2005, tableName, query.StartRecordIndex, query.PageSize, ConditionsString, SortOrder, SelectFieldsString);
                        }
                        break;
                }
            }
            else
            {
                //0:TableName
                //1:Where
                //2:Order
                //3:Selects
                sqlSelect.AppendFormat(sqlSelectAll, tableName, ConditionsString, SortOrder, SelectFieldsString);
            }
            DataTable dt = null;
            DataSet ds = _SqlHelper.ExecuteDataAdapter(connectionString, CommandType.Text, sqlSelect.ToString(), parameters.ToArray());
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }          

            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }

        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>List＜T＞对象集</returns>
        /// <remarks>示例:
        /// List＜T＞ list = SqlServerDAL.GetList();</remarks>
        public override DataTable GetDataTable<T>(Query query)
        {
            DateTime dtBegin = DateTime.Now;
            List<DbParameter> parameters = new List<DbParameter>();
            Type type = typeof(T);
            SqlDbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type) as SqlDbTableSchema;
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            //List<T> list = new List<T>();
            string tableName = GetTableName(dbTableSchema, query.TableName);
            string SortOrder = GetSortOrder(dbColumnSchemaList, base.GetOrderString(query.Orders));
            string SelectFieldsString = GetSelectFieldsString(query.SelectFields);
            string ConditionsString = GetConditionsString(query.Conditions, ref parameters);
            ConditionsString += query.ConditionString;
            parameters.AddRange(query.DbParameters);
            if (!String.IsNullOrEmpty(query.PartitionName))
            {
                ConditionsString = String.Format(" and {0} {1} ", query.PartitionName, ConditionsString);
            }
            StringBuilder sqlSelect = new StringBuilder();
            if (query.IsPaging)
            {
                switch (dbTableSchema.SqlVerson)
                {
                    case SqlServerVerson.Sql2000:
                        {
                            //0:TableName
                            //1:StartRecordIndex
                            //2:PageSize
                            //3:Where
                            //4:Order
                            //5:Selects
                            //6:PageKey
                            sqlSelect.AppendFormat(sqlSelectPaging2000, tableName, query.StartRecordIndex, query.PageSize, ConditionsString, SortOrder, SelectFieldsString, GetPageKey(dbTableSchema, dbColumnSchemaList));
                        }
                        break;
                    case SqlServerVerson.Sql2005:
                    case SqlServerVerson.Sql2008:
                        {
                            //0:TableName
                            //1:StartRecordIndex
                            //2:PageSize
                            //3:Where
                            //4:Order
                            //5:Selects
                            sqlSelect.AppendFormat(sqlSelectPaging2005, tableName, query.StartRecordIndex, query.PageSize, ConditionsString, SortOrder, SelectFieldsString);
                        }
                        break;
                }
            }
            else
            {
                //0:TableName
                //1:Where
                //2:Order
                //3:Selects
                sqlSelect.AppendFormat(sqlSelectAll, tableName, ConditionsString, SortOrder, SelectFieldsString);
            }

            DataTable dt = null;
            DataSet ds = _SqlHelper.ExecuteDataAdapter(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray());
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }      

            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
        #endregion

        #region 查询实体总数
        /// <summary>
        /// 查询实体数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>实体的总数</returns>
        /// <remarks>示例:
        /// int val = SqlServerDAL.GetCount();</remarks>
        public override long GetCount(Query query, string connectionString)
        {
            DateTime dtBegin = DateTime.Now;
            if (String.IsNullOrEmpty(query.TableName))
            {
                throw new Exception("查询条件 表名不能为空！");
            }
            List<DbParameter> parameters = new List<DbParameter>();
            long rel = 0;

            string ConditionsString = GetConditionsString(query.Conditions, ref parameters);
            ConditionsString += query.ConditionString;
            parameters.AddRange(query.DbParameters);
            if (!String.IsNullOrEmpty(query.PartitionName))
            {
                ConditionsString = String.Format(" and {0} {1} ", query.PartitionName, ConditionsString);
            }
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.AppendFormat(sqlSelectCount, query.TableName, ConditionsString);

            rel = Convert.ToInt64(_SqlHelper.ExecuteScalar(connectionString, CommandType.Text, sqlSelect.ToString(), parameters.ToArray()));
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 查询实体数
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>实体的总数</returns>
        /// <remarks>示例:
        /// int val = SqlServerDAL.GetCount();</remarks>
        public override long GetCount<T>(Query query)
        {
            Type type = typeof(T);
            SqlDbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type) as SqlDbTableSchema;
            query.TableName = GetTableName(dbTableSchema, query.TableName);
            return GetCount(query, GetConnectionString(dbTableSchema));
        }
        #endregion

        #region 根据 指定字段 指定值，检测是否存在
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>数量</returns>
        public override int IsExist<T>(List<Condition> conditions)
        {
            return IsExist<T>(conditions, "");
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>数量</returns>
        public override int IsExist<T>(string Field, string Value)
        {
            return IsExist<T>(Field, Value, "");
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns>数量</returns>
        public override int IsExist<T>(List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            int count = 0;

            //SqlParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();

            //string[] keys = new string[dict.Count];
            //dict.Keys.CopyTo(keys, 0);
            //for (int i = 0; i < keys.Length; i++)
            //{
            //    wheres.Append(" and ").Append(keys[i]).Append(" = ").Append(" '").Append(dict[keys[i]].ToString()).Append("' ");
            //}

            sqlSelect.Append("select count(*) from [").Append(GetTableName(dbTableSchema, tableName)).Append("]   with  (nolock)  where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString);
            count = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray()));

            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return count;
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>数量</returns>
        public override int IsExist<T>(string Field, string Value, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            conditions.Add(new Condition(Field, Value));
            return IsExist<T>(conditions, tableName);
        }
        #endregion

        #region 获取 指定字段的 最大值
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue<T>(string Field)
        {
            return GetMaxValue<T>(Field, "");
        }
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue<T>(string Field, List<Condition> conditions)
        {
            return GetMaxValue<T>(Field, conditions, "");
        }

        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue<T>(string Field, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            return GetMaxValue<T>(Field, conditions, tableName);           
        }
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue<T>(string Field, List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            string max = string.Empty;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select max(").Append(Field).Append(") from [").Append(GetTableName(dbTableSchema, tableName));
            sqlSelect.Append("]   with  (nolock)  where len(").Append(Field).Append(") = (select max(len(").Append(Field).Append(")) from  ").Append(GetTableName(dbTableSchema, tableName)).Append("   with  (nolock)  where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString.ToString());
            sqlSelect.Append(") ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString.ToString());

            max = _SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray()).ToString();
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return max;
        }
        #endregion

        #region 获取 指定字段的 最小值
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最小值</returns>		
        public override string GetMinValue<T>(string Field)
        {
            return GetMinValue<T>(Field, "");
        }
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最小值</returns>		
        public override string GetMinValue<T>(string Field, List<Condition> conditions)
        {
            return GetMinValue<T>(Field, conditions, "");
        }
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public override string GetMinValue<T>(string Field, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            return GetMinValue<T>(Field, conditions, tableName);
        }
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public override string GetMinValue<T>(string Field, List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            string min = string.Empty;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select min(").Append(Field).Append(") from [").Append(GetTableName(dbTableSchema, tableName));
            sqlSelect.Append("]   with  (nolock)  where len(").Append(Field).Append(") = (select min(len(").Append(Field).Append(")) from  ").Append(GetTableName(dbTableSchema, tableName)).Append("   with  (nolock)  where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString.ToString());
            sqlSelect.Append(") ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString.ToString());
            min = _SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray()).ToString();
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return min;
        }
        #endregion

        #region 获取 指定字段的  唯一行
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable<T>(string Fields)
        {
            return GetDistinctTable<T>(Fields, "");
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable<T>(string Fields, List<Condition> conditions)
        {
            return GetDistinctTable<T>(Fields, conditions, "");
        }

        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable<T>(string Fields, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            return GetDistinctTable<T>(Fields, conditions, tableName);
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable<T>(string Fields, List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            DataTable dt = null;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select DISTINCT ").Append(Fields).Append(" from [").Append(GetTableName(dbTableSchema, tableName)).Append("]   with  (nolock)  where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString);
            DataSet ds = _SqlHelper.ExecuteDataAdapter(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray());
            dt = ds.Tables[0];
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
        #endregion

        #region 获取 指定字段的  唯一行数
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public override long GetDistinctCount<T>(string Fields)
        {
            return GetDistinctCount<T>(Fields, "");
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public override long GetDistinctCount<T>(string Fields, List<Condition> conditions)
        {
            return GetDistinctCount<T>(Fields, conditions, "");
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public override long GetDistinctCount<T>(string Fields, string tableName)
        {
            List<Condition> conditions = new List<Condition>();
            return GetDistinctCount<T>(Fields, conditions, tableName);


        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 结果集中的唯一行
        /// </returns>
        public override long GetDistinctCount<T>(string Fields, List<Condition> conditions, string tableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            List<DbParameter> parameters = new List<DbParameter>();
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            //List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            string _conditionsString = GetConditionsString(conditions, ref parameters);
            long rel = 0;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("Select count(*) from  (select DISTINCT ").Append(Fields).Append(" from [").Append(GetTableName(dbTableSchema, tableName)).Append("]   with  (nolock)  where 1=1 ");
            //if (!IsNullOrEmpty(partitionName))
            //{
            //    sqlSelect.Append(" and ").Append(partitionName);
            //}
            sqlSelect.Append(_conditionsString).Append(" ) as t");
            rel = Convert.ToInt64(_SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), parameters.ToArray()));
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        #endregion

        #region 检测表是否存在
        /// <summary>
        /// 根据表名 检测表是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>是/否</returns>
        public override bool IsExistTable<T>()
        {
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            return IsExistTable<T>(GetTableName(dbTableSchema, ""));
        }

        /// <summary>
        /// 根据表名 检测表是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>是/否</returns>
        public override bool IsExistTable<T>(string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            bool bRel = false;
            string sqlStr = "select count(*) from sysobjects where id = object_id('" + TableName + "') and objectproperty(id, 'IsUserTable') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null));
            if (rel > 0)
            {
                bRel = true;
            }
            else
            {
                bRel = false;
            }
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return bRel;
        }
        /// <summary>
        /// 根据表名 查询符合条件的 表信息 （模糊查询） 
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public override DataTable GetTableList<T>(string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            DataTable dt;
            string sqlStr = "select * from sysobjects where name like '%" + TableName + "%' and objectproperty(id, 'IsUserTable') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null).Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
        #endregion

        #region 检测视图是否存在
        /// <summary>
        /// 根据视图名 检测视图是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns>是/否</returns>
        public override bool IsExistView<T>()
        {
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            return IsExistView<T>(GetTableName(dbTableSchema, ""));
        }
        /// <summary>
        /// 根据视图名 检测视图是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns>是/否</returns>
        public override bool IsExistView<T>(string ViewName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            bool bRel = false;
            string sqlStr = "select count(*) from sysobjects where id = object_id('" + ViewName + "') and objectproperty(id, 'IsView') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null));
            if (rel > 0)
            {
                bRel = true;
            }
            else
            {
                bRel = false;
            }
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return bRel;
        }
        /// <summary>
        /// 根据视图名 查询符合条件的 视图信息 （模糊查询） 
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns></returns>
        public override DataTable GetViewList<T>(string ViewName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            DataTable dt;
            string sqlStr = "select * from sysobjects where name like '%" + ViewName + "%' and objectproperty(id, 'IsView') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null).Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
        #endregion

        #region 检测字段是否存在
        /// <summary>
        /// 根据字段名 检测字段是否存在 （精确查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <returns>是/否</returns>
        public override bool IsExistField<T>(string FieldName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            bool bRel = false;
            string sqlStr = "select count(*) from syscolumns where name = '" + FieldName + "' and id = object_id('" + GetTableName(dbTableSchema, "") + "')  and objectproperty(id, 'IsUserTable') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null));
            if (rel > 0)
            {
                bRel = true;
            }
            else
            {
                bRel = false;
            }
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return bRel;
        }

        /// <summary>
        /// 根据字段名 检测字段是否存在 （精确查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名</param>
        /// <returns>
        ///   是/否
        /// </returns>
        public override bool IsExistField<T>(string FieldName, string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            bool bRel = false;
            string sqlStr = "select count(*) from syscolumns where name = '" + FieldName + "' and id = object_id('" + TableName + "')  and objectproperty(id, 'IsUserTable') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null));
            if (rel > 0)
            {
                bRel = true;
            }
            else
            {
                bRel = false;
            }
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return bRel;
        }

        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <returns></returns>
        public override DataTable GetFieldList<T>(string FieldName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            DataTable dt;
            string sqlStr = "select OBJECT_NAME(a.id) as TABLE_NAME, a.name as COLUMN_NAME, b.value as COMMENTS, a.* from syscolumns a left join sys.extended_properties b on a.id = b.major_id and a.colid=b.minor_id where a.name like '%" + FieldName + "%' and a.id = object_id('" + GetTableName(dbTableSchema, "") + "')  and objectproperty(a.id, 'IsUserTable') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null).Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return dt;
        }

        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名(精确)</param>
        /// <returns></returns>
        public override DataTable GetFieldList<T>(string FieldName, string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            Type type = typeof(T);
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            DataTable dt;
            string sqlStr = "select OBJECT_NAME(a.id) as TABLE_NAME, a.name as COLUMN_NAME, b.value as COMMENTS, a.* from syscolumns a left join sys.extended_properties b on a.id = b.major_id and a.colid=b.minor_id where a.name like '%" + FieldName + "%' and a.id = object_id('" + TableName + "')  and objectproperty(a.id, 'IsUserTable') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(dbTableSchema), CommandType.Text, sqlStr, null).Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return dt;
        }

        #endregion

        #region 执行SQL语句
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public override int ExecuteSQL(string SQLString)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery(base.ConnectionString, CommandType.Text, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public override int ExecuteSQL(string SQLString, CommandType commandType)
        {

            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery(base.ConnectionString, commandType, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public override int ExecuteSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters)
        {

            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery(base.ConnectionString, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public int ExecuteSQL(string SQLString, CommandType commandType, params SqlParameter[] commandParameters)
        {

            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery(base.ConnectionString, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public override int ExecuteSQL(DbTransaction trans, string SQLString)
        {

            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public override int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType)
        {

            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, commandType, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public override int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType, params DbParameter[] commandParameters)
        {

            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType, params SqlParameter[] commandParameters)
        {

            DateTime dtBegin = DateTime.Now;
            int rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public override object ExecuteScalarSQL(string SQLString)
        {

            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar(base.ConnectionString, CommandType.Text, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public override object ExecuteScalarSQL(string SQLString, CommandType commandType)
        {

            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar(base.ConnectionString, commandType, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public override object ExecuteScalarSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar(base.ConnectionString, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public object ExecuteScalarSQL(string SQLString, CommandType commandType, params SqlParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar(base.ConnectionString, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>影响的行数</returns>
        public override object ExecuteScalarSQL(DbTransaction trans, string SQLString)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar((SqlTransaction)trans, CommandType.Text, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public override object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar((SqlTransaction)trans, commandType, SQLString, null);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public override object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType, params DbParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar((SqlTransaction)trans, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the scalar SQL.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType, params SqlParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _SqlHelper.ExecuteScalar((SqlTransaction)trans, commandType, SQLString, commandParameters);
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <returns>DataTable</returns>
        public override DataTable ExecuteDataAdapterSQL(string SQLString)
        {

            DateTime dtBegin = DateTime.Now;
            DataTable rel = _SqlHelper.ExecuteDataAdapter(base.ConnectionString, CommandType.Text, SQLString, null).Tables[0];
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the data adapter SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public override DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable rel = _SqlHelper.ExecuteDataAdapter(base.ConnectionString, commandType, SQLString, null).Tables[0];
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the data adapter SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public override DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType, params DbParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable rel = _SqlHelper.ExecuteDataAdapter(base.ConnectionString, commandType, SQLString, commandParameters).Tables[0];
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        /// <summary>
        /// Executes the data adapter SQL.
        /// </summary>
        /// <param name="SQLString">The SQL string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType, params SqlParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable rel = _SqlHelper.ExecuteDataAdapter(base.ConnectionString, commandType, SQLString, commandParameters).Tables[0];
            base.WriteLog(SQLString.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        #endregion

        #region 事务
        /// <summary>
        /// 创建一个事务
        /// </summary>
        /// <returns>返回一个事务</returns>
        public override DbTransaction BeginTransaction()
        {
            return (DbTransaction)_SqlHelper.CreateTransaction(base.ConnectionString);
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        public override void TransactionCommit(DbTransaction trans)
        {
            _SqlHelper.TransactionCommit((SqlTransaction)trans);
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        public override void TransactionRollBack(DbTransaction trans)
        {
            _SqlHelper.TransactionRollBack((SqlTransaction)trans);
        }
        #endregion


        #region 格式化日期查询参数

        /// <summary>
        /// Gets the begin date string.
        /// </summary>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetDateToDbSelectString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
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
        protected override string GetConditionsString(List<Condition> conditions, ref List<DbParameter> parameters)
        {
            SqlParameter param = new SqlParameter();
            StringBuilder condition = new StringBuilder();
            foreach (Condition c in conditions)
            {
                //if (condition.Length > 0)
                //{
                    
                //}
                condition.Append(" and ");
                switch (c.QueryOperator)
                {
                    case QueryOperator.Equal:
                        {
                            condition.AppendFormat("{0} {1} @{0}_eq", c.Key, "=");
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = String.Format("@{0}_eq", c.Key);
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.Greater:
                        {
                            condition.AppendFormat("{0} {1} @{0}_gt", c.Key, ">");
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = String.Format("@{0}_gt",c.Key);
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.GreaterOrEqual:
                        {
                            condition.AppendFormat("{0} {1} @{0}_gteq", c.Key, ">=");
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = String.Format("@{0}_gteq" , c.Key);
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.Less:
                        {
                            condition.AppendFormat("{0} {1} @{0}_lt", c.Key, "<");
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = String.Format("@{0}_lt", c.Key);
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.LessOrEqual:
                        {
                            condition.AppendFormat("{0} {1} @{0}_lteq", c.Key, "<=");
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = String.Format("@{0}_lteq", c.Key);
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.Like:
                        {
                            condition.AppendFormat("{0} like  '%' + @{0} + '%'", c.Key);
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = "@" + c.Key;
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.LikeStart:
                        {
                            condition.AppendFormat("{0} like @{0} + '%'", c.Key);
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = "@" + c.Key;
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.NotEqual:
                        {
                            condition.AppendFormat("{0} {1} @{0}_noteq", c.Key, "<>");
                            param = new SqlParameter();
                            param.SqlDbType = GetSqlDbType(c.Value.GetType());
                            param.ParameterName = String.Format("@{0}_noteq", c.Key);
                            param.Value = c.Value;
                            parameters.Add(param);
                        }
                        break;
                    case QueryOperator.IsNull:
                        {
                            condition.AppendFormat("{0} is null ", c.Key);
                        }
                        break;
                    case QueryOperator.IsNotNull:
                        {
                            condition.AppendFormat("{0} is not null ", c.Key);
                        }
                        break;
                    case QueryOperator.In:
                        {

                            if (c.Values != null)
                            {
                                int len = c.Values.Length;
                                if (len > 0)
                                {
                                    string ps = "";
                                    List<string> pList = new List<string>();
                                    SqlDbType sqlDbType = GetSqlDbType(c.Values[0].GetType());
                                    for (int i = 0; i < len; i++)
                                    {
                                        ps = String.Format("@{0}{1}", c.Key, i);
                                        pList.Add(ps);
                                        param = new SqlParameter();
                                        param.SqlDbType = sqlDbType;
                                        param.ParameterName = ps;
                                        param.Value = c.Values[i];
                                        parameters.Add(param);
                                    }
                                    condition.AppendFormat("{0} {1} ({2})", c.Key, "in", String.Join(",", pList.ToArray()));
                                }
                            }
                            else
                            {
                                condition.AppendFormat("{0} {1} (@{0})", c.Key, "in");
                                param = new SqlParameter();
                                param.SqlDbType = GetSqlDbType(c.Value.GetType());
                                param.ParameterName = "@" + c.Key;
                                param.Value = c.Value;
                                parameters.Add(param);
                            }
                        }
                        break;

                }

            }
            return condition.ToString();
        }
        /// <summary>
        /// Gets the sets string.
        /// </summary>
        /// <param name="sets">The sets.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string GetSetsString(Dictionary<string, object> sets, ref List<DbParameter> parameters)
        {
            SqlParameter param = new SqlParameter();
            StringBuilder setsString = new StringBuilder();
            foreach (KeyValuePair<string, object> set in sets)
            {
                if (setsString.Length > 0)
                {
                    setsString.Append(" , ");
                }
                setsString.AppendFormat(" {0} = @Input{0} ", set.Key);
                param = new SqlParameter();
                param.SqlDbType = GetSqlDbType(set.Value.GetType());
                param.ParameterName = "@Input" + set.Key;
                param.Value = set.Value;
                parameters.Add(param);
            }
            return setsString.ToString();
        } 
        #endregion

        #region 私有方法
        /// <summary>
        /// 跟据对像数据类型  获取  数据库对应该类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private SqlDbType GetSqlDbType(Type type)
        {
            SqlDbType sqlDbType = SqlDbType.VarChar;
            switch (type.Name)
            {
                case "String":
                    {
                        sqlDbType = SqlDbType.VarChar;
                    }
                    break;
                case "Boolean":
                    {
                        sqlDbType = SqlDbType.Bit;
                    }
                    break;
                case "Decimal":
                    {
                        sqlDbType = SqlDbType.Decimal;
                    }
                    break;
                case "Single":
                    {
                        sqlDbType = SqlDbType.Float;
                    }
                    break;
                case "Double":
                    {
                        sqlDbType = SqlDbType.Real;
                    }
                    break;
                case "Int16":
                    {
                        sqlDbType = SqlDbType.SmallInt;
                    }
                    break;
                case "Int32":
                    {
                        sqlDbType = SqlDbType.Int;
                    }
                    break;
                case "Int64":
                    {
                        sqlDbType = SqlDbType.BigInt;
                    }
                    break;
                case "DateTime":
                    {
                        sqlDbType = SqlDbType.DateTime;
                    }
                    break;
                case "Nullable`1":
                    {
                        NullableConverter nullableConverter = new NullableConverter(type);
                        switch (nullableConverter.UnderlyingType.Name)
                        {
                            case "String":
                                {
                                    sqlDbType = SqlDbType.VarChar;
                                }
                                break;
                            case "Boolean":
                                {
                                    sqlDbType = SqlDbType.Bit;
                                }
                                break;
                            case "Decimal":
                                {
                                    sqlDbType = SqlDbType.Decimal;
                                }
                                break;
                            case "Single":
                                {
                                    sqlDbType = SqlDbType.Float;
                                }
                                break;
                            case "Double":
                                {
                                    sqlDbType = SqlDbType.Real;
                                }
                                break;
                            case "Int16":
                                {
                                    sqlDbType = SqlDbType.SmallInt;
                                }
                                break;
                            case "Int32":
                                {
                                    sqlDbType = SqlDbType.Int;
                                }
                                break;
                            case "Int64":
                                {
                                    sqlDbType = SqlDbType.BigInt;
                                }
                                break;
                            case "DateTime":
                                {
                                    sqlDbType = SqlDbType.DateTime;
                                }
                                break;
                        }
                    }
                    break;
            }
            return sqlDbType;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sqlDbColumnInfos"></param>
        ///// <returns></returns>
        //private List<DbColumnInfo> GetDbColumnInfos(List<SqlDbColumnInfo> sqlDbColumnInfos)
        //{
        //    List<DbColumnInfo> list = new List<DbColumnInfo>();
        //    foreach (SqlDbColumnInfo sqlDbColumnInfo in sqlDbColumnInfos)
        //    {
        //        list.Add(sqlDbColumnInfo as DbColumnInfo);
        //    }
        //    return list;
        //}




        /// <summary>
        /// 获取排序条件
        /// </summary>
        /// <param name="Sort"></param>
        /// <returns></returns>
        private string GetSortOrder(List<DbColumnSchema> list, string Sort)
        {
            string SortOrder = string.Empty;
            if (!String.IsNullOrEmpty(Sort))
            {
                SortOrder = Sort;
            }
            else
            {
                List<DbColumnSchema> primaryList = base.GetPrimaryColumnList(list);
                if (primaryList.Count > 0)
                {
                    SortOrder = " Order by " + primaryList[0].ColumnName + " asc";
                }
                else
                {
                    SortOrder = " Order by 1 asc ";
                }
            }
            return SortOrder;
        }

        /// <summary>
        /// 获取分页 关键 字段
        /// </summary>
        /// <returns></returns>
        private string GetPageKey(DbTableSchema dbTableSchema, List<DbColumnSchema> list)
        {
            string pageKey = string.Empty;
            if (list.Count > 0)
            {
                pageKey = list[0].ColumnName;
            }
            else
            {
                string sqlSelect = "select column_name from information_schema.columns where table_name='" + dbTableSchema.TableName + "' and ordinal_position=1";
                pageKey = _SqlHelper.ExecuteScalar(GetConnectionString(dbTableSchema), CommandType.Text, sqlSelect.ToString(), null).ToString();
            }
            return pageKey;
        }

        /// <summary>
        /// 判断 StringBuilder 是否为空
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        private bool IsNullOrEmpty(StringBuilder sb)
        {
            if (sb == null)
            {
                return true;
            }
            if (sb.Length <= 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected List<T> PopulateFromDr<T>(IDataReader dr)
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            T _obj;
            DbTableSchema dbTableSchema = SqlDbTableSchemaBuilder.GetInstance().Generate(type);
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            while (dr.Read())
            {
                _obj = Activator.CreateInstance<T>();
                foreach (SqlDbColumnSchema dbColumn in dbColumnSchemaList)
                {
                    try
                    {
                        if (!IsNullOrDBNull(dr[dbColumn.ColumnName]))
                        {
                            dbColumn.Property.SetValue(_obj, ReflectionHelper.ChangeType(dr[dbColumn.ColumnName], dbColumn.Property.PropertyType), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(String.Format("{0}\nTableName:{1}\nColumnName:{2}", ex.Message.ToString(), dbTableSchema.TableName.ToString(), dbColumn.ColumnName.ToString()));
                    }
                }
                list.Add(_obj);
            }
            dr.Close();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public List<T> PopulateFromDrByEmit<T>(IDataReader dr)
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            Func<IDataRecord, object> func = FuncManager.IDataRecordToEntityFuncFactory(type, dbColumnSchemaList);
            while (dr.Read())
            {
                list.Add((T)func(dr));
            }
            return list;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public T PopulateFromDrByEmitFirstOrDefault<T>(IDataReader dr)
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            List<DbColumnSchema> dbColumnSchemaList = SqlDbColumnSchemaBuilder.GetInstance().Generate(type);
            Func<IDataRecord, object> func = FuncManager.IDataRecordToEntityFuncFactory(type, dbColumnSchemaList);
            while (dr.Read())
            {
                return (T)func(dr);
            }
            return default(T);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected T PopulateFromDrFirstOrDefault<T>(IDataReader dr)
        {
            List<T> list = PopulateFromDr<T>(dr);
            if (list.Count > 0)
            {
                return list[0];
            }
            return default(T);
        }

        /// <summary>
        /// Gets the empty table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private DataTable GetEmptyTable(string connectionString, string tableName)
        {
            DataTable dt = CacheHelper.GetCache(tableName + "_EmptyTable") as DataTable;
            if (dt == null)
            {
                StringBuilder sqlSelect = new StringBuilder();
                sqlSelect.AppendFormat("SELECT * FROM [{0}]  with  (nolock)   WHERE 1=2 ", tableName);
                dt = _SqlHelper.ExecuteDataAdapter(connectionString, CommandType.Text, sqlSelect.ToString(), null).Tables[0];
                CacheHelper.SetCache(tableName + "_EmptyTable", dt);
            }
            return dt;
        }




        #endregion
    }
}
