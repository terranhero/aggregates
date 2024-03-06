using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class UserDefinedFunctions
{
	/// <summary>
	/// 分割字符串函数会犯
	/// </summary>
	/// <param name="source">需要分割的字符串</param>
	/// <param name="splitor">分割符号。</param>
	/// <returns>返回分割分割完成的整型数组</returns>
	[SqlFunction(DataAccess = DataAccessKind.Read, TableDefinition = "ITEM int",
		FillRowMethodName = "FillStringSpliter", Name = "SYSF_STRINGSPLITER")]
	public static IEnumerable StringSpliter(SqlString source, SqlString splitor)
	{
		List<SqlInt32> list = new List<SqlInt32>();
		string[] strArray = Regex.Split(source.ToString(), splitor.ToString());
		foreach (string item in strArray)
		{
			if (string.IsNullOrEmpty(item))
				continue;
			if (Regex.IsMatch(item, "^\\d+$"))
			{
				list.Add(SqlInt32.Parse(item));
			}
		}
		// 在此处放置代码
		return list.ToArray();
	}

	public static void FillStringSpliter(Object obj, out SqlInt32 SN)
	{
		SN = (SqlInt32)obj;
	}
};

