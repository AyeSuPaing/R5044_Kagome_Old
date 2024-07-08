/*
=========================================================================================================
  Module      : インポート情報構造体(ApiEntry.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace w2.ExternalAPI.Common.Entry
{
	/// <summary>
	///	インポート情報構造体
	/// </summary>
	/// <remarks>
	/// インポートしたファイルの１レコード単位の情報を格納するための構造体
	/// 構造体内の情報を利用して各種APIコマンドの実行などが行われる
	/// インポートの処理単位
	/// </remarks>
	public struct ApiEntry
	{
		/// <summary> レコードデータ </summary>
		public DataRow Data;

		/// <summary>
		/// 指定した箇所のデータを型変換を施して返却する。データが存在しなければNullを返す。
		/// </summary>
		/// <typeparam name="T">変換後の型</typeparam>
		/// <param name="index">インデックス</param>
		/// <returns>データ</returns>
		public T? GetData<T>(int index)
			where T : struct
		{
			if (Data.ItemArray.Length <= index) return null;
			if (string.IsNullOrEmpty(Data[index].ToString())) return null;
			
			// 型変換して返却
			return new T?((T)Convert.ChangeType(Data[index], typeof(T)));
		}
	}
}
