using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections;

public partial class UserDefinedFunctions
{
	private class NewKey32Class
	{
		public SqlInt32 NewKey;
		public SqlInt32 SN;
		public NewKey32Class(SqlInt32 sn, SqlInt32 newKey) { SN = sn; NewKey = newKey; }
	}

	/// <summary>
	/// 创建表的新关键字
	/// </summary>
	/// <param name="tableName"></param>
	/// <param name="keyName"></param>
	/// <param name="recordNumber">需要创建的新关键字的行数</param>
	/// <returns></returns>
	[SqlFunction(DataAccess = DataAccessKind.Read, TableDefinition = "SN int,NewKey int", FillRowMethodName = "FillRow", Name = "SYSF_CREATENEWKEY")]
	public static IEnumerable CreateNewKeyInt(SqlString tableName, SqlString keyName, SqlInt32 recordNumber)
	{
		using (SqlConnection conn = new SqlConnection("context connection=true"))
		{
			conn.Open();
			string sql = string.Format("SELECT ISNULL(MAX({0}),0)+1 FROM {1}", keyName.Value, tableName.Value);
			SqlCommand cmd = new SqlCommand(sql, conn);
			int result = Convert.ToInt32(cmd.ExecuteScalar());
			ArrayList list = new ArrayList();
			if (recordNumber.IsNull || recordNumber <= 0)
				recordNumber = 1;
			for (int index = 1; index <= recordNumber; index++)
				list.Add(new NewKey32Class(index, result + index - 1));
			return list;
		}
	}

	public static void FillRow(Object obj, out SqlInt32 SN, out SqlInt32 NewKey)
	{
		NewKey32Class entity = (NewKey32Class)obj;
		SN = entity.SN;
		NewKey = entity.NewKey;
	}
};

