using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using DP.Common;
using DP.Data.Common;
using System.Data.Common;

namespace DP.Data.OracleClient
{
    public class OracleDALOld<T> : DALOld<T>
    {
        #region 变量
        OracleDbTableInfo _dbTableInfo = null;
        List<OracleDbColumnInfo> _dbColumnInfos = new List<OracleDbColumnInfo>();
        List<OracleDbColumnInfo> _dbPrimaryKeyColumns = new List<OracleDbColumnInfo>();
        OracleHelper _OracleHelper;
        #endregion       

        #region 属性
        /// <summary>
        /// 数据库访问对像
        /// </summary>
        public OracleHelper OracleHelper
        {
            get { return _OracleHelper; }
            set { _OracleHelper = value; }
        }
        #endregion

        #region 构造函数
        public OracleDAL()
            : this("")
        {

        }
        public OracleDAL(string connectionString)
        {
            _connectionString = connectionString;
            _dbTableInfo = new OracleDbTableInfo(typeof(T));
            foreach (PropertyInfo property in _properties)
            {
                OracleDbColumnInfo dbColumn = new OracleDbColumnInfo(property);
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
            _OracleHelper = new OracleHelper(GetConnectionString(_dbTableInfo));
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
        public override int Insert(T obj)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            int parmsCount = 0;
            string SelectSequence;

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
            OracleParameter[] parms = new OracleParameter[parmsCount];

            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();
            PropertyInfo propertyKey = null;

            foreach (OracleDbColumnInfo dbColumn in _dbColumnInfos)
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
                values.Append(":").Append(dbColumn.ColumnName).Append(",");
                parms[i] = new OracleParameter();
                parms[i].OracleType = GetOracleType(dbColumn.Type);
                parms[i].ParameterName = ":" + dbColumn.ColumnName;
                if (dbColumn.Property.GetValue(obj, null) != null)
                {
                    parms[i].Value = dbColumn.Property.GetValue(obj, null);
                }
                else
                {
                    parms[i].Value = DBNull.Value;
                }
                if (dbColumn.PrimaryKey && !String.IsNullOrEmpty(dbColumn.SequenceName))
                {
                    SelectSequence = "select " + dbColumn.SequenceName + ".NEXTVAL from dual ";
                    object relobj = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SelectSequence.ToString(), null);
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return 0;
                    }
                    parms[i].Value = relobj;
                    try
                    {
                        //dbColumn.Property.SetValue(obj, Convert.ToInt64(relobj), null);
                        dbColumn.Property.SetValue(obj, ReflectionHelper.ChangeType(relobj, dbColumn.Property.PropertyType), null);
                    }
                    catch(Exception ex)
                    {
                        LogHelper.WriteLog(String.Format("{0}\nColumnName:{0},ColumnValue", ex.Message.ToString(), dbColumn.ColumnName, relobj));
                    }
                }
                i++;
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);
            #endregion

