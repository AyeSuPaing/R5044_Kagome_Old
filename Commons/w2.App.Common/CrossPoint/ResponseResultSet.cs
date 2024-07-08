/*
=========================================================================================================
  Module      : CrossPoint API レスポンスの結果セットモデル (ResponseResultSet.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint
{
	/// <summary>
	/// 結果セットモデル
	/// </summary>
	/// <typeparam name="TResult">メッセージ本体のモデル</typeparam>
	public class ResultSet<TResult>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ResultSet()
		{
			this.TotalResult = 0;
			this.ResultStatus = new ResultStatus();
			this.Result = new TResult[0];
		}

		/// <summary>取得件数</summary>
		[XmlAttribute("TotalResult")]
		public int TotalResult { get; set; }
		/// <summary>リターンコード</summary>
		[XmlElement("ResultStatus")]
		public ResultStatus ResultStatus { get; set; }
		/// <summary>メッセージ本体</summary>
		[XmlElement("Result")]
		public TResult[] Result { get; set; }
		/// <summary>Xml response</summary>
		public string XmlResponse { get; set; }
	}
}
