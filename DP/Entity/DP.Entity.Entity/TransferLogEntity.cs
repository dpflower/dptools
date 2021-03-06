//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由T4模板自动生成
//	   生成时间 2015-02-09 23:15:26 
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using DP.Data.OracleClient.Mapping;
using DP.Data.SqlClient.Mapping;
using DP.Data.Common;
using System.Configuration;
namespace VoiceMail.Entity
{
	
	/// <summary>
	/// 转发日志
	/// </summary>	
	[Serializable]
	[OracleTable(TableName = "TransferLog" , TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString_VoiceMail")]
    [SqlTable(TableName = "TransferLog" , TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString_VoiceMail")]
	public class TransferLogEntity 
	{
				/// <summary>
		/// 日志编号
		/// </summary>		
				[OracleColumn(IsDbGenerated = true , IsPrimaryKey = true, ColumnDescription = "日志编号", ColumnName = "LogID")]
        [SqlColumn(IsDbGenerated = true , IsPrimaryKey = true, ColumnDescription = "日志编号", ColumnName = "LogID")]
				public long LogID { get; set; }

				/// <summary>
		/// 转发用户编号
		/// </summary>		
				[OracleColumn(ColumnDescription = "转发用户编号", ColumnName = "FromUserId")]
        [SqlColumn(ColumnDescription = "转发用户编号", ColumnName = "FromUserId")]		 
				public long FromUserId { get; set; }

				/// <summary>
		/// 转发用户姓名
		/// </summary>		
				[OracleColumn(ColumnDescription = "转发用户姓名", ColumnName = "FromUserName")]
        [SqlColumn(ColumnDescription = "转发用户姓名", ColumnName = "FromUserName")]		 
				public string FromUserName { get; set; }

				/// <summary>
		/// 接收用户编号
		/// </summary>		
				[OracleColumn(ColumnDescription = "接收用户编号", ColumnName = "ToUserId")]
        [SqlColumn(ColumnDescription = "接收用户编号", ColumnName = "ToUserId")]		 
				public long ToUserId { get; set; }

				/// <summary>
		/// 接收用户姓名
		/// </summary>		
				[OracleColumn(ColumnDescription = "接收用户姓名", ColumnName = "ToUserName")]
        [SqlColumn(ColumnDescription = "接收用户姓名", ColumnName = "ToUserName")]		 
				public string ToUserName { get; set; }

				/// <summary>
		/// 留言编号
		/// </summary>		
				[OracleColumn(ColumnDescription = "留言编号", ColumnName = "RecId")]
        [SqlColumn(ColumnDescription = "留言编号", ColumnName = "RecId")]		 
				public long RecId { get; set; }

				/// <summary>
		/// 最后修改时间
		/// </summary>		
				[OracleColumn(ColumnDescription = "最后修改时间", ColumnName = "LastUpdateTime")]
        [SqlColumn(ColumnDescription = "最后修改时间", ColumnName = "LastUpdateTime")]		 
				public DateTime? LastUpdateTime { get; set; }

				/// <summary>
		/// 转发时间
		/// </summary>		
				[OracleColumn(ColumnDescription = "转发时间", ColumnName = "CreatedTime")]
        [SqlColumn(ColumnDescription = "转发时间", ColumnName = "CreatedTime")]		 
				public DateTime? CreatedTime { get; set; }

				/// <summary>
		/// 组编号
		/// </summary>		
				[OracleColumn(ColumnDescription = "组编号", ColumnName = "GroupId")]
        [SqlColumn(ColumnDescription = "组编号", ColumnName = "GroupId")]		 
				public long GroupId { get; set; }

				/// <summary>
		/// 组名称
		/// </summary>		
				[OracleColumn(ColumnDescription = "组名称", ColumnName = "GroupName")]
        [SqlColumn(ColumnDescription = "组名称", ColumnName = "GroupName")]		 
				public string GroupName { get; set; }

				/// <summary>
		/// 备注
		/// </summary>		
				[OracleColumn(ColumnDescription = "备注", ColumnName = "Remark")]
        [SqlColumn(ColumnDescription = "备注", ColumnName = "Remark")]		 
				public string Remark { get; set; }

		 

	}
}
