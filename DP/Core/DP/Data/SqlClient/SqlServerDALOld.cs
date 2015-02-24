using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using DP.Common;
using DP.Data.Common;
using System.Data.Common;

namespace DP.Data.SqlClient
{
    public class SqlServerDALOld<T> : DALOld<T>
    {
        #region 变量
        SqlDbTableInfo _dbTableInfo = null;
        List<SqlDbColumnInfo> _dbColumnInfos = new List<SqlDbColumnInfo>();
        List<SqlDbColumnInfo> _dbPrimaryKeyColumns = new List<SqlDbColumnInfo>();
        SqlHelper _SqlHelper;
        #endregion       

        #region 属性
        /// <summary>
        /// 数据库访问对像
        /// </summary>
        public SqlHelper SqlHelper
        {
            get { return _SqlHelper; }
            set { _SqlHelper = value; }
        }
        #endregion

        #region 构造函数
        public SqlServerDAL()
            : this("")
        {

        }
        public SqlServerDAL(string connectionString)
        {
            _connectionString = connectionString;
            _dbTableInfo = new SqlDbTableInfo(typeof(T));
            foreach (PropertyInfo property in _properties)
            {
                SqlDbColumnInfo dbColumn = new SqlDbColumnInfo(property);
                //是否有自增主键
                if (dbColumn.IncrementPrimaryKey)
                {
                    _IncrementPrimaryKey = true;
                }
                //统计主键和非主键的个数
                if (dbColumn.PrimaryKey)
                {
                    _primaryColumnCount++;
                    _dbPrimaryKeyColumns.Add(dbColumn);
                }
                else
                {
                    _nonPrimaryColumnCount++;
                }
                _dbColumnInfos.Add(dbColumn);
            }
            _entityConverter = new EntityConverter<T>(GetDbColumnInfos(_dbColumnInfos));
            _SqlHelper = new SqlHelper(GetConnectionString(_dbTableInfo));
        }
        #endregion

        #region 添加
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <param name="T">T实体</param>
        /// <returns>影响的行数</returns>
        public override int Insert(T obj)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            int parmsCount = 0;

            #region 设置参数及Insert语句
            if (_IncrementPrimaryKey)
            {
                parmsCount = _nonPrimaryColumnCount;
            }
            else
            {
                parmsCount = _nonPrimaryColumnCount + _primaryColumnCount;
            }

            int i = 0;
            SqlParameter[] parms = new SqlParameter[parmsCount];

            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();
            PropertyInfo propertyKey = null;

            foreach (SqlDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey && _IncrementPrimaryKey)
                {
                    if (dbColumn.IncrementPrimaryKey)
                    {
                        propertyKey = dbColumn.Property;
                    }
                    continue;
                }
                fields.Append(dbColumn.ColumnName).Append(",");
                values.Append("@").Append(dbColumn.ColumnName).Append(",");
                parms[i] = new SqlParameter();
                parms[i].SqlDbType = GetSqlDbType(dbColumn.Type);
                parms[i].ParameterName = "@" + dbColumn.ColumnName;
                if (dbColumn.Property.GetValue(obj, null) != null)
                {
                    parms[i].Value = dbColumn.Property.GetValue(obj, null);

                }
                else
                {
                    parms[i].Value = DBNull.Value;
                }
                i++;
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);
            #endregion

            #region 跟据是否是自增主键进行Insert
            if (_IncrementPrimaryKey)
            {
                sqlInsert.Append("SET NOCOUNT ON;Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ); select @@identity ");

                object relobj = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms);
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
                sqlInsert.Append("Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");

                rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms);
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
        public override int Insert(DbTransaction trans, T obj)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            int parmsCount = 0;

            #region 设置参数及Insert语句
            if (_IncrementPrimaryKey)
            {
                parmsCount = _nonPrimaryColumnCount;
            }
            else
            {
                parmsCount = _nonPrimaryColumnCount + _primaryColumnCount;
            }

            int i = 0;
            SqlParameter[] parms = new SqlParameter[parmsCount];

            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();
            PropertyInfo propertyKey = null;

            foreach (SqlDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey && _IncrementPrimaryKey)
                {
                    if (dbColumn.IncrementPrimaryKey)
                    {
                        propertyKey = dbColumn.Property;
                    }
                    continue;
                }
                fields.Append(dbColumn.ColumnName).Append(",");
                values.Append("@").Append(dbColumn.ColumnName).Append(",");
                parms[i] = new SqlParameter();
                parms[i].SqlDbType = GetSqlDbType(dbColumn.Type);
                parms[i].ParameterName = "@" + dbColumn.ColumnName;
                if (dbColumn.Property.GetValue(obj, null) != null)
                {
                    parms[i].Value = dbColumn.Property.GetValue(obj, null);
                }
                else
                {
                    parms[i].Value = DBNull.Value;
                }
                i++;
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);
            #endregion

            #region 跟据是否是自增主键进行Insert
            if (_IncrementPrimaryKey)
            {
                sqlInsert.Append("SET NOCOUNT ON;Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ); select @@identity ");

                object relobj = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms);
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
                sqlInsert.Append("Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");

                rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlInsert.ToString(), parms);
            }
            #endregion
            base.WriteLog(sqlInsert.ToString(), dtBegin, DateTime.Now);
            return rel;

        }

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
        public override int Insert(string tableName, string[] primaryKeys, PrimaryKeyType primaryKeyType, Dictionary<string, string> dicts, out string primaryValues)
        {
            primaryValues = string.Empty;
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();
            DataTable dt = GetEmptyTable(tableName);

            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm;
            foreach (DataColumn column in dt.Columns)
            {
                if (StringHelper.IsExist(column.ColumnName, primaryKeys))
                {
                    switch (primaryKeyType)
                    {
                        case PrimaryKeyType.Auto:
                        case PrimaryKeyType.Sequence:
                            {

                            }
                            break;
                        case PrimaryKeyType.Guid:
                            {
                                if (dicts.ContainsKey(column.ColumnName.ToLower()))
                                {
                                    fields.Append(column.ColumnName).Append(",");
                                    values.Append("@").Append(column.ColumnName).Append(",");
                                    parm = new SqlParameter();
                                    parm.SqlDbType = GetSqlDbType(column.DataType);
                                    parm.ParameterName = "@" + column.ColumnName;
                                    parm.Value = Guid.NewGuid().ToString();
                                    parms.Add(parm);
                                    if (!String.IsNullOrEmpty(primaryValues))
                                    {
                                        primaryValues += ",";
                                    }
                                    primaryValues += parm.Value.ToString();
                                }
                            }
                            break;
                        case PrimaryKeyType.Other:
                            {
                                if (dicts.ContainsKey(column.ColumnName.ToLower()))
                                {
                                    fields.Append(column.ColumnName).Append(",");
                                    values.Append("@").Append(column.ColumnName).Append(",");
                                    parm = new SqlParameter();
                                    parm.SqlDbType = GetSqlDbType(column.DataType);
                                    parm.ParameterName = "@" + column.ColumnName;
                                    parm.Value = GetValue(column.DataType, dicts[column.ColumnName.ToLower()].ToString());
                                    parms.Add(parm);
                                    if (!String.IsNullOrEmpty(primaryValues))
                                    {
                                        primaryValues += ",";
                                    }
                                    primaryValues += parm.Value.ToString();
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if (dicts.ContainsKey(column.ColumnName.ToLower()))
                    {
                        fields.Append(column.ColumnName).Append(",");
                        values.Append("@").Append(column.ColumnName).Append(",");
                        parm = new SqlParameter();
                        parm.SqlDbType = GetSqlDbType(column.DataType);
                        parm.ParameterName = "@" + column.ColumnName;
                        parm.Value = GetValue(column.DataType, dicts[column.ColumnName.ToLower()].ToString());
                        parms.Add(parm);
                    }
                }
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);

            switch (primaryKeyType)
            {
                case PrimaryKeyType.Auto:
                case PrimaryKeyType.Sequence:
                    {
                        sqlInsert.Append("SET NOCOUNT ON;Insert Into ").Append(tableName).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ); select @@identity ");
                        object relobj = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms.ToArray());
                        long temp = System.Convert.ToInt64(relobj);
                        if (temp > 0)
                        {
                            primaryValues = temp.ToString();
                            rel = 1;
                        }
                        else
                        {
                            rel = 0;
                        }
                    }
                    break;
                case PrimaryKeyType.Guid:
                case PrimaryKeyType.Other:
                    {
                        sqlInsert.Append("Insert Into ").Append(tableName).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");
                        rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms.ToArray());
                    }
                    break;
            }

            base.WriteLog(sqlInsert.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        #endregion

        #region 更新
        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="T">T</param>
        /// <returns>影响的行数</returns>
        public override int Update(T obj)
        {
            return Update(obj, new StringBuilder());           
        }

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public override int Update(DbTransaction trans, T obj)
        {
            return Update(trans, obj, new StringBuilder());
        }

        /// <summary>
        /// 向数据表T更新一条记录。
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public override int Update(T obj, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;

            #region 设置参数及Update语句
            int i = 0;
            SqlParameter[] parms = new SqlParameter[_dbColumnInfos.Count];
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();

            foreach (SqlDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {                    
                    wheres.Append(" and ").Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(" ");
                    if (dbColumn.Property.GetValue(obj, null) == null)
                    {
                        throw new Exception("更新主键不能为空！");
                    }
                }
                else
                {
                    sets.Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(",");
                }
                parms[i] = new SqlParameter();
                parms[i].SqlDbType = GetSqlDbType(dbColumn.Type);
                parms[i].ParameterName = "@" + dbColumn.ColumnName;
                if (dbColumn.Property.GetValue(obj, null) != null)
                {
                    parms[i].Value = dbColumn.Property.GetValue(obj, null);

                }
                else
                {
                    parms[i].Value = DBNull.Value;
                }
                i++;
            }
            sets.Remove(sets.Length - 1, 1);
            #endregion

            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo)).Append(" set ").Append(sets).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" and ").Append(partitionName);
            }
            sqlUpdate.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), parms);

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
        public override int Update(DbTransaction trans, T obj, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;

            #region 设置参数及Update语句
            int i = 0;
            SqlParameter[] parms = new SqlParameter[_dbColumnInfos.Count];
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();

            foreach (SqlDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {
                    wheres.Append(" and ").Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(" ");
                    if (dbColumn.Property.GetValue(obj, null) == null)
                    {
                        throw new Exception("更新主键不能为空！");
                    }
                }
                else
                {
                    sets.Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(",");
                }
                parms[i] = new SqlParameter();
                parms[i].SqlDbType = GetSqlDbType(dbColumn.Type);
                parms[i].ParameterName = "@" + dbColumn.ColumnName;
                if (dbColumn.Property.GetValue(obj, null) != null)
                {
                    parms[i].Value = dbColumn.Property.GetValue(obj, null);

                }
                else
                {
                    parms[i].Value = DBNull.Value;
                }
                i++;
            }
            sets.Remove(sets.Length - 1, 1);
            #endregion

            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo)).Append(" set ").Append(sets).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" and ").Append(partitionName);
            }
            sqlUpdate.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlUpdate.ToString(), parms);

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// </summary>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
        public override int Update(string Sets, string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            int i = 0;
            StringBuilder sqlUpdate = new StringBuilder();
            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            sqlUpdate.Append(" set ").Append(Sets).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" and ").Append(partitionName);
            }
            sqlUpdate.Append(Conditions);
            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), null);

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);

            return rel;
        }

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// Updates the specified sets.
        /// </summary>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <returns></returns>
        public override int Update(string Sets, string Conditions)
        {
            return Update(Sets, Conditions, new StringBuilder());
        }

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
        public override int Update(string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            int i = 0;
            string SortOrder = GetSortOrder("");
            StringBuilder sqlUpdate = new StringBuilder();
            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            sqlUpdate.Append(" set ").Append(Sets).Append(" where ").Append(GetPageKey()).Append(" IN ((Select Top ").Append(PrecedingNumber).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" and ").Append(partitionName);
            }
            sqlUpdate.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" and ").Append(partitionName);
            }
            sqlUpdate.Append(Conditions);
            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), null);

            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);

            return rel;
        }

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// Updates the specified sets.
        /// </summary>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="PrecedingNumber">The preceding number.</param>
        /// <returns></returns>
        public override int Update(string Sets, string Conditions, int PrecedingNumber)
        {
            return Update(Sets, Conditions, PrecedingNumber, new StringBuilder());
        }

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
        public override int Update(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            int i = 0;
            string SortOrder = GetSortOrder("");
            StringBuilder sqlUpdate = new StringBuilder();
            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            sqlUpdate.Append(" set ").Append(Sets).Append(" where ").Append(GetPageKey()).Append(" IN ((Select Top ").Append(PrecedingNumber).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" and ").Append(partitionName);
            }
            sqlUpdate.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" and ").Append(partitionName);
            }
            sqlUpdate.Append(Conditions);
            rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlUpdate.ToString(), null);


            base.WriteLog(sqlUpdate.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 向数据表T更新符合条件的  前 PrecedingNumber 条记录。
        /// Updates the specified sets.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Sets">The sets.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="PrecedingNumber">The preceding number.</param>
        /// <returns></returns>
        public override int Update(DbTransaction trans, string Sets, string Conditions, int PrecedingNumber)
        {
            return Update(trans, Sets, Conditions, PrecedingNumber, new StringBuilder());
        }

        /// <summary>
        /// 向数据表T更新一条记录。
        /// Updates the specified table name.
        /// </summary>
        /// <param name="tableName">表名    Name of the table.</param>
        /// <param name="primaryKeys">主键  The primary keys.</param>
        /// <param name="dicts">字段    The dicts.</param>
        /// <returns>影响的行数</returns>
        public override int Update(string tableName, string[] primaryKeys, Dictionary<string, string> dicts)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            DataTable dt = GetEmptyTable(tableName);
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

            sqlUpdate.Append("Update ").Append(tableName).Append(" set ").Append(sets).Append(" where 1=1 ");
            sqlUpdate.Append(wheres);

            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), parms.ToArray());

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
        public override int Delete(T obj)
        {
            return Delete(obj, new StringBuilder());
        }
        /// <summary>
        ///  删除数据表T中的一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete(T obj, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;

            #region 设置参数及Delete语句
            SqlParameter[] parms = new SqlParameter[_primaryColumnCount];
            int i = 0;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlDelete = new StringBuilder();

            foreach (SqlDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {
                    wheres.Append(" and ").Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(" ");
                    parms[i] = new SqlParameter();
                    parms[i].SqlDbType = GetSqlDbType(dbColumn.Type);
                    parms[i].ParameterName = "@" + dbColumn.ColumnName;
                    if (dbColumn.Property.GetValue(obj, null) != null)
                    {
                        parms[i].Value = dbColumn.Property.GetValue(obj, null);
                    }
                    else
                    {
                        throw new Exception("删除对像主键不能为空！");
                    }
                    i++;
                }
            }
            #endregion

            sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlDelete.Append(" and ").Append(partitionName);
            }
            sqlDelete.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlDelete.ToString(), parms);

            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, T obj)
        {
            return this.Delete(trans, obj, new StringBuilder());
        }
        /// <summary>
        /// 删除数据表T中的一条记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, T obj, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;

            #region 设置参数及Delete语句
            SqlParameter[] parms = new SqlParameter[_primaryColumnCount];
            int i = 0;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlDelete = new StringBuilder();

            foreach (SqlDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {
                    wheres.Append(" and ").Append(dbColumn.ColumnName).Append(" = ").Append("@").Append(dbColumn.ColumnName).Append(" ");
                    parms[i] = new SqlParameter();
                    parms[i].SqlDbType = GetSqlDbType(dbColumn.Type);
                    parms[i].ParameterName = "@" + dbColumn.ColumnName;
                    if (dbColumn.Property.GetValue(obj, null) != null)
                    {
                        parms[i].Value = dbColumn.Property.GetValue(obj, null);
                    }
                    else
                    {
                        throw new Exception("删除对像主键不能为空！");
                    }
                    i++;
                }
            }
            #endregion

            sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlDelete.Append(" and ").Append(partitionName);
            }
            sqlDelete.Append(wheres);
            rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlDelete.ToString(), parms);

            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            return rel;
        }

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <returns></returns>
        public override int Delete(string Conditions)
        {
            return Delete(Conditions, new StringBuilder());

        }
        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="Conditions"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete(string Conditions, StringBuilder partitionName)
        {
            int rel = 0;
            if (!String.IsNullOrEmpty(Conditions.Trim()))
            {
                DateTime dtBegin = DateTime.Now;
                StringBuilder sqlDelete = new StringBuilder();
                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" and ").Append(partitionName);
                }
                sqlDelete.Append(Conditions);
                rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlDelete.ToString(), null);
                base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            }
            return rel;
        }

        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, string Conditions)
        {
            return Delete(trans, Conditions, new StringBuilder());
        }
        /// <summary>
        /// 删除数据表T中的符合条件的记录
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, string Conditions, StringBuilder partitionName)
        {
            int rel = 0;
            if (!String.IsNullOrEmpty(Conditions.Trim()))
            {
                DateTime dtBegin = DateTime.Now;
                StringBuilder sqlDelete = new StringBuilder();
                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" and ").Append(partitionName);
                }
                sqlDelete.Append(Conditions);
                rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlDelete.ToString(), null);
                base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            }
            return rel;
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override int Delete(string Field, string Value)
        {
            return Delete(Field, Value, new StringBuilder());
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete(string Field, string Value, StringBuilder partitionName)
        {
            int rel = 0;
            if (!String.IsNullOrEmpty(Field.Trim()))
            {
                DateTime dtBegin = DateTime.Now;
                StringBuilder wheres = new StringBuilder();
                StringBuilder sqlDelete = new StringBuilder();


                wheres.Append(" and ").Append(Field.Trim()).Append(" = ").Append("'").Append(Value.Trim()).Append("' ");

                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" and ").Append(partitionName);
                }
                sqlDelete.Append(wheres);
                rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlDelete.ToString(), null);
                base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            }
            return rel;
        }


        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, string Field, string Value)
        {
            return Delete(trans, Field, Value, new StringBuilder());
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,删除一个T对象
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, string Field, string Value, StringBuilder partitionName)
        {
            int rel = 0;
            if (!String.IsNullOrEmpty(Field.Trim()))
            {
                DateTime dtBegin = DateTime.Now;
                StringBuilder wheres = new StringBuilder();
                StringBuilder sqlDelete = new StringBuilder();

               
                wheres.Append(" and ").Append(Field.Trim()).Append(" = ").Append("'").Append(Value.Trim()).Append("' ");

                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" and ").Append(partitionName);
                }
                sqlDelete.Append(wheres);
                rel = _SqlHelper.ExecuteNonQuery((SqlTransaction)trans, CommandType.Text, sqlDelete.ToString(), null);
                base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            }
            return rel;
        }





        #endregion

        #region 根据 指定字段 指定值，返回单个实体类
        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <returns></returns>
        public override T GetEntity(Dictionary<string, string> dict)
        {
            return GetEntity(dict, new StringBuilder());
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>T 对象</returns>
        public override T GetEntity(string Field, string Value)
        {
            return GetEntity(Field, Value, new StringBuilder());
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T对象
        /// </summary>
        /// <param name="dict">对应 指定 名和值的 数据字典</param>
        /// <param name="partitionName">分区名</param>
        /// <returns></returns>
        public override T GetEntity(Dictionary<string, string> dict, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            T _obj = Activator.CreateInstance<T>();

            SqlParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();
            string[] keys = new string[dict.Count];
            dict.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                wheres.Append(" and ").Append(keys[i]).Append(" = ").Append(" '").Append(dict[keys[i]].ToString()).Append("' ");
            }

            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(wheres.ToString());
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms))
            {
                //_obj = _entityConverter.FirstOrDefault(dr); 
                _obj = PopulateFromDrFirstOrDefault(dr);
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
        public override T GetEntity(string Field, string Value, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            T _obj = Activator.CreateInstance<T>();

            SqlParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();
            wheres.Append(" and ").Append(Field).Append(" = '").Append(Value).Append("' ");
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(wheres.ToString());
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms))
            {
                //_obj = _entityConverter.FirstOrDefault(dr); 
                _obj = PopulateFromDrFirstOrDefault(dr);
            }
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return _obj;
        }
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
        public override List<T> GetList()
        {
            return GetList(new StringBuilder());
        }
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>List＜T＞对象集</returns>
        public override List<T> GetList(string Conditions)
        {
            return GetList(Conditions, new StringBuilder());
        }
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
        public override List<T> GetList(string Conditions, string Sort)
        {
            return GetList(Conditions, Sort, new StringBuilder());
        }
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
        public override List<T> GetList(string Conditions, int StartRecordIndex, int PageSize)
        {
            return GetList(Conditions, StartRecordIndex, PageSize, new StringBuilder());
        }
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
        public override List<T> GetList(string Conditions, int StartRecordIndex, int PageSize, string Sort)
        {
            return GetList(Conditions, StartRecordIndex, PageSize, Sort, new StringBuilder());
        }


        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// List＜T＞ list = SqlServerDAL.GetList();
        /// </remarks>
        /// <param name="partitionName">分区名</param>
        /// <returns>List＜T＞对象集</returns>
        public override List<T> GetList(StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            List<T> list = new List<T>();
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 "); 
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                //list = _entityConverter.Select(dr);
                list = PopulateFromDr(dr);
            }
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return list;
        }
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
        public override List<T> GetList(string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            List<T> list = new List<T>();
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions);
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                //list = _entityConverter.Select(dr);
                list = PopulateFromDr(dr);
            }
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return list;
        }
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
        public override List<T> GetList(string Conditions, string Sort, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            List<T> list = new List<T>();
            string SortOrder = GetSortOrder(Sort);
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions).Append(" order by ").Append(SortOrder);
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                //list = _entityConverter.Select(dr);
                list = PopulateFromDr(dr);
            }
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return list;
        }
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
        public override List<T> GetList(string Conditions, int StartRecordIndex, int PageSize, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            List<T> list = new List<T>();
            string SortOrder = GetSortOrder("");
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                //list = _entityConverter.Select(dr);
                list = PopulateFromDr(dr);
            }
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return list;
        }
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
        public override List<T> GetList(string Conditions, int StartRecordIndex, int PageSize, string Sort, StringBuilder partitionName)
        {

            DateTime dtBegin = DateTime.Now;
            List<T> list = new List<T>();
            string SortOrder = GetSortOrder(Sort);
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            using (SqlDataReader dr = _SqlHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                //list = _entityConverter.Select(dr);
                list = PopulateFromDr(dr);
            }
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return list;
        }
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
        public override DataTable GetDataTable()
        {
            return GetDataTable(new StringBuilder());
        }
        /// <summary>
        /// 得到符合条件的，数据表T所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>DataSet集</returns>
        public override DataTable GetDataTable(string Conditions)
        {
            return GetDataTable(Conditions, new StringBuilder());
        }
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
        public override DataTable GetDataTable(string Conditions, string Sort)
        {
            return GetDataTable(Conditions, Sort, new StringBuilder());
        }
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
        public override DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize)
        {
            return GetDataTable(Conditions, StartRecordIndex, PageSize, new StringBuilder());
        }
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
        public override DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize, string Sort)
        {
            return GetDataTable(Conditions, StartRecordIndex, PageSize, Sort, new StringBuilder());
        }


        /// <summary>
        /// 得到数据表T所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// DataTable table = SqlServerDAL.GetDataTable();
        /// </remarks>
        /// <param name="partitionName">分区名</param>
        /// <returns>DataSet集</returns>
        public override DataTable GetDataTable(StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt = new DataTable();
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
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
        public override DataTable GetDataTable(string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt = new DataTable();
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions);
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
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
        public override DataTable GetDataTable(string Conditions, string Sort, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt = new DataTable();
            string SortOrder = GetSortOrder(Sort);
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions).Append(" order by ").Append(SortOrder);
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
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
        public override DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt = new DataTable();
            string SortOrder = GetSortOrder("");
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
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
        public override DataTable GetDataTable(string Conditions, int StartRecordIndex, int PageSize, string Sort, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt = new DataTable();
            string SortOrder = GetSortOrder(Sort);
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
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
        public override long GetCount()
        {
            return GetCount(new StringBuilder());
        }
        /// <summary>
        /// 查询符合条件的实体总数
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// int val = SqlServerDAL.GetCount(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>实体的总数</returns>
        public override long GetCount(string Conditions)
        {
            return GetCount(Conditions, new StringBuilder()); 
        }
        /// <summary>
        /// 查询实体数
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// int val = SqlServerDAL.GetCount();
        /// </remarks>
        /// <param name="partitionName">分区名</param>
        /// <returns>实体的总数</returns>
        public override long GetCount(StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            long rel = 0;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("Select count(*) from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            rel = Convert.ToInt64(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null));
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
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
        public override long GetCount(string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            long rel = 0;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("Select count(*) from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions);
            rel = Convert.ToInt64(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null));
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return rel;
        }
        #endregion

        #region 根据 指定字段 指定值，检测是否存在
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>数量</returns>
        public override int IsExist(Dictionary<string, string> dict)
        {
            return IsExist(dict, new StringBuilder());
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>数量</returns>
        public override int IsExist(string Field, string Value)
        {
            return IsExist(Field, Value, new StringBuilder());
        }

        /// <summary>
        /// 根据 指定字段 指定值 ,返回存在数量
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="partitionName">分区名</param>
        /// <returns>数量</returns>
        public override int IsExist(Dictionary<string, string> dict, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int count = 0;

            SqlParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();

            string[] keys = new string[dict.Count];
            dict.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                wheres.Append(" and ").Append(keys[i]).Append(" = ").Append(" '").Append(dict[keys[i]].ToString()).Append("' ");
            }

            sqlSelect.Append("select count(*) from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(wheres.ToString());
            count = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms));

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
        public override int IsExist(string Field, string Value, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            int count = 0;
            SqlParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();
            wheres.Append(" and ").Append(Field).Append(" = '").Append(Value).Append("' ");
            sqlSelect.Append("select count(*) from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(wheres.ToString());
            count = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms));
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return count;
        }
        #endregion
         
        #region 获取 指定字段的 最大值
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue(string Field)
        {
            return GetMaxValue(Field, new StringBuilder());
        }
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue(string Field, string Conditions)
        {
            return GetMaxValue(Field, Conditions, new StringBuilder());
        }

        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue(string Field, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            string max = string.Empty;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select max(").Append(Field).Append(") from ").Append(GetTableName(_dbTableInfo));
            sqlSelect.Append(" where len(").Append(Field).Append(") = (select max(len(").Append(Field).Append(")) from  ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(") ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }

            max = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).ToString();
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return max;
        }
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最大值</returns>		
        public override string GetMaxValue(string Field, string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            string max = string.Empty;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select max(").Append(Field).Append(") from ").Append(GetTableName(_dbTableInfo));
            sqlSelect.Append(" where len(").Append(Field).Append(") = (select max(len(").Append(Field).Append(")) from  ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString());
            sqlSelect.Append(") ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString());

            max = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).ToString();
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
        public override string GetMinValue(string Field)
        {
            return GetMinValue(Field, new StringBuilder());
        }
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>最小值</returns>		
        public override string GetMinValue(string Field, string Conditions)
        {
            return GetMinValue(Field, Conditions, new StringBuilder());
        }
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public override string GetMinValue(string Field, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            string min = string.Empty;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select min(").Append(Field).Append(") from ").Append(GetTableName(_dbTableInfo));
            sqlSelect.Append(" where len(").Append(Field).Append(") = (select min(len(").Append(Field).Append(")) from  ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(") ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }

            min = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).ToString();
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return min;
        }
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="partitionName">分区名</param>
        /// <returns>最小值</returns>		
        public override string GetMinValue(string Field, string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            string min = string.Empty;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select min(").Append(Field).Append(") from ").Append(GetTableName(_dbTableInfo));
            sqlSelect.Append(" where len(").Append(Field).Append(") = (select min(len(").Append(Field).Append(")) from  ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString());
            sqlSelect.Append(") ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions.ToString());
            min = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).ToString();
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
        public override DataTable GetDistinctTable(string Fields)
        {
            return GetDistinctTable(Fields, new StringBuilder());
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable(string Fields, string Conditions)
        {
            return GetDistinctTable(Fields, Conditions, new StringBuilder());
        }

        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable(string Fields, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt = null; 
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select DISTINCT ").Append(Fields).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            DataSet ds = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null);
            dt = ds.Tables[0];
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return dt;
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable(string Fields, string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt = null;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("select DISTINCT ").Append(Fields).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions);
            DataSet ds = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null);
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
        public override long GetDistinctCount(string Fields)
        {
            return GetDistinctCount(Fields, new StringBuilder());
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public override long GetDistinctCount(string Fields, string Conditions)
        {
            return GetDistinctCount(Fields, Conditions, new StringBuilder());
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="partitionName">分区名</param>
        /// <returns>结果集中的唯一行</returns>
        public override long GetDistinctCount(string Fields, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            long rel = 0;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("Select count(*) from ( select DISTINCT ").Append(Fields).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(" ) ");
            rel = Convert.ToInt64(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null));
            base.WriteLog(sqlSelect.ToString(), dtBegin, DateTime.Now);
            return rel;

            
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
        public override long GetDistinctCount(string Fields, string Conditions, StringBuilder partitionName)
        {
            DateTime dtBegin = DateTime.Now;
            long rel = 0;
            StringBuilder sqlSelect = new StringBuilder();
            sqlSelect.Append("Select count(*) from  (select DISTINCT ").Append(Fields).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ");
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" and ").Append(partitionName);
            }
            sqlSelect.Append(Conditions).Append(" ) ");
            rel = Convert.ToInt64(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null));
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
        public override bool IsExistTable()
        {
            return IsExistTable(GetTableName(_dbTableInfo));
        }

        /// <summary>
        /// 根据表名 检测表是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>是/否</returns>
        public override bool IsExistTable(string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            bool bRel = false;
            string sqlStr = "select count(*) from sysobjects where id = object_id('" + TableName + "') and objectproperty(id, 'IsUserTable') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
        public override DataTable GetTableList(string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt;
            string sqlStr = "select * from sysobjects where name like '%" + TableName + "%' and objectproperty(id, 'IsUserTable') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
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
        public override bool IsExistView()
        {
            return IsExistView(GetTableName(_dbTableInfo));
        }
        /// <summary>
        /// 根据视图名 检测视图是否存在 （精确查询）
        /// </summary>
        /// <param name="TableName">视图名</param>
        /// <returns>是/否</returns>
        public override bool IsExistView(string ViewName)
        {
            DateTime dtBegin = DateTime.Now;
            bool bRel = false;
            string sqlStr = "select count(*) from sysobjects where id = object_id('" + ViewName + "') and objectproperty(id, 'IsView') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
        public override DataTable GetViewList(string ViewName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt;
            string sqlStr = "select * from sysobjects where name like '%" + ViewName + "%' and objectproperty(id, 'IsView') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
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
        public override bool IsExistField(string FieldName)
        {
            DateTime dtBegin = DateTime.Now;
            bool bRel = false;
            string sqlStr = "select count(*) from syscolumns where name = '" + FieldName + "' and id = object_id('" + GetTableName(_dbTableInfo) + "')  and objectproperty(id, 'IsUserTable') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
        public override bool IsExistField(string FieldName, string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            bool bRel = false;
            string sqlStr = "select count(*) from syscolumns where name = '" + FieldName + "' and id = object_id('" + TableName + "')  and objectproperty(id, 'IsUserTable') = 1 ";
            int rel = Convert.ToInt32(_SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
        public override DataTable GetFieldList(string FieldName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt;
            string sqlStr = "select OBJECT_NAME(a.id) as TABLE_NAME, a.name as COLUMN_NAME, b.value as COMMENTS, a.* from syscolumns a left join sys.extended_properties b on a.id = b.major_id and a.colid=b.minor_id where a.name like '%" + FieldName + "%' and a.id = object_id('" + GetTableName(_dbTableInfo) + "')  and objectproperty(a.id, 'IsUserTable') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return dt;
        }

        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名(精确)</param>
        /// <returns></returns>
        public override DataTable GetFieldList(string FieldName, string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt;
            string sqlStr = "select OBJECT_NAME(a.id) as TABLE_NAME, a.name as COLUMN_NAME, b.value as COMMENTS, a.* from syscolumns a left join sys.extended_properties b on a.id = b.major_id and a.colid=b.minor_id where a.name like '%" + FieldName + "%' and a.id = object_id('" + TableName + "')  and objectproperty(a.id, 'IsUserTable') = 1 ";
            dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
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
            int rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, SQLString, null);
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
            int rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), commandType, SQLString, null);
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
            int rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
            int rel = _SqlHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
            object rel = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SQLString, null);
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
            object rel = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), commandType, SQLString, null);
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
            object rel = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
            object rel = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
            DataTable rel = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, SQLString, null).Tables[0];
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
            DataTable rel = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), commandType, SQLString, null).Tables[0];
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
            DataTable rel = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters).Tables[0];
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
            DataTable rel = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters).Tables[0];
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
            return (DbTransaction)_SqlHelper.CreateTransaction(GetConnectionString(_dbTableInfo));
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
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public override string GetBeginDateString(string fieldName, DateTime beginDate)
        {
            string rel = "";
            rel = " (" + fieldName + " >= '" + beginDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
            return rel;
        }
        /// <summary>
        /// Gets the end date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public override string GetEndDateString(string fieldName, DateTime endDate)
        {
            string rel = "";
            rel = " (" + fieldName + " <= '" + endDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
            return rel;
        }
        /// <summary>
        /// Gets the date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="date">The date.</param>
        /// <param name="operators">The operators.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override string GetDateString(string fieldName, DateTime date, string operators, int offset)
        {
            string rel = "";
            rel = " (" + fieldName + operators + " '" + date.AddHours(0 - offset).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
            return rel;
        }

        /// <summary>
        /// Gets the begin date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public override string GetBeginDateString(string fieldName, string beginDate)
        {
            string rel = "";
            DateTime dt = DateTime.Now;
            if (DateTimeHelper.ToDateTime(beginDate, out dt))
            {
                rel = GetBeginDateString(fieldName, dt);
            }
            return rel;
        }
        /// <summary>
        /// Gets the end date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public override string GetEndDateString(string fieldName, string endDate)
        {
            string rel = "";
            DateTime dt = DateTime.Now;  
            if (DateTimeHelper.ToDateTime(endDate, out dt))
            {
                if (dt.Second == 0)
                {
                    dt.AddSeconds(59);
                }
                if (dt.Minute == 0)
                {
                    dt.AddMinutes(59);
                }
                if (dt.Hour == 0)
                {
                    dt.AddHours(23);
                }
                rel = GetBeginDateString(fieldName, dt);
            }
            return rel;
        }
        /// <summary>
        /// Gets the date string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="date">The date.</param>
        /// <param name="operators">The operators.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override string GetDateString(string fieldName, string date, string operators, int offset)
        {
            string rel = "";
            DateTime dt = DateTime.Now;
            if (DateTimeHelper.ToDateTime(date, out dt))
            {               
                rel = GetDateString(fieldName, dt, operators, offset);
            }
            return rel;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlDbColumnInfos"></param>
        /// <returns></returns>
        private List<DbColumnInfo> GetDbColumnInfos(List<SqlDbColumnInfo> sqlDbColumnInfos)
        {
            List<DbColumnInfo> list = new List<DbColumnInfo>();
            foreach (SqlDbColumnInfo sqlDbColumnInfo in sqlDbColumnInfos)
            {
                list.Add(sqlDbColumnInfo as DbColumnInfo);
            }
            return list;
        }




        /// <summary>
        /// 获取排序条件
        /// </summary>
        /// <param name="Sort"></param>
        /// <returns></returns>
        private string GetSortOrder(string Sort)
        {
            string SortOrder = string.Empty;
            if (!String.IsNullOrEmpty(Sort))
            {
                SortOrder = Sort;
            }
            else
            {
                if (_dbPrimaryKeyColumns.Count > 0)
                {
                    SortOrder = _dbPrimaryKeyColumns[0].ColumnName + " asc";
                }
                else
                {
                    SortOrder = " 1 asc ";
                }
            }
            return SortOrder;
        }

        /// <summary>
        /// 获取分页 关键 字段
        /// </summary>
        /// <returns></returns>
        private string GetPageKey()
        {
            string pageKey = string.Empty;
            if (_dbPrimaryKeyColumns.Count > 0)
            {
                pageKey = _dbPrimaryKeyColumns[0].ColumnName;
            }
            else
            {
                string sqlSelect = "select column_name from information_schema.columns where table_name='" + _dbTableInfo.TableName + "' and ordinal_position=1";
                pageKey = _SqlHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).ToString();
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
        protected List<T> PopulateFromDr(IDataReader dr)
        {
            List<T> list = new List<T>();
            T _obj;
            while (dr.Read())
            {
                _obj = Activator.CreateInstance<T>();
                foreach (SqlDbColumnInfo dbColumn in _dbColumnInfos)
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
                        LogHelper.WriteLog(String.Format("{0}\nTableName:{1}\nColumnName:{2}", ex.Message.ToString(), _dbTableInfo.TableName.ToString(), dbColumn.ColumnName.ToString()));
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
        /// <param name="dr"></param>
        /// <returns></returns>
        protected T PopulateFromDrFirstOrDefault(IDataReader dr)
        {
            List<T> list = PopulateFromDr(dr);
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
        private DataTable GetEmptyTable(string tableName)
        {
            DataTable dt = CacheHelper.GetCache(tableName + "_EmptyTable") as DataTable;
            if (dt == null)
            {
                StringBuilder sqlSelect = new StringBuilder();
                sqlSelect.AppendFormat("SELECT * FROM {0} WHERE 1=2 ", tableName);
                dt = _SqlHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
                CacheHelper.SetCache(tableName + "_EmptyTable", dt);
            }
            return dt;
        }


        
        
        #endregion
    }
}
