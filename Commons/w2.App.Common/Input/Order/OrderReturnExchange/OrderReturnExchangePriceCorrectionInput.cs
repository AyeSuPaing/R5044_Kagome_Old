/*
=========================================================================================================
  Module      : 注文返品交換金額補正入力クラス (OrderReturnExchangePriceCorrectionInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Input.Order.OrderReturnExchange
{
	/// <summary>
	/// 注文返品金額補正入力クラス
	/// </summary>
	[Serializable]
	public class OrderReturnExchangePriceCorrectionInput : InputBaseDto
	{
		#region プロパティ
		/// <summary>商品価格調整金額</summary>
		public string PriceCorrection
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE] = value; }
		}
		/// <summary>税率</summary>
		public string ProductTaxRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE] = value; }
		}
		#endregion
	}
}