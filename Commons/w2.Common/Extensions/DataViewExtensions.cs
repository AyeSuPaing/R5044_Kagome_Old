/*
=========================================================================================================
  Module      : DataView系クラス拡張モジュール(DataViewExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace w2.Common.Extensions
{
	///**************************************************************************************
	/// <summary>
	/// DataView系クラスに拡張メソッドを追加する
	/// </summary>
	///**************************************************************************************
	public static class DataViewExtensions
	{
		/// <summary>
		/// ハッシュテーブルを取得する
		/// </summary>
		/// <param name="dataRowView">dataRowView</param>
		/// <returns>ハッシュテーブル</returns>
		public static Hashtable ToHashtable(this DataRowView dataRowView)
		{
			var hashtable = new Hashtable();
			foreach (DataColumn dataColumn in dataRowView.Row.Table.Columns)
			{
				hashtable.Add(dataColumn.ColumnName, dataRowView[dataColumn.ColumnName]);
			}

			return hashtable;
		}

		/// <summary>
		/// ハッシュテーブルのリストを取得する
		/// </summary>
		/// <param name="dataRowView">dataRowView</param>
		/// <returns>ハッシュテーブル</returns>
		public static List<Hashtable> ToHashtableList(this DataView dataView)
		{
			var hashtableList = new List<Hashtable>();
			foreach (DataRowView dataRowView in dataView)
			{
				hashtableList.Add(dataRowView.ToHashtable());
			}

			return hashtableList;
		}

		/// <summary>
		/// カラム名を取得
		/// </summary>
		/// <param name="dataView">データ</param>
		/// <returns>ヘッダー</returns>
		public static string[] GetColumn(this DataView dataView)
		{
			return dataView.Table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToArray();
		}
	}
}
