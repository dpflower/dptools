// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： OracleHelper.cs
// 项目名称：UltraCRM 
// 创建时间：2009-7-21
// 负责人：
// ===================================================================

using System;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Collections;
using DP.Common;
using System.Text;
using System.Data.Common;

namespace DP.Data.OracleClient
{

    /// <summary>
    /// 数据访问基础类(基于OracleServer)
    /// The OracleHelper class is intended to encapsulate high performance, 
    /// scalable best practices for common uses of OracleClient.
    /// </summary>
    public class OracleHelper
    {
        #region 变量
        /// <summary>
        /// 数据连接字符串
        /// Database connection strings
        /// </summary>
        private string _ConnectionString = String.Empty;

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
        public OracleHelper()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public OracleHelper(string connectionString)
        {
            _ConnectionString = connectionString;
        } 
        #endregion


        /// <summary>
        /// HashTable 存储Cache参数
        /// Hashtable to store cached parameters
        /// </summary>
        private  Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns>返回一个数据库连接</returns>
        public  OracleConnection OpenConnection()
        {
            OracleConnection conn = new OracleConnection(_ConnectionString);
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
        public  OracleConnection OpenConnection(string connectionString)
        {
            OracleConnection conn = new OracleConnection(connectionString);
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
        public  void CloseConnection(OracleConnection conn)
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

        /// <summary>
        /// 开起一个事务
        /// </summary>
        /// <returns>事务</returns>
        public  OracleTransaction CreateTransaction()
        {
            OracleTransaction tran = null;
            OracleConnection conn = new OracleConnection(_ConnectionString);
            try
            {
                conn.Open();
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
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// 事务
        /// </returns>
        public OracleTransaction CreateTransaction(string connectionString)
        {
            OracleTransaction tran = null;
            OracleConnection conn = new OracleConnection(connectionString);
            try
            {
                conn.Open();
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
        public  OracleTransaction CreateTransaction(OracleConnection conn)
        {
            OracleTransaction tran = null;
            try
            {
                conn.Open();
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
        public  void TransactionCommit(OracleTransaction Trans)
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
        public  void TransactionRollBack(OracleTransaction Trans)
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

        /// <summary>
        /// 执行的OracleCommand （即回报没有结果）对数据库
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            int val;
            OracleCommand cmd = new OracleCommand();
            try
            {
                using (OracleConnection conn = new OracleConnection(_ConnectionString))
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
        /// 执行的OracleCommand （即回报没有结果）对数据库中指定的连接字符串
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为Oracleconnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            int val;
            OracleCommand cmd = new OracleCommand();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
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
        /// 执行的OracleCommand （即回报没有结果）对数据库中指定的连接
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">现有的数据库连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {

            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
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
        /// 执行的OracleCommand （即回报没有结果）使用带事务的SQL 
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            int val = 0;
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            try
            {
                val = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
            }
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        /// <returns>返回事务成功于否， 成功 “0” 失败 “-1”</returns>
        public  int ExecuteNonQuery(CommandType cmdType, Hashtable SQLStringList)
        {
            int val = 0;
            using (OracleConnection conn = new OracleConnection(_ConnectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] commandParameters = (OracleParameter[])myDE.Value;
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
        /// <param name="connectionString">一个有效的连接字符串为Oracleconnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        /// <returns>返回事务成功于否， 成功 “0” 失败 “-1”</returns>
        public  int ExecuteNonQuery(string connectionString, CommandType cmdType, Hashtable SQLStringList)
        {
            int val = 0;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] commandParameters = (OracleParameter[])myDE.Value;
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

        /// <summary>
        /// 将 System.Data.OracleClient.OracleCommand.CommandText 发送到 System.Data.OracleClient.OracleCommand.Connection
        /// 并生成一个 System.Data.OracleClient.OracleDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  OracleDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>返回OracleDataReader 对象。</returns>
        public OracleDataReader ExecuteReader(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(_ConnectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
        /// 将 System.Data.OracleClient.OracleCommand.CommandText 发送到 System.Data.OracleClient.OracleCommand.Connection
        /// 并生成一个 System.Data.OracleClient.OracleDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <param name="connectionString">一个有效的连接字符串为Oracleconnection</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>返回OracleDataReader 对象。</returns>
        /// <remarks>
        /// 例如：
        /// OracleDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        public OracleDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
        /// 将 System.Data.OracleClient.OracleCommand.CommandText 发送到 System.Data.OracleClient.OracleCommand.Connection
        /// 并生成一个 System.Data.OracleClient.OracleDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  OracleDataReader r = ExecuteReader(connection, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的连接 Oracleconnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>返回OracleDataReader 对象。</returns>
        public OracleDataReader ExecuteReader(OracleConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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

        /// <summary>
        /// 执行 OracleDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(_ConnectionString);
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
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
        /// 执行 OracleDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为Oracleconnection</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connectionString);
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
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
        /// 执行 OracleDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="connection">一个有效的连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(OracleConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
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

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// 使用提供的参数。
        /// 开起一个事务，SQL语句在事务中执行。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            object val;
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection connection = new OracleConnection(_ConnectionString))
            {
                connection.Open();
                using (OracleTransaction trans = connection.BeginTransaction())
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
                        throw;
                        val = null;
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
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">现有的数据库连接字符串</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            object val;
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleTransaction trans = connection.BeginTransaction())
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
                        throw;
                        val = null;
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
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">现有的数据库连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(OracleConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();

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
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组Oracleparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(OracleTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            object val = 0;
            OracleCommand cmd = new OracleCommand();

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

        /// <summary>
        /// 添加参数数组缓存
        /// </summary>
        /// <param name="cacheKey">关键参数缓存</param>
        /// <param name="commandParameters">OracleParamters	数组</param>
        public  void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 检索缓存的参数
        /// </summary>
        /// <param name="cacheKey">关键参数缓存</param>
        /// <returns>缓存Oracleparamters阵列</returns>
        public  OracleParameter[] GetCachedParameters(string cacheKey)
        {
            OracleParameter[] cachedParms = (OracleParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            OracleParameter[] clonedParms = new OracleParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OracleParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 编写一份命令执行
        /// </summary>
        /// <param name="cmd">OracleCommand对象</param>
        /// <param name="conn">OracleConnection 对象</param>
        /// <param name="trans">OracleTransaction 对象</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="cmdParms">数组Oracleparamters用来执行命令</param>
        private  void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, DbParameter[] cmdParms)
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
                foreach (DbParameter parm in cmdParms)
                    cmd.Parameters.Add((OracleParameter)parm);
            }
        }
        
        /// <summary>
        /// 获取参数列表字符串
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GetParametersString(DbParameter[] parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }
            StringBuilder sbParameters = new StringBuilder();
            foreach (DbParameter param in parameters)
            {
                if (sbParameters.Length != 0)
                {
                    sbParameters.Append("\n");
                }
                sbParameters.Append(param.ParameterName + "=" + ((param.Value == null) ? "" : param.Value.ToString()) + ",Size=" + StringHelper.GetbyteLen((param.Value == null) ? "" : param.Value.ToString(), Encoding.Default).ToString() + ",Type=" + param.DbType.ToString());
            }
            return sbParameters.ToString();
        }
    }
}
