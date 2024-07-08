/*
=========================================================================================================
  Module      : 注文フロー（注文カートページ）プロセス(OrderFlowProcess_OrderCartPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Domain.ContentsLog;
using w2.Domain.Coupon;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.SubscriptionBox;
using w2.Domain.User;

/// <summary>
/// OrderLpInputs の概要の説明です
/// </summary>
public partial class OrderFlowProcess
{
	/// <summary>
	/// 商品カート投入
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="iProductCount">商品数</param>
	/// <param name="lProductOptionSelectedValues">付帯情報</param>
	/// <param name="contentsLog">コンテンツログ</param>
	/// <param name="timeSaleId">コンテンツログ</param>
	/// <param name="addCartUrlFlg">カート投入URLフラグ</param>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns>エラーメッセージ</returns>
	public string AddProductToCart(
		string strShopId,
		string strProductId,
		string strVariationId,
		Constants.AddCartKbn addCartKbn,
		int iProductCount,
		List<string> lProductOptionSelectedValues,
		ContentsLogModel contentsLog = null,
		string timeSaleId = "",
		bool addCartUrlFlg = false,
		string subscriptionBoxCourseId = "")
	{
		//------------------------------------------------------
		// 商品取得・状態チェック
		//------------------------------------------------------
		DataView dvProduct = GetProduct(strShopId, strProductId, strVariationId);
		var fixedPurchaseProduct = (addCartUrlFlg == true)
			? dvProduct
			: ProductCommon.GetProductInfo(strShopId, strProductId, this.LoginUserMemberRankId, this.UserFixedPurchaseMemberFlg);
		if ((dvProduct.Count == 0) || (fixedPurchaseProduct.Count == 0))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
		}
		OrderErrorcode oeProductError = OrderCommon.CheckProductStatus(dvProduct[0], iProductCount, addCartKbn, this.LoginUserId, addCartUrlFlg);
		if (oeProductError != OrderErrorcode.NoError)
		{
			return OrderCommon.GetErrorMessage(
				oeProductError,
				CreateProductJointName(dvProduct[0]),
				MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
		}

		// 商品付帯情報チェック
		ProductOptionSettingList posl = new ProductOptionSettingList((string)dvProduct[0][Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]);
		var productOptionErrorMessage = CheckProductOption(lProductOptionSelectedValues, posl);
		if (string.IsNullOrEmpty(productOptionErrorMessage) == false) return productOptionErrorMessage;
		timeSaleId = (string.IsNullOrEmpty(timeSaleId) == false)
			? timeSaleId
			: StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]);

		if ((StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]) == Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON)
			&& ((this.IsLoggedIn == false) || (this.IsLoggedIn && (this.LoginUser.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF))))
		{
			return Constants.TAG_REPLACER_DATA_SCHEMA
				.ReplaceTextAll(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_BUYABLE_FIXED_PURCHASE_MEMBER))
				.Replace("@@ 1 @@", (string)dvProduct[0][Constants.FIELD_PRODUCT_NAME]);
		}

		// 販売可能数量チェック(カート内の数量を考慮する)
		var maxSellQuantityError = new ProductPage().GetMaxSellQuantityError(
			strShopId,
			strProductId,
			strVariationId,
			iProductCount);
		if (string.IsNullOrEmpty(maxSellQuantityError) == false)
		{
			return maxSellQuantityError;
		}

		//------------------------------------------------------
		// 問題無ければカート投入
		//------------------------------------------------------
		this.CartList.AddProduct(
			dvProduct[0],
			addCartKbn,
			timeSaleId,
			iProductCount,
			posl,
			contentsLog,
			subscriptionBoxCourseId: subscriptionBoxCourseId);

		return "";
	}

	/// <summary>
	/// 商品カート投入
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="productCount">商品数</param>
	/// <param name="productOptionSelectedValues">商品付帯情報</param>
	/// <param name="contentsLog">コンテンツログ</param>
	/// <param name="subscriptionBoxCourseId">subscription Box Course Id</param>
	/// <param name="userId">ユーザID</param>
	/// <returns>エラーメッセージ</returns>
	public string AddProductToLandingCart(
		string shopId,
		string productId,
		string variationId,
		Constants.AddCartKbn addCartKbn,
		int productCount,
		List<string> productOptionSelectedValues,
		ContentsLogModel contentsLog = null,
		string subscriptionBoxCourseId = "",
		string userId = null)
	{
		var result = string.Empty;

		// 商品取得・状態チェック
		var product = GetProduct(shopId, productId, variationId);
		var fixedPurchaseProduct = ProductCommon.GetProductInfo(shopId, productId, this.LoginUserMemberRankId, this.UserFixedPurchaseMemberFlg);
		if ((product.Count == 0) || (fixedPurchaseProduct.Count == 0))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
		}
		var productErrors = OrderCommon.CheckProduct(product[0], productCount, addCartKbn, this.LoginUserId);
		if (productErrors[0] != OrderErrorcode.NoError)
		{
			result = OrderCommon.GetErrorMessage(
				productErrors[0],
				CreateProductJointName(product[0]),
				MemberRankOptionUtility.GetMemberRankName((string)product[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
			if ((productErrors[0] != OrderErrorcode.ProductNoStockBeforeCart) || (productErrors.Length > 1))
			{
				return result;
			}
		}

		// 商品付帯情報チェック
		var posl = new ProductOptionSettingList((string)product[0][Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]);
		var productOptionErrorMessage = CheckProductOption(productOptionSelectedValues, posl);
		if (string.IsNullOrEmpty(productOptionErrorMessage) == false) return productOptionErrorMessage;

		// 問題無ければカート投入
		this.CartList.AddProduct(
			product[0],
			addCartKbn,
			StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
			productCount,
			posl,
			contentsLog,
			null,
			null,
			subscriptionBoxCourseId,
			null,
			default(DateTime),
			userId);

		return result;
	}

	/// <summary>
	/// 商品付帯情報チェック
	/// </summary>
	/// <param name="productOptionSelectedValues">商品付帯情報</param>
	/// <param name="posl">商品付帯情報設定値</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckProductOption(List<string> productOptionSelectedValues, ProductOptionSettingList posl)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)
		{
			productOptionSelectedValues = SetPriceForOptionValues(productOptionSelectedValues, posl);
		}

		var loop = 0;
		foreach (ProductOptionSetting pos in posl)
		{
			if ((productOptionSelectedValues.Count > loop)
				&& pos.IsCheckBox)
			{
				if (pos.IsNecessary && string.IsNullOrEmpty(productOptionSelectedValues[loop]))
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				}

				if (pos.IsValidSelectedSettingValue(productOptionSelectedValues[loop]) == false)
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_PRODUCT_OPTION_VALUE_VALID_ERROR);
				}
			}

			if ((productOptionSelectedValues.Count > loop)
				&& pos.IsSelectMenu)
			{
				if (pos.IsNecessary && string.IsNullOrEmpty(productOptionSelectedValues[loop]))
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				}

				if (pos.IsValidSelectedSettingValue(productOptionSelectedValues[loop]) == false)
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_PRODUCT_OPTION_VALUE_VALID_ERROR);
				}
			}

			if ((productOptionSelectedValues.Count > loop)
				&& pos.IsTextBox)
			{
				if (pos.IsNecessary && string.IsNullOrEmpty(productOptionSelectedValues[loop]))
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				}

				var errorMessage = pos.CheckValidText(productOptionSelectedValues[loop]);
				if (errorMessage.Length > 0) return errorMessage;
			}

			if (productOptionSelectedValues.Count > loop)
			{
				pos.SelectedSettingValue = productOptionSelectedValues[loop];
			}

			loop++;
		}

		return "";
	}

	/// <summary>
	/// 付帯情報の価格を設定
	/// </summary>
	/// <param name="productOptionSelectedValues">オプションのリスト</param>
	/// <param name="posl">オプション設定</param>
	/// <returns>価格付付帯情報</returns>
	private List<string> SetPriceForOptionValues(List<string> productOptionSelectedValues, ProductOptionSettingList posl)
	{
		var priceIncludeOptionValues = new List<string>();

		foreach (var val in productOptionSelectedValues)
		{
			var tempValue = string.Empty;
			var tempSelectedValues = val
				.Split(
					new string[] { Constants.PRODUCTOPTION_VALUES_URL_SEPARATING_CHAR_SETTING_VALUE },
					StringSplitOptions.RemoveEmptyEntries);

			if (tempSelectedValues.Length > 0)
			{
				var settingStrings = posl.Items
					.Select(pos => GetSettingStringWithSeparator(pos, tempSelectedValues))
					.ToArray();
				tempValue = string.Join(string.Empty, settingStrings);
			}
			priceIncludeOptionValues.Add(tempValue != string.Empty ? tempValue : val);
		}

		return priceIncludeOptionValues;
	}

	/// <summary>
	/// 商品付帯情報の設定値毎に区切り文字を結合して取得
	/// </summary>
	/// <param name="pos">商品付帯情報</param>
	/// <param name="selectedValues">商品付帯情報選択値</param>
	/// <returns>区切り文字を結合した付帯情報設定値</returns>
	private string GetSettingStringWithSeparator(ProductOptionSetting pos, string[] selectedValues)
	{
		var result = selectedValues.Select(
			selectedValue =>
			{
				// 設定値毎にチェックボックスの区切り文字を付けて返す
				var settingValuesWithSeparator = pos.SettingValues
					.Where(settingValue => settingValue.Contains("(") && (selectedValue == settingValue.Substring(0, settingValue.IndexOf("("))))
					.Select(settingStr => settingStr + Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE)
					.ToArray();
				return string.Join(string.Empty, settingValuesWithSeparator);
			}).ToArray();

		var resultText = string.Join(string.Empty, result);
		return resultText;
	}

	/// <summary>
	/// 商品セットカート投入
	/// </summary>
	/// <param name="strShopId">商品ID</param>
	/// <param name="strProductSetId">商品セットID</param>
	/// <param name="lProductSetItems">商品セットアイテム</param>
	/// <param name="addCartUrlFlg">カート投入URLフラグ</param>
	public string AddProductSetToCart(string strShopId, string strProductSetId, List<Dictionary<string, string>> lProductSetItems, bool addCartUrlFlg = false)
	{
		//------------------------------------------------------
		// 商品セット情報取得（存在しない場合は追加せずカート画面へ）
		//------------------------------------------------------
		DataView dvProductSet = ProductCommon.GetProductSetInfo(strShopId, strProductSetId);
		if (dvProductSet.Count == 0)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTSET_NOPRODUCT);
		}

		//------------------------------------------------------
		// 商品セット情報作成（とりあえずNo.0として作成）
		//------------------------------------------------------
		CartProductSet cpsProductSet = new CartProductSet(dvProductSet[0], 1, 0, true);
		foreach (Dictionary<string, string> dicProductSetItem in lProductSetItems)
		{
			// 商品セット情報をパラメタから取得
			string strProductId = dicProductSetItem[Constants.FIELD_CART_PRODUCT_ID];
			string strVariationId = dicProductSetItem[Constants.FIELD_CART_VARIATION_ID];
			int iProductCount;
			iProductCount = int.Parse(dicProductSetItem[Constants.FIELD_CART_PRODUCT_COUNT]);

			// 商品情報取得（取得できなければカートに追加せずカート画面へ）
			DataView dvProduct = GetProduct(strShopId, strProductId, strVariationId);
			if (dvProduct.Count == 0)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTSET_NOPRODUCT);
			}

			// 商品状態チェック・エラー表示（セットはギフト非対応とする）
			OrderErrorcode oeProductError = OrderCommon.CheckProductStatus(dvProduct[0], iProductCount, Constants.AddCartKbn.Normal, this.LoginUserId, addCartUrlFlg);
			if (oeProductError != OrderErrorcode.NoError)
			{
				return OrderCommon.GetErrorMessage(
					oeProductError,
					CreateProductJointName(dvProduct[0]),
					MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
			}

			// 商品セットへ追加（追加できなかった場合はカート画面へ）
			CartProduct cpTmp = cpsProductSet.AddProductVirtual(dvProduct[0], iProductCount);
			if (cpTmp == null)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTSET_NOPRODUCT);
			}
			cpTmp.ProductOptionSettingList = new ProductOptionSettingList(cpTmp.ShopId, cpTmp.ProductId);
		}

		//------------------------------------------------------
		// 同じ商品セットが無いかチェック
		//------------------------------------------------------
		foreach (CartObject co in this.CartList.Items)
		{
			foreach (CartProduct cp in co.Items)
			{
				if (cp.IsSetItem)
				{
					if (cpsProductSet.IsSameAs(cp.ProductSet))
					{
						// カートに追加しないで単にカート画面へ
						return "";
					}
				}
			}
		}

		//------------------------------------------------------
		// カート分割基準チェック
		//------------------------------------------------------
		if (cpsProductSet.Items.Count != 0)
		{
			string strShippingType = cpsProductSet.Items[0].ShippingType;
			foreach (CartProduct cp in cpsProductSet.Items)
			{
				if ((strShopId != cp.ShopId) || (strShippingType != cp.ShippingType))
				{
					// カートに追加しないで単にカート画面へ
					return "";
				}
			}
		}

		//------------------------------------------------------
		// 商品セットカート投入
		//------------------------------------------------------
		this.CartList.AddProductSet(cpsProductSet);

		return "";
	}

	/// <summary>
	/// クーポンの初期設定
	/// </summary>
	/// <param name="repeaterItem">親リピーターアイテム</param>
	public void InitializeCouponComponents(RepeaterItem repeaterItem)
	{
		//クーポンリストの設定
		var wddlCouponList = GetWrappedControl<WrappedDropDownList>(repeaterItem, "ddlCouponList");
		var listItemCollection = GetUsableCouponListWithBlank(this.CartList.Items[repeaterItem.ItemIndex]);
		foreach (ListItem item in listItemCollection)
		{
			wddlCouponList.AddItem(item);
		}
		wddlCouponList.SelectedValue = GetSelectedCoupon(this.CartList.Items[repeaterItem.ItemIndex], listItemCollection);

		var existsUsableCoupon = ExistsUsableCoupon(this.CartList.Items[repeaterItem.ItemIndex]);

		var wdivCouponInputMethod = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "divCouponInputMethod");
		wdivCouponInputMethod.Visible = existsUsableCoupon;
		var whgcCouponSelect = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "hgcCouponSelect");
		whgcCouponSelect.Visible = existsUsableCoupon && (this.CartList.Items[repeaterItem.ItemIndex].CouponInputMethod != CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		var whgcCouponCodeInputArea = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "hgcCouponCodeInputArea");
		whgcCouponCodeInputArea.Visible = (existsUsableCoupon == false) || (this.CartList.Items[repeaterItem.ItemIndex].CouponInputMethod == CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		var wlbShowCouponBox = GetWrappedControl<WrappedLinkButton>(repeaterItem, "lbShowCouponBox");
		wlbShowCouponBox.Visible = existsUsableCoupon;
	}

	/// <summary>
	/// カートに対し利用可能なクーポンリスト取得(先頭ブランク)
	/// </summary>
	/// <param name="cart">カートオブジェクト</param>
	/// <returns>クーポンリスト</returns>
	protected ListItemCollection GetUsableCouponListWithBlank(CartObject cart)
	{
		string languageCode = null;
		string languageLocaleId = null;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			languageCode = RegionManager.GetInstance().Region.LanguageCode;
			languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
		}

		CheckReferralCode(this.LoginUserId);

		var result = CouponOptionUtility.GetUsableCouponListWithBlank(
			this.LoginUserId,
			this.LoginUserMail,
			cart,
			languageCode,
			languageLocaleId,
			Constants.INTRODUCTION_COUPON_OPTION_ENABLED
				? SessionManager.ReferralCode
				: string.Empty);

		return result;
	}

	/// <summary>
	/// 選択されたクーポン取得
	/// </summary>
	/// <param name="cart">カートオブジェクト</param>
	/// <param name="usableCouponListWithBlankList">カートに対し利用可能なクーポンリスト(先頭にブランクあり)</param>
	/// <returns>クーポン</returns>
	protected string GetSelectedCoupon(CartObject cart, ListItemCollection usableCouponListWithBlankList)
	{
		return CouponOptionUtility.GetSelectedCoupon(cart, usableCouponListWithBlankList);
	}

	/// <summary>
	/// カートに適用できるクーポンがあるか？
	/// </summary>
	/// <param name="cart">カートオブジェクト</param>
	/// <returns>あればtrue</returns>
	protected bool ExistsUsableCoupon(CartObject cart)
	{
		CheckReferralCode(this.LoginUserId);

		var result = CouponOptionUtility.ExistsUsableCoupon(
			this.LoginUserId,
			this.LoginUserMail,
			cart,
			Constants.INTRODUCTION_COUPON_OPTION_ENABLED
				? SessionManager.ReferralCode
				: string.Empty);

		return result;
	}

	/// <summary>
	/// ポイント情報セット（再計算は呼び出し元で行う）
	/// </summary>
	/// <param name="wrCartList">カートリスト</param>
	public void SetUsePointData(WrappedRepeater wrCartList)
	{
		// 全利用ポイント取得
		decimal dTotalUsePoint = 0;
		foreach (RepeaterItem riCart in wrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wtbOrderPointUse = GetWrappedControl<WrappedTextBox>(riCart, "tbOrderPointUse");

			string strPoint = StringUtility.ToHankaku(wtbOrderPointUse.Text).Trim();

			decimal dPoint;
			if (decimal.TryParse(strPoint, out dPoint) == false)
			{
				dPoint = 0;
			}
			dTotalUsePoint += dPoint;
		}

		// 本ポイントがマイナスだとポイントチェックでエラーが発生するので、0補正する
		decimal dUserPointCorrectedZero = (this.LoginUserPointUsable > 0) ? this.LoginUserPointUsable : 0;

		// 本ポイント < 利用ポイントであればエラー
		if (dUserPointCorrectedZero < dTotalUsePoint)
		{
			this.ErrorMessages.Add(this.CurrentCartIndex, OrderPage.CartErrorMessages.ErrorKbn.Point, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_USE_MAX_ERROR).Replace("@@ 1 @@", GetNumeric(this.LoginUserPointUsable) + Constants.CONST_UNIT_POINT_PT));
		}
		else
		{
			// リピータに対してループ
			foreach (RepeaterItem riCart in wrCartList.Items)
			{
				// ラップ済みコントロール宣言
				var wtbOrderPointUse = GetWrappedControl<WrappedTextBox>(riCart, "tbOrderPointUse");

				// 利用ポイント、利用ポイント金額取得
				string strInputUsePoint = StringUtility.ToHankaku(wtbOrderPointUse.Text).Trim();
				decimal dInputUsePoint = (strInputUsePoint != "") ? decimal.Parse(strInputUsePoint) : 0;
				decimal dUsePointPrice = new PointService().GetOrderPointUsePrice(dInputUsePoint, Constants.FLG_POINT_POINT_KBN_BASE);

				CartObject coCart = this.CartList.GetCart(this.CartList.Items[riCart.ItemIndex].CartId);
				if (coCart != null) // 削除時に取得できないことがある
				{
					// ポイント利用可能額を超えていたらエラー
					if (dUsePointPrice > coCart.PointUsablePrice)
					{
						this.ErrorMessages.Add(this.CurrentCartIndex, OrderPage.CartErrorMessages.ErrorKbn.Point, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR).Replace("@@ 1 @@", CurrencyManager.ToPrice(coCart.PointUsablePrice)));
						continue;
					}

					// 利用ポイントセット（再計算は呼び出し元で行う）
					coCart.SetUsePoint(dInputUsePoint, dUsePointPrice);
				}
			}
		}
	}

	/// <summary>
	/// クーポン情報セット（再計算は呼び出し元で行う）
	/// </summary>
	/// <param name="wrCartList">カートリスト</param>
	public void SetUseCouponData(WrappedRepeater wrCartList)
	{
		// ユーザID取得
		var userId = StringUtility.ToEmpty(this.LoginUserId);

		//------------------------------------------------------
		// クーポン情報の初期化
		//------------------------------------------------------
		var index = (this.CurrentCartIndex <= this.CartList.Items.Count - 1) ? this.CurrentCartIndex : 0;
		if (this.CartList.Items.Count <= index) return;
		this.CartList.Items[index].Coupon = null;

		//------------------------------------------------------
		// クーポン指定チェック（指定無しの場合は抜ける）
		//------------------------------------------------------
		var couponCodeInputFlg = false;
		foreach (RepeaterItem riCart in wrCartList.Items)
		{
			var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode");

			// クーポンが指定されていない場合、クーポンプルダウンがクリアされてしまうため、この時点でカート情報にプルダウン値をセット
			var wddlCouponCodeList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlCouponList");
			var coCart = this.CartList.GetCart(this.CartList.Items[riCart.ItemIndex].CartId);
			coCart.SelectedCoupon = wddlCouponCodeList.SelectedValue;

			var wrblCouponInputMethod = GetWrappedControl<WrappedRadioButtonList>(riCart, "rblCouponInputMethod");
			coCart.CouponInputMethod = wrblCouponInputMethod.SelectedValue;

			if (wtbCouponCode.Text.Trim() != "")
			{
				couponCodeInputFlg = true;
				break;
			}
		}
		if (couponCodeInputFlg == false)
		{
			return;
		}

		//------------------------------------------------------
		// 各クーポンチェック
		//------------------------------------------------------
		var cartIndex = -1;
		foreach (RepeaterItem riCart in wrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode");
			var wtbOrderPointUse = GetWrappedControl<WrappedTextBox>(riCart, "tbOrderPointUse");
			var wddlCoupon = GetWrappedControl<WrappedDropDownList>(riCart, "ddlCouponList");

			cartIndex++;

			// 該当カート情報取得
			CartObject coCart = this.CartList.GetCart(this.CartList.Items[riCart.ItemIndex].CartId);

			// エラーの場合にクーポンコードの入力値が残るのを防ぐためクリア
			coCart.SelectedCoupon = "";

			if (coCart != null)
			{
				//------------------------------------------------------
				// クーポンコード取得（空の場合は飛ばす）
				//------------------------------------------------------
				var couponCode = StringUtility.ToHankaku(wtbCouponCode.Text).Trim();
				if (couponCode == "")
				{
					continue;
				}

				//------------------------------------------------------
				// 入力クーポン情報取得、存在チェック
				//------------------------------------------------------
				CheckReferralCode(userId);
				var inputCoupons = new CouponService().GetAllUserCouponsFromCouponCodeIncludeUnavailable(
					Constants.W2MP_DEPT_ID,
					userId,
					couponCode,
					Constants.INTRODUCTION_COUPON_OPTION_ENABLED
						? SessionManager.ReferralCode
						: string.Empty);

				if (inputCoupons.Length == 0)
				{
					this.ErrorMessages.Add(riCart.ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon, CouponOptionUtility.GetErrorMessage(CouponErrorcode.NoCouponError).Replace("@@ 1 @@", couponCode));
					continue;
				}

				//------------------------------------------------------
				// クーポン利用可能チェック（回数制限、未使用チェック）
				//------------------------------------------------------
				CouponErrorcode couponErrorCode = CouponErrorcode.CouponUsedError;
				foreach (var coupon in inputCoupons)
				{
					// 利用可能なクーポンが１個でも見つかれば次の処理へ
					var mailAddress = (coCart.Owner != null) ? coCart.Owner.MailAddr : StringUtility.ToEmpty(this.LoginUserMail);
					couponErrorCode = CouponOptionUtility.CheckUseCoupon(coupon, userId, mailAddress);
					if (couponErrorCode == CouponErrorcode.NoError)
					{
						break;
					}
				}
				if (couponErrorCode != CouponErrorcode.NoError)
				{
					this.ErrorMessages.Add(riCart.ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon, CouponOptionUtility.GetErrorMessage(couponErrorCode).Replace("@@ 1 @@", couponCode));
					continue;
				}

				var inputCouponsIndex = 0;
				//------------------------------------------------------
				// クーポン重複チェック(回数制限有りクーポンのみ)
				//------------------------------------------------------
				// 回数制限有りクーポンの場合
				if (CouponOptionUtility.IsCouponLimit(inputCoupons[0].CouponType))
				{
					var errorFlg = false;
					foreach (var inputCoupon in inputCoupons)
					{
						// 未使用クーポンでない場合は次へ
						if (inputCoupon.UseFlg != Constants.FLG_USERCOUPON_USE_FLG_UNUSE)
						{
							inputCouponsIndex++;
							continue;
						}

						// 別カートで同じクーポンを利用していないかチェック
						var anotherCartIndex = -1;
						var couponUseCount = 0;
						foreach (CartObject coAnother in this.CartList.Items)
						{
							anotherCartIndex++;

							if (coCart == coAnother)
							{
								continue;
							}

							// クーポンID、枝番が同じ場合はエラーメッセージ追加
							if (coAnother.Coupon != null)
							{
								if ((coAnother.Coupon.CouponId == inputCoupon.CouponId)
									&& (coAnother.Coupon.CouponNo == inputCoupon.CouponNo))
								{
									couponUseCount++;
									if (inputCoupons.Length > couponUseCount)
									{
										inputCoupons = inputCoupons.Where(
											value => (value.CouponId != inputCoupon.CouponId || value.CouponNo != inputCoupon.CouponNo)).ToArray();
										continue;
									}

									this.ErrorMessages.Add(this.CurrentCartIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon, CouponOptionUtility.GetErrorMessage(CouponErrorcode.CouponDuplicationError).Replace("@@ 1 @@", couponCode));
									errorFlg = true;
								}
							}
						}
					}
					if (errorFlg)
					{
						continue;
					}
				}

				//------------------------------------------------------
				// 使用回数制限ありクーポンの残り利用回数チェック
				//------------------------------------------------------
				if (CouponOptionUtility.IsCouponAllLimit(inputCoupons[0].CouponType))
				{
					// 利用クーポン数集計
					var couponUseCount = 0;
					for (var iAnotherCartIndex = 0; iAnotherCartIndex < this.CartList.Items.Count; iAnotherCartIndex++)
					{
						CartObject coAnother = this.CartList.Items[iAnotherCartIndex];

						if (coCart == coAnother)
						{
							continue;
						}

						if ((coAnother.Coupon != null)
							&& (coAnother.Coupon.CouponId == inputCoupons[0].CouponId))
						{
							couponUseCount++;
						}
					}
					// 利用回数を超える場合はエラーメッセージ追加
					if (couponUseCount >= inputCoupons[0].CouponCount)
					{
						this.ErrorMessages.Add(riCart.ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon, CouponOptionUtility.GetErrorMessage(CouponErrorcode.CouponUsableCountError).Replace("@@ 1 @@", couponCode));
						continue;
					}
				}

				//ブラックリスト型クーポンの重複チェック
				if (CouponOptionUtility.IsBlacklistCoupon(inputCoupons[0].CouponType))
				{
					var errorFlg = false;
					foreach (var inputCoupon in inputCoupons)
					{
						// 別カートで同じクーポンを利用していないかチェック
						foreach (var coAnother in this.CartList.Items)
						{
							if (coCart == coAnother)
							{
								continue;
							}

							if ((coAnother.Coupon != null)
								&& (coAnother.Coupon.CouponId == inputCoupon.CouponId))
							{
								this.ErrorMessages.Add(this.CurrentCartIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon, CouponOptionUtility.GetErrorMessage(CouponErrorcode.CouponDuplicationError).Replace("@@ 1 @@", couponCode));
								errorFlg = true;
							}
						}
					}
					if (errorFlg)
					{
						continue;
					}
				}

				// 会員限定回数制限付きクーポンの利用回数チェック
				if (CouponOptionUtility.IsCouponLimitedForRegisteredUser(inputCoupons[inputCouponsIndex].CouponType))
				{
					var couponUseCount = 0;
					int? maxCouponCount = 0;
					// 利用可能最大枚数のクーポンを選ぶ
					for (var count = 0; count < inputCoupons.Length; count++)
					{
						// クーポンが有効でない場合はコンテニュー
						if (string.IsNullOrEmpty(CouponOptionUtility.CheckCouponValid(inputCoupons[count])) == false) continue;
						// 最大枚数よりも少ない場合はコンテニュー
						if (maxCouponCount >= inputCoupons[count].UserCouponCount) continue;

						// 対象カートに紐づくクーポンが他カートで使用されている場合は使用数の合計を取得
						var useCouponCount = this.CartList.Items.Count(
							cart => (coCart != cart) && ((cart.Coupon != null)
								&& (cart.Coupon.CouponId == inputCoupons[count].CouponId)
								&& (cart.Coupon.CouponNo == inputCoupons[count].CouponNo)));

						// 他のカートでクーポンを使用しており、クーポン利用可能枚数以上の場合はコンテニュー
						if (useCouponCount >= inputCoupons[count].UserCouponCount) continue;

						maxCouponCount = inputCoupons[count].UserCouponCount;
						inputCouponsIndex = count;
					}
					foreach (var coAnother in this.CartList.Items)
					{
						if (coCart == coAnother) continue;
						if ((coAnother.Coupon != null)
							&& (coAnother.Coupon.CouponId == inputCoupons[inputCouponsIndex].CouponId))
						{
							couponUseCount++;
						}
					}

					// クーポン利用可能枚数の合計を取得
					var userCouponCount = inputCoupons.Sum(coupon => coupon.UserCouponCount ?? 0);

					// 利用回数を超える場合はエラーメッセージ追加
					if (couponUseCount >= userCouponCount)
					{
						this.ErrorMessages.Add(riCart.ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon, CouponOptionUtility.GetErrorMessage(CouponErrorcode.CouponUsableCountError).Replace("@@ 1 @@", couponCode));
						continue;
					}
				}

				//------------------------------------------------------
				// クーポン有効性チェック
				//------------------------------------------------------
				var errorMessage = CouponOptionUtility.CheckCouponValidWithCart(coCart, inputCoupons[inputCouponsIndex]);
				if (errorMessage != "")
				{
					this.ErrorMessages.Add(riCart.ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon, errorMessage.Replace("@@ 1 @@", couponCode));
					continue;
				}

				//------------------------------------------------------
				// 有効な場合はクーポン情報をカート情報に設定
				//------------------------------------------------------
				coCart.Coupon = new CartCoupon(inputCoupons[inputCouponsIndex]);
				coCart.SelectedCoupon = wddlCoupon.SelectedValue;

				coCart.CalculateWithCartShipping();

				//------------------------------------------------------
				// ポイント金額越えチェック（クーポンを優先して適用）
				//------------------------------------------------------
				// ポイント利用額 > (カート内商品合計金額(税込み) - 会員ランク割引額 - クーポン割引額(商品への割引額 - 定期購入割引額 - 定期会員割引額))
				if (coCart.UsePointPrice >
					(TaxCalculationUtility.GetPriceTaxIncluded(coCart.PriceSubtotal, coCart.PriceSubtotalTax)
						- coCart.MemberRankDiscount
						- coCart.UseCouponPriceForProduct
						- coCart.FixedPurchaseDiscount
						- coCart.FixedPurchaseMemberDiscountAmount)
				)
				{
					wtbOrderPointUse.Text = "0";

					this.ErrorMessages.Add(this.CurrentCartIndex, OrderPage.CartErrorMessages.ErrorKbn.Point, WebMessages.GetMessages(WebMessages.ERRMSG_COUPON_PRICE_SUBTOTAL_MINUS_COUPON_ERROR));
					continue;
				}
			}
		}
	}

	/// <summary>
	/// カート内容一括チェック
	/// </summary>
	/// <remarks>
	/// カート内容をチェックします。
	/// 初回読み込み時、および各イベント入力チェック＆データセット後に実行すべし
	/// </remarks>
	public void CheckCartData()
	{
		OrderErrorcode productError = OrderErrorcode.NoError;

		//------------------------------------------------------
		// カート内セット商品整合性チェック（カート整理）
		//------------------------------------------------------
		this.ErrorMessages.Add(this.CartList.CheckProductSet(true));

		// 商品購入制限チェック
		ProductOrderLimitItemDelete();

		//------------------------------------------------------
		// カート内商品情報削除チェック（カート整理）
		//------------------------------------------------------
		this.ErrorMessages.Add(this.CartList.CheckProductDeleted());

		//------------------------------------------------------
		// 配送種別チェック
		//------------------------------------------------------
		// カート内分割基準変更チェック(UpdateProductCartDivTypeChanges)にて、
		// カート内容が再更新される前にチェック処理を行う
		foreach (CartObject co in this.CartList.Items)
		{
			DataView cartProducts = co.GetCartProductsEither();
			productError = OrderErrorcode.NoError;

			foreach (CartProduct cp in co.Items)
			{
				DataRowView product = OrderCommon.GetCartProductFromDataView(cartProducts, cp.ShopId, cp.ProductId, cp.VariationId, (cp.IsFixedPurchase || cp.IsSubscriptionBox), cp.ProductSaleId);
				if (product != null)
				{
					// ※配送種別チェック
					productError = OrderCommon.CheckProductShippingType(cp, product);

					if (productError != OrderErrorcode.NoError)
					{
						this.ErrorMessages.Add(OrderCommon.GetErrorMessage(productError, cp.ProductJointName));
					}
				}
				// 注文同梱されている場合、注文同梱前のカートの情報がDBに格納され、処理対象のカートが注文同梱後のものとなり不整合がある状態のため
				// カート不整合エラーは無視する
				else if (this.IsOrderCombined == false)
				{
					// 複数ブラウザ上で同じユーザとしてカート投入した場合、
					// カートオブジェクトとＤＢで保持している値に不整合が生じる。
					// ここの処理では、他ユーザで同じカートに投入した商品がある場合、
					// co.GetProduct で nullが返るときがある。
					// その場合PKGとしてはカート不整合エラーと見なす。

					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_NO_ADJUSTMENT);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}

		//------------------------------------------------------
		// カート内分割基準変更チェック（カート整理）
		//------------------------------------------------------
		this.CartList.UpdateProductCartDivTypeChanges();

		var errorProducts = new List<CartProduct>();
		var isProductNotError = false;

		//------------------------------------------------------
		// 各種チェック
		//------------------------------------------------------
		foreach (CartObject coCart in this.CartList.Items)
		{
			foreach (CartProduct cp in coCart.Items)
			{
				var product = GetProduct(
					cp.ShopId,
					cp.ProductId,
					cp.VariationId);
				if (product.Count != 0)
				{
					DataRowView productVariation = product[0];
					isProductNotError = false;

					//------------------------------------------------------
					// １．商品状態チェック（最新の商品情報より判定）
					//------------------------------------------------------
					productError = OrderCommon.CheckProductStatus(productVariation, this.CartList.GetProductCountTotalInCartList(cp), cp.AddCartKbn, this.LoginUserId);
					if (productError != OrderErrorcode.NoError)
					{
						if (productError == OrderErrorcode.ProductNoStockBeforeCart) productError = OrderErrorcode.ProductNoStock;

						// 商品系エラーの場合、エラーメッセージを格納する
						if (HasErrorProduct(errorProducts, cp.ShopId, cp.ProductId, cp.VariationId) == false)
						{
							// 販売可能数量超過エラー
							if (productError == OrderErrorcode.MaxSellQuantityError)
							{
								this.ErrorMessages.Add(
									OrderCommon.GetErrorMessage(
										productError,
										cp.ProductJointName,
										(cp.ProductMaxSellQuantity + 1).ToString()));
							}
							else
							{
								this.ErrorMessages.Add(
									OrderCommon.GetErrorMessage(
										productError,
										cp.ProductJointName,
										MemberRankOptionUtility.GetMemberRankName((string)productVariation[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK])));
							}
						}

						isProductNotError = true;
					}

					// ２．商品セールチェック
					if (productError == OrderErrorcode.NoError)
					{
						// ２.１商品セール期間チェック
						productError = OrderCommon.CheckProductSalesValid(cp);
						if (productError == OrderErrorcode.ProductSalesInvalid)
						{
							this.ErrorMessages.Add(OrderCommon.GetErrorMessage(productError, cp.ProductJointName));

							// 商品セール期間が無効の場合、カートにセールIDをクリアする
							cp.UpdateProductSaleId(coCart.CartId, "");
						}

						// ２.２商品セール変更チェック
						productError = OrderCommon.CheckProductSalesIdChange(cp, productVariation);
						if (productError == OrderErrorcode.ProductSalesChanged)
						{
							this.ErrorMessages.Add(OrderCommon.GetErrorMessage(productError, cp.ProductJointName));

							string validity = StringUtility.ToEmpty(productVariation["validity"]);
							string productSaleId = (validity == "1") ? StringUtility.ToEmpty(productVariation[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]) : "";
							// 商品セールIDが変更の場合、カートにセールIDを更新する
							cp.UpdateProductSaleId(coCart.CartId, productSaleId);
						}
					}

					//------------------------------------------------------
					// ３．カート商品価格整合性チェック（価格整合性など・最新の商品情報より判定）
					//------------------------------------------------------
					if (this.IsOrderCombined == false)
					{
						productError = OrderCommon.CheckProductPrice(cp, productVariation, true);
						if (productError != OrderErrorcode.NoError)
						{
							if (HasErrorProduct(errorProducts, cp.ShopId, cp.ProductId, cp.VariationId) == false)
							{
								this.ErrorMessages.Add(OrderCommon.GetErrorMessage(productError, cp.ProductJointName));
							}

							// 商品価格が更新されているので送料計算する
							// （カート画面の場合はデフォルト配送先で計算する）
							coCart.Calculate(true, isCartItemChanged: true);

							isProductNotError = true;
						}

						if (isProductNotError) errorProducts.Add(cp);
					}
				}
			}

			//------------------------------------------------------
			// ３．支払額合計・利用ポイントチェック
			//------------------------------------------------------
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// ※利用ポイントチェック
				this.ErrorMessages.Add(CheckUsePoint(coCart));
			}
		}

		//------------------------------------------------------
		// ４．ユーザポイント・利用ポイントチェック★共通化？
		//------------------------------------------------------
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			if (this.IsLoggedIn)
			{
				// ポストバック時ではない時
				// Page_Loadメソッド内の初期表示時のみ実行する
				// ※lbNext_Clickメソッド内で入力チェックの内容が重複しているため
				// TODO:ポイント関連ロジック見直し必要
				if (!IsPostBack)
				{
					// 全利用ポイント取得
					decimal totalUsePoint = 0;
					foreach (CartObject co in this.CartList.Items)
					{
						totalUsePoint += co.UsePoint;
					}

					// 本ポイントがマイナスだとポイントチェックでエラーが発生するので、0補正する
					decimal userPointCorrectedZero = (this.LoginUserPointUsable > 0) ? this.LoginUserPointUsable : 0;

					// 本ポイント < 利用ポイント
					if (userPointCorrectedZero < totalUsePoint)
					{
						this.ErrorMessages.Add(this.CurrentCartIndex, OrderPage.CartErrorMessages.ErrorKbn.Point, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_USE_MAX_ERROR).Replace("@@ 1 @@", StringUtility.ToNumeric(this.LoginUserPointUsable) + Constants.CONST_UNIT_POINT_PT));
					}
				}
			}
		}

		//------------------------------------------------------
		// ５．クーポン情報チェック
		//------------------------------------------------------
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			foreach (CartObject coCart in this.CartList.Items)
			{
				if (coCart.Coupon == null)
				{
					continue;
				}

				//------------------------------------------------------
				// クーポン情報取得(※クーポンID、枝番に該当)
				//------------------------------------------------------
				var coupons = new CouponService().GetAllUserCouponsFromCouponId(
					coCart.Coupon.DeptId,
					StringUtility.ToEmpty(this.LoginUserId),
					coCart.Coupon.CouponId,
					coCart.Coupon.CouponNo);

				//------------------------------------------------------
				// クーポンコード存在チェック
				//------------------------------------------------------
				var errorMessage = string.Empty;
				if (coupons.Any() == false)
				{
					errorMessage = CouponOptionUtility.GetErrorMessage(CouponErrorcode.NoCouponError).Replace("@@ 1 @@", coCart.Coupon.CouponCode);
				}

				//------------------------------------------------------
				// 未使用クーポンチェック(回数制限有りクーポンのみ)
				//------------------------------------------------------
				if (string.IsNullOrEmpty(errorMessage))
				{
					var mailAddress = (coCart.Owner != null) ? coCart.Owner.MailAddr : StringUtility.ToEmpty(this.LoginUserMail);
					var couponErrorCode = CouponOptionUtility.CheckUseCoupon(coupons[0], StringUtility.ToEmpty(this.LoginUserId), mailAddress);
					if (couponErrorCode != CouponErrorcode.NoError)
					{
						errorMessage = CouponOptionUtility.GetErrorMessage(couponErrorCode).Replace("@@ 1 @@", coCart.Coupon.CouponCode);
					}
				}

				//------------------------------------------------------
				// クーポン有効性チェック
				//------------------------------------------------------
				if (string.IsNullOrEmpty(errorMessage))
				{
					if (CouponOptionUtility.CheckCouponValidWithCart(coCart, coupons[0]) != "")
					{
						errorMessage = CouponOptionUtility.CheckCouponValidWithCart(coCart, coupons[0]).Replace("@@ 1 @@", coCart.Coupon.CouponCode);
					}
				}

				//------------------------------------------------------
				// クーポン割引額チェック
				// ※カート情報にクーポン情報が設定されているケース場合のみチェックを行う。
				//------------------------------------------------------
				if (string.IsNullOrEmpty(errorMessage))
				{
					var ceCouponErrorCode = CouponOptionUtility.CheckDiscount(coCart.Coupon, coupons[0]);
					if (ceCouponErrorCode != CouponErrorcode.NoError)
					{
						errorMessage = CouponOptionUtility.GetErrorMessage(ceCouponErrorCode).Replace("@@ 1 @@", coCart.Coupon.CouponCode);
					}
				}

				//------------------------------------------------------
				// エラーメッセージ追加
				//------------------------------------------------------
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					this.ErrorMessages.Add(errorMessage);

					// カート情報更新
					coCart.Coupon = null;
					coCart.Calculate(false, false, false, false, 0, this.IsOrderCombined);
				}
			}
		}

		// 定期会員割引が無効な場合、エラーメッセージを表示し定期会員割引額を0にする。
		if (this.IsLoggedIn && IsApplyFixedPurchaseMemberDiscountInvaild()) ProcessForFixedPurchaseMemberDiscountInvaild();

		//------------------------------------------------------
		// 定期購入可能チェック（ユーザー管理レベルにより）
		//------------------------------------------------------
		var cartObject = this.CartList.Items.ToArray();
		foreach (var coCart in cartObject)
		{
			var products = coCart.Items.ToArray();
			foreach (var cp in products)
			{
				var isLimitedUserLevel = CheckFixedPurchaseLimitedUserLevel(cp.ShopId, cp.ProductId);

				if (isLimitedUserLevel)
				{
					this.CartList.DeleteProduct(
						cp.ShopId,
						cp.ProductId,
						cp.VariationId,
						w2.App.Common.Constants.AddCartKbn.FixedPurchase,
						cp.ProductSaleId,
						cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());
				}
			}
		}

		// 定期購入可能チェック（全額無料になってないか）
		if (this.CartList.Items.Any(cart => (cart.HasFixedPurchase && (cart.PriceTotal == 0))))
		{
			this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_FIXED_PURCHASE_TOTAL_FREE_REGISTRATION_FAILURE_ALERT));
		}
	}

	/// <summary>
	/// 頒布会のエラーがあるか
	/// </summary>
	/// <returns>エラーか</returns>
	public bool SubscriptionErrorCheck()
	{
		var result = false;
		var count = 0;
		foreach (var coCart in this.CartList.Items)
		{
			if (((coCart.IsSubscriptionBox == false) && (string.IsNullOrEmpty(coCart.SubscriptionBoxCourseId))) || (coCart.Items.Count == 0))
			{
				this.CartList.Items[count].SubscriptionBoxErrorMsg = string.Empty;
				count++;
				continue;
			}
			var totalAmount = coCart.Items.Sum(x => x.PriceSubtotal);
			var totalItemQuantity = coCart.Items.Sum(product => product.Count);
			this.CartList.Items[count].SubscriptionBoxErrorMsg = OrderCommon.CheckLimitProductOrderForSubscriptionBox(coCart.SubscriptionBoxCourseId, totalItemQuantity);
			this.CartList.Items[count].SubscriptionBoxErrorMsg += OrderCommon.GetSubscriptionBoxProductOfNumberError(coCart.SubscriptionBoxCourseId, coCart.Items.Count, true);
			this.CartList.Items[count].SubscriptionBoxErrorMsg += OrderCommon.GetSubscriptionBoxTotalAmountError(coCart.SubscriptionBoxCourseId, totalAmount);

			if (this.CartList.Items[count].SubscriptionBoxErrorMsg != string.Empty)
			{
				result = true;
			}
			else
			{
				this.CartList.Items[count].SubscriptionBoxErrorMsg = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_ERROR_MESSAGE);
			}
			count++;
		}
		return result;
	}

	/// <summary>
	/// エラー発生商品があるか？
	/// </summary>
	/// <param name="errorProducts">エラー発生商品</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>true：エラー発生商品あり、false：エラー発生商品なし</returns>
	protected bool HasErrorProduct(List<CartProduct> errorProducts, string shopId, string productId, string variationId)
	{
		var hasErrorProduct = errorProducts
			.Any(product => (product.ShopId == shopId) && (product.ProductId == productId) && (product.VariationId == variationId));
		return hasErrorProduct;
	}

	/// <summary>
	/// 入力チェック＆オブジェクトへセット
	/// </summary>
	public void CheckAndSetInputData()
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		// 商品/セット商品数入力項目チェック  ※セット購入制限チェックも行う
		CheckInputDataForCartList();

		// ポイント入力チェック
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			CheckInputDataForPoint();
		}

		//------------------------------------------------------
		// エラーがなければ入力値セット（商品数はDB反映する）
		//------------------------------------------------------
		if (this.ErrorMessages.Count == 0)
		{
			// 商品注文数設定
			SetInputDataForCartList();

			// 利用クーポンセット
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				SetUseCouponData(this.WrCartList);
			}

			// 利用ポイント数セット
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				SetUsePointData(this.WrCartList);
			}

			// 配送方法の更新
			//  ユーザが配送方法を未選択カート内から配送先を決定
			// 「メール便」でカート内がメール便不可の場合はデフォルト配送先で「宅配便」に変更）
			this.CartList.CartListShippingMethodUserUnSelected();

			// 再計算
			foreach (CartObject coCart in this.CartList.Items)
			{
				coCart.Calculate(false, false, false, false, 0, this.IsOrderCombined);
			}
		}
	}

	/// <summary>
	/// 過去に購入した商品がある場合カートから削除する
	/// </summary>
	/// <returns>カート商品削除数</returns>
	public void ProductOrderLimitItemDelete()
	{
		// 以下の場合、処理を抜ける
		// ・商品購入制限なし
		// ・制限利用区分が文言のみ
		// ・ランディングカート（カートリストLPではない）
		if ((Constants.PRODUCT_ORDER_LIMIT_ENABLED == false)
			|| (Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY == Constants.ProductOrderLimitKbn.ProductOrderLimitOff)
			|| (this.CartList.IsLandingCart && (this.IsCartListLp == false)))
		{
			return;
		}

		// 制限対象商品をカートから削除
		// 過去注文でのチェック
		ProductOrderLimitItemDelete(false);
		// カート間でのチェック
		ProductOrderLimitItemDelete(true);
	}

	/// <summary>
	/// 過去に購入した商品がある場合カートから削除する
	/// </summary>
	/// <param name="isCart">カート間の制限か</param>
	/// <returns>カート商品削除数</returns>
	private void ProductOrderLimitItemDelete(bool isCart)
	{
		var cartObjects = new List<CartObject>();
		foreach (CartObject lco in this.CartList.Items)
		{
			cartObjects.Add(lco);
		}

		var productNames = new List<string>();
		var orderService = new OrderService();
		var deleteProductList = new List<string>();
		var duplicateProductNames = new List<string>();
		foreach (CartObject co in cartObjects)
		{
			//購入制限があるかチェック（無い場合次の商品へ）
			if (co.IsOrderLimit == false) continue;

			var limitProducts = co.Items.Where(product => product.IsOrderLimitProduct).ToArray();
			var productIds = new List<string>();
			foreach (var product in limitProducts)
			{
				var isDuplicate = co.DuplicateLimitProductIds.Contains(product.ProductId);
				if ((isDuplicate == false) && productIds.Contains(product.ProductId)) continue;

				//過去に購入履歴があるかチェック
				var isDelete = isCart
					? (co.HasNotFirstTimeByCart && (deleteProductList.Contains(product.ProductId)))
					: (co.ProductOrderLmitOrderIds
						.Any(orderId => orderService.Get(orderId).Items
							.Select(item => item.ProductId)
							.Contains(product.ProductId))
						|| isDuplicate);
				if (isCart) deleteProductList.Add(product.ProductId);

				if (isDelete)
				{
					//セット商品内の商品の場合
					if (product.ProductSet != null)
					{
						this.CartList.DeleteProductSet(product.ProductSet.ProductSetId, product.ProductSet.ProductSetNo);
						foreach (var set in limitProducts)
						{
							if (set.ProductSet == null) continue;
							if (set.ProductSet.ProductSetId == product.ProductSet.ProductSetId) productIds.Add(set.ProductId);
						}
					}
					else
					{
						var checkDelete = this.CartList.DeleteProduct(
							product.ShopId,
							product.ProductId,
							product.VariationId,
							w2.App.Common.Constants.AddCartKbn.Normal,
							product.ProductSaleId,
							product.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());

						if (checkDelete == false)
						{
							this.CartList.DeleteProduct(
							product.ShopId,
							product.ProductId,
							product.VariationId,
							w2.App.Common.Constants.AddCartKbn.FixedPurchase,
							product.ProductSaleId,
							product.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());
						}
					}
					if (isDuplicate && (duplicateProductNames.Contains(product.ProductName) == false))
					{
						duplicateProductNames.Add(product.ProductName);
					}
					else if (isDuplicate == false)
					{
						productNames.Add("「" + product.ProductName + "」");
					}

					FileLogger.Write(
						"ProductOrderLimit",
						String.Format(
							"ユーザーID:「{0}」は、商品ID:「{1}({2})」を過去に購入しています。(注文ID:{3})",
							string.IsNullOrEmpty(this.LoginUserId) ? "ゲスト" : this.LoginUserId,
							product.ProductId,
							product.ProductName,
							string.Join(",", co.ProductOrderLmitOrderIds)),
						true);
					productIds.Add(product.ProductId);
				}
			}
			co.DuplicateLimitProductIds = new string[0];
			co.ProductOrderLmitOrderIds = new string[0];
		}

		if (productNames.Count > 0)
		{
			this.ErrorMessages.Add(
				string.Format(
					CommerceMessages.GetMessages(
						isCart
							? CommerceMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT_CART
							: CommerceMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT),
					string.Join("", productNames)));
		}

		if (duplicateProductNames.Count > 0)
		{
			this.ErrorMessages.Add(
				string.Format(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_BY_CART),
					string.Join(",", duplicateProductNames)));
		}
	}

	/// <summary>
	/// 定期会員割引無効時の処理実行
	/// </summary>
	private void ProcessForFixedPurchaseMemberDiscountInvaild()
	{
		this.ErrorMessages.Add(MemberRankOptionUtility.GetErrorMessage(MemberRankErrorcode.FixedPurchaseMemberDiscountInvaildError));

		// カートオブジェクトの定期会員割引額、定期会員フラグを再設定
		var cartObjList = this.CartList.Items.Cast<CartObject>().ToList();
		cartObjList.ForEach(cartObj => cartObj.FixedPurchaseMemberDiscountAmount = 0);
		this.LoginUserFixedPurchaseMemberFlg = Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF;
		SetFixedPurchaseMemberFlgForCartObject(cartObjList);
	}

	/// <summary>
	/// カートオブジェクトの定期会員フラグ設定
	/// </summary>
	/// <param name="cartObjList">カートオブジェクトリスト</param>
	public void SetFixedPurchaseMemberFlgForCartObject(List<CartObject> cartObjList)
	{
		cartObjList.ForEach(cartObj => cartObj.IsFixedPurchaseMember = (this.LoginUserFixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON));
	}

	/// <summary>
	/// 頒布会コースIDからカートに投入する商品を取得
	/// </summary>
	/// <param name="courseId">コースID</param>
	public void AddProductsToCartListForSubscriptionBox(string courseId)
	{
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (string.IsNullOrEmpty(courseId) == false))
		{
			var subscriptionBoxService = new SubscriptionBoxService();
			var subscriptionBox = subscriptionBoxService.GetByCourseId(courseId);
			if (subscriptionBox == null)
			{
				this.ErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_ID_INVALID));
				return;
			}

			if (subscriptionBox.ValidFlg == Constants.FLG_SUBSCRIPTIONBOX_VALID_TRUE)
			{
				var dateNow = DateTime.Now;
				var isMeetPeriodOrNumberTime = false;
				switch (subscriptionBox.OrderItemDeterminationType)
				{
					case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
						foreach (var item in subscriptionBox.DefaultOrderProducts)
						{
							if ((item.IsInTerm(dateNow) == false)
								|| subscriptionBox.SelectableProducts.Any(
									sbm => ((sbm.ProductId == item.ProductId)
										&& (sbm.VariationId == item.VariationId)
										&& (sbm.IsInTerm(dateNow) == false)))) continue;

							// 前回商品を引き継ぐで商品を取得できなかった場合はエラー
							if (string.IsNullOrEmpty(item.ProductId)) break;
							var product = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, this.MemberRankId);
							var cartProduct = new CartProduct(product[0], w2.App.Common.Constants.AddCartKbn.SubscriptionBox, "", item.ItemQuantity, true, new ProductOptionSettingList(), string.Empty, null, item.SubscriptionBoxCourseId);
							this.CartList.AddProduct(cartProduct);
							isMeetPeriodOrNumberTime = true;
						}
						break;

					case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
						foreach (var item in subscriptionBox.DefaultOrderProducts)
						{
							if ((item.Count != 1)
								|| subscriptionBox.SelectableProducts.Any(
									sbm => ((sbm.ProductId == item.ProductId)
										&& (sbm.VariationId == item.VariationId)
										&& (sbm.IsInTerm(dateNow) == false)))) continue;
							var product = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, this.MemberRankId);
							var cartProduct = new CartProduct(product[0], w2.App.Common.Constants.AddCartKbn.SubscriptionBox, "", item.ItemQuantity, true, new ProductOptionSettingList(), string.Empty, null, item.SubscriptionBoxCourseId);
							this.CartList.AddProduct(cartProduct);
							isMeetPeriodOrNumberTime = true;
						}
						break;
				}

				if (isMeetPeriodOrNumberTime == false)
				{
					this.ErrorMessages.Add(
						string.Format(
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_NOT_MEET_PERIOD_NUMBERTIME),
							subscriptionBox.DisplayName));
				}
			}
		}
	}

	/// <summary>
	/// 定期会員割引が無効であるか判定
	/// </summary>
	/// <returns>true：定期会員割引無効、false：定期会員割引有効</returns>
	public bool IsApplyFixedPurchaseMemberDiscountInvaild()
	{
		if (this.LoginUserId == null) return false;
		var user = new UserService().Get(this.LoginUserId);
		if (user == null) return true;
		return ((this.LoginUserFixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON)
			&& (user.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF)
			&& (this.CartList.HasFixedPurchase == false));
	}

	/// <summary>
	/// カートオブジェクト検索
	/// </summary>
	/// <param name="objTarget">ターゲットオブジェクト</param>
	/// <returns>カートオブジェクト</returns>
	public CartObject FindCart(object objTarget)
	{
		foreach (CartObject co in this.CartList)
		{
			if (objTarget is CartShipping)
			{
				return ((CartShipping)objTarget).CartObject;
			}
			else if (objTarget is CartProduct)
			{
				foreach (CartProduct cp in co.Items)
				{
					if (cp.Equals(objTarget))
					{
						return co;
					}
				}
			}
			else if (objTarget is CartShipping.ProductCount)
			{
				foreach (CartShipping cs in co.Shippings)
				{
					foreach (CartShipping.ProductCount pc in cs.ProductCounts)
					{
						if (pc.Equals(objTarget))
						{
							return co;
						}
					}
				}
			}
		}
		return null;
	}

	/// <summary>入力チェック用カートID</summary>
	public int CurrentCartIndex
	{
		get
		{
			int currentCartIndex;
			int.TryParse(Convert.ToString(Session[Constants.SESSION_KEY_CURRENT_CART_INDEX]), out currentCartIndex);
			return currentCartIndex;
		}
		set { Session[Constants.SESSION_KEY_CURRENT_CART_INDEX] = value; }
	}
	/// <summary> 注文同梱有無 </summary>
	public bool IsOrderCombined
	{
		get { return (SessionManager.OrderCombineCartList != null); }
	}
	/// <summary> 注文同梱後 決済再選択有無 </summary>
	public bool IsPaymentReselect
	{
		get { return SessionManager.OrderCombinePaymentReselect; }
	}
	/// <summary>カートリストランディングページかどうか</summary>
	public bool IsCartListLp
	{
		get
		{
			var path = StringUtility.ToEmpty(Request.Url.AbsolutePath);
			var beforePath = ((Request.UrlReferrer != null) ? Request.UrlReferrer.AbsolutePath : string.Empty);
			if (Constants.CART_LIST_LP_OPTION
				&& (path.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)
					|| beforePath.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)
					|| this.CartList.LandingCartInputAbsolutePath.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)))
			{
				return true;
			}
			return false;
		}
	}
}
