/*
=========================================================================================================
  Module      : 注文配送先情報モデル (OrderShippingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.User.Helper;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文配送先情報モデル
	/// </summary>
	public partial class OrderShippingModel
	{
		#region 住所項目を結合（国名は含めない）
		/// <summary>
		/// 住所項目を結合（国名は含めない）
		/// </summary>
		/// <returns>結合した住所</returns>
		public string ConcatenateAddressWithoutCountryName()
		{
			var address = AddressHelper.ConcatenateAddressWithoutCountryName(
				this.ShippingAddr1,
				this.ShippingAddr2,
				this.ShippingAddr3,
				this.ShippingAddr4);

			return address;
		}
		#endregion

		#region プロパティ
		/// <summary>商品リスト</summary>
		public OrderItemModel[] Items
		{
			get { return (OrderItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>配送会社名</summary>
		public string DeliveryCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME] = value; }
		}
		/// <summary>配送希望時間帯メッセージ</summary>
		public string DeliveryCompanyShippingTimeMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE] = value; }
		}
		/// <summary>郵便番号（ハイフンなし）</summary>
		public string HyphenlessShippingZip
		{
			get { return this.ShippingZip.Replace("-", ""); }
		}
		#endregion
	}
}
