/*
=========================================================================================================
  Module      : Extends of SubscriptionBoxItemModel (SubscriptionBoxItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// Extends of SubscriptionBoxItemModel
	/// </summary>
	public partial class SubscriptionBoxItemModel : ModelBase<SubscriptionBoxItemModel>
	{
		/// <summary>
		/// キャンペーン価格を適用できるか判定
		/// </summary>
		/// <param name="criteria">基準日時</param>
		/// <returns>適用できる: true</returns>
		public bool CanApplyCampaignPrice(DateTime criteria)
		{
			var isInTermSince = ((this.CampaignSince.HasValue) && (this.CampaignSince.Value <= criteria));
			var isInTermUntil = ((this.CampaignUntil.HasValue) && (criteria <= this.CampaignUntil.Value));
			var result = (isInTermSince && isInTermUntil);
			return result;
		}

		#region プロパティ
		/// <summary>Product Name</summary>
		public string ProductName { get; set; }
		/// <summary>Shipping Type</summary>
		public string ShippingType { get; set; }
		/// <summary>商品名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>バリエーション名1 </summary>
		public string VariationName1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = value; }
		}
		/// <summary>バリエーション名2</summary>
		public string VariationName2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = value; }
		}
		/// <summary>バリエーション名3</summary>
		public string VariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = value; }
		}
		/// <summary>単価</summary>
		public decimal Price
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] == DBNull.Value) return 0;
				return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] = value; }
		}
		#endregion
	}
}
