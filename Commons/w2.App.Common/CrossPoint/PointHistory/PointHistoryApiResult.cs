/*
=========================================================================================================
  Module      : Point History Api Result (PointHistoryApiResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.PointHistory
{
	/// <summary>
	/// ポイント履歴結果モデル
	/// </summary>
	public class PointHistoryApiResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointHistoryApiResult()
		{
			this.TotalCount = 0;
		}

		/// <summary>総件数</summary>
		[XmlElement("TotalCount")]
		public int TotalCount { get; set; }
		/// <summary>ポイント履歴一覧</summary>
		[XmlElement("List")]
		public PointHistoryItem[] List { get; set; }
	}
}
