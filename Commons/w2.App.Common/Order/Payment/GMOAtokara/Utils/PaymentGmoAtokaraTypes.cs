/*
=========================================================================================================
  Module      : GMOアトカラ タイプ(PaymentGmoAtokaraTypes.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMOAtokara.Utils
{
	public class PaymentGmoAtokaraTypes
	{
		/// <summary>取引更新種別</summary>
		public enum UpdateKind
		{
			/// <summary>修正</summary>
			[XmlEnum(Name = "1")]
			Modify = 1,
			/// <summary>キャンセル</summary>
			[XmlEnum(Name = "2")]
			Cancel = 2,
		}	}
}
