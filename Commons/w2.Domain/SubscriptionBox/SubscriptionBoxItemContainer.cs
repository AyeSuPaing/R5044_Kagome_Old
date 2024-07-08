/*
=========================================================================================================
  Module      : 頒布会選択可能商品コンテナ (SubscriptionBoxItemContainer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// 頒布会選択可能商品コンテナ 一覧クラス
	/// </summary>
	public class SubscriptionBoxItemContainerList : IEnumerable<SubscriptionBoxItemContainer>
	{
		private readonly SubscriptionBoxItemContainer[] _items;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="items">アイテム</param>
		public SubscriptionBoxItemContainerList(IEnumerable<SubscriptionBoxItemContainer> items)
		{
			_items = items.ToArray();
		}

		/// <summary>
		/// イテレータを取得
		/// </summary>
		/// <returns>IEnumerator</returns>
		public IEnumerator<SubscriptionBoxItemContainer> GetEnumerator()
		{
			return _items.Cast<SubscriptionBoxItemContainer>().GetEnumerator();
		}
		/// <summary>
		/// イテレータを取得
		/// </summary>
		/// <returns>IEnumerator</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// インデクサ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>コンテナ</returns>
		public SubscriptionBoxItemContainer this[string shopId, string productId, string variationId]
		{
			get
			{
				var found = _items.FirstOrDefault(
					i => (i.ShopId == shopId)
						&& (i.ProductId == productId)
						&& (i.VariationId == variationId));
				return found;
			}
		}
	}

	/// <summary>
	/// 頒布会選択可能商品コンテナ
	/// </summary>
	public class SubscriptionBoxItemContainer : SubscriptionBoxItemModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SubscriptionBoxItemContainer()
		{
			this.DataSource = new Hashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データソース</param>
		public SubscriptionBoxItemContainer(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データソース</param>
		public SubscriptionBoxItemContainer(Hashtable source)
			: base(source)
		{
		}

		/// <summary>
		/// 価格を取得
		/// </summary>
		/// <param name="criteria">基準日時</param>
		/// <returns>価格</returns>
		public decimal GetPrice(DateTime criteria)
		{
			if (CanApplyCampaignPrice(criteria)) return this.CampaignPrice.Value;

			var result
				= this.VariationFixedPurchasePrice
					?? this.SpecialPrice
					?? this.Price
					?? this.FixedPurchasePrice
					?? this.DisplaySpecialPrice
					?? this.DisplayPrice;
			return result;
		}

		// SubscriptionBoxItemModelに定義済み（なぜか）
		// /// <summary>商品名</summary>
		// public string ProductName
		// {
		// 	get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
		// 	set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		// }
		/// <summary>数量</summary>
		public int Quantity
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>商品画像ヘッダ</summary>
		public string ProductImageHeader
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>バリエーション定期購入価格</summary>
		public decimal? VariationFixedPurchasePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] = value; }
		}
		/// <summary>バリエーション価格</summary>
		public new decimal? Price
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] = value; }
		}
		/// <summary>バリエーション特別価格</summary>
		public decimal? SpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] = value; }
		}
		/// <summary>定期購入価格</summary>
		public decimal? FixedPurchasePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE] = value; }
		}
		/// <summary>表示価格</summary>
		public decimal DisplayPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = value; }
		}
		/// <summary>表示特別価格</summary>
		public decimal? DisplaySpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = value; }
		}
		/// <summary>バリエーション名１</summary>
		public string ProductVariationName1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = value; }
		}
		/// <summary>バリエーション名２</summary>
		public string ProductVariationName2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = value; }
		}
		/// <summary>バリエーション名３</summary>
		public string ProductVariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = value; }
		}
	}
}
