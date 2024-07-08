/*
=========================================================================================================
  Module      : 注文セットプロモーションテーブルモデル (OrderSetPromotionModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文セットプロモーションテーブルモデル
	/// </summary>
	public partial class OrderSetPromotionModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>配送料無料か</summary>
		public bool IsDiscountTypeShippingChargeFree
		{
			get { return (this.ShippingChargeFreeFlg == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON); }
		}
		/// <summary>決済手数料無料か</summary>
		public bool IsDiscountTypePaymentChargeFree
		{
			get { return (this.PaymentChargeFreeFlg == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON); }
		}
		#endregion
	}
}
