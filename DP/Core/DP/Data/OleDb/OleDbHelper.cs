using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Collections;
using System.Data;
using DP.Common;

namespace DP.Data.OleDb
{
    /// <summary>
    /// 数据访问基础类(基于OleDb)
    /// The SqlHelper class is intended to encapsulate high performance, 
    /// </summary>
    public class OleDbHelper
    {
        #region 变量
        /// <summary>
        /// 数据连接字符串
        /// Database connection strings
        /// </summary>
        private string _ConnectionString = String.Empty;             //ConfigurationManager.ConnectionStrings["SQLConnString"]._ConnectionString;
        /// <summary>
        /// 数据连接字符串
        /// Database connection strings
        /// </summary>
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 
        /// </summary>
        public OleDbHelper()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public OleDbHelper(string connectionString)
        {
            _ConnectionString = connectionString;
        } 
        #endregion

        #region 方法
        /// <summary>
        /// HashTable 存储Cache参数
        /// Hashtable to store cached parameters
        /// </summary>
        private Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        #region Connection
        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns>返回一个数据库连接</returns>
        public OleDbConnection OpenConnection()
        {
            OleDbConnection conn = new OleDbConnection(_ConnectionString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            return conn;
        }

        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>返回一个数据库连接</returns>
        public OleDbConnection OpenConnection(string connectionString)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            return conn;
        }

        /// <summary>
        /// 关闭指定数据库连接
        /// </summary>
        /// <param name="conn">数据库连接</param>
        public void CloseConnection(OleDbConnection conn)
        {
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            
        } 
        #endregion

        #region Transaction
        /// <summary>
        /// 开起一个事务
        /// </summary>
        /// <returns>事务</returns>
        public OleDbTransaction CreateTransaction()
        {
            OleDbTransaction tran = null;
            OleDbConnection conn = new OleDbConnection(_ConnectionString);
            try
            {
                tran = conn.BeginTransaction();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            return tran;
        }

        /// <summary>
        /// 开起一个事务
        /// </summary>
        /// <param name="conn">打开的数据库连接</param>
        /// <returns>事务</returns>
        public OleDbTransaction CreateTransaction(OleDbConnection conn)
        {
            OleDbTransaction tran = null;
            try
            {
                tran = conn.BeginTransaction();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            return tran;
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="Trans">事务</param>
        public void TransactionCommit(OleDbTransaction Trans)
        {
            try
            {
                Trans.Commit();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="Trans">事务</param>
        public void TransactionRollBack(OleDbTransaction Trans)
        {
            try
            {
                Trans.Rollback();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
        } 
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行的OleDbCommand （即回报没有结果）对数据库
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            int val = 0;
            OleDbCommand cmd = new OleDbCommand();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(_ConnectionString))
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
            return val;
        }

        /// <summary>
        /// 执行的OleDbCommand （即回报没有结果）对数据库中指定的连接字符串
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为OleDbConnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            int val = 0;
            OleDbCommand cmd = new OleDbCommand();
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
            return val;
        }

        /// <summary>
        /// 执行的OleDbCommand （即回报没有结果）对数据库中指定的连接
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">现有的数据库连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(OleDbConnection connection, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            int val = 0;
            OleDbCommand cmd = new OleDbCommand();

            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));

                throw;
            }
            return val;
        }

