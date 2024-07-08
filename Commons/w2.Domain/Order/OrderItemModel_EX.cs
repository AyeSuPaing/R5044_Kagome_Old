/*
=========================================================================================================
  Module      : 注文商品情報モデル (OrderItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文商品情報モデル
	/// </summary>
	public partial class OrderItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseFlg
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
		}
		/// <summary>>商品バリエーション画像ヘッダ</summary>
		public string ProductVariationImageHead
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] = value; }
		}
		/// <summary>>商品画像ヘッダ</summary>
		public string ProductImageHead
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD] = value; }
		}
		/// <summary>在庫管理方法</summary>
		public string StockManagementKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] = value; }
		}
		/// <summary>商品同梱された商品か</summary>
		public bool IsProductBundleItem
		{
			get { return (string.IsNullOrEmpty(this.ProductBundleId) == false); }
		}
		/// <summary>実在庫引当済？(1つ以上引当されている？)</summary>
		public bool IsRealStockReserved
		{
			get { return (this.ItemRealstockReserved > 0); }
		}
		/// <summary>返品商品？</summary>
		public bool IsReturnItem
		{
			get { return (this.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN); }
		}
		/// <summary>交換商品？</summary>
		public bool IsExchangeItem
		{
			get { return (this.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE); }
		}
		/// <summary>注文更新時削除対象フラグ</summary>
		public bool DeleteTarget { get; set; }
		/// <summary>定期商品？</summary>
		public bool IsFixedPurchaseItem
		{
			get { return (this.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON); }
		}
		/// <summary>定期購入割引商品か</summary>
		public bool IsFixedPurchaseDiscountItem
		{
			get { return (string.IsNullOrEmpty(this.FixedPurchaseDiscountType) == false); }
		}
		/// <summary>レコメンド商品？</summary>
		public bool IsRecommendItem
		{
			get { return (string.IsNullOrEmpty(this.RecommendId) == false); }
		}
		/// <summary>商品小計(割引按分計算後)</summary>
		public decimal PriceSubtotalAfterDistribution { get; set; }
		/// <summary>調整金額(割引按分計算後)</summary>
		public decimal ItemPriceRegulation { get; set; }
		/// <summary>定期購入の注文か</summary>
		public bool IsFixedPurchaseOrder
		{
			get { return this.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON; }
		}
		/// <summary>商品連携IDリスト</summary>
		public List<string> CooperationIdList
		{
			get
			{
				var list = new List<string>
				{
					this.CooperationId1,
					this.CooperationId2,
					this.CooperationId3,
					this.CooperationId4,
					this.CooperationId5,
					this.CooperationId6,
					this.CooperationId7,
					this.CooperationId8,
					this.CooperationId9,
					this.CooperationId10
				};
				return list;
			}
		}
		/// <summary>頒布会商品か</summary>
		public bool IsSubscriptionBox => string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false;
		/// <summary>頒布会定額コース商品か</summary>
		public bool IsSubscriptionBoxFixedAmount => this.SubscriptionBoxFixedAmount.HasValue;
		#endregion
	}
}
