/*
=========================================================================================================
  Module      : Point Api Result (PointApiResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// 発行ポイント結果モデル
	/// </summary>
	public class PointApiResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointApiResult()
		{
			this.No = 0;
			this.PointId = string.Empty;
			this.BaseGrantPoint = 0m;
			this.SpecialGrantPoint = 0m;
			this.EffectivePoint = 0m;
			this.PointChangeReasonCd = 0;
			this.PointChangeReason = string.Empty;
		}

		/// <summary>連番</summary>
		[XmlAttribute("No")]
		public int No { get; set; }
		/// <summary>ポイントID</summary>
		[XmlElement("PointId")]
		public string PointId { get; set; }
		/// <summary>基本発行ポイント</summary>
		[XmlElement("BaseGrantPoint")]
		public decimal BaseGrantPoint { get; set; }
		/// <summary>特別発行ポイント</summary>
		[XmlElement("SpecialGrantPoint")]
		public decimal SpecialGrantPoint { get; set; }
		/// <summary>有効ポイント</summary>
		[XmlElement("EffectivePoint")]
		public decimal EffectivePoint { get; set; }
		/// <summary>ポイント変動理由コード</summary>
		[XmlElement("PointChangeReasonCd")]
		public int PointChangeReasonCd { get; set; }
		/// <summary>ポイント変動理由</summary>
		[XmlElement("PointChangeReason")]
		public string PointChangeReason { get; set; }
	}
}