            #region 跟据是否是自增主键进行Insert
            if (_IncrementPrimaryKey)
            {
                sqlInsert.Append("Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");

                rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms);
                if (rel > 0)
                {
                    foreach (OracleDbColumnInfo dbColumn in _dbPrimaryKeyColumns)
                    {
                        SelectSequence = "select " + dbColumn.SequenceName + ".CURRVAL from dual ";
                        object relobj = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SelectSequence.ToString(), null);
                        if (!(Object.Equals(relobj, null)) && !(Object.Equals(relobj, System.DBNull.Value)))
                        {
                            try
                            {
                                dbColumn.Property.SetValue(obj, ReflectionHelper.ChangeType(relobj, dbColumn.Property.PropertyType), null);
                                //dbColumn.Property.SetValue(obj, Convert.ToInt64(relobj), null);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.WriteLog(String.Format("{0}\nColumnName:{0},ColumnValue", ex.Message.ToString(), dbColumn.ColumnName, relobj));
                            }
                        }
                    }
                }
            }
            else
            {
                sqlInsert.Append("Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");

                rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms);
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
            string SelectSequence;

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
            OracleParameter[] parms = new OracleParameter[parmsCount];

            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();
            PropertyInfo propertyKey = null;

            foreach (OracleDbColumnInfo dbColumn in _dbColumnInfos)
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
                values.Append(":").Append(dbColumn.ColumnName).Append(",");
                parms[i] = new OracleParameter();
                parms[i].OracleType = GetOracleType(dbColumn.Type);
                parms[i].ParameterName = ":" + dbColumn.ColumnName;
                if (dbColumn.Property.GetValue(obj, null) != null)
                {
                    parms[i].Value = dbColumn.Property.GetValue(obj, null);
                }
                else
                {
                    parms[i].Value = DBNull.Value;
                }
                if (dbColumn.PrimaryKey && !String.IsNullOrEmpty(dbColumn.SequenceName))
                {
                    SelectSequence = "select " + dbColumn.SequenceName + ".NEXTVAL from dual ";
                    object relobj = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SelectSequence.ToString(), null);
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return 0;
                    }
                    parms[i].Value = relobj;
                    try
                    {
                        //dbColumn.Property.SetValue(obj, Convert.ToInt64(relobj), null);
                        dbColumn.Property.SetValue(obj, ReflectionHelper.ChangeType(relobj, dbColumn.Property.PropertyType), null);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(String.Format("{0}\nColumnName:{0},ColumnValue", ex.Message.ToString(), dbColumn.ColumnName, relobj));
                    }
                }
                i++;
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);
            #endregion

            #region 跟据是否是自增主键进行Insert
            if (_IncrementPrimaryKey)
            {
                sqlInsert.Append("Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");

                rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms);
                if (rel > 0)
                {
                    foreach (OracleDbColumnInfo dbColumn in _dbPrimaryKeyColumns)
                    {
                        SelectSequence = "select " + dbColumn.SequenceName + ".CURRVAL from dual ";
                        object relobj = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SelectSequence.ToString(), null);
                        if (!(Object.Equals(relobj, null)) && !(Object.Equals(relobj, System.DBNull.Value)))
                        {
                            try
                            {
                                dbColumn.Property.SetValue(obj, ReflectionHelper.ChangeType(relobj, dbColumn.Property.PropertyType), null);
                                //dbColumn.Property.SetValue(obj, Convert.ToInt64(relobj), null);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.WriteLog(String.Format("{0}\nColumnName:{0},ColumnValue", ex.Message.ToString(), dbColumn.ColumnName, relobj));
                            }
                        }
                    }
                }
            }
            else
            {
                sqlInsert.Append("Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");

                rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, CommandType.Text, sqlInsert.ToString(), parms);
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
            string SelectSequence;
            primaryValues = string.Empty;
            DateTime dtBegin = DateTime.Now;
            int rel = 0;
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder sqlInsert = new StringBuilder();
            DataTable dt = GetEmptyTable(tableName);

            List<OracleParameter> parms = new List<OracleParameter>();
            OracleParameter parm;
            foreach (DataColumn column in dt.Columns)
            {
                if (StringHelper.IsExist(column.ColumnName, primaryKeys))
                {
                    switch (primaryKeyType)
                    {
                        case PrimaryKeyType.Auto:
                            {

                            }
                            break;
                        case PrimaryKeyType.Sequence:
                            {
                                SelectSequence = "select SQ_" + tableName + ".NEXTVAL from dual ";
                                object relobj = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SelectSequence.ToString(), null);

                                fields.Append(column.ColumnName).Append(",");
                                values.Append("@").Append(column.ColumnName).Append(",");
                                parm = new OracleParameter();
                                parm.OracleType = GetOracleType(column.DataType);
                                parm.ParameterName = "@" + column.ColumnName;
                                parm.Value = relobj;
                                parms.Add(parm);
                                if (!String.IsNullOrEmpty(primaryValues))
                                {
                                    primaryValues += ",";
                                }
                                primaryValues += parm.Value.ToString();

                            }
                            break;
                        case PrimaryKeyType.Guid:
                            {
                                if (dicts.ContainsKey(column.ColumnName.ToLower()))
                                {
                                    fields.Append(column.ColumnName).Append(",");
                                    values.Append("@").Append(column.ColumnName).Append(",");
                                    parm = new OracleParameter();
                                    parm.OracleType = GetOracleType(column.DataType);
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
                                    parm = new OracleParameter();
                                    parm.OracleType = GetOracleType(column.DataType);
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
                        parm = new OracleParameter();
                        parm.OracleType = GetOracleType(column.DataType);
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
                    {
                        sqlInsert.Append("Insert Into ").Append(GetTableName(_dbTableInfo)).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");

                        rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms.ToArray());
                        if (rel > 0)
                        {
                            SelectSequence = "select SQ_" + tableName + ".CURRVAL from dual ";
                            object relobj = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SelectSequence.ToString(), null);
                            primaryValues = relobj.ToString();
                        }
                    }
                    break;
                case PrimaryKeyType.Sequence:
                case PrimaryKeyType.Guid:
                case PrimaryKeyType.Other:
                    {
                        sqlInsert.Append("Insert Into ").Append(tableName).Append(" ( ").Append(fields).Append(" ) values ( ").Append(values).Append(" ) ");
                        rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlInsert.ToString(), parms.ToArray());
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
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 影响的行数
        /// </returns>
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
            OracleParameter[] parms = new OracleParameter[_dbColumnInfos.Count];
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();

            foreach (OracleDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {
                    if (wheres.Length != 0)
                    {
                        wheres.Append(" and ");
                    }
                    wheres.Append(dbColumn.ColumnName).Append(" = ").Append(":").Append(dbColumn.ColumnName).Append(" ");

                    if (dbColumn.Property.GetValue(obj, null) == null)
                    {
                        throw new Exception("更新主键不能为空！");
                    }
                }
                else
                {
                    sets.Append(dbColumn.ColumnName).Append(" = ").Append(":").Append(dbColumn.ColumnName).Append(",");
                }
                parms[i] = new OracleParameter();
                parms[i].OracleType = GetOracleType(dbColumn.Type);
                parms[i].ParameterName = ":" + dbColumn.ColumnName;
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

            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlUpdate.Append(" set ").Append(sets).Append(" where ").Append(wheres);
            rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), parms);

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
            OracleParameter[] parms = new OracleParameter[_dbColumnInfos.Count];
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();

            foreach (OracleDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {
                    if (wheres.Length != 0)
                    {
                        wheres.Append(" and ");
                    }
                    wheres.Append(dbColumn.ColumnName).Append(" = ").Append(":").Append(dbColumn.ColumnName).Append(" ");

                    if (dbColumn.Property.GetValue(obj, null) == null)
                    {
                        throw new Exception("更新主键不能为空！");
                    }
                }
                else
                {
                    sets.Append(dbColumn.ColumnName).Append(" = ").Append(":").Append(dbColumn.ColumnName).Append(",");
                }
                parms[i] = new OracleParameter();
                parms[i].OracleType = GetOracleType(dbColumn.Type);
                parms[i].ParameterName = ":" + dbColumn.ColumnName;
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

            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlUpdate.Append(" set ").Append(sets).Append(" where ").Append(wheres);
            rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, CommandType.Text, sqlUpdate.ToString(), parms);

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
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlUpdate.Append(" set ").Append(Sets).Append(" where 1=1 ").Append(Conditions);
            rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), null);


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
            StringBuilder sqlUpdate = new StringBuilder();
            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlUpdate.Append(" set ").Append(Sets).Append(" where 1=1 ").Append(Conditions).Append(String.Format(" and rownum <= {0} ", PrecedingNumber));
            rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), null);


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
            StringBuilder sqlUpdate = new StringBuilder();
            sqlUpdate.Append("Update ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlUpdate.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlUpdate.Append(" set ").Append(Sets).Append(" where 1=1 ").Append(Conditions).Append(String.Format(" and rownum <= {0} ", PrecedingNumber));
            rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, CommandType.Text, sqlUpdate.ToString(), null);

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
            List<OracleParameter> parms = new List<OracleParameter>();
            OracleParameter parm;
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
                    parm = new OracleParameter();
                    parm.OracleType = GetOracleType(column.DataType);
                    parm.ParameterName = "@" + column.ColumnName;
                    parm.Value = GetValue(column.DataType, dicts[column.ColumnName.ToLower()].ToString());
                    parms.Add(parm);
                }
            }
            sets.Remove(sets.Length - 1, 1);

            sqlUpdate.Append("Update ").Append(tableName).Append(" set ").Append(sets).Append(" where 1=1 ");
            sqlUpdate.Append(wheres);

            rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlUpdate.ToString(), parms.ToArray());

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
            return this.Delete(obj, new StringBuilder());
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
        /// <param name="trans">The trans.</param>
        /// <param name="Conditions">The conditions.</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, string Conditions)
        {
            return Delete(trans, Conditions, new StringBuilder());
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
        /// <param name="trans">The trans.</param>
        /// <param name="Field">The field.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public override int Delete(DbTransaction trans, string Field, string Value)
        {
            return Delete(trans, Field, Value, new StringBuilder());
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
            OracleParameter[] parms = new OracleParameter[_primaryColumnCount];
            int i = 0;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlDelete = new StringBuilder();

            foreach (OracleDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {
                    if (wheres.Length != 0)
                    {
                        wheres.Append(" and ");
                    }
                    wheres.Append(dbColumn.ColumnName).Append(" = ").Append(":").Append(dbColumn.ColumnName).Append(" ");
                    parms[i] = new OracleParameter();
                    parms[i].OracleType = GetOracleType(dbColumn.Type);
                    parms[i].ParameterName = ":" + dbColumn.ColumnName;
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

            sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlDelete.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlDelete.Append(" where ").Append(wheres);
            rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlDelete.ToString(), parms);

            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            return rel;
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
            OracleParameter[] parms = new OracleParameter[_primaryColumnCount];
            int i = 0;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlDelete = new StringBuilder();

            foreach (OracleDbColumnInfo dbColumn in _dbColumnInfos)
            {
                if (dbColumn.PrimaryKey)
                {
                    if (wheres.Length != 0)
                    {
                        wheres.Append(" and ");
                    }
                    wheres.Append(dbColumn.ColumnName).Append(" = ").Append(":").Append(dbColumn.ColumnName).Append(" ");
                    parms[i] = new OracleParameter();
                    parms[i].OracleType = GetOracleType(dbColumn.Type);
                    parms[i].ParameterName = ":" + dbColumn.ColumnName;
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

            sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlDelete.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlDelete.Append(" where ").Append(wheres);
            rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, CommandType.Text, sqlDelete.ToString(), parms);

            base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            return rel;
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
                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo));
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" partition(").Append(partitionName).Append(") ");
                }
                sqlDelete.Append(" where 1=1 ").Append(Conditions);
                rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlDelete.ToString(), null);
                base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            }
            return rel;
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
                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo));
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" partition(").Append(partitionName).Append(") ");
                }
                sqlDelete.Append(" where 1=1 ").Append(Conditions);
                rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, CommandType.Text, sqlDelete.ToString(), null);
                base.WriteLog(sqlDelete.ToString(), dtBegin, DateTime.Now);
            }
            return rel;
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

                if (wheres.Length != 0)
                {
                    wheres.Append(" and ");
                }
                wheres.Append(Field.Trim()).Append(" = ").Append("'").Append(Value.Trim()).Append("' ");

                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo));
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" partition(").Append(partitionName).Append(") ");
                }
                sqlDelete.Append(" where ").Append(wheres);
                rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, sqlDelete.ToString(), null);
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

                if (wheres.Length != 0)
                {
                    wheres.Append(" and ");
                }
                wheres.Append(Field.Trim()).Append(" = ").Append("'").Append(Value.Trim()).Append("' ");

                sqlDelete.Append("Delete from ").Append(GetTableName(_dbTableInfo));
                if (!IsNullOrEmpty(partitionName))
                {
                    sqlDelete.Append(" partition(").Append(partitionName).Append(") ");
                }
                sqlDelete.Append(" where ").Append(wheres);
                rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, CommandType.Text, sqlDelete.ToString(), null);
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

            OracleParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();
            #region MyRegion
            string[] keys = new string[dict.Count];
            dict.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                if (wheres.Length != 0)
                {
                    wheres.Append(" and ");
                }
                wheres.Append(keys[i]).Append(" = ").Append(" '").Append(dict[keys[i]].ToString()).Append("' ");
            }

            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where ").Append(wheres.ToString());
            using (OracleDataReader dr = _OracleHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms))
            {
                if (_dbTableInfo.TableOrView == TableType.Table)
                {
                    _obj = PopulateFromDrFirstOrDefault(dr);
                    //_obj = _entityConverter.FirstOrDefault(dr);
                }
                else
                {
                    _obj = PopulateFromDrFirstOrDefault(dr);
                }
            } 
            #endregion
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

            OracleParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();
            wheres.Append(Field).Append(" = '").Append(Value).Append("' ");
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where ").Append(wheres.ToString());
            using (OracleDataReader dr = _OracleHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms))
            {
                if (_dbTableInfo.TableOrView == TableType.Table)
                {
                    _obj = PopulateFromDrFirstOrDefault(dr);
                    //_obj = _entityConverter.FirstOrDefault(dr);
                }
                else
                {
                    _obj = PopulateFromDrFirstOrDefault(dr);
                }
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
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            //SqlHelper _OracleHelper = new SqlHelper(GetConnectionString(_dbTableInfo));
            using (OracleDataReader dr = _OracleHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                if (_dbTableInfo.TableOrView == TableType.Table)
                {
                    list = PopulateFromDr(dr);
                    //list = _entityConverter.Select(dr);
                }
                else
                {
                    list = PopulateFromDr(dr);
                }
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
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));                
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where 1=1 ").Append(Conditions);
            using (OracleDataReader dr = _OracleHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                if (_dbTableInfo.TableOrView == TableType.Table)
                {
                    list = PopulateFromDr(dr);
                    //list = _entityConverter.Select(dr);
                }
                else
                {
                    list = PopulateFromDr(dr);
                }
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
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));                
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where 1=1 ").Append(Conditions).Append(" order by ").Append(SortOrder);
            using (OracleDataReader dr = _OracleHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                if (_dbTableInfo.TableOrView == TableType.Table)
                {
                    list = PopulateFromDr(dr);
                    //list = _entityConverter.Select(dr);
                }
                else
                {
                    list = PopulateFromDr(dr);
                }
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
            sqlSelect.Append("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT * FROM ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where (1=1) ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" ) A WHERE ROWNUM <= ").Append((StartRecordIndex + PageSize)).Append(" ) WHERE RN > ").Append(StartRecordIndex);
            //sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ").Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            using (OracleDataReader dr = _OracleHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                if (_dbTableInfo.TableOrView == TableType.Table)
                {
                    list = PopulateFromDr(dr);
                    //list = _entityConverter.Select(dr);
                }
                else
                {
                    list = PopulateFromDr(dr);
                }
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
            sqlSelect.Append("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT * FROM ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where (1=1) ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" ) A WHERE ROWNUM <= ").Append((StartRecordIndex + PageSize)).Append(" ) WHERE RN > ").Append(StartRecordIndex);
            //sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ").Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            using (OracleDataReader dr = _OracleHelper.ExecuteReader(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null))
            {
                if (_dbTableInfo.TableOrView == TableType.Table)
                {
                    list = PopulateFromDr(dr);
                    //list = _entityConverter.Select(dr);
                }
                else
                {
                    list = PopulateFromDr(dr);
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
            return GetDataTable(Conditions,StartRecordIndex, PageSize, new StringBuilder());
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
            return GetDataTable(Conditions,StartRecordIndex, PageSize, Sort, new StringBuilder());
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
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
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
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where 1=1 ").Append(Conditions);
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
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
            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where 1=1 ").Append(Conditions).Append(" order by ").Append(SortOrder);
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
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
            sqlSelect.Append("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT * FROM ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where (1=1) ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" ) A WHERE ROWNUM <= ").Append((StartRecordIndex + PageSize)).Append(" ) WHERE RN > ").Append(StartRecordIndex);
            //sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ").Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
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
            sqlSelect.Append("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT * FROM ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where (1=1) ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" ) A WHERE ROWNUM <= ").Append((StartRecordIndex + PageSize)).Append(" ) WHERE RN > ").Append(StartRecordIndex);
            //sqlSelect.Append("select top ").Append(PageSize).Append(" * from ").Append(GetTableName(_dbTableInfo)).Append(" where ").Append(GetPageKey()).Append(" NOT IN ((Select Top ").Append(StartRecordIndex).Append(" ").Append(GetPageKey()).Append(" from ").Append(GetTableName(_dbTableInfo)).Append(" where 1=1 ").Append(Conditions).Append(" Order by ").Append(SortOrder).Append(" )) ").Append(Conditions.ToString()).Append(" Order by ").Append(SortOrder);
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
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
            sqlSelect.Append("Select count(*) from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            rel = Convert.ToInt64(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null));
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
            sqlSelect.Append("Select count(*) from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where 1=1 ").Append(Conditions);
            rel = Convert.ToInt64(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null)); 
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
            OracleParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();

            string[] keys = new string[dict.Count];
            dict.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                if (wheres.Length != 0)
                {
                    wheres.Append(" and ");
                }
                wheres.Append(keys[i]).Append(" = ").Append(" '").Append(dict[keys[i]].ToString()).Append("' ");
            }

            sqlSelect.Append("select * from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where ").Append(wheres.ToString());
            int count = Convert.ToInt32(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms));
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
            OracleParameter[] parms = null;
            StringBuilder wheres = new StringBuilder();
            StringBuilder sqlSelect = new StringBuilder();
            wheres.Append(Field).Append(" = '").Append(Value).Append("' ");
            sqlSelect.Append("select count(*) from ").Append(GetTableName(_dbTableInfo));
            if (!IsNullOrEmpty(partitionName))
            {
                sqlSelect.Append(" partition(").Append(partitionName).Append(") ");
            }
            sqlSelect.Append(" where ").Append(wheres.ToString());
            int count = Convert.ToInt32(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), parms));
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
            //string sqlStr = "select max(" + Field + ") from " + GetTableName(_dbTableInfo) + " where length(" + Field + ") = (select max(length(" + Field + ")) from  " + GetTableName(_dbTableInfo) + ") ";
            //return _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
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
            //string sqlStr = "select max(" + Field + ") from " + GetTableName(_dbTableInfo) + " where length(" + Field + ") = (select max(length(" + Field + ")) from  " + GetTableName(_dbTableInfo) + " where 1=1 " + Conditions.ToString() + ") " + Conditions.ToString();
            //return _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
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
            string rel = string.Empty;
            string sqlStr = "select max(" + Field + ") from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " where length(" + Field + ") = (select max(length(" + Field + ")) from  " + GetTableName(_dbTableInfo);

            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += ") ";
            rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return rel;
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
            string rel = string.Empty;
            string sqlStr = "select max(" + Field + ") from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " where length(" + Field + ") = (select max(length(" + Field + ")) from  " + GetTableName(_dbTableInfo);

            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " where 1=1 " + Conditions.ToString() + ") " + Conditions.ToString();
            rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return rel;
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
            //string sqlStr = "select min(" + Field + ") from " + GetTableName(_dbTableInfo) + " where length(" + Field + ") = (select min(length(" + Field + ")) from  " + GetTableName(_dbTableInfo) + " where " + Field + " is not null )";
            //return _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
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
            //string sqlStr = "select min(" + Field + ") from " + GetTableName(_dbTableInfo) + " where length(" + Field + ") = (select min(length(" + Field + ")) from  " + GetTableName(_dbTableInfo) + " where " + Field + " is not null  " + Conditions.ToString() + ") " + Conditions.ToString();
            //return _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
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
            string rel = string.Empty;
            string sqlStr = "select min(" + Field + ") from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " where length(" + Field + ") = (select min(length(" + Field + ")) from  " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " where " + Field + " is not null )";
            rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return rel;
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
            string rel = string.Empty;
            string sqlStr = "select min(" + Field + ") from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " where length(" + Field + ") = (select min(length(" + Field + ")) from  " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " where " + Field + " is not null  " + Conditions.ToString() + ") " + Conditions.ToString();
            rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).ToString();
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return rel;
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
            //string sqlStr = "select DISTINCT " + Fields + " from " + GetTableName(_dbTableInfo) + " ";
            //DataSet ds = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null);
            //return ds.Tables[0];
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public override DataTable GetDistinctTable(string Fields, string Conditions)
        {
            return GetDistinctTable(Fields, Conditions, new StringBuilder());
            //string sqlStr = "select DISTINCT " + Fields + " from " + GetTableName(_dbTableInfo) + "  where 1=1 " + Conditions.ToString();
            //DataSet ds = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null);
            //return ds.Tables[0];
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
            DataTable dt;
            string sqlStr = "select DISTINCT " + Fields + " from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " ";
            DataSet ds = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null);
            dt = ds.Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
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
            DataTable dt;
            string sqlStr = "select DISTINCT " + Fields + " from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += "  where 1=1 " + Conditions.ToString();
            DataSet ds = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null);
            dt = ds.Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
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
            string sqlStr = "select count(*) from ( select DISTINCT " + Fields + " from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += " ) ";
            rel = Convert.ToInt64(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));

            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
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
            string sqlStr = "select count(*) from (select DISTINCT " + Fields + " from " + GetTableName(_dbTableInfo);
            if (!IsNullOrEmpty(partitionName))
            {
                sqlStr += " partition(" + partitionName.ToString() + ") ";
            }
            sqlStr += "  where 1=1 " + Conditions.ToString() + " ) ";
            rel = Convert.ToInt64(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));

            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
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
            string sqlStr = "select count(*) from user_tables where upper(table_name) = upper('" + TableName + "') ";
            int rel = Convert.ToInt32(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
            string strTableName = TableName.ToUpper();
            string sqlStr = "select * from user_tables where upper(table_name) like '%" + strTableName + "%' ";
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
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
            string sqlStr = "select count(*) from user_views where upper(table_name) = upper('" + ViewName + "') ";
            int rel = Convert.ToInt32(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
        /// <param name="ViewName">视图名</param>
        /// <returns></returns>
        public override DataTable GetViewList(string ViewName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt;
            string strViewName = ViewName.ToUpper();
            string sqlStr = "select * from user_views where upper(table_name) like '%" + strViewName + "%' ";
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
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
            string sqlStr = "select count(*) from user_col_comments where upper(column_name) = upper('" + FieldName + "') and upper(table_name) = upper('" + GetTableName(_dbTableInfo) + "')  ";
            int rel = Convert.ToInt32(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
        /// 是/否
        /// </returns>
        public override bool IsExistField(string FieldName, string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            bool bRel = false;
            string sqlStr = "select count(*) from user_col_comments where upper(column_name) = upper('" + FieldName + "') and upper(table_name) = upper('" + TableName + "')  ";
            int rel = Convert.ToInt32(_OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null));
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
            string strFieldName = FieldName.ToUpper();
            string sqlStr = "select * from user_col_comments where upper(column_name) like '%" + strFieldName + "%' and upper(table_name) = upper('" + GetTableName(_dbTableInfo) + "')  ";
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
            base.WriteLog(sqlStr.ToString(), dtBegin, DateTime.Now);
            return dt;
        }


        /// <summary>
        /// 根据字段名 查询符合条件的 字段信息 （模糊查询）
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public override DataTable GetFieldList(string FieldName, string TableName)
        {
            DateTime dtBegin = DateTime.Now;
            DataTable dt;
            string strFieldName = FieldName.ToUpper();
            string sqlStr = "select * from user_col_comments where upper(column_name) like '%" + strFieldName + "%' and upper(table_name) = upper('" + TableName + "')  ";
            dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlStr, null).Tables[0];
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
            int rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), CommandType.Text, SQLString, null);
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
            int rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), commandType, SQLString, null);
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
            int rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
        public int ExecuteSQL(string SQLString, CommandType commandType, params OracleParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = _OracleHelper.ExecuteNonQuery(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
            int rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, CommandType.Text, SQLString, null);
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
            int rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, commandType, SQLString, null);
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
            int rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, commandType, SQLString, commandParameters);
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
        public int ExecuteSQL(DbTransaction trans, string SQLString, CommandType commandType, params OracleParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            int rel = _OracleHelper.ExecuteNonQuery((OracleTransaction)trans, commandType, SQLString, commandParameters);
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
            object rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), CommandType.Text, SQLString, null);
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
            object rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), commandType, SQLString, null);
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
            object rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
        public object ExecuteScalarSQL(string SQLString, CommandType commandType, params OracleParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _OracleHelper.ExecuteScalar(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters);
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
            object rel = _OracleHelper.ExecuteScalar((OracleTransaction)trans, CommandType.Text, SQLString, null);
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
            object rel = _OracleHelper.ExecuteScalar((OracleTransaction)trans, commandType, SQLString, null);
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
            object rel = _OracleHelper.ExecuteScalar((OracleTransaction)trans, commandType, SQLString, commandParameters);
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
        public object ExecuteScalarSQL(DbTransaction trans, string SQLString, CommandType commandType, params OracleParameter[] commandParameters)
        {
            DateTime dtBegin = DateTime.Now;
            object rel = _OracleHelper.ExecuteScalar((OracleTransaction)trans, commandType, SQLString, commandParameters);
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
            DataTable rel = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, SQLString, null).Tables[0];
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
            DataTable rel = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), commandType, SQLString, null).Tables[0];
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
            DataTable rel = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters).Tables[0];
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
        public DataTable ExecuteDataAdapterSQL(string SQLString, CommandType commandType, params OracleParameter[] commandParameters)
        {

            DateTime dtBegin = DateTime.Now;
            DataTable rel = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), commandType, SQLString, commandParameters).Tables[0];
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
            return (DbTransaction)_OracleHelper.CreateTransaction(GetConnectionString(_dbTableInfo));
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        public override void TransactionCommit(DbTransaction trans)
        {
            _OracleHelper.TransactionCommit((OracleTransaction)trans);
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="trans">事务名称</param>
        public override void TransactionRollBack(DbTransaction trans)
        {
            _OracleHelper.TransactionRollBack((OracleTransaction)trans);
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
            rel = " (" + fieldName + " >= to_date('" + beginDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-MM-dd hh24:mi.ss')) ";
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
            rel = " (" + fieldName + " <= to_date('" + endDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-MM-dd hh24:mi.ss')) ";
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
            rel = " (" + fieldName + operators + " to_date('" + date.AddHours(0 - offset).ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-MM-dd hh24:mi.ss')) ";
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
            string format = string.Empty;
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
        private OracleType GetOracleType(Type type)
        {
            OracleType OracleType = OracleType.VarChar;
            switch (type.Name)
            {
                case "String":
                    {
                        OracleType = OracleType.VarChar;
                    }
                    break;
                case "Boolean":
                    {
                        OracleType = OracleType.Number;
                    }
                    break;
                case "Decimal":
                    {
                        OracleType = OracleType.Number;
                    }
                    break;
                case "Single":
                    {
                        OracleType = OracleType.Float;
                    }
                    break;
                case "Double":
                    {
                        OracleType = OracleType.Number;
                    }
                    break;
                case "Int16":
                    {
                        OracleType = OracleType.Number;
                    }
                    break;
                case "Int32":
                    {
                        OracleType = OracleType.Number;
                    }
                    break;
                case "Int64":
                    {
                        OracleType = OracleType.Number;
                    }
                    break;
                case "DateTime":
                    {
                        OracleType = OracleType.DateTime;
                    }
                    break;
                case "Nullable`1":
                    {
                        NullableConverter nullableConverter = new NullableConverter(type);
                        switch (nullableConverter.UnderlyingType.Name)
                        {
                            case "String":
                                {
                                    OracleType = OracleType.VarChar;
                                }
                                break;
                            case "Boolean":
                                {
                                    OracleType = OracleType.Number;
                                }
                                break;
                            case "Decimal":
                                {
                                    OracleType = OracleType.Number;
                                }
                                break;
                            case "Single":
                                {
                                    OracleType = OracleType.Float;
                                }
                                break;
                            case "Double":
                                {
                                    OracleType = OracleType.Number;
                                }
                                break;
                            case "Int16":
                                {
                                    OracleType = OracleType.Number;
                                }
                                break;
                            case "Int32":
                                {
                                    OracleType = OracleType.Number;
                                }
                                break;
                            case "Int64":
                                {
                                    OracleType = OracleType.Number;
                                }
                                break;
                            case "DateTime":
                                {
                                    OracleType = OracleType.DateTime;
                                }
                                break;
                        }
                    }
                    break;
            }
            return OracleType;
        }

        /// <summary>
        /// Gets the db column infos.
        /// </summary>
        /// <param name="OracleDbColumnInfos">The oracle db column infos.</param>
        /// <returns></returns>
        private List<DbColumnInfo> GetDbColumnInfos(List<OracleDbColumnInfo> OracleDbColumnInfos)
        {
            List<DbColumnInfo> list = new List<DbColumnInfo>();
            foreach (OracleDbColumnInfo OracleDbColumnInfo in OracleDbColumnInfos)
            {
                list.Add(OracleDbColumnInfo as DbColumnInfo);
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
                if (_dbPrimaryKeyColumns.Count > 0)
                {
                    SortOrder += " , " + _dbPrimaryKeyColumns[0].ColumnName + " asc";
                }
                else
                {
                    SortOrder += " , 1 asc ";
                }
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
                foreach (OracleDbColumnInfo dbColumn in _dbColumnInfos)
                {
                    try
                    {
                        if(!IsNullOrDBNull(dr[dbColumn.ColumnName]))
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
                dt = _OracleHelper.ExecuteDataAdapter(GetConnectionString(_dbTableInfo), CommandType.Text, sqlSelect.ToString(), null).Tables[0];
                CacheHelper.SetCache(tableName + "_EmptyTable", dt);
            }
            return dt;
        }
        #endregion

    }
}
