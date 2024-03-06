using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

/// <summary>计算集合四分位值</summary>
[Serializable, SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToDuplicates = false, IsInvariantToNulls = true,
	IsInvariantToOrder = false, MaxByteSize = 8000, Name = "SYSF_QUARTILE1")]
public struct Quartile1 : IBinarySerialize
{
	private System.Collections.Generic.List<SqlDecimal> datas;
	/// <summary>聚合函数初始化</summary>
	public void Init() { datas = new System.Collections.Generic.List<SqlDecimal>(); }

	/// <summary>当前聚合函数新增一个字段数据</summary>
	/// <param name="Value">需要新增数 SqlDecimal 类型值。</param>
	/// <param name="quart">表示取哪个四分位值,1:下四分位值，2：中位值，3：上四分位值</param>
	public void Accumulate(SqlDecimal Value)
	{
		if (Value.IsNull) { return; }
		datas.Add(Value);
	}

	/// <summary>聚合函数实例合并</summary>
	/// <param name="Group">需要合并的 Quartile 类实例。</param>
	public void Merge(Quartile1 Group)
	{
		datas.AddRange(Group.datas);
	}

	/// <summary>聚合结束返回最终值。</summary>
	/// <returns>返回指定的四分位值</returns>
	public SqlDecimal Terminate()
	{
		if (datas.Count == 0) { return new SqlDecimal(0); }
		if (datas.Count == 1) { return datas[0]; }
		datas.Sort(); SqlDecimal middleVal = 0;

		double quart = (datas.Count - 1) / 4 + 1;

		double intQuart = Math.Truncate(quart);
		if (intQuart == quart) { return datas[Convert.ToInt32(intQuart)]; }

		return datas[Convert.ToInt32(intQuart)] * new SqlDecimal(1 - quart + intQuart) + datas[Convert.ToInt32(intQuart) + 1] * new SqlDecimal(quart - intQuart);
	}

	// 这是占位符成员字段
	public int _var1;

	void IBinarySerialize.Read(System.IO.BinaryReader r)
	{
		int size = r.ReadInt32();
		// 初始化数据.
		datas = new System.Collections.Generic.List<SqlDecimal>(size);
		// 先读取总数量.
		// 依次读取数据，加入列表.
		for (int i = 0; i < size; i++)
		{
			datas.Add(new SqlDecimal(r.ReadDecimal()));
		}
	}

	void IBinarySerialize.Write(System.IO.BinaryWriter w)
	{
		// 先写入一个 总数量
		w.Write(datas.Count);
		// 依次写入每一个数据.
		foreach (SqlDecimal data in datas)
		{
			w.Write(data.Value);
		}
	}
}
