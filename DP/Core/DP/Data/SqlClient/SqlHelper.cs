// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： SqlHelper.cs
// 项目名称：DP 
// 创建时间：2008-8-20
// 负责人：DP
// ===================================================================
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using DP.Common;
using System.Text;
using System.Data.Common;

namespace DP.Data.SqlClient
{
    /// <summary>
    /// 数据访问基础类(基于SQLServer)
    /// The SqlHelper class is intended to encapsulate high performance, 
    /// </summary>
    public class SqlHelper
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
        public SqlHelper()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlHelper(string connectionString)
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

        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns>返回一个数据库连接</returns>
        public SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(_ConnectionString);
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
        public SqlConnection OpenConnection(string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
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
        public void CloseConnection(SqlConnection conn)
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
        public SqlTransaction CreateTransaction()
        {
            SqlTransaction tran = null;
            SqlConnection conn = new SqlConnection(_ConnectionString);
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
        public SqlTransaction CreateTransaction(string connectionString)
        {
            SqlTransaction tran = null;
            SqlConnection conn = new SqlConnection(connectionString);
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
        public SqlTransaction CreateTransaction(SqlConnection conn)
        {
            SqlTransaction tran = null;
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
        public void TransactionCommit(SqlTransaction Trans)
        {
            SqlConnection conn = null;
            try
            {
                conn = Trans.Connection;
                Trans.Commit();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="Trans">事务</param>
        public void TransactionRollBack(SqlTransaction Trans)
        {
            SqlConnection conn = null;
            try
            {
                conn = Trans.Connection;
                Trans.Rollback();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(String.Format("{0}\n{1}", ex.Message.ToString(), _ConnectionString));
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
        }

        #region ExecuteNonQuery
        /// <summary>
        /// 执行的SqlCommand （即回报没有结果）对数据库
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            int val;
            SqlCommand cmd = new SqlCommand();
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
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
        /// 执行的SqlCommand （即回报没有结果）对数据库中指定的连接字符串
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为sqlconnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            int val;
            SqlCommand cmd = new SqlCommand();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
        /// 执行的SqlCommand （即回报没有结果）对数据库中指定的连接
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">现有的数据库连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            int val;
            SqlCommand cmd = new SqlCommand();

            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                val = cmd.ExecuteNonQuery();
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
        /// 执行的SqlCommand （即回报没有结果）使用带事务的SQL 
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            int val = 0;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            try
            {
                val = cmd.ExecuteNonQuery();
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
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的DbParameter[]）</param>
        /// <returns>返回事务成功于否， 成功 “0” 失败 “-1”</returns>
        public int ExecuteNonQuery(CommandType cmdType, Hashtable SQLStringList)
        {
            int val = 0;
            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] commandParameters = (SqlParameter[])myDE.Value;
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
        /// <param name="connectionString">一个有效的连接字符串为sqlconnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        /// <returns>返回事务成功于否， 成功 “0” 失败 “-1”</returns>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, Hashtable SQLStringList)
        {
            int val = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] commandParameters = (SqlParameter[])myDE.Value;
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
        /// 将 System.Data.SqlClient.SqlCommand.CommandText 发送到 System.Data.SqlClient.SqlCommand.Connection
        /// 并生成一个 System.Data.SqlClient.SqlDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回SqlDataReader 对象。</returns>
        public SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_ConnectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
        /// 将 System.Data.SqlClient.SqlCommand.CommandText 发送到 System.Data.SqlClient.SqlCommand.Connection
        /// 并生成一个 System.Data.SqlClient.SqlDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为sqlconnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回SqlDataReader 对象。</returns>
        public SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
        /// 将 System.Data.SqlClient.SqlCommand.CommandText 发送到 System.Data.SqlClient.SqlCommand.Connection
        /// 并生成一个 System.Data.SqlClient.SqlDataReader。
        /// 使用提供的参数。
        /// </summary>
        /// <remarks>
        /// 例如： 
        ///  SqlDataReader r = ExecuteReader(connection, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的连接 sqlconnection </param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回SqlDataReader 对象。</returns>
        public SqlDataReader ExecuteReader(SqlConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
        /// 执行 SqlDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_ConnectionString);
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
        /// 执行 SqlDataAdapter  返回一个DataSet 对象。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  DataSet ds = ExecuteDataAdapter(CommandType.Text, "select * from order", null);
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串为sqlconnection</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>返回 DataSet 对象</returns>
        public DataSet ExecuteDataAdapter(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
        /// 执行 SqlDataAdapter  返回一个DataSet 对象。
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
        public DataSet ExecuteDataAdapter(SqlConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            object val = 0 ;
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
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
                    }
                }
                connection.Close();
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
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">现有的数据库连接字符串</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            object val;
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
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
                }
                connection.Close();
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
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">现有的数据库连接</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            object val;
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                connection.Close();
                LogHelper.WriteLog(String.Format("{0}\n{1}\n{2}\n{3}", ex.Message.ToString(), _ConnectionString, cmdText, GetParametersString(commandParameters)));
                throw;
                val = null;
            }
            return val;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// 使用提供的参数。  
        /// 开始事务的。
        /// </summary>
        /// <remarks>
        /// 例如：
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="commandParameters">数组sqlparamters用来执行命令</param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用</returns>
        public object ExecuteScalar(SqlTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            object val = null;
            SqlCommand cmd = new SqlCommand();
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

        /// <summary>
        /// 添加参数数组缓存
        /// </summary>
        /// <param name="cacheKey">关键参数缓存</param>
        /// <param name="commandParameters">SqlParamters	数组 </param>
        public void CacheParameters(string cacheKey, params DbParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 检索缓存的参数
        /// </summary>
        /// <param name="cacheKey">关键参数缓存</param>
        /// <returns>缓存sqlparamters阵列</returns>
        public SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 编写一份命令执行
        /// </summary>
        /// <param name="cmd">SqlCommand对象</param>
        /// <param name="conn">SqlConnection 对象</param>
        /// <param name="trans">SqlTransaction 对象</param>
        /// <param name="cmdType">命令类型 (存储过程, T - SQL命令, etc.)</param>
        /// <param name="cmdText">存储过程名称或T - SQL命令</param>
        /// <param name="cmdParms">数组sqlparamters用来执行命令</param>
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, DbParameter[] cmdParms)
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
                    cmd.Parameters.Add((SqlParameter)parm);
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
        #endregion
    }
}
