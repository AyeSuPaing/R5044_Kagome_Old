/*
=========================================================================================================
  Module      : Point History Item (PointHistoryItem.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.PointHistory
{
	/// <summary>
	/// ポイント履歴モデル
	/// </summary>
	public class PointHistoryItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointHistoryItem()
		{
			this.PointGrantDate = string.Empty;
			this.PointDecideDate = string.Empty;
			this.ShopName = string.Empty;
			this.Point = 0m;
			this.PointValidPeriod = string.Empty;
			this.EffectiveFlg = string.Empty;
			this.PointChangeReason = string.Empty;
		}

		/// <summary>ポイント付与日</summary>
		[XmlElement("PointGrantDate")]
		public string PointGrantDate { get; set; }
		/// <summary>ポイント確定日</summary>
		[XmlElement("PointDecideDate")]
		public string PointDecideDate { get; set; }
		/// <summary>ショップ名</summary>
		[XmlElement("ShopName")]
		public string ShopName { get; set; }
		/// <summary>ポイント数</summary>
		[XmlElement("Point")]
		public decimal Point { get; set; }
		/// <summary>ポイント有効期限</summary>
		[XmlElement("PointValidPeriod")]
		public string PointValidPeriod { get; set; }
		/// <summary>有効ポイントフラグ</summary>
		[XmlElement("EffectiveFlg")]
		public string EffectiveFlg { get; set; }
		/// <summary>理由</summary>
		[XmlElement("PointChangeReason")]
		public string PointChangeReason { get; set; }
		/// <summary>有効ポイントか</summary>
		public bool IsEffectivePoint
		{
			get { return (this.EffectiveFlg == Constants.CROSS_POINT_FLG_POINT_TYPE_COMP); }
		}
	}
}
