/*
=========================================================================================================
  Module      : Csv情報構造体(CsvSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.Text;

namespace w2.ExternalAPI.Common.FrameWork.File
{
	/// <summary>
	///	CSV情報構造体
	/// </summary>
	/// <remarks>
	/// CSVファイルのエンコード、区切り文字、囲み文字、改行コードに関する設定情報を持つ
	/// 読み込み及び、書き込みに利用される
	/// </remarks>
	public struct CsvSetting
	{
		/// <summary>エンコード</summary>
		public Encoding Encoding;
		/// <summary>項目区切り文字</summary>
		public string Delimiter;
		/// <summary>項目囲み文字</summary>
		public string Enclosure;
		/// <summary>行区切り文字</summary>
		public string LineDelimiter;
		/// <summary>項目内改行可否</summary>
		public bool DoesEscapeLineDelimiter;
		/// <summary>項目内改行エスケープ文字</summary>
		public string LineDelimiterEscapeString;
	}
}
