using System;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;

/// <summary>创建聚合函数，允许串联字符串</summary>
[Serializable, SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToDuplicates = false, IsInvariantToNulls = true,
	IsInvariantToOrder = false, IsNullIfEmpty = true, MaxByteSize = 8000, Name = "SYSF_ARRAYJOIN")]
public struct ArrayJoin : IBinarySerialize
{
	private StringBuilder _result;
	/// <summary>聚合函数初始化</summary>
	public void Init() { _result = new StringBuilder(); }

	/// <summary>当前聚合函数新增一个字段数据</summary>
	/// <param name="Value">需要新增数 SqlDecimal 类型值。</param>
	/// <param name="separator">串联字符串的符号</param>
	public void Accumulate(SqlString Value, SqlString separator)
	{
		if (Value.IsNull)
		{
			return;
		}
		else
		{
			if (_result.Length > 0)
				_result.Append(separator);
			_result.Append(Value.Value);
		}
	}
	/// <summary>聚合函数实例合并</summary>
	/// <param name="Group">需要合并的 ArrayJoin 类实例。</param>
	public void Merge(ArrayJoin Group)
	{
		_result.Append(Group._result.ToString());
	}

	/// <summary>聚合结束返回最终值。</summary>
	/// <returns>返回指定的四分位值</returns>
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
