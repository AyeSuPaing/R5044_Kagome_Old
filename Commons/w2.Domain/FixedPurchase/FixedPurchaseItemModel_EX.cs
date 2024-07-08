/*
=========================================================================================================
  Module      : 定期購入商品情報モデル (FixedPurchaseItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入商品情報モデル
	/// </summary>
	public partial class FixedPurchaseItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>デジタルコンテンツ商品フラグ</summary>
		public string DigitalContentsFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] = value; }
		}
		/// <summary>バリエーション商品か</summary>
		public bool HasVariation
		{
			get { return (this.ProductId != this.VariationId); }
		}
		#endregion
	}
}
