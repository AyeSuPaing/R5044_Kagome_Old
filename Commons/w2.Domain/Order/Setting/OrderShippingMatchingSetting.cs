/*
=========================================================================================================
  Module      : 配送先マッチング設定クラス(OrderShippingMatchingSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain.Helper;

namespace w2.Domain.Order.Setting
{
	/// <summary>配送先マッチング設定クラス</summary>
	public static class OrderShippingMatchingSetting
	{
		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static OrderShippingMatchingSetting()
		{
			UpdateSetting();
		}

		/// <summary>
		/// 設定更新
		/// </summary>
		public static void UpdateSetting()
		{
			Setting = new MatchingSetting(@"OrderShippingMatchingSetting.xml", "OrderShippingMatchingSetting");
		}

		/// <summary>マッチング設定情報</summary>
		public static MatchingSetting Setting { get; private set; }
	}
}
