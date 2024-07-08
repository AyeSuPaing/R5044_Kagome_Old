/*
=========================================================================================================
  Module      : 商品同梱設定編集画面入力クラス (ProductBundleInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Domain.AdvCode;
using w2.Domain.Coupon;
using w2.Domain.ProductBundle;
using w2.Domain.ProductCategory;
using w2.Domain.TargetList;

/// <summary>
/// 商品同梱設定編集画面入力クラス
/// </summary>
public class ProductBundleInput : InputBase<ProductBundleModel>
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ProductBundleInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductBundleInput(ProductBundleModel model)
		: this()
	{
		this.ProductBundleId = model.ProductBundleId;
		this.ProductBundleName = model.ProductBundleName;
		this.TargetOrderType = model.TargetOrderType;
		this.Description = model.Description;
		this.StartDatetime = model.StartDatetime.ToString();
		this.EndDatetime = (model.EndDatetime != null) ? model.EndDatetime.ToString() : null;
		this.TargetProductKbn = model.TargetProductKbn;
		this.TargetProductIds = model.TargetProductIds;
		this.TargetOrderFixedPurchaseCountFrom = model.TargetOrderFixedPurchaseCountFrom.HasValue
			? model.TargetOrderFixedPurchaseCountFrom.ToString()
			: string.Empty;
		this.TargetOrderFixedPurchaseCountTo = model.TargetOrderFixedPurchaseCountTo.HasValue
			? model.TargetOrderFixedPurchaseCountTo.ToString()
			: string.Empty;
		this.UsableTimesKbn = model.UsableTimesKbn;
		this.UsableTimes = model.UsableTimes.HasValue ? model.UsableTimes.ToString() : null;
		this.ApplyType = model.ApplyType;
		this.ValidFlg = model.ValidFlg;
		this.MultipleApplyFlg = model.MultipleApplyFlg;
		this.ApplyOrder = model.ApplyOrder.ToString();
		this.LastChanged = model.LastChanged;
		this.BundleItems = model.Items.Select(item => new ProductBundleItemInput(item)).ToArray();
		this.TargetProductCategoryIds = model.TargetProductCategoryIds;
		this.ExceptProductIds = model.ExceptProductIds;
		this.ExceptProductCategoryIds = model.ExceptProductCategoryIds;
		this.TargetId = model.TargetId;
		this.TargetIdExceptFlg =
			(model.TargetIdExceptFlg == Constants.FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_EXCEPT);
		this.TargetOrderPriceSubtotalMin = model.TargetOrderPriceSubtotalMin.HasValue ? model.TargetOrderPriceSubtotalMin.ToString() : null;
		this.TargetProductCountMin = model.TargetProductCountMin.HasValue ? model.TargetProductCountMin.ToString() : null;
		this.TargetAdvCodesFirst = model.TargetAdvCodesFirst;
		this.TargetAdvCodesNew = model.TargetAdvCodesNew;
		this.TargetPaymentIds = model.TargetPaymentIds;
	}
	#endregion

	#region +SetStartDatetime 開始日時入力
	/// <summary>
	/// 開始日時入力
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// <param name="day">日</param>
	/// <param name="hour">時</param>
	/// <param name="minute">分</param>
	/// <param name="second">秒</param>
	public void SetStartDatetime(string year, string month, string day, string hour, string minute, string second)
	{
		this.StartDatetime = CreateDatetimeString(year, month, day, hour, minute, second);
	}
	#endregion

	#region +SetEndDatetime 終了日時入力
	/// <summary>
	/// 終了日時入力
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// <param name="day">日</param>
	/// <param name="hour">時</param>
	/// <param name="minute">分</param>
	/// <param name="second">秒</param>
	public void SetEndDatetime(string year, string month, string day, string hour, string minute, string second)
	{
		this.EndDatetime = CreateDatetimeString(year, month, day, hour, minute, second);
	}
	#endregion

	#region -CreateDatetimeString 日時文字列生成
	/// <summary>
	/// 日時文字列生成
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// <param name="day">日</param>
	/// <param name="hour">時</param>
	/// <param name="minute">分</param>
	/// <param name="second">秒</param>
	/// <returns>日時文字列</returns>
	private string CreateDatetimeString(string year, string month, string day, string hour, string minute, string second)
	{
		var datetimeString = string.Empty;
		if ((string.IsNullOrEmpty(year) == false)
			|| (string.IsNullOrEmpty(month) == false)
			|| (string.IsNullOrEmpty(day) == false)
			|| (string.IsNullOrEmpty(hour) == false)
			|| (string.IsNullOrEmpty(minute) == false)
			|| (string.IsNullOrEmpty(second) == false))
		{
			datetimeString = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
				year,
				month,
				day,
				hour,
				minute,
				second);
		}
		return datetimeString;
	}
	#endregion

	#region モデル生成
	#region +CreateModel モデル生成
	/// <summary>
	/// モデル生成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductBundleModel CreateModel()
	{
		var model = new ProductBundleModel
		{
			ProductBundleId = this.ProductBundleId,
			ProductBundleName = this.ProductBundleName,
			TargetOrderType = this.TargetOrderType,
			Description = this.Description,
			StartDatetime = DateTime.Parse(this.StartDatetime),
			EndDatetime = (string.IsNullOrEmpty(this.EndDatetime) == false) ? DateTime.Parse(this.EndDatetime) : (DateTime?)null,
			TargetProductKbn = this.TargetProductKbn,
			TargetOrderFixedPurchaseCountFrom = ((this.TargetOrderType != Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL)
				&& (string.IsNullOrEmpty(this.TargetOrderFixedPurchaseCountFrom) == false))
				? int.Parse(this.TargetOrderFixedPurchaseCountFrom)
					: (int?)null,
			TargetOrderFixedPurchaseCountTo = ((this.TargetOrderType != Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL)
				&& (string.IsNullOrEmpty(this.TargetOrderFixedPurchaseCountTo) == false))
				? int.Parse(this.TargetOrderFixedPurchaseCountTo)
					: (int?)null,
			UsableTimesKbn = this.UsableTimesKbn,
			UsableTimes =
				(string.IsNullOrEmpty(this.UsableTimes) == false) ? int.Parse(this.UsableTimes) : (int?)null,
			ApplyType = this.ApplyType,
			ValidFlg = this.ValidFlg,
			MultipleApplyFlg = this.MultipleApplyFlg,
			ApplyOrder = int.Parse(this.ApplyOrder),
			LastChanged = this.LastChanged,
			Items = CreateItemModel().ToArray(),
			TargetProductCategoryIds = this.TargetProductCategoryIds,
			ExceptProductCategoryIds = this.ExceptProductCategoryIds,
			TargetId = this.TargetId,
			TargetIdExceptFlg = this.TargetIdExceptFlg
				? Constants.FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_EXCEPT
				: Constants.FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_TARGET,
			TargetOrderPriceSubtotalMin =
				(string.IsNullOrEmpty(this.TargetOrderPriceSubtotalMin) == false)
					? decimal.Parse(this.TargetOrderPriceSubtotalMin)
					: (decimal?)null,
			TargetProductCountMin = (string.IsNullOrEmpty(this.TargetProductCountMin) == false)
				? int.Parse(this.TargetProductCountMin)
				: (int?)null,
			TargetAdvCodesFirst = this.TargetAdvCodesFirst,
			TargetAdvCodesNew = this.TargetAdvCodesNew,
			TargetPaymentIds = this.TargetPaymentIds,
			TargetCouponCodes = this.TargetCouponCodes,
		};

		var targetProductIds = new[] { this.TargetProductIds, this.TargetProductVariationIds }
			.Where(id => (string.IsNullOrEmpty(id) == false))
			.ToArray();
		model.TargetProductIds = string.Join(Environment.NewLine, targetProductIds);

		var exceptProductIds = new[] { this.ExceptProductIds, this.ExceptProductVariationIds }
			.Where(id => (string.IsNullOrEmpty(id) == false))
			.ToArray();
		model.ExceptProductIds = string.Join(Environment.NewLine, exceptProductIds);
		return model;
	}
	#endregion

	#region -CreateItemModel 同梱商品モデル生成
	/// <summary>
	/// 同梱商品モデル生成
	/// </summary>
	/// <returns>同梱商品モデル</returns>
	private IEnumerable<ProductBundleItemModel> CreateItemModel()
	{
		var itemModel = this.BundleItems.Select(item =>
			{
				var model = item.CreateModel();
				model.ProductBundleId = this.ProductBundleId;
				model.ProductBundleItemNo = Array.IndexOf(this.BundleItems, item) + 1;
				model.LastChanged = this.LastChanged;
				return model;
			});
		return itemModel;
	}
	#endregion
	#endregion

	#region バリデーション
	#region +Validate バリデーション
	/// <summary>
	/// バリデーション
	/// </summary>
	/// <param name="checkDuplication">重複チェックを行うか</param>
	/// <param name="loginOperatorDeptId">ログインオペレータ識別ID</param>
	/// <returns>エラーの際はエラーメッセージを返す</returns>
	public string Validate(bool checkDuplication, string loginOperatorDeptId = null)
	{
		var errorMessages = new List<string>();
		if (string.IsNullOrEmpty(this.ProductBundleId))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY).Replace("@@ 1 @@", "商品同梱ID"));
		}
		else if ((checkDuplication) && (new ProductBundleService().Get(this.ProductBundleId) != null))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", "商品同梱ID"));
		}

		if (string.IsNullOrEmpty(this.ProductBundleName))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY).Replace("@@ 1 @@", "商品同梱名"));
		}

		if (string.IsNullOrEmpty(this.ApplyOrder))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY).Replace("@@ 1 @@", "適用優先順"));
		}

		if (this.TargetOrderType != Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL)
		{
			errorMessages.Add(ValidateTargetOrderFixedPurchaseCount());
		}

		errorMessages.Add(ValidateStartDateEndDate());
		if (this.TargetProductKbn == Constants.FLG_PRODUCTBUNDLE_TARGET_PRODUCT_KBN_SELECT)
		{
			if ((string.IsNullOrEmpty(this.TargetProductIds))
				&& (string.IsNullOrEmpty(this.TargetProductVariationIds))
				&& (string.IsNullOrEmpty(this.TargetProductCategoryIds)))
			{
				errorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_CONDITION_SETTING_NOT_INPUT));
			}
			else
			{
				errorMessages.Add(ValidateProductId(this.TargetProductIdsList));
				errorMessages.Add(ValidateProductVariationId(this.TargetProductVariationIdsList));
				errorMessages.Add(ValidateProductCategoryId(this.TargetProductCategoryIdsList));
				if (errorMessages.Contains(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_SHIPPING_TYPE_NOT_SAME)) == false)
				{
					errorMessages.Add(ValidateShippingType());
				}
			}
		}

		errorMessages.Add(ValidateGrantProduct());

		// 対象外商品
		errorMessages.Add(ValidateProductId(this.ExceptProductIdsList, false));
		errorMessages.Add(ValidateProductVariationId(this.ExceptProductVariationIdsList, false));
		errorMessages.Add(ValidateProductCategoryId(this.ExceptProductCategoryIdsList, false));

		// ターゲットリストID
		errorMessages.Add(ValidateTargetList());

		// ユーザ利用可能回数
		if (this.IsNumSpecify && string.IsNullOrEmpty(this.UsableTimes))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY).Replace("@@ 1 @@", "ユーザ利用可能回数"));
		}
		errorMessages.Add(ValidateNumber(this.UsableTimes, "ユーザ利用可能回数"));

		// 対象注文の商品合計
		errorMessages.Add(Validator.CheckCurrency(
			"対象注文の商品合計",
			this.TargetOrderPriceSubtotalMin,
			Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId));

		// 対象商品個数
		errorMessages.Add(ValidateNumber(this.TargetProductCountMin, "対象商品個数"));

		// 初回広告コード
		errorMessages.Add(ValidateTargetAdvCodesFirst());

		// 最新広告コード
		errorMessages.Add(ValidateTargetAdvCodesNew());

		// クーポンコード
		errorMessages.Add(ValidateTargetCouponCodes(loginOperatorDeptId));

		// 適用優先順チェック
		int checkApplyOrder;
		if (((int.TryParse(this.ApplyOrder, out checkApplyOrder)) == false)
			|| (checkApplyOrder < 0))
		{
			errorMessages.Add(
				WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_APPLY_ORDER_INPUTCHECK_VALID_NUMBER));
		}

		// 同梱商品チェック
		foreach (var bundleItem in this.BundleItems)
		{
			int checkBundleItem;
			if (((int.TryParse(bundleItem.GrantProductCount, out checkBundleItem)) == false)
				|| (checkBundleItem < 0))
			{
				errorMessages.Add(
					WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_GRANT_PRODUCT_COUNT_INPUTCHECK_VALID_NUMBER)
						.Replace("@@ 1 @@", bundleItem.GrantProductId));
			}
		}

		return string.Join("<br/>", errorMessages.Where(message => string.IsNullOrEmpty(message) == false).ToArray());
	}
	#endregion

	#region -ValidateTargetOrderFixedPurchaseCount 対象定期注文回数の整合性チェック
	/// <summary>
	/// 対象定期注文回数の整合性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateTargetOrderFixedPurchaseCount()
	{
		var from = 0;
		var to = 0;
		var checkFrom = (string.IsNullOrEmpty(this.TargetOrderFixedPurchaseCountFrom) == false);
		var checkTo = (string.IsNullOrEmpty(this.TargetOrderFixedPurchaseCountTo) == false);

		if ((checkFrom && (int.TryParse(this.TargetOrderFixedPurchaseCountFrom, out from) == false))
			|| (checkTo && (int.TryParse(this.TargetOrderFixedPurchaseCountTo, out to) == false))
			|| (checkFrom && checkTo && (to < from)))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT);
		}
		return string.Empty;
	}
	#endregion

	#region -ValidateTerm 有効期間の整合性チェック
	/// <summary>
	/// 有効期間の整合性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateStartDateEndDate()
	{
		var start = DateTime.MinValue;
		var end = DateTime.MinValue;

		if (((DateTime.TryParse(this.StartDatetime, out start)) == false)
			|| ((DateTime.TryParse(this.EndDatetime, out end))
				&& (end <= start)))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_START_DATE_END_DATE);
		}
		return string.Empty;
	}
	#endregion

	#region -ValidateProductId 商品IDで指定された商品の整合性チェック
	/// <summary>
	/// 商品IDで指定された商品の整合性チェック
	/// </summary>
	/// <param name="productIdsList">商品IDリスト</param>
	/// <param name="isTargetProduct">対象商品か</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateProductId(string[] productIdsList, bool isTargetProduct = true)
	{
		if (productIdsList.Length == 0) return string.Empty;

		if (productIdsList.Any(targetId => targetId.IndexOf(',') >= 0))
		{
			return isTargetProduct
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_PRODUCT_INPUT_ERROR)
				: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_EXCLUDED_PRODUCT_INPUT_ERROR);
		}

		var products = ProductCommon.GetProductsInfo(Constants.CONST_DEFAULT_SHOP_ID, productIdsList);
		// 対象商品の場合のみ、配送種別を判定する
		if (isTargetProduct)
		{
			var shippingType = GetShippingType(productIdsList[0]);
			if (string.IsNullOrEmpty(shippingType))
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_PRODUCT_NOT_FOUND)
					.Replace("@@ 1 @@", ("商品ID：" + productIdsList[0]));
			}

			var sameShippingTypeProductsCount = products.Cast<DataRowView>()
				.Count(product =>
					(string)product[Constants.FIELD_PRODUCT_SHIPPING_TYPE] == shippingType);
			if (sameShippingTypeProductsCount != productIdsList.Length)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_SHIPPING_TYPE_NOT_SAME);
			}
		}

		var existProductIds = (products.Count > 0)
			? products.Cast<DataRowView>().Select(drv => (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]).ToArray()
			: new string[0];
		foreach (var productId in productIdsList)
		{
			if (existProductIds.Contains(productId) == false)
			{
				var message = isTargetProduct
					? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_PRODUCT_NOT_FOUND)
					: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_EXCLUDED_PRODUCT_NOT_FOUND);
				return message.Replace("@@ 1 @@", ("商品ID：" + productId));
			}
		}

		// 商品有効性チェック
		var invalidProduct = products.Cast<DataRowView>().FirstOrDefault(product =>
			(string)product[Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID);
		if (invalidProduct != null)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID)
				.Replace("@@ 1 @@", (string)invalidProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]);
		}

		return string.Empty;
	}
	#endregion

	#region -ValidateProductVariationId バリエーションIDで指定された商品の整合性チェック
	/// <summary>
	/// バリエーションIDで指定された商品の整合性チェック
	/// </summary>
	/// <param name="productVariationIdsList">バリエーションIDリスト</param>
	/// <param name="isTargetProduct">対象商品か</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateProductVariationId(string[] productVariationIdsList, bool isTargetProduct = true)
	{
		if (productVariationIdsList.Length == 0) return string.Empty;

		if (productVariationIdsList.Any(targetId => targetId.IndexOf(',') < 0))
		{
			return isTargetProduct
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_VARIATION_INPUT_ERROR)
				: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_EXCLUDED_VARIATION_INPUT_ERROR);
		}

		var shippingType = string.Empty;
		// 対象商品の場合のみ、配送種別を判定する
		if (isTargetProduct)
		{
			var productId = string.IsNullOrEmpty(this.TargetProductIds)
				? productVariationIdsList[0].Split(',')[0]
				: this.TargetProductIdsList[0];
			shippingType = GetShippingType(productId);
			if (string.IsNullOrEmpty(shippingType))
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_PRODUCT_NOT_FOUND)
					.Replace("@@ 1 @@", ("商品ID：" + productId));
			}
		}

		foreach (var productVariationId in productVariationIdsList)
		{
			var products = ProductCommon.GetProductInfoUnuseMemberRankPrice(Constants.CONST_DEFAULT_SHOP_ID, productVariationId.Split(',')[0]);
			var productInfo = products.Cast<DataRowView>().FirstOrDefault(product =>
				(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == productVariationId.Split(',')[1]);
			if (productInfo == null)
			{
				var message = isTargetProduct
					? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_PRODUCT_NOT_FOUND)
					: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_EXCLUDED_PRODUCT_NOT_FOUND);
				return message.Replace("@@ 1 @@", ("バリエーションID：" + productVariationId));
			}
			if ((isTargetProduct)
				&& ((string)productInfo[Constants.FIELD_PRODUCT_SHIPPING_TYPE] != shippingType))
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_SHIPPING_TYPE_NOT_SAME);
			}

			// 商品有効性チェック
			var invalidProduct = products.Cast<DataRowView>().FirstOrDefault(product =>
				(string)product[Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID);
			if (invalidProduct != null)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID)
					.Replace("@@ 1 @@", (string)invalidProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]);
			}
		}

		return string.Empty;
	}
	#endregion

	#region -ValidateProductCategoryId 商品カテゴリIDで指定された商品の整合性チェック
	/// <summary>
	/// 商品カテゴリIDで指定された商品の整合性チェック
	/// </summary>
	/// <param name="productCategoryIdsList">商品カテゴリIDリスト</param>
	/// <param name="isTargetProduct">対象商品か</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateProductCategoryId(string[] productCategoryIdsList, bool isTargetProduct = true)
	{
		if (productCategoryIdsList.Length == 0) return string.Empty;

		// 存在性チェック
		var existCategoryIds = new ProductCategoryService().CheckValidProductCategory(
			Constants.CONST_DEFAULT_SHOP_ID,
			productCategoryIdsList);

		var message = string.Empty;
		foreach (var categoryId in productCategoryIdsList)
		{
			if (existCategoryIds.Contains(categoryId) == false)
			{
				message = isTargetProduct
					? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_PRODUCT_NOT_FOUND)
					: WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_EXCLUDED_PRODUCT_NOT_FOUND);
				message = message.Replace("@@ 1 @@", ("商品カテゴリID：" + categoryId));
				break;
			}
		}

		return message;
	}
	#endregion

	#region -GetShippingType 配送種別の取得
	/// <summary>
	/// 配送種別の取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>配送種別</returns>
	private string GetShippingType(string productId)
	{
		var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(Constants.CONST_DEFAULT_SHOP_ID, productId);
		if (product.Count == 0) return string.Empty;

		return (string)product.Cast<DataRowView>().First()[Constants.FIELD_PRODUCT_SHIPPING_TYPE];
	}
	#endregion

	#region -ValidateGrantProduct 同梱商品の整合性チェック
	/// <summary>
	/// 同梱商品の整合性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateGrantProduct()
	{
		if (this.BundleItems.Any(item => string.IsNullOrEmpty(item.GrantProductId)))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_GRANT_PRODUCT_ID_NO_INPUT);
		}

		var targetProducts = ProductCommon.GetProductInfoUnuseMemberRankPrice(
			Constants.CONST_DEFAULT_SHOP_ID,
			this.BundleItems.First().GrantProductId);
		if (targetProducts.Count == 0) return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_GRANT_PRODUCT_NOT_FOUND);

		var targetProductShippingType = (string)targetProducts.Cast<DataRowView>().First()[Constants.FIELD_PRODUCT_SHIPPING_TYPE];

		foreach (var bundleItem in this.BundleItems.Where(item => string.IsNullOrEmpty(item.GrantProductId) == false).ToArray())
		{
			var products = ProductCommon.GetProductInfoUnuseMemberRankPrice(Constants.CONST_DEFAULT_SHOP_ID, bundleItem.GrantProductId);
			var targetProduct = products.Cast<DataRowView>().FirstOrDefault(product =>
				(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == (bundleItem.GrantProductId + bundleItem.GrantProductVariationId));
			if (targetProduct == null)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_GRANT_PRODUCT_NOT_FOUND);
			}
			if ((string)targetProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE] != targetProductShippingType)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_SHIPPING_TYPE_NOT_SAME);
			}
			if ((string)targetProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_GRANT_PRODUCT_EXIST_PRODUCT_FIXED_PURCHASE_ONLY);
			}

			// 商品有効性チェック
			var invalidProduct = products.Cast<DataRowView>().FirstOrDefault(product =>
				(string)product[Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID);
			if (invalidProduct != null)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID)
					.Replace("@@ 1 @@", (string)invalidProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]);
			}
		}

		return string.Empty;
	}
	#endregion

	#region -ValidateShippingType 同梱商品と対象購入商品の配送種別チェック
	/// <summary>
	/// 同梱商品と対象購入商品の配送種別チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateShippingType()
	{
		// 商品IDで指定された対象購入商品の配送種別
		var productIdShippingType =
			this.TargetProductIdsList.Length == 0 ? "" : GetShippingType(this.TargetProductIdsList[0]);

		// バリエーションIDで指定された対象購入商品の配送種別
		var productId = string.IsNullOrEmpty(this.TargetProductIds) ? this.TargetProductVariationIdsList.Length == 0
				?
				""
				: this.TargetProductVariationIdsList[0].Split(',')[0] :
			this.TargetProductIdsList.Length == 0 ? "" : this.TargetProductIdsList[0];

		var variationIdShippingType = GetShippingType(productId);

		// 同梱商品配送種別
		var targetProducts = ProductCommon.GetProductInfoUnuseMemberRankPrice(
			Constants.CONST_DEFAULT_SHOP_ID,
			this.BundleItems.First().GrantProductId);

		// 商品取得できていなければスキップ（商品が存在しないエラーは別でバリデーションされる）
		if (targetProducts.Count == 0) return string.Empty;

		var targetProductShippingType = targetProducts.Table.AsEnumerable().First()
			.Field<string>(Constants.FIELD_PRODUCT_SHIPPING_TYPE);

		if ((string.IsNullOrEmpty(productIdShippingType)
				&& (string.IsNullOrEmpty(variationIdShippingType) == false)
				&& (variationIdShippingType != targetProductShippingType))
			|| (string.IsNullOrEmpty(variationIdShippingType)
				&& (string.IsNullOrEmpty(productIdShippingType) == false)
				&& (productIdShippingType != targetProductShippingType))
			|| ((string.IsNullOrEmpty(productIdShippingType) == false)
				&& (string.IsNullOrEmpty(variationIdShippingType) == false)
				&& ((productIdShippingType != variationIdShippingType)
					|| (productIdShippingType != targetProductShippingType))))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_SHIPPING_TYPE_NOT_SAME);
		}

		return string.Empty;
	}
	#endregion

	#region -ValidateTargetList ターゲットリストの整合性チェック
	/// <summary>
	/// ターゲットリストの整合性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateTargetList()
	{
		if (string.IsNullOrEmpty(this.TargetId)) return string.Empty;

		var result = new TargetListService().CheckValidTargetList(this.DeptId, this.TargetId);
		return result
			? string.Empty
			: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGETLIST_NOT_FOUND);
	}
	#endregion

	#region -ValidateNumber 数値の整合性チェック
	/// <summary>
	/// 数値の整合性チェック
	/// </summary>
	/// <param name="target">チェック対象</param>
	/// <param name="str">文言</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateNumber(string target, string str)
	{
		if (string.IsNullOrEmpty(target)) return string.Empty;

		return Validator.CheckHalfwidthNumberError(str, target);
	}
	#endregion

	#region -ValidateTargetAdvCodesFirst 初回広告コードの整合性チェック
	/// <summary>
	/// 初回広告コードの整合性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateTargetAdvCodesFirst()
	{
		if (string.IsNullOrEmpty(this.TargetAdvCodesFirst)) return string.Empty;

		// 存在性チェック
		foreach (var advCode in this.TargetAdvCodesFirstList)
		{
			var model = new AdvCodeService().GetAdvCodeFromAdvertisementCode(advCode);
			if (model == null)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_ADVCODES_FIRST_NOT_FOUND)
					.Replace("@@ 1 @@", advCode);
			}
		}

		return string.Empty;
	}
	#endregion

	#region -ValidateTargetAdvCodesNew 最新広告コードの整合性チェック
	/// <summary>
	/// 最新広告コードの整合性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateTargetAdvCodesNew()
	{
		if (string.IsNullOrEmpty(this.TargetAdvCodesNew)) return string.Empty;

		// 存在性チェック
		foreach (var advCode in this.TargetAdvCodesNewList)
		{
			var model = new AdvCodeService().GetAdvCodeFromAdvertisementCode(advCode);
			if (model == null)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_ADVCODES_NEW_NOT_FOUND)
					.Replace("@@ 1 @@", advCode);
			}
		}

		return string.Empty;
	}
	#endregion

	/// <summary>
	/// クーポンコードの整合性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidateTargetCouponCodes(string loginOperatorDeptId)
	{
		if (string.IsNullOrEmpty(this.TargetCouponCodes)
			|| string.IsNullOrEmpty(loginOperatorDeptId)) return string.Empty;

		// 存在性チェック
		foreach (var targetCouponCode in this.TargetCouponCodeList)
		{
			var model = new CouponService().GetCouponFromCouponCodePerfectMatch(loginOperatorDeptId, targetCouponCode);
			if (model == null)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_TARGET_COUPON_CODE_NOT_FOUND)
					.Replace("@@ 1 @@", targetCouponCode);
			}
		}

		return string.Empty;
	}
	#endregion

	#region プロパティ
	/// <summary>商品同梱ID</summary>
	public string ProductBundleId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID] = value; }
	}
	/// <summary>商品同梱名</summary>
	public string ProductBundleName
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME] = value; }
	}
	/// <summary>対象注文種別</summary>
	public string TargetOrderType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE] = value; }
	}
	/// <summary>説明文</summary>
	public string Description
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DESCRIPTION]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DESCRIPTION] = value; }
	}
	/// <summary>開始日時</summary>
	public string StartDatetime
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_START_DATETIME]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_START_DATETIME] = value; }
	}
	/// <summary>終了日時</summary>
	public string EndDatetime
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_END_DATETIME]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_END_DATETIME] = value; }
	}
	/// <summary>対象商品指定方法</summary>
	public string TargetProductKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_KBN] = value; }
	}
	/// <summary>対象商品ID</summary>
	public string TargetProductIds
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_IDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_IDS] = value; }
	}
	/// <summary>対象定期注文回数_FROM</summary>
	public string TargetOrderFixedPurchaseCountFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM] = value; }
	}
	/// <summary>対象定期注文回数_TO</summary>
	public string TargetOrderFixedPurchaseCountTo
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO] = value; }
	}
	/// <summary>ユーザ利用可能回数</summary>
	public string UsableTimesKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES_KBN] = value; }
	}
	/// <summary>ユーザ利用可能指定回数</summary>
	public string UsableTimes
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES] = value; }
	}
	/// <summary>商品同梱設定適用種別</summary>
	public string ApplyType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_TYPE] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_VALID_FLG] = value; }
	}
	/// <summary>重複適用フラグ</summary>
	public string MultipleApplyFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG] = value; }
	}
	/// <summary>適用優先順</summary>
	public string ApplyOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_ORDER]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_ORDER] = value; }
	}
	/// <summary>最終更新者名</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_LAST_CHANGED] = value; }
	}
	/// <summary>対象商品バリエーションID</summary>
	public string TargetProductVariationIds
	{
		get { return (string)this.DataSource["TargetProductVariationIds"]; }
		set { this.DataSource["TargetProductVariationIds"] = value; }
	}
	/// <summary>同梱商品</summary>
	public ProductBundleItemInput[] BundleItems
	{
		get { return (ProductBundleItemInput[])this.DataSource["ProductBundleItems"]; }
		set { this.DataSource["ProductBundleItems"] = value; }
	}
	/// <summary>対象商品 商品IDリスト</summary>
	public string[] TargetProductIdsList
	{
		get
		{
			return string.IsNullOrEmpty(this.TargetProductIds)
				? new string[0]
				: this.TargetProductIds.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>対象商品 バリエーションIDリスト</summary>
	public string[] TargetProductVariationIdsList
	{
		get
		{
			return string.IsNullOrEmpty(this.TargetProductVariationIds)
				? new string[0]
				: this.TargetProductVariationIds.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>対象商品カテゴリID</summary>
	public string TargetProductCategoryIds
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_CATEGORY_IDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_CATEGORY_IDS] = value; }
	}
	/// <summary>対象商品 商品カテゴリIDリスト</summary>
	public string[] TargetProductCategoryIdsList
	{
		get
		{
			return string.IsNullOrEmpty(this.TargetProductCategoryIds)
				? new string[0]
				: this.TargetProductCategoryIds.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>対象外商品 商品ID</summary>
	public string ExceptProductIds
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_IDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_IDS] = value; }
	}
	/// <summary>対象外商品 バリエーションID</summary>
	public string ExceptProductVariationIds
	{
		get { return (string)this.DataSource["ExceptProductVariationIds"]; }
		set { this.DataSource["ExceptProductVariationIds"] = value; }
	}
	/// <summary>対象外商品 商品カテゴリID</summary>
	public string ExceptProductCategoryIds
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_CATEGORY_IDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_CATEGORY_IDS] = value; }
	}
	/// <summary>対象外商品 商品IDリスト</summary>
	public string[] ExceptProductIdsList
	{
		get
		{
			return string.IsNullOrEmpty(this.ExceptProductIds)
				? new string[0]
				: this.ExceptProductIds.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>対象外商品 バリエーションIDリスト</summary>
	public string[] ExceptProductVariationIdsList
	{
		get
		{
			return string.IsNullOrEmpty(this.ExceptProductVariationIds)
				? new string[0]
				: this.ExceptProductVariationIds.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>対象外商品 商品カテゴリIDリスト</summary>
	public string[] ExceptProductCategoryIdsList
	{
		get
		{
			return string.IsNullOrEmpty(this.ExceptProductCategoryIds)
				? new string[0]
				: this.ExceptProductCategoryIds.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>ターゲットリストID</summary>
	public string TargetId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID] = value; }
	}
	/// <summary>ターゲットリストID除外フラグ</summary>
	public bool TargetIdExceptFlg
	{
		get { return (bool)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG] = value; }
	}
	/// <summary>対象注文商品合計金額適用下限</summary>
	public string TargetOrderPriceSubtotalMin
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_PRICE_SUBTOTAL_MIN]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_PRICE_SUBTOTAL_MIN] = value; }
	}
	/// <summary>対象商品個数適用下限</summary>
	public string TargetProductCountMin
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_COUNT_MIN]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_COUNT_MIN] = value; }
	}
	/// <summary>初回広告コード</summary>
	public string TargetAdvCodesFirst
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_FIRST]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_FIRST] = value; }
	}
	/// <summary>初回広告コードリスト</summary>
	public string[] TargetAdvCodesFirstList
	{
		get
		{
			return string.IsNullOrEmpty(this.TargetAdvCodesFirst)
				? new string[0]
				: this.TargetAdvCodesFirst.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>最新広告コード</summary>
	public string TargetAdvCodesNew
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_NEW]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_NEW] = value; }
	}
	/// <summary>最新広告コードリスト</summary>
	public string[] TargetAdvCodesNewList
	{
		get
		{
			return string.IsNullOrEmpty(this.TargetAdvCodesNew)
				? new string[0]
				: this.TargetAdvCodesNew.Trim().Replace("\r\n", "\n").Split('\n');
		}
	}
	/// <summary>決済種別</summary>
	public string TargetPaymentIds
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PAYMENT_IDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PAYMENT_IDS] = value; }
	}
	/// <summary>識別ID</summary>
	public string DeptId { get; set; }
	/// <summary>回数指定か</summary>
	public bool IsNumSpecify
	{
		get { return (this.UsableTimesKbn == Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NUMSPECIFY); }
	}
	/// <summary>クーポンコード</summary>
	public string TargetCouponCodes
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_COUPON_CODES]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_COUPON_CODES] = value; }
	}
	/// <summary>クーポンコードリスト</summary>
	public string[] TargetCouponCodeList
	{
		get
		{
			return string.IsNullOrEmpty(this.TargetCouponCodes) == false
				? this.TargetCouponCodes.Trim().Replace("\r\n", "\n").Split('\n')
				: Array.Empty<string>();
		}

	}
	#endregion
}
