
// =================================================================== 
// 项目说明
//====================================================================
// @Copy Right 2006-2008
// 文件： T_Role_BLL.cs
// 项目名称：DP.PMS 
// 创建时间：2013/3/9
// 负责人：
// ===================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using DP.Web;
using DP.Common;
using DP.Data.Common;

using DP.PMS.Entity;

namespace DP.PMS.BLL
{
    /// <summary>
    /// 业务层实例类 dbo.T_Role.
    /// </summary>
    public partial class T_Role_BLL
    {
        #region 成员
        ///<summary>
        /// 数据访问层 接口对象
        ///</summary>
        public readonly DAL _dal = DALFactory.CreateDal();
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public T_Role_BLL()
        {
        }
        #endregion

        #region 实例化函数

        #region 添加
        /// <summary>
        /// 向数据库中插入一条新记录。
        /// </summary>
        /// <param name="_T_Role_Entity">T_Role实体</param>
        /// <returns>影响的行数</returns>
        public long Insert(T_Role_Entity entity)
        {
            CacheHelper.ClearSession("T_Role_List");
            CacheHelper.ClearCache("T_Role_List");
            return _dal.Insert(entity);

        }
        #endregion

        #region 更新

        /// <summary>
        /// 向数据表T_Role更新一条记录。
        /// </summary>
        /// <param name="_T_Role_Entity">entity</param>
        /// <returns>影响的行数</returns>
        public int Update(T_Role_Entity entity)
        {
            CacheHelper.ClearSession("T_Role_List");
            CacheHelper.ClearCache("T_Role_List");
            return _dal.Update(entity);

        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除数据表T_Role中的一条记录
        /// </summary>
        /// <param name="RoleID">RoleID</param>
        /// <returns>影响的行数</returns>
        public int Delete(T_Role_Entity obj)
        {
            CacheHelper.ClearSession("T_Role_List");
            CacheHelper.ClearCache("T_Role_List");
            return _dal.Delete(obj);
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,删除符合条件的。
        /// </summary>
        /// <param name="Conditions">Where条件数组</param>
        /// <returns>影响行数</returns>
        public int Delete(List<Condition> Conditions)
        {
            CacheHelper.ClearSession("T_Role_List");
            CacheHelper.ClearCache("T_Role_List");
            return _dal.Delete(Conditions);
        }
        /// <summary>
        /// 删除数据表的一表记录，根据 指定字段，指定值。
        /// </summary>
        /// <param name="Filed">字段</param>
        /// <param name="Value">值</param>
        /// <returns>影响行数</returns>
        public int Delete(string Field, string Value)
        {
            CacheHelper.ClearSession("T_Role_List");
            CacheHelper.ClearCache("T_Role_List");
            return _dal.Delete(Field, Value);

        }

        #endregion

        #region 根据 指定字段 指定值，返回单个实体类
        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T_Role对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>T_Role_Entity 对象</returns>
        public T_Role_Entity GetEntity(string Field, string Value)
        {
            return _dal.GetEntity<T_Role_Entity>(Field, Value);
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T_Role对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>T_Role_Entity 对象</returns>
        public T_Role_Entity GetEntity(string Field, int Value)
        {
            return _dal.GetEntity<T_Role_Entity>(Field, Value.ToString());
        }
        /// <summary>
        /// 根据 指定字段 指定值 ,返回一个T_Role对象
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Value">字段值</param>
        /// <returns>T_Role_Entity 对象</returns>
        public T_Role_Entity GetEntity(string Field, long Value)
        {
            return _dal.GetEntity<T_Role_Entity>(Field, Value.ToString());
        }
        #endregion

        #region 返回实体类集合
        /// <summary>
        /// 得到数据表T_Role所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// List＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List(new Query());
        /// </remarks>
        /// <param name="Query">查询条件对象</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(Query query)
        {
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到数据表T_Role所有记录
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// List＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List();
        /// </remarks>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList()
        {
            Query query = new Query();
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(string Conditions)
        {
            Query query = new Query();
            query.ConditionString = Conditions;
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List(new List<Condition>());
        /// </remarks>
        /// <param name="Conditions">Where条件数组</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(List<Condition> Conditions)
        {
            Query query = new Query();
            query.Conditions = Conditions;
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="Sort">排序字段</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(string Conditions, string Sort)
        {
            Query query = new Query();
            query.ConditionString = Conditions;
            string[] sorts = Sort.Split(new char[] { ',' });
            foreach (string s in sorts)
            {
                string[] ss = s.Trim().Split(new char[] { ' ' });
                if (ss.Length == 1)
                {
                    query.Orders.Add(new Data.Common.SortOrder(ss[0], OrderDirection.ASC));
                }
                else if (ss.Length > 1)
                {
                    if (ss[1].Trim().ToLower() == "desc")
                    {
                        query.Orders.Add(new Data.Common.SortOrder(ss[0], OrderDirection.DESC));
                    }
                    else
                    {
                        query.Orders.Add(new Data.Common.SortOrder(ss[0], OrderDirection.ASC));
                    }
                }

            }
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// </remarks>
        /// <param name="Conditions">Where条件数组</param>
        /// <param name="Orders">Order排序数组</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(List<Condition> Conditions, List<DP.Data.Common.SortOrder> Orders)
        {
            Query query = new Query();
            query.Conditions = Conditions;
            query.Orders = Orders;
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List(" and 1=1 ", 0, 10);
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始</param>
        /// <param name="PageSize">页大小</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(string Conditions, int StartRecordIndex, int PageSize)
        {
            Query query = new Query();
            query.ConditionString = Conditions;
            query.StartRecordIndex = StartRecordIndex;
            query.PageSize = PageSize;
            query.IsPaging = true;
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件数组</param>
        /// <param name="StartRecordIndex">记录开始</param>
        /// <param name="PageSize">页大小</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(List<Condition> Conditions, int StartRecordIndex, int PageSize)
        {
            Query query = new Query();
            query.Conditions = Conditions;
            query.StartRecordIndex = StartRecordIndex;
            query.PageSize = PageSize;
            query.IsPaging = true;
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// IList＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List(" and 1=1 ", 0, 10, "ID");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <param name="StartRecordIndex">记录开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="Sort">排序字段</param>
        /// <returns>IList＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(string Conditions, int StartRecordIndex, int PageSize, string Sort)
        {
            Query query = new Query();
            query.ConditionString = Conditions;
            query.StartRecordIndex = StartRecordIndex;
            query.PageSize = PageSize;
            query.IsPaging = true;
            string[] sorts = Sort.Split(new char[] { ',' });
            foreach (string s in sorts)
            {
                string[] ss = s.Trim().Split(new char[] { ' ' });
                if (ss.Length == 1)
                {
                    query.Orders.Add(new Data.Common.SortOrder(ss[0], OrderDirection.ASC));
                }
                else if (ss.Length > 1)
                {
                    if (ss[1].Trim().ToLower() == "desc")
                    {
                        query.Orders.Add(new Data.Common.SortOrder(ss[0], OrderDirection.DESC));
                    }
                    else
                    {
                        query.Orders.Add(new Data.Common.SortOrder(ss[0], OrderDirection.ASC));
                    }
                }

            }
            return _dal.GetList<T_Role_Entity>(query);
        }
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// List＜T_Role_Entity＞ list = T_Role_DAL.Get_T_Role_List(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件数组</param>
        /// <param name="StartRecordIndex">记录开始</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="Orders">Order排序数组</param>
        /// <returns>List＜T_Role_Entity＞对象集</returns>
        public List<T_Role_Entity> GetList(List<Condition> Conditions, int StartRecordIndex, int PageSize, List<DP.Data.Common.SortOrder> Orders)
        {
            Query query = new Query();
            query.Conditions = Conditions;
            query.StartRecordIndex = StartRecordIndex;
            query.PageSize = PageSize;
            query.IsPaging = true;
            query.Orders = Orders;
            return _dal.GetList<T_Role_Entity>(query);
        }
        #endregion

        #region 返回DataSet 对象
        /// <summary>
        /// 得到符合条件的，数据表T_Role所有记录
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// DataTable table = T_Role_DAL.GetDataTable(" and 1=1 ");
        /// </remarks>
        /// <param name="Query">查询条件对象</param>
        /// <returns>List< T_Role_Entity> 对象集</returns>
        public DataTable GetDataTable(Query query)
        {
            return _dal.GetDataTable<T_Role_Entity>(query);
        }
        #endregion

        #region 查询实体总数
        /// <summary>
        /// 查询实体数
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// int val = T_Role_DAL.GetCount();
        /// </remarks>
        /// <param name="Query">查询条件对象</param>
        /// <returns>实体的总数</returns>
        public long GetCount(Query query)
        {
            return _dal.GetCount<T_Role_Entity>(query);
        }

        /// <summary>
        /// 查询实体数
        /// </summary>	
        /// <remarks>
        /// 示例:
        /// int val = T_Role_DAL.GetCount();
        /// </remarks>
        /// <returns>实体的总数</returns>
        public long GetCount()
        {
            Query query = new Query();
            return _dal.GetCount<T_Role_Entity>(query);
        }

        /// <summary>
        /// 查询符合条件的实体总数
        /// </summary>		
        /// <remarks>
        /// 示例:
        /// int val = T_Role_DAL.GetCount(" and 1=1 ");
        /// </remarks>
        /// <param name="Conditions">Where条件，不需写“Where”</param>
        /// <returns>实体的总数</returns>
        public long GetCount(string Conditions)
        {
            Query query = new Query();
            query.ConditionString = Conditions;
            return _dal.GetCount<T_Role_Entity>(query);
        }

        #endregion

        #region 检测是否存在
        /// <summary>
        /// 检测记录是否存在
        /// </summary>
        /// <param name="Field">字段</param>
        /// <param name="Value">值</param>
        /// <returns></returns>
        public int IsExist(string Field, string Value)
        {
            return _dal.IsExist<T_Role_Entity>(Field, Value);
        }
        #endregion

        #region 获取 指定字段的 最大值
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最大值</returns>		
        public string GetMaxValue(string Field)
        {
            return _dal.GetMaxValue<T_Role_Entity>(Field);
        }
        /// <summary>
        /// 获取 指定字段的 返回 最大值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件数组</param>
        /// <returns>最大值</returns>		
        public string GetMaxValue(string Field, List<Condition> Conditions)
        {
            return _dal.GetMaxValue<T_Role_Entity>(Field, Conditions);
        }

        #endregion

        #region 获取 指定字段的 最小值
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <returns>最小值</returns>		
        public string GetMinValue(string Field)
        {
            return _dal.GetMinValue<T_Role_Entity>(Field);
        }
        /// <summary>
        /// 获取 指定字段的 返回 最小值
        /// </summary>
        /// <param name="Field">字段名</param>
        /// <param name="Conditions">Where条件数组</param>
        /// <returns>最小值</returns>		
        public string GetMinValue(string Field, List<Condition> Conditions)
        {
            return _dal.GetMinValue<T_Role_Entity>(Field, Conditions);
        }

        #endregion

        #region 获取 指定字段的  唯一行
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <returns>结果集中的唯一行</returns>
        public DataTable GetDistinctTable(string Fields)
        {
            return _dal.GetDistinctTable<T_Role_Entity>(Fields);
        }
        /// <summary>
        /// 获取 指定字段的 返回 唯一行
        /// </summary>
        /// <param name="Fields">字段名 多个字段 用"," 分隔 </param>
        /// <param name="Conditions">查询条件数组 </param>
        /// <returns>结果集中的唯一行</returns>
        public DataTable GetDistinctTable(string Fields, List<Condition> Conditions)
        {
            return _dal.GetDistinctTable<T_Role_Entity>(Fields, Conditions);
        }
        #endregion

        #region 缓存
        #region Session
        /// <summary>
        /// 得到数据表T_Role所有缓存记录
        /// 缓存 会 缓存 表的全部数据到 程序内存中。对于记录 量大的表请勿使用些方法。
        /// </summary>
        /// <param name="page">当前页面对象</param>
        ///<returns></returns>
        public List<T_Role_Entity> GetSessionList(Page page)
        {
            List<T_Role_Entity> list = CacheHelper.GetSession("T_Role_List") as List<T_Role_Entity>;
            if (list == null)
            {
                list = _dal.GetList<T_Role_Entity>(new Query()) as List<T_Role_Entity>;
                CacheHelper.SetSession("T_Role_List", list);
            }
            return list;
        }

        #endregion

        #region PageItem
        /// <summary>
        /// 得到数据表T_Role所有缓存记录
        /// 缓存 会 缓存 表的全部数据到 程序内存中。对于记录 量大的表请勿使用些方法。
        /// </summary>
        /// <param name="page">当前页面对象</param>
        ///<returns></returns>
        public List<T_Role_Entity> GetPageItemList(Page page)
        {
            List<T_Role_Entity> list = CacheHelper.GetPageItem(page, "T_Role_List") as List<T_Role_Entity>;
            if (list == null)
            {
                list = _dal.GetList<T_Role_Entity>(new Query()) as List<T_Role_Entity>;
                CacheHelper.SetPageItem(page, "T_Role_List", list);
            }
            return list;
        }

        #endregion

        #region Cache
        /// <summary>
        /// 得到数据表T_Role所有缓存记录
        /// 缓存 会 缓存 表的全部数据到 程序内存中。对于记录 量大的表请勿使用些方法。
        /// </summary>
        /// <param name="page">当前页面对象</param>
        ///<returns></returns>
        public List<T_Role_Entity> GetCacheList(Page page)
        {
            List<T_Role_Entity> list = CacheHelper.GetCache("T_Role_List") as List<T_Role_Entity>;
            if (list == null)
            {
                list = _dal.GetList<T_Role_Entity>(new Query()) as List<T_Role_Entity>;
                CacheHelper.SetCache("T_Role_List", list);
            }
            return list;
        }

        #endregion
        #endregion

        #endregion

        #region 自定义实例化函数

        #endregion
    }
}
