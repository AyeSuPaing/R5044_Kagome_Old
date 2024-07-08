/*
=========================================================================================================
  Module      : 注文取得する結果モデルクラス (GetOrderResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Xml.Serialization;

namespace w2.App.Common.CrossMall.Order
{
	/// <summary>
	/// 注文結果モデルクラス
	/// </summary>
	public class GetOrderResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GetOrderResult()
		{
			this.No = 0;
			this.OrderCode = string.Empty;
			this.PhaseName = string.Empty;
		}
		#endregion

		#region プロパティ
		/// <summary> 枝番 </summary>
		[XmlAttribute("No")]
		public int No { get; set; }
		/// <summary> 注文番号 </summary>
		[XmlElement("order_code")]
		public string OrderCode { get; set; }
		/// <summary> フェーズ名 </summary>
		[XmlElement("phase_name")]
		public string PhaseName { get; set; }
		/// <summary> 出荷完了フラグ </summary>
		public bool IsAlreadyShipped
		{
			get
			{
				if (string.IsNullOrEmpty(PhaseName)) return false;
				var shippedPhaseNameList = Constants.CROSS_MALL_INTEGRATION_PHASE_NAME.Split(
					new string[] { Constants.CROSS_MALL_INTEGRATION_PHASE_NAME_DELIMITER },
					StringSplitOptions.None);
				return shippedPhaseNameList.Contains(PhaseName);
			}
		}
		#endregion
	}
}
