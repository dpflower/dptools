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
	/// 配置表
	/// </summary>	
	[Serializable]
	[OracleTable(TableName = "Config" , TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString_VoiceMail")]
    [SqlTable(TableName = "Config" , TableOrView = DP.Data.Common.TableType.Table, ConnectionStringKey = "SQLConnString_VoiceMail")]
	public class ConfigEntity 
	{
				/// <summary>
		/// 配置编号
		/// </summary>		
				[OracleColumn(IsDbGenerated = true , IsPrimaryKey = true, ColumnDescription = "配置编号", ColumnName = "ConfigId")]
        [SqlColumn(IsDbGenerated = true , IsPrimaryKey = true, ColumnDescription = "配置编号", ColumnName = "ConfigId")]
				public long ConfigId { get; set; }

				/// <summary>
		/// 配置键
		/// </summary>		
				[OracleColumn(ColumnDescription = "配置键", ColumnName = "ConfigKey")]
        [SqlColumn(ColumnDescription = "配置键", ColumnName = "ConfigKey")]		 
				public string ConfigKey { get; set; }

				/// <summary>
		/// 配置值
		/// </summary>		
				[OracleColumn(ColumnDescription = "配置值", ColumnName = "ConfigValue")]
        [SqlColumn(ColumnDescription = "配置值", ColumnName = "ConfigValue")]		 
				public string ConfigValue { get; set; }

				/// <summary>
		/// 最后更新时间
		/// </summary>		
				[OracleColumn(ColumnDescription = "最后更新时间", ColumnName = "LastUpdateTime")]
        [SqlColumn(ColumnDescription = "最后更新时间", ColumnName = "LastUpdateTime")]		 
				public DateTime? LastUpdateTime { get; set; }

				/// <summary>
		/// 最后更新人
		/// </summary>		
				[OracleColumn(ColumnDescription = "最后更新人", ColumnName = "LastUpdateUser")]
        [SqlColumn(ColumnDescription = "最后更新人", ColumnName = "LastUpdateUser")]		 
				public string LastUpdateUser { get; set; }

				/// <summary>
		/// 备注
		/// </summary>		
				[OracleColumn(ColumnDescription = "备注", ColumnName = "Remark")]
        [SqlColumn(ColumnDescription = "备注", ColumnName = "Remark")]		 
				public string Remark { get; set; }

		 

	}
}
