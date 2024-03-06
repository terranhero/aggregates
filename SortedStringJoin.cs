using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

/// <summary>创建聚合函数，允许串联字符串</summary>
[Serializable, SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToDuplicates = false, IsInvariantToNulls = true,
	IsInvariantToOrder = false, IsNullIfEmpty = true, MaxByteSize = 8000, Name = "SYSF_SORTEDSTRINGJOIN")]
public struct SortedStringJoin : IBinarySerialize
{
	private List<string> _results;

	public void Init() { _results = new List<string>(100); }

	public void Accumulate(SqlString Value)
	{
		if (Value.IsNull)
		{
			return;
		}
		else
		{
			_results.Add(Value.Value);
		}

	}

	public void Merge(SortedStringJoin Group)
	{
		_results.AddRange(Group._results);
	}

	public SqlString Terminate()
	{
		if (_results.Count > 0)
		{
			_results.Sort();
			return new SqlString(string.Join(",", _results));
		}
		return new SqlString("");
	}

	#region IBinarySerialize 成员

	public void Read(System.IO.BinaryReader reader)
	{
		string result = reader.ReadString();
		_results = new List<string>(100);
		if (string.IsNullOrEmpty(result) == false)
		{
			_results.AddRange(result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
		}
	}

	public void Write(System.IO.BinaryWriter writer)
	{
		if (_results.Count == 0) { writer.Write(""); }
		else { writer.Write(string.Join(",", _results)); }
	}

	#endregion
}
