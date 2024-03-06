using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;

/// <summary>创建聚合函数，允许串联字符串</summary>
[Serializable, SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToDuplicates = false, IsInvariantToNulls = true,
	IsInvariantToOrder = false, IsNullIfEmpty = true, MaxByteSize = 8000, Name = "SYSF_STRINGJOIN")]
public struct StringJoin : IBinarySerialize
{
	private StringBuilder _result;

	public void Init() { _result = new StringBuilder();}

	public void Accumulate(SqlString Value)
	{
		if (Value.IsNull)
		{
			return;
		}
		else
		{
			if (_result.Length > 0)
				_result.AppendFormat(",{0}", Value.Value);
			else
				_result.Append(Value.Value);
		}

	}

	public void Merge(StringJoin Group)
	{
		_result.Append(Group._result.ToString());
	}

	public SqlString Terminate()
	{
		if (_result.Length > 0)
		{
			return new SqlString(_result.ToString());
		}
		return new SqlString("");
	}

	#region IBinarySerialize 成员

	public void Read(System.IO.BinaryReader r)
	{
		_result = new StringBuilder(r.ReadString());
	}

	public void Write(System.IO.BinaryWriter w)
	{
		w.Write(_result.ToString());
	}

	#endregion
}
