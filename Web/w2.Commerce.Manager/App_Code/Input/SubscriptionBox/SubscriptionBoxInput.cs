/*
=========================================================================================================
  Module      : 頒布会コース入力クラス (SubscriptionBoxInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Cs.ErrorPoint;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Common.Extensions;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;

namespace Input.SubscriptionBox
{
	/// <summary>
	/// 頒布会コース入力クラス
	/// </summary>
	[Serializable]
	public class SubscriptionBoxInput : InputBase<SubscriptionBoxModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SubscriptionBoxInput()
		{
			this.OrderItemDeterminationType = Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME;
			this.DefaultItems = new List<SubscriptionBoxDefaultItemInput>();
			this.Items = new List<SubscriptionBoxItemInput>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public SubscriptionBoxInput(SubscriptionBoxModel model)
			: this()
		{
			this.CourseId = model.CourseId;
			this.ManagementName = model.ManagementName;
			this.DisplayName = model.DisplayName;
			this.AutoRenewal = model.AutoRenewal;
			this.ItemsChangeableByUser = model.ItemsChangeableByUser;
			this.OrderItemDeterminationType = model.OrderItemDeterminationType;
			this.MinimumPurchaseQuantity = (model.MinimumPurchaseQuantity != null)
				? model.MinimumPurchaseQuantity.ToString()
				: null;
			this.MaximumPurchaseQuantity = (model.MaximumPurchaseQuantity != null)
				? StringUtility.ToEmpty(model.MaximumPurchaseQuantity)
				: null;
			this.MinimumNumberOfProducts = (model.MinimumNumberOfProducts != null)
				? StringUtility.ToEmpty(model.MinimumNumberOfProducts)
				: null;
			this.MaximumNumberOfProducts = (model.MaximumNumberOfProducts != null)
				? StringUtility.ToEmpty(model.MaximumNumberOfProducts)
				: null;
			this.ValidFlg = model.ValidFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.FixedAmountFlg = model.FixedAmountFlg;
			this.FixedAmount = (model.FixedAmount != null)
				? model.FixedAmount.ToString()
				: null;
			this.TaxCategoryId = model.TaxCategoryId;
			this.DisplayPriority = model.DisplayPriority.ToString();
			this.MinimumAmount = (model.MinimumAmount != null)
				? model.MinimumAmount.ToString()
				: null;
			this.MaximumAmount = (model.MaximumAmount != null)
				? model.MaximumAmount.ToString()
				: null;
			this.IndefinitePeriodFlg = model.IndefinitePeriod;

			this.Items = model.SelectableProducts
				.Select(i => new SubscriptionBoxItemInput(i))
				.ToList();

			// 商品名を取得
			foreach (var item in this.Items
				.GroupBy(dop => new
				{
					ShopId = dop.ShopId,
					ProductId = dop.ProductId,
					VariationId = dop.VariationId,
				}))
			{
				var product = new ProductService().GetProductVariationAtDataRowView(
					item.Key.ShopId,
					item.Key.ProductId,
					item.Key.VariationId,
					"");

				var productName = ProductCommon.CreateProductJointName(product);
				foreach (var item2 in this.Items
					.Where(dop => (dop.ShopId == item.Key.ShopId)
						&& (dop.ProductId == item.Key.ProductId)
						&& (dop.VariationId == item.Key.VariationId)))
				{
					item2.ProductName = productName;
					item2.ShippingType = (string)ProductPage.GetKeyValue(product, Constants.FIELD_PRODUCT_SHIPPING_TYPE);
				}
			}

			this.DefaultItems = SubscriptionBoxDefaultItemInput.CreateFromModel(model.DefaultOrderProducts, model.OrderItemDeterminationType)
				.OrderBy(defaultItem => int.Parse(defaultItem.Count))
				.ToList();
			this.FirstSelectableFlg = model.FirstSelectableFlg;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override SubscriptionBoxModel CreateModel()
		{
			var model = new SubscriptionBoxModel
			{
				CourseId = this.CourseId,
				ManagementName = this.ManagementName,
				DisplayName = this.DisplayName,
				AutoRenewal = this.AutoRenewal,
				ItemsChangeableByUser = this.ItemsChangeableByUser,
				OrderItemDeterminationType = this.OrderItemDeterminationType,
				MinimumPurchaseQuantity = string.IsNullOrEmpty(this.MinimumPurchaseQuantity)
					? (int?)null
					: int.Parse(this.MinimumPurchaseQuantity),
				MaximumPurchaseQuantity = string.IsNullOrEmpty(this.MaximumPurchaseQuantity)
					? (int?)null
					: int.Parse(this.MaximumPurchaseQuantity),
				MinimumNumberOfProducts = string.IsNullOrEmpty(this.MinimumNumberOfProducts)
					? (int?)null
					: int.Parse(this.MinimumNumberOfProducts),
				MaximumNumberOfProducts = string.IsNullOrEmpty(this.MaximumNumberOfProducts)
					? (int?)null
					: int.Parse(this.MaximumNumberOfProducts),
				ValidFlg = this.ValidFlg,
				LastChanged = this.LastChanged,
				FixedAmountFlg = this.FixedAmountFlg,
				FixedAmount = string.IsNullOrEmpty(this.FixedAmount)
					? (decimal?)null
					: decimal.Parse(this.FixedAmount),
				TaxCategoryId = this.TaxCategoryId,
				DisplayPriority = int.Parse(this.DisplayPriority),
				DefaultOrderProducts = SubscriptionBoxDefaultItemInput.CreateModel(
					this.DefaultItems.ToArray(),
					this.OrderItemDeterminationType),
				SelectableProducts = this.Items.Select(i => i.CreateModel()).ToArray(),
				MinimumAmount = string.IsNullOrEmpty(this.MinimumAmount)
					? (decimal?)null
					: decimal.Parse(this.MinimumAmount),
				MaximumAmount = string.IsNullOrEmpty(this.MaximumAmount)
					? (decimal?)null
					: decimal.Parse(this.MaximumAmount),
				FirstSelectableFlg = this.FirstSelectableFlg,
				IndefinitePeriod = this.IndefinitePeriodFlg,
			};

			// 0はNULLに補正
			model.MinimumPurchaseQuantity
				= ((model.MinimumPurchaseQuantity.HasValue) && (model.MinimumPurchaseQuantity.Value == 0))
					? null
					: model.MinimumPurchaseQuantity;
			model.MaximumPurchaseQuantity
				= ((model.MaximumPurchaseQuantity.HasValue) && (model.MaximumPurchaseQuantity.Value == 0))
					? null
					: model.MaximumPurchaseQuantity;
			model.FixedAmount = ((model.FixedAmount.HasValue) && (model.FixedAmount.Value == 0))
				? null
				: model.FixedAmount;
			model.MinimumAmount = ((model.MinimumAmount.HasValue) && (model.MinimumAmount.Value == 0))
				? null
				: model.MinimumAmount;
			model.MaximumAmount = ((model.MaximumAmount.HasValue) && (model.MaximumAmount.Value == 0))
				? null
				: model.MaximumAmount;
			return model;
		}

		/// <summary>
		/// デフォルト注文商品の再採番
		/// </summary>
		public void RenumberDefaultItems()
		{
			var branchNo = 0;
			var count = 1;
			foreach (var defaultItem in this.DefaultItems)
			{
				defaultItem.Count = (count++).ToString();
				foreach (var subItem in defaultItem.Items)
				{
					subItem.BranchNo = (++branchNo).ToString();
				}
			}
		}

		/// <summary>
		/// 選択可能商品の再採番
		/// </summary>
		public void RenumberItems()
		{
			var branchNo = 0;
			foreach (var item in this.Items)
			{
				item.BranchNo = (++branchNo).ToString();
			}
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var errors = new List<string>();
			errors.Add(Validator.Validate("SubscriptionBox", this.DataSource));
			errors.AddRange(this.Items.Select(i => i.Validate()));
			errors.AddRange(this.DefaultItems.Select(i => i.Validate(this.Items, shouldValidatePeriods: this.IsOrderDeterminationTypePeriod)));

			errors = errors.Where(e => (string.IsNullOrWhiteSpace(e) == false)).ToList();

			// 選択可能商品とデフォルト注文商品がが1件以上？
			if (errors.Any() == false)
			{
				if ((this.Items.Any() == false)
					|| (this.DefaultItems.Any() == false))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_NULL));
				}
			}

			// デフォルト設定の日付が入力されているのか
			if ((errors.Any() == false)
				&& (this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD))
			{
				if (this.DefaultItems.Any(item => (string.IsNullOrWhiteSpace(item.TermSince) || string.IsNullOrWhiteSpace(item.TermUntil))))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_DEFAULT_ORDER_PRODUCT_TERM_EMPTY));
				}
			}

			// デフォルト設定の終了期間が開始期間より前になっていないか
			if ((errors.Any() == false)
				&& (this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD))
			{
				if (this.DefaultItems.Any(item => (Validator.CheckDateRange(item.TermSince, item.TermUntil) == false)))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)
						.Replace("@@ 1 @@", Constants.TAG_REPLACER_DATA_SCHEMA
							.GetValue("@@DispText.subscription_box.subscription_box_default_item_since@@")));
				}
			}

			// デフォルト設定の日付が連日になっているか
			if ((errors.Any() == false)
				&& (this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD))
			{
				var defaultItem = this.DefaultItems.OrderBy(a => a.TermSince).ToList();
				foreach (var item in defaultItem.Select((x, i) => new { Index = i, Value = x }))
				{
					if ((item.Index == defaultItem.Count - 1)) continue;
					var term = this.DefaultItems.Any(a => DateTime.Parse(a.TermSince)
						.Equals(DateTime.Parse(item.Value.TermUntil).AddDays(1)));

					if (term) continue;
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PERIOD_NOT_FLUENT));
				}
			}

			// デフォルト注文商品が選択されているか
			if (errors.Any() == false)
			{
				if (this.DefaultItems
					.SelectMany(di => di.Items)
					.Any(di => string.IsNullOrEmpty(di.ShopId + di.ProductId + di.VariationId)))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_DEFAULT_ORDER_PRODUCT_EMPTY));
				}
			}

			// "期間別 デフォルト注文商品" 設定の日付が、その商品の "選択可能期間" 内かどうか
			if ((errors.Any() == false)
				&& (this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD))
			{
				// ネストが深いがデバッグのしやすさを重視
				foreach (var defualtItem in this.DefaultItems)
				{
					foreach (var defaultItemProduct in defualtItem.Items)
					{
						foreach (var item in this.Items)
						{
							if (item.VariationId != defaultItemProduct.VariationId) continue;

							var isValidTermValue = true;
							if (string.IsNullOrWhiteSpace(item.SelectableSince) == false)
							{
								// "期間別 デフォルト注文商品"の始まりの"期間"が、"選択可能商品"の始まりの"選択可能期間"以後であれば検証パス
								if (DateTime.Parse(defualtItem.TermSince) < DateTime.Parse(item.SelectableSince))
								{
									isValidTermValue = false;
								}
							}

							if (string.IsNullOrWhiteSpace(item.SelectableUntil) == false)
							{
								// "期間別 デフォルト注文商品"の終わりの"期間"が、"選択可能商品"の終わりの"選択可能期間"以前であれば検証パス
								if (DateTime.Parse(defualtItem.TermUntil) > DateTime.Parse(item.SelectableUntil))
								{
									isValidTermValue = false;
								}
							}

							if (isValidTermValue) continue;
							var errMsg = string.Format(
								WebMessages.GetMessages(
									WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_DEFAULT_ORDER_PRODUCT_UNAVAILABLE_TERM),
								item.ProductName);
							errors.Add(errMsg);
						}
					}
				}
			}

			// 配送種別すべて同一かチェック
			if (errors.Any() == false)
			{
				if (this.Items.Select(i => new { i.ShippingType }).Distinct().Count() >= 2)
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_SHIPPING_TYPE_MUST_BE_SAME));
				}
			}

			// 最低＜最大購入数量になってるかチェック
			if (errors.Any() == false)
			{
				int tmp;
				var minimumPurchaseQuantity = int.TryParse(this.MinimumPurchaseQuantity, out tmp)
					? tmp
					: int.MinValue;
				var maximumPurchaseQuantity = int.TryParse(this.MaximumPurchaseQuantity, out tmp)
					? tmp
					: int.MaxValue;

				if ((minimumPurchaseQuantity <= maximumPurchaseQuantity) == false)
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MIN_LARGER_MAX_ERROR));
				}
			}

			// 最低＜最大購入種類数になっているかチェック
			if (errors.Any() == false)
			{
				int tmp;
				var minimumNumberOfProducts = int.TryParse(this.MinimumNumberOfProducts, out tmp)
					? tmp
					: int.MinValue;
				var maximumNumberOfProducts = int.TryParse(this.MaximumNumberOfProducts, out tmp)
					? tmp
					: int.MaxValue;

				if (maximumNumberOfProducts < minimumNumberOfProducts)
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCTS_MAX_ERROR));
				}
			}

			//最低種類数＞最大数量、最低数量 > 
			if (errors.Any() == false)
			{
				int tmp;
				var minimumNumberOfProducts = int.TryParse(this.MinimumNumberOfProducts, out tmp)
					? tmp
					: int.MinValue;
				var maximumPurchaseQuantity = int.TryParse(this.MaximumPurchaseQuantity, out tmp)
					? tmp
					: int.MaxValue;
				if (minimumNumberOfProducts > maximumPurchaseQuantity)
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCT_EXCEEDS_MAX_QUANTITY));
				}
			}

			// デフォルト注文商品に、最低/最大購入数量を違反するものがないかチェック
			if ((errors.Any() == false)
				&& ((this.FirstSelectableFlg == Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_FALSE)
					|| (this.DefaultItems.Count > 1)))
			{
				// 初回選択画面が有効の場合1回目のデフォルト商品をスキップ
				var skipNumber = this.FirstSelectableFlg == Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_TRUE ? 1 : 0;
				var max = this.DefaultItems
					.Skip(skipNumber)
					.Select((defaultItem, index) => defaultItem.Items)
					.Select(items => items.Sum(item => int.Parse(item.ItemQuantity)))
					.Max();
				var min = this.DefaultItems
					.Skip(skipNumber)
					.Select(defaultItem => defaultItem.Items)
					.Select(items => items.Sum(item => int.Parse(item.ItemQuantity)))
					.Min();

				int tmp;
				var minimumPurchaseQuantity = int.TryParse(this.MinimumPurchaseQuantity, out tmp)
					? tmp
					: (int?)null;
				var maximumPurchaseQuantity = int.TryParse(this.MaximumPurchaseQuantity, out tmp)
					? tmp
					: (int?)null;

				if ((maximumPurchaseQuantity.HasValue && maximumPurchaseQuantity.Value < max)
					|| (minimumPurchaseQuantity.HasValue && minimumPurchaseQuantity.Value > min))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_QUANTITY_ERROR));
				}
			}

			// 同一注文で同じ商品購入がないかチェック
			if (errors.Any() == false)
			{
				var duplicated = this.DefaultItems.Any(di => di.Items.GroupBy(dii => dii.VariationId).Any(g => g.Count() >= 2));
				if (duplicated)
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_DUPLICATE_ERROR));
				}
			}

			// デフォルト注文に最低/最大購入種類数を違反するものがないか
			if (errors.Any() == false)
			{
				var max = this.DefaultItems.Select(defaultItem => defaultItem.Items).Max(item => item.Count);
				var min = this.DefaultItems.Select(defaultItem => defaultItem.Items).Min(item => item.Count);

				int tmp;
				var minimumNumberOfProducts = int.TryParse(this.MinimumNumberOfProducts, out tmp)
					? tmp
					: (int?)null;
				var maximumNumberOfProducts = int.TryParse(this.MaximumNumberOfProducts, out tmp)
					? tmp
					: (int?)null;

				if ((minimumNumberOfProducts.HasValue && (minimumNumberOfProducts.Value > min))
					|| (maximumNumberOfProducts.HasValue && (maximumNumberOfProducts.Value < max)))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_NUMBER_OF_PRODUCTS_ERROR));
				}

				// 2回目以降で必須商品設定がある場合は、必須商品有効の数でチェック
				if ((errors.Any() == false)
					&& (this.DefaultItems.Count >= 2)
					&& (this.DefaultItems.Any(
						item => item.Items.Any(
							subItem => subItem.IsNecessary))))
				{
					var afterSecondMax = this.DefaultItems
						.Skip(1)
						.Select(defaultItem => defaultItem.Items)
						.Max(item => item.Count(subItem => subItem.IsNecessary));
					var afterSecondMin = this.DefaultItems
						.Skip(1)
						.Select(defaultItem => defaultItem.Items)
						.Min(item => item.Count(subItem => subItem.IsNecessary));

					if ((minimumNumberOfProducts.HasValue && (minimumNumberOfProducts.Value > afterSecondMin))
						|| (maximumNumberOfProducts.HasValue && (maximumNumberOfProducts.Value < afterSecondMax)))
					{
						errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_NUMBER_OF_PRODUCTS_ERROR));
					}
				}
			}

			// 定額機能がONなのに定額が指定されていない
			if (errors.Any() == false)
			{
				if (this.IsFixedAmount && string.IsNullOrEmpty(this.FixedAmount))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_NOT_SETTING_AMOUNT));
				}
			}

			// 商品合計金額下限（税込）と商品合計金額上限（税込）の入力値が不適切
			if (errors.Any() == false)
			{
				decimal minimumAmount = 0;
				decimal maximumAmount = 0;

				// 商品合計金額下限（税込）の入力値が半角数値でない
				if ((string.IsNullOrEmpty(this.MinimumAmount) == false) && (decimal.TryParse(this.MinimumAmount, out minimumAmount) == false))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_WRONG_SETTING_MINIMUM_AMOUNT));
				}

				// 商品合計金額上限（税込）の入力値が半角数値でない
				if ((errors.Any() == false) && (string.IsNullOrEmpty(this.MaximumAmount) == false) && (decimal.TryParse(this.MaximumAmount, out maximumAmount) == false))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_WRONG_SETTING_MAXIMUM_AMOUNT));
				}

				// 下限金額が上限金額を上回っている
				if ((errors.Any() == false) && (string.IsNullOrEmpty(this.MaximumAmount) == false) && (minimumAmount > maximumAmount))
				{
					errors.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MINIMUM_AMOUNT_IS_GREATER_THAN_MAXIMUM_AMOUNT));
				}
			}

			// キャンペーン期間が指定されていて対象商品のキャンペーン価格と定期初回価格に値が存在しない、
			if (errors.Any() == false)
			{
				var productIdList = new List<string>();
				foreach (var item in this.Items)
				{
					// キャンペーン期間が指定されていて対象商品のキャンペーン価格が存在しない場合
					if (string.IsNullOrEmpty(item.CampaignPrice)
						&& (string.IsNullOrEmpty(item.CampaignSince) == false)
						&& (string.IsNullOrEmpty(item.CampaignUntil) == false))
					{
						var product = new ProductService();
						bool isError = false;

						// 該当商品の定期購入初回価格が存在しない場合エラーと判定
						if (string.IsNullOrEmpty(item.VariationId) == false)
						{
							isError = (product.GetProductVariation(item.ShopId, item.ProductId, item.VariationId, string.Empty).VariationFixedPurchaseFirsttimePrice == null);
						}
						else if (string.IsNullOrEmpty(item.ProductId) == false)
						{
							isError = (product.Get(item.ShopId, item.ProductId).FixedPurchaseFirsttimePrice == null);
						}

						if (isError == false) continue;

						errors.Add(WebMessages
							.GetMessages(
								WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_WRONG_SETTING_CAMPAIGN_PRICE_AND_FIXED_PURCHASE_FIRSTTIME_PRICE_NULL,
								item.ProductId));
					}
				}
			}

			// TODO: 項目矛盾のチェックはまだまだあるがキリガないのでまた後ほど

			var result = errors.Where(e => string.IsNullOrWhiteSpace(e) == false).JoinToString(Environment.NewLine);
			return result;
		}

		/// <summary>
		/// 頒布会初回選択画面フラグが有効か
		/// </summary>
		/// <param name="isCheckedFirstSelectableFlg">頒布会初回選択画面のチェックボックスがチェック済みか</param>
		/// <returns>true：有効,false：無効</returns>
		public bool CheckFirstSelectableFlg(bool isCheckedFirstSelectableFlg)
		{
			return isCheckedFirstSelectableFlg && this.IsOrderDeterminationTypeNumberTime;
		}
		#endregion

		#region プロパティ
		/// <summary>頒布会コースID</summary>
		public string CourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会管理名</summary>
		public string ManagementName
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME] = value; }
		}
		/// <summary>頒布会表示名</summary>
		public string DisplayName
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME] = value; }
		}
		/// <summary>自動更新</summary>
		public string AutoRenewal
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_AUTO_RENEWAL]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_AUTO_RENEWAL] = value; }
		}
		/// <summary>フロント商品変更可否</summary>
		public string ItemsChangeableByUser
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER] = value; }
		}
		/// <summary>商品決定方法</summary>
		public string OrderItemDeterminationType
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE] = value; }
		}
		/// <summary>最低購入数量</summary>
		public string MinimumPurchaseQuantity
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_PURCHASE_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_PURCHASE_QUANTITY] = value; }
		}
		/// <summary>最大購入数量</summary>
		public string MaximumPurchaseQuantity
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_PURCHASE_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_PURCHASE_QUANTITY] = value; }
		}
		/// <summary>最低購入種類数</summary>
		public string MinimumNumberOfProducts
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_NUMBER_OF_PRODUCTS]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_NUMBER_OF_PRODUCTS] = value; }
		}
		/// <summary>最大購入種類数</summary>
		public string MaximumNumberOfProducts
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_NUMBER_OF_PRODUCTS]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_NUMBER_OF_PRODUCTS] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_LAST_CHANGED] = value; }
		}
		/// <summary>定額設定</summary>
		public string FixedAmountFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT_FLG] = value; }
		}
		/// <summary>定額価格</summary>
		public string FixedAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT] = value; }
		}
		/// <summary>税率カテゴリID</summary>
		public string TaxCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_TAX_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_TAX_CATEGORY_ID] = value; }
		}
		/// <summary>優先順位</summary>
		public string DisplayPriority
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_PRIORITY] = value; }
		}
		/// <summary>商品合計金額下限（税込）</summary>
		public string MinimumAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_AMOUNT] = value; }
		}
		/// <summary>商品合計金額上限（税込）</summary>
		public string MaximumAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_AMOUNT] = value; }
		}
		/// <summary>頒布会初回選択画面フラグ</summary>
		public string FirstSelectableFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIRST_SELECTABLE_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIRST_SELECTABLE_FLG] = value; }
		}
		/// <summary>無期限設定フラグ</summary>
		public string IndefinitePeriodFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FLG] = value; }
		}
		#endregion

		#region Exended properties
		/// <summary>デフォルト注文商品</summary>
		public List<SubscriptionBoxDefaultItemInput> DefaultItems { get; set; }
		/// <summary>選択可能商品</summary>
		public List<SubscriptionBoxItemInput> Items { get; set; }
		/// <summary>商品決定方法が「期間」</summary>
		public bool IsOrderDeterminationTypePeriod
		{
			get { return this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD; }
		}
		/// <summary>商品決定方法が「回数」</summary>
		public bool IsOrderDeterminationTypeNumberTime
		{
			get { return this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME; }
		}
		/// <summary>有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_SUBSCRIPTIONBOX_VALID_TRUE); }
			set
			{
				this.ValidFlg = value
					? Constants.FLG_SUBSCRIPTIONBOX_VALID_TRUE
					: Constants.FLG_SUBSCRIPTIONBOX_VALID_FALSE;
			}
		}
		/// <summary>自動更新が有効？</summary>
		public bool IsAutoRenewal
		{
			get { return this.AutoRenewal == Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_TRUE; }
			set
			{
				this.AutoRenewal = value
					? Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_TRUE
					: Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_FALSE;
			}
		}
		/// <summary>エンドユーザーによる商品変更が可能か</summary>
		public bool AreItemsChangeableByUser
		{
			get { return (this.ItemsChangeableByUser == Constants.FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_TRUE); }
			set
			{
				this.ItemsChangeableByUser = value
					? Constants.FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_TRUE
					: Constants.FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_FALSE;
			}
		}
		/// <summary>定額か</summary>
		public bool IsFixedAmount
		{
			get { return (this.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE); }
			set
			{
				this.FixedAmountFlg = value
					? Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE
					: Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE;
			}
		}
		/// <summary>頒布会初回選択画面を使うか</summary>
		public bool IsUsingFirstSelectablePage
		{
			get
			{
				return (this.FirstSelectableFlg == Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_TRUE)
					&& this.IsOrderDeterminationTypeNumberTime;
			}
		}
		/// <summary>最終商品の無限繰り返しか</summary>
		public bool IsIndefinitePeriod
		{
			get { return this.IndefinitePeriodFlg == Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_TRUE; }
			set
			{
				this.IndefinitePeriodFlg = value
					? Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_TRUE
					: Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FALSE;
			}
		}
		#endregion
	}
}