        /// <summary>
        /// 执行的OleDbCommand （即回报没有结果）使用带事务的SQL 
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            int val = 0;
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            try
            {
                val = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
                val = -1;
            }
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OleDbParameter[]）</param>
        /// <returns>返回事务成功于否， 成功 “0” 失败 “-1”</returns>
        public int ExecuteNonQuery(CommandType cmdType, Hashtable SQLStringList)
        {
            int val = 0;
            using (OleDbConnection conn = new OleDbConnection(_ConnectionString))
            {
                conn.Open();
                using (OleDbTransaction trans = conn.BeginTransaction())
                {
                    OleDbCommand cmd = new OleDbCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OleDbParameter[] commandParameters = (OleDbParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdType, cmdText, commandParameters);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                        val = -1;
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return val;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="connectionString">一个有效的连接字符串为OleDbConnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OleDbParameter[]）</param>
        /// <returns>返回事务成功于否， 成功 “0” 失败 “-1”</returns>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, Hashtable SQLStringList)
        {
            int val = 0;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                using (OleDbTransaction trans = conn.BeginTransaction())
                {
                    OleDbCommand cmd = new OleDbCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OleDbParameter[] commandParameters = (OleDbParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdType, cmdText, commandParameters);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                        val = -1;
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
                return val;
            }
        }
        #endregion

        #region ExecuteReader
        /// <summary>
        /// 将 System.Data.SqlClient.OleDbCommand.CommandText 发送到 System.Data.SqlClient.OleDbCommand.Connection
        /// 并生成一个 System.Data.SqlClient.OleDbDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  OleDbDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回OleDbDataReader 对象。</returns>
        public OleDbDataReader ExecuteReader(CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(_ConnectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception ex)
            {
                conn.Close();
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
        }

        /// <summary>
        /// 将 System.Data.SqlClient.OleDbCommand.CommandText 发送到 System.Data.SqlClient.OleDbCommand.Connection
        /// 并生成一个 System.Data.SqlClient.OleDbDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  OleDbDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为OleDbConnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回OleDbDataReader 对象。</returns>
        public OleDbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception ex)
            {
                conn.Close();
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
        }

        /// <summary>
        /// 将 System.Data.SqlClient.OleDbCommand.CommandText 发送到 System.Data.SqlClient.OleDbCommand.Connection
        /// 并生成一个 System.Data.SqlClient.OleDbDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  OleDbDataReader r = ExecuteReader(connection, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的连接 OleDbConnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回OleDbDataReader 对象。</returns>
        public OleDbDataReader ExecuteReader(OleDbConnection connection, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
        } 
        #endregion

        #region ExecuteDataAdapter
        /// <summary>
        /// 执行 OleDbDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(_ConnectionString);
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// 执行 OleDbDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为OleDbConnection</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connectionString);
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// 执行 OleDbDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="connection">一个有效的连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(OleDbConnection connection, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
            finally
            {
                connection.Close();
            }

        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// 使用提供的参数。
        /// 开起一个事务，SQL语句在事务中执行。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            object val;
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection connection = new OleDbConnection(_ConnectionString))
            {
                connection.Open();
                using (OleDbTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, trans, cmdType, cmdText, commandParameters);
                        val = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        trans.Commit();

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                        val = null;
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return val;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// 使用提供的参数。
        /// 开起一个事务，SQL语句在事务中执行。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">现有的数据库连接字符串</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            object val;
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                using (OleDbTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, trans, cmdType, cmdText, commandParameters);
                        val = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        trans.Commit();

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                        val = null;
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return val;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// 使用提供的参数。  
        /// 不开始事务的。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">现有的数据库连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(OleDbConnection connection, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();

            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// 使用提供的参数。  
        /// 开始事务的。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            object val = 0;
            OleDbCommand cmd = new OleDbCommand();

            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            try
            {
                val = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
            }
            cmd.Parameters.Clear();
            return val;
        }
        #endregion

        public DataTable GetSchemaTable()
        {
            DataTable dt;
            try
            {
                using (OleDbConnection conn = new OleDbConnection(_ConnectionString))
                {
                    conn.Open();
                    dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            return dt;
        }

        public DataTable GetSchemaTable(string connectionString)
        {
            DataTable dt;
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            return dt;
        }

        /// <summary>
        /// 添加参数数组缓存
        /// </summary>
        /// <param name="cacheKey">关键参数缓存</param>
        /// <param name="commandParameters">SqlParamters	数组 </param>
        public void CacheParameters(string cacheKey, params OleDbParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 检索缓存的参数
        /// </summary>
        /// <param name="cacheKey">关键参数缓存</param>
        /// <returns>缓存sqlparamters阵列</returns>
        public OleDbParameter[] GetCachedParameters(string cacheKey)
        {
            OleDbParameter[] cachedParms = (OleDbParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            OleDbParameter[] clonedParms = new OleDbParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OleDbParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 编写一份命令执行
        /// </summary>
        /// <param name="cmd">OleDbCommand对象</param>
        /// <param name="conn">OleDbConnection 对象</param>
        /// <param name="trans">OleDbTransaction 对象</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="cmdParms">数组sqlparamters用来执行命令</param>
        private void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        
        /// <summary>
        /// 获取参数列表字符串
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GetParametersString(OleDbParameter[] parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }
            StringBuilder sbParameters = new StringBuilder();
            foreach (OleDbParameter param in parameters)
            {
                if (sbParameters.Length != 0)
                {
                    sbParameters.Append("\n");
                }
                sbParameters.Append(param.ParameterName + "=" + param.Value.ToString() + ",Size=" + StringHelper.GetbyteLen(param.Value.ToString(), Encoding.Default).ToString() + ",Type=" + param.OleDbType.ToString());
            }
            return sbParameters.ToString();
        }
        #endregion

    }
}
