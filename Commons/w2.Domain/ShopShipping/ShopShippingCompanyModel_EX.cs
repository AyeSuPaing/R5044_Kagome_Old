/*
=========================================================================================================
  Module      : 配送種別配送会社マスタモデル (ShopShippingCompanyModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 配送種別配送会社マスタモデル
	/// </summary>
	public partial class ShopShippingCompanyModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>宅配便か？</summary>
		public bool IsExpress { get { return (this.ShippingKbn == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS); } }
		/// <summary>メール便か？</summary>
		public bool IsMail { get { return (this.ShippingKbn == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL); } }
		/// <summary>デフォルト？</summary>
		public bool IsDefault { get { return (this.DefaultDeliveryCompany == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID); } }
		#endregion
	}
}
