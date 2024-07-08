/*
=========================================================================================================
  Module      : 頒布会次回お届け商品入力クラス (SubscriptionBoxNextShippingProductInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.Domain;
using w2.Domain.SubscriptionBox;

namespace Input.FixedPurchase
{
	/// <summary>
	/// 頒布会次回お届け商品入力クラス
	/// </summary>
	[Serializable]
	public class SubscriptionBoxNextShippingProductInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isFixedAmount">定額頒布会か？</param>
		/// <param name="items">商品群</param>
		private SubscriptionBoxNextShippingProductInput(
			bool isFixedAmount,
			SubscriptionBoxNextShippingProductInputItemList items)
		{
			this.IsFixedAmount = isFixedAmount;
			this.Items = items;
		}

		/// <summary>
		/// 初期値作成
		/// </summary>
		/// <param name="course">頒布会コース</param>
		/// <param name="subscriptionBoxItems">選択可能商品</param>
		/// <param name="preserveSelection">選択中の商品を保持</param>
		/// <param name="criteria">基準日時</param>
		/// <param name="subscriptionBoxOrderCount">頒布会注文回数</param>
		/// <param name="isFixedAmount">定額頒布会か？</param>
		/// <returns>入力値クラス</returns>
		public static SubscriptionBoxNextShippingProductInput Create(
			SubscriptionBoxModel course,
			SubscriptionBoxItemContainerList subscriptionBoxItems,
			bool preserveSelection,
			DateTime criteria,
			int subscriptionBoxOrderCount,
			bool isFixedAmount)
		{
			var result = new SubscriptionBoxNextShippingProductInput(
				isFixedAmount,
				new SubscriptionBoxNextShippingProductInputItemList(
					subscriptionBoxItems
						.Select(i => SubscriptionBoxNextShippingProductInputItem.CreateFromDomainContainer(course, subscriptionBoxOrderCount, i, preserveSelection, criteria))
						.OrderBy(i => (i.IsSelected == false)))); // 初期状態では選択されている商品を先頭に持っていく
			return result;
		}

		/// <summary>
		/// 頒布会の注文条件を満たしているか確認
		/// </summary>
		/// <param name="course">頒布会コースモデル</param>
		/// <param name="criteria">基準日時</param>
		/// <param name="violationWithMinimum">最小条件に違反した</param>
		/// <param name="subscriptionBoxOrderCount">頒布会注文回数</param>
		/// <returns>エラーメッセージ</returns>
		public string[] IsMeetingCondition(
			SubscriptionBoxModel course,
			int subscriptionBoxOrderCount,
			DateTime criteria,
			out bool violationWithMinimum)
		{
			var errors = new List<string>();

			// 本当はここのチェックロジックはシステム内で統一したい‥
			// そしてコードの見通しが悪い

			violationWithMinimum = false;
			// 購入種類数のチェック
			if (course.MinimumNumberOfProducts.HasValue && course.MaximumNumberOfProducts.HasValue)
			{
				if (course.MinimumNumberOfProducts > this.Items.TotalSelectedItemCount
					|| course.MaximumNumberOfProducts < this.Items.TotalSelectedItemCount)
				{
					errors.Add(
						WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_NUMBER_OF_PRODUCTS_ERROR_DISPLAY_REQUIRED_NUMBER_OF_PRODUCTS)
							.Replace("@@ 1 @@", course.DisplayName)
							.Replace("@@ 2 @@", course.MinimumNumberOfProducts.ToString())
							.Replace("@@ 3 @@", course.MaximumNumberOfProducts.ToString()));
				}

				if (course.MinimumNumberOfProducts > this.Items.TotalSelectedItemCount)
				{
					violationWithMinimum = true;
				}
			}
			else if ((course.MinimumNumberOfProducts ?? 1) > this.Items.TotalSelectedItemCount)
			{
				errors.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCTS_ERROR)
						.Replace("@@ 1 @@", course.DisplayName)
						.Replace("@@ 2 @@", (course.MinimumNumberOfProducts ?? 1).ToString()));
				violationWithMinimum = true;
			}
			else if (course.MaximumNumberOfProducts.HasValue
					&& course.MaximumNumberOfProducts < this.Items.TotalSelectedItemCount)
			{
				errors.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_MAX_NUMBER_OF_PRODUCTS_ERROR)
						.Replace("@@ 1 @@", course.DisplayName)
						.Replace("@@ 2 @@", course.MaximumNumberOfProducts.ToString()));
			}

			// 数量のチェック
			if (course.MinimumPurchaseQuantity.HasValue && course.MaximumPurchaseQuantity.HasValue)
			{
				if (course.MinimumPurchaseQuantity > this.Items.TotalQuantity
					|| course.MaximumPurchaseQuantity < this.Items.TotalQuantity)
				{
					errors.Add(
						WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY)
							.Replace("@@ 1 @@", course.DisplayName)
							.Replace("@@ 2 @@", course.MinimumPurchaseQuantity.ToString())
							.Replace("@@ 3 @@", course.MaximumPurchaseQuantity.ToString()));
				}

				if (course.MinimumPurchaseQuantity > this.Items.TotalQuantity)
				{
					violationWithMinimum = true;
				}
			}
			else if (course.MinimumPurchaseQuantity.HasValue
					&& course.MinimumPurchaseQuantity > this.Items.TotalQuantity)
			{
				errors.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MIN)
						.Replace("@@ 1 @@", course.DisplayName)
						.Replace("@@ 2 @@", course.MinimumPurchaseQuantity.ToString()));
				violationWithMinimum = true;
			}
			else if (course.MaximumPurchaseQuantity.HasValue
					&& course.MaximumPurchaseQuantity < this.Items.TotalQuantity)
			{
				errors.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MAX)
						.Replace("@@ 1 @@", course.DisplayName)
						.Replace("@@ 2 @@", course.MaximumPurchaseQuantity.ToString()));
			}

			// 金額のチェック
			if (course.MinimumAmount.HasValue && course.MaximumAmount.HasValue)
			{
				if (course.MinimumAmount > this.Items.Subtotal
					|| course.MaximumAmount < this.Items.Subtotal)
				{
					errors.Add(
						WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_NON_FIXED)
							.Replace("@@ 1 @@", CurrencyManager.ToPrice(course.MinimumAmount))
							.Replace("@@ 2 @@", CurrencyManager.ToPrice(course.MaximumAmount)));
				}

				if (course.MinimumAmount > this.Items.Subtotal)
				{
					violationWithMinimum = true;
				}
			}
			else if (course.MinimumAmount.HasValue
					&& course.MinimumAmount > this.Items.Subtotal)
			{
				errors.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_NON_FIXED)
						.Replace("@@ 1 @@", CurrencyManager.ToPrice(course.MinimumAmount)));
				violationWithMinimum = true;
			}
			else if (course.MaximumAmount.HasValue
					&& course.MaximumAmount < this.Items.Subtotal)
			{
				errors.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_NON_FIXED)
						.Replace("@@ 1 @@", CurrencyManager.ToPrice(course.MaximumAmount)));
			}

			// 必須商品のチェック
			if (course.GetDefaultOrderProduct(criteria, subscriptionBoxOrderCount)
				.Where(defaultOrderProduct => defaultOrderProduct.IsNecessary)
				.Any(
					necessaryProduct => this.Items
						.Where(product => product.IsSelected)
						.FirstOrDefault(
							selectedProduct => (selectedProduct.VariationId == necessaryProduct.VariationId)
								&& (selectedProduct.Quantity < necessaryProduct.ItemQuantity)) != null))
			{
				errors.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_CONTAIN_NECESSARY_PRODUCTS));
				violationWithMinimum = true;
			}

			return errors.ToArray();
		}

		/// <summary>一覧</summary>
		public SubscriptionBoxNextShippingProductInputItemList Items { get; set; }
		/// <summary>定額頒布会か？</summary>
		public bool IsFixedAmount { get; set; }
	}

	/// <summary>
	/// 頒布会次回お届け商品入力クラス 商品アイテム
	/// </summary>
	[Serializable]
	public class SubscriptionBoxNextShippingProductInputItem : IModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dataSource">データソース</param>
		private SubscriptionBoxNextShippingProductInputItem(Hashtable dataSource)
		{
			this.DataSource = dataSource;
		}

		/// <summary>
		/// コンテナから作成
		/// </summary>
		/// <param name="course">コースモデル</param>
		/// <param name="subscriptionBoxOrderCount">頒布会注文回数</param>
		/// <param name="container">コンテナ</param>
		/// <param name="preserveSelection">選択中の項目を保持</param>
		/// <param name="criteria">基準日時</param>
		/// <returns>インスタンス</returns>
		public static SubscriptionBoxNextShippingProductInputItem CreateFromDomainContainer(
			SubscriptionBoxModel course,
			int subscriptionBoxOrderCount,
			SubscriptionBoxItemContainer container,
			bool preserveSelection,
			DateTime criteria)
		{
			var result = new SubscriptionBoxNextShippingProductInputItem(container.DataSource)
			{
				IsAppliedCampaignPrice = false,
				IsSelectable = true,
				IsSelected = preserveSelection && container.Quantity >= 1,
				ProductName = ProductCommon.CreateProductJointName(
					container.Name,
					container.ProductVariationName1,
					container.ProductVariationName2,
					container.ProductVariationName3)
			};

			result.IsNecessaryItem = course.GetDefaultOrderProduct(criteria, subscriptionBoxOrderCount)
				.Any(dop => dop.IsNecessary && (dop.VariationId == result.VariationId));

			if (result.IsNecessaryItem)
			{
				result.IsSelected = true;
			}

			if (result.IsSelected == false)
			{
				result.Quantity = 0;
			}

			result.FixedPurchasePrice = container.GetPrice(criteria);
			result.IsAppliedCampaignPrice = container.CanApplyCampaignPrice(criteria);

			return result;
		}

		// ProductCommon.GetKeyValueに対応させるため、IModelを実装し、内部にHashtableのDataSourceを持つ。
		// w2c:ProductImageコントロールで使用している。

		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID] = value; }
		}
		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>定期購入価格</summary>
		public decimal FixedPurchasePrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE] = value; }
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
		/// <summary>商品画像ヘッダ</summary>
		public string ProductImageHeader
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>バリエーションイメージヘッダ</summary>
		public string VariationImageHeader
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] = value; }
		}
		/// <summary>商品数量</summary>
		public int Quantity
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>キャンペーン期間（開始）</summary>
		public DateTime? CampaignSince
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE] = value; }
		}
		/// <summary>キャンペーン期間（終了）</summary>
		public DateTime? CampaignUntil
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL] = value; }
		}
		/// <summary>選択可能か？</summary>
		public bool IsSelectable { get; set; }
		/// <summary>選択済みか？</summary>
		public bool IsSelected { get; set; }
		/// <summary>バリエーション管理商品か？</summary>
		public bool IsVariation
		{
			get { return this.ProductId.Length != this.VariationId.Length; }
		}
		/// <summary>キャンペーン価格が適用されているか</summary>
		public bool IsAppliedCampaignPrice { get; set; }
		/// <summary>必須商品か？</summary>
		public bool IsNecessaryItem { get; set; }
		/// <summary>データソース</summary>
		public Hashtable DataSource { get; set; }
	}

	/// <summary>
	/// 頒布会次回お届け商品入力クラス 商品アイテムリスト
	/// </summary>
	[Serializable]
	public class SubscriptionBoxNextShippingProductInputItemList : IEnumerable<SubscriptionBoxNextShippingProductInputItem>
	{
		private readonly List<SubscriptionBoxNextShippingProductInputItem> _list;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="list">リスト</param>
		public SubscriptionBoxNextShippingProductInputItemList(IEnumerable<SubscriptionBoxNextShippingProductInputItem> list)
		{
			_list = list.ToList();
		}

		/// <summary>
		/// インデクサ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>アイテム</returns>
		public SubscriptionBoxNextShippingProductInputItem this[string shopId, string productId, string variationId]
		{
			get
			{
				return this.FirstOrDefault(
					i => (i.ShopId == shopId)
						&& (i.ProductId == productId)
						&& (i.VariationId == variationId));
			}
		}
		
		/// <summary>
		/// GetEnumerator
		/// </summary>
		/// <returns>Enumerator</returns>
		public IEnumerator<SubscriptionBoxNextShippingProductInputItem> GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		/// <summary>
		/// GetEnumerator
		/// </summary>
		/// <returns>Enumerator</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>合計数量</summary>
		public int TotalQuantity
		{
			get { return this.Where(i => i.IsSelected).Sum(i => i.Quantity); }
		}
		/// <summary>選択済み商品数</summary>
		public int TotalSelectedItemCount
		{
			get { return this.Count(i => i.IsSelected); }
		}
		/// <summary>商品小計</summary>
		public decimal Subtotal
		{
			get { return this.Where(i => i.IsSelected).Sum(i => i.FixedPurchasePrice * i.Quantity); }
		}
	}
}
