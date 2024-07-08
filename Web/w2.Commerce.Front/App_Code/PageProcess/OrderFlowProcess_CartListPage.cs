/*
=========================================================================================================
  Module      : 注文フロー（カートリスト）プロセス(OrderFlowProcess_CartListPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Payment;
using w2.Domain.SetPromotion;
using w2.Domain.User;
using w2.Domain.UserDefaultOrderSetting;

/// <summary>
/// OrderLpInputs の概要の説明です
/// </summary>
public partial class OrderFlowProcess
{
	#region カート一覧画面系処理

	/// <summary>
	/// 入力チェック（カート一覧画面）
	/// </summary>
	public void CheckInputDataForCartList()
	{
		//------------------------------------------------------
		// 商品/セット商品数入力項目チェック  ※セット購入制限チェックも行う
		//------------------------------------------------------
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wrCart = GetWrappedControl<WrappedRepeater>(riCart, "rCart");
			var wrCartSetPromotion = GetWrappedControl<WrappedRepeater>(riCart, "rCartSetPromotion");
			var totalProductOrder = 0;
			var coCart = (this.CartList.Items.Count > riCart.ItemIndex) ? this.CartList.GetCart(this.CartList.Items[riCart.ItemIndex].CartId) : null;

			if (coCart != null)
			{
				// セット数チェック用
				Dictionary<string, int> dicProductSetTotalCounts = new Dictionary<string, int>();
				Dictionary<string, CartProductSet> dicProductSet = new Dictionary<string, CartProductSet>();

				foreach (RepeaterItem riProduct in wrCart.Items)
				{
					// ラップ済みコントロール宣言
					var whfUnallocatedQuantity = GetWrappedControl<WrappedHiddenField>(riProduct, "hfUnallocatedQuantity", "0");
					var whfIfSetItem = GetWrappedControl<WrappedHiddenField>(riProduct, "hfIsSetItem", "false");
					var whfProductSetItemNo = GetWrappedControl<WrappedHiddenField>(riProduct, "hfProductSetItemNo", "0");
					var wrProductSet = GetWrappedControl<WrappedRepeater>(riProduct, "rProductSet");
					var wtbProductCount = GetWrappedControl<WrappedTextBox>(riProduct, "tbProductCount", "1");

					if (whfUnallocatedQuantity.Value == "0") continue;

					//------------------------------------------------------
					// チェック対象情報取得
					//------------------------------------------------------
					string strCount = null;
					bool blIsSetItem = bool.Parse(whfIfSetItem.Value);
					int iSetItemNo = int.Parse(whfProductSetItemNo.Value);

					if (blIsSetItem)
					{
						var wtbProductSetCount = GetWrappedControl<WrappedTextBox>(wrProductSet.Items[0], "tbProductSetCount", "0");
						strCount = StringUtility.ToHankaku(wtbProductSetCount.Text);

						if (iSetItemNo != 1)
						{
							continue;
						}
					}
					else
					{
						strCount = StringUtility.ToHankaku(wtbProductCount.Text);
					}

					this.ErrorMessages.Add(riCart.ItemIndex, riProduct.ItemIndex, CheckInputQuantity(strCount));
					var quantityProduct = 0;
					int.TryParse(strCount, out quantityProduct);
					totalProductOrder += quantityProduct;
				} // foreach (RepeaterItem riProduct in rCart.Items)

				// in the case of subscription box: check limit product order
				if (coCart.IsSubscriptionBox && (string.IsNullOrEmpty(coCart.SubscriptionBoxCourseId) == false))
				{
					if (coCart.IsSubscriptionBoxFixedAmount)
					{
						totalProductOrder = coCart.Items.Sum(item => item.Count);
					}
					coCart.SubscriptionBoxErrorMsg = OrderCommon.CheckLimitProductOrderForSubscriptionBox(coCart.SubscriptionBoxCourseId, totalProductOrder);
					coCart.SubscriptionBoxErrorMsg += OrderCommon.GetSubscriptionBoxProductOfNumberError(coCart.SubscriptionBoxCourseId, wrCart.Items.Count, true);
				}

				foreach (RepeaterItem riCartSetPromotion in wrCartSetPromotion.Items)
				{
					var wrCartSetPromotionItem = GetWrappedControl<WrappedRepeater>(riCartSetPromotion, "rCartSetPromotionItem");
					foreach (RepeaterItem riCartSetPromotionItem in wrCartSetPromotionItem.Items)
					{
						var wtbSetPromotionItemCount = GetWrappedControl<WrappedTextBox>(riCartSetPromotionItem, "tbSetPromotionItemCount", "0");
						string quantity = StringUtility.ToHankaku(wtbSetPromotionItemCount.Text);

						this.ErrorMessages.Add(riCart.ItemIndex, riCartSetPromotion.ItemIndex, riCartSetPromotionItem.ItemIndex, CheckInputQuantity(quantity));
					}
				}
			}
		}
	}

	/// <summary>
	/// 数量の入力チェック
	/// </summary>
	/// <param name="quantity">数量（入力値）</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckInputQuantity(string quantity)
	{
		//------------------------------------------------------
		// 半角英数チェック＆文字数3桁チェック
		//------------------------------------------------------
		Hashtable cartInput = new Hashtable()
			{
				{Constants.FIELD_CART_PRODUCT_COUNT, quantity}
			};

		return Validator.Validate("Cart", cartInput);
	}

	/// <summary>
	/// 入力情報をオブジェクトへセット（カート一覧画面）
	/// </summary>
	public void SetInputDataForCartList()
	{
		//------------------------------------------------------
		// 商品注文数設定
		//------------------------------------------------------
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wrCart = GetWrappedControl<WrappedRepeater>(riCart, "rCart");
			var whfSubscriptionBoxCourseId = GetWrappedControl<WrappedHiddenField>(riCart, "hfSubscriptionBoxCourseId", "");
			var wrCartSetPromotion = GetWrappedControl<WrappedRepeater>(riCart, "rCartSetPromotion");

			if (this.CartList.Items.Count <= riCart.ItemIndex) continue;
			var cart = this.CartList.GetCart(this.CartList.Items[riCart.ItemIndex].CartId);
			if (cart == null) continue;

			bool isProductCountChanged = false;

			foreach (RepeaterItem riProduct in wrCart.Items)
			{
				// ラップ済みコントロール宣言
				var whfProductSetItemNo = GetWrappedControl<WrappedHiddenField>(riProduct, "hfProductSetItemNo", "0");
				var whfShopId = GetWrappedControl<WrappedHiddenField>(riProduct, "hfShopId", "");
				var whfProductId = GetWrappedControl<WrappedHiddenField>(riProduct, "hfProductId", "");
				var whfVariationId = GetWrappedControl<WrappedHiddenField>(riProduct, "hfVariationId", "");
				var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(riProduct, "hfIsFixedPurchase", "false");
				var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(riProduct, "hfProductSaleId", "");
				var whfProductSetId = GetWrappedControl<WrappedHiddenField>(riProduct, "hfProductSetId", "");
				var whfProductSetNo = GetWrappedControl<WrappedHiddenField>(riProduct, "hfProductSetNo", "");
				var whfProductOptionValue = GetWrappedControl<WrappedHiddenField>(riProduct, "hfProductOptionValue", "");
				var wrProductSet = GetWrappedControl<WrappedRepeater>(riProduct, "rProductSet");
				var wtbProductCount = GetWrappedControl<WrappedTextBox>(riProduct, "tbProductCount", "1");
				var whfUnallocatedQuantity = GetWrappedControl<WrappedHiddenField>(riProduct, "hfUnallocatedQuantity", "0");
				var whfIsSubscriptionBox = GetWrappedControl<WrappedHiddenField>(riProduct, "hfIsSubscriptionBox", "false");

				string productSetId = whfProductSetId.Value;
				int productSetItemNo = int.Parse(whfProductSetItemNo.Value);
				int unallocatedQuantity = int.Parse(whfUnallocatedQuantity.Value);

				//------------------------------------------------------
				// 通常商品
				//------------------------------------------------------
				if ((productSetId == "") && (unallocatedQuantity != 0))
				{
					// 対象カート商品取得
					CartProduct product = cart.GetProduct(
						whfShopId.Value,
						whfProductId.Value,
						whfVariationId.Value,
						false,
						(
							(Constants.FIXEDPURCHASE_OPTION_ENABLED && bool.Parse(whfIsFixedPurchase.Value))
							|| (Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE && bool.Parse(whfIsFixedPurchase.Value))
						),
						whfProductSaleId.Value,
						whfProductOptionValue.Value,
						"",
						whfSubscriptionBoxCourseId.Value);

					if (product != null)
					{
						// 画面から購入数取得
						string productCountString = StringUtility.ToHankaku(wtbProductCount.Text);

						int productCount;
						if (int.TryParse(productCountString, out productCount) == false)
						{
							productCount = 1;
						}

						// 商品購入制限チェック
						if (product.CountSingle + (productCount - product.QuantitiyUnallocatedToSet) > product.ProductMaxSellQuantity)
						{
							this.ErrorMessages.Add(
								riCart.ItemIndex,
								riProduct.ItemIndex,
								OrderCommon.GetErrorMessage(
									OrderErrorcode.MaxSellQuantityError,
									product.ProductJointName,
									(product.ProductMaxSellQuantity + 1).ToString("N0")));
						}
						// 購入数が変更されていたら更新
						else if (productCount != product.QuantitiyUnallocatedToSet)
						{
							// セットプロモーション情報が変わる可能性があるためあとで再計算する
							product.SetProductCount(cart.CartId, product.CountSingle + (productCount - product.QuantitiyUnallocatedToSet));
							isProductCountChanged = true;
						}
					}
				}

				//------------------------------------------------------
				// （先頭）セット商品
				//------------------------------------------------------
				if (productSetItemNo == 1)
				{
					var wtbProductSetCount = GetWrappedControl<WrappedTextBox>(wrProductSet.Items[0], "tbProductSetCount", "0");

					// 各種情報取得
					string productSetNoString = whfProductSetNo.Value;

					int productSetNo = 0;
					if (int.TryParse(productSetNoString, out productSetNo) == false)
					{
						productSetNo = 0;
					}

					string productSetCountString = StringUtility.ToHankaku(wtbProductSetCount.Text);

					int productSetCount = 0;
					if (int.TryParse(productSetCountString, out productSetCount) == false)
					{
						productSetCount = 1;
					}

					// 削除時に取得できないことがあるため取得してチェック
					CartProductSet productSet = cart.GetProductSet(productSetId, productSetNo);
					if (productSet != null)
					{
						// 商品購入制限チェック
						if (productSetCount > productSet.MaxSellQuantity)
						{
							this.ErrorMessages.Add(
								riCart.ItemIndex,
								riProduct.ItemIndex,
								OrderCommon.GetErrorMessage(
									OrderErrorcode.MaxSellQuantityError,
									productSet.ProductSetName,
									(productSet.MaxSellQuantity + 1).ToString()));
						}
						// 購入数が変更されていたら更新
						else if (productSetCount != productSet.ProductSetCount)
						{
							// セットプロモーション情報が変わる可能性があるためあとで再計算する
							productSet.SetCount(cart.CartId, productSetCount);
							isProductCountChanged = true;
						}
					}
				}
			}

			//------------------------------------------------------
			// セットプロモーション商品
			//------------------------------------------------------
			foreach (RepeaterItem riCartSetPromotion in wrCartSetPromotion.Items)
			{
				var whfCartSetPromotionNo = GetWrappedControl<WrappedHiddenField>(riCartSetPromotion, "hfCartSetPromotionNo", "0");
				var wrCartSetPromotionItem = GetWrappedControl<WrappedRepeater>(riCartSetPromotion, "rCartSetPromotionItem");

				int cartSetPromotionNo = 0;
				int.TryParse(whfCartSetPromotionNo.Value, out cartSetPromotionNo);

				foreach (RepeaterItem riCartSetPromotionItem in wrCartSetPromotionItem.Items)
				{
					var wtbSetPromotionItemCount = GetWrappedControl<WrappedTextBox>(riCartSetPromotionItem, "tbSetPromotionItemCount", "0");
					var whfShopId = GetWrappedControl<WrappedHiddenField>(riCartSetPromotionItem, "hfShopId", "");
					var whfProductId = GetWrappedControl<WrappedHiddenField>(riCartSetPromotionItem, "hfProductId", "");
					var whfVariationId = GetWrappedControl<WrappedHiddenField>(riCartSetPromotionItem, "hfVariationId", "");
					var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(riCartSetPromotionItem, "hfIsFixedPurchase", "false");
					var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(riCartSetPromotionItem, "hfProductSaleId", "");
					var whfProductOptionValue = GetWrappedControl<WrappedHiddenField>(riCartSetPromotionItem, "hfProductOptionValue", "");

					// 対象カート商品取得
					CartProduct product = cart.GetProduct(
						whfShopId.Value,
						whfProductId.Value,
						whfVariationId.Value,
						false,
						(Constants.FIXEDPURCHASE_OPTION_ENABLED && bool.Parse(whfIsFixedPurchase.Value)),
						whfProductSaleId.Value,
						whfProductOptionValue.Value,
						string.Empty,
						whfSubscriptionBoxCourseId.Value);

					// 対象セットの数量取得
					int allocatedQuantity = product.QuantityAllocatedToSet.ContainsKey(cartSetPromotionNo) ? product.QuantityAllocatedToSet[cartSetPromotionNo] : 0;

					// 画面から購入数取得
					string productCountString = StringUtility.ToHankaku(wtbSetPromotionItemCount.Text);

					int productCount;
					if (int.TryParse(productCountString, out productCount) == false)
					{
						productCount = 1;
					}

					// 商品購入制限チェック
					if (product.CountSingle + (productCount - allocatedQuantity) > product.ProductMaxSellQuantity)
					{
						this.ErrorMessages.Add(
							riCart.ItemIndex,
							riCartSetPromotion.ItemIndex,
							riCartSetPromotionItem.ItemIndex,
							OrderCommon.GetErrorMessage(
								OrderErrorcode.MaxSellQuantityError,
								product.ProductJointName,
								(product.ProductMaxSellQuantity + 1).ToString("N0")));
					}
					// 購入数が変更されていたら更新
					else if (productCount != allocatedQuantity)
					{
						// セットプロモーション情報が変わる可能性があるためあとで再計算する
						product.SetProductCount(cart.CartId, product.CountSingle + (productCount - allocatedQuantity));
						isProductCountChanged = true;
					}
				}
			}

			// 購入数が変更されていれば再計算
			if (isProductCountChanged)
			{
				cart.Calculate(true, isCartItemChanged: true);
			}
		}
	}

	/// <summary>
	/// 入力チェック（ポイント）
	/// </summary>
	public void CheckInputDataForPoint()
	{
		// リピータに対してループ
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wtbOrderPointUse = GetWrappedControl<WrappedTextBox>(riCart, "tbOrderPointUse");

			string strUsePointUse = StringUtility.ToHankaku(wtbOrderPointUse.Text);

			// 半角英数チェック＆文字数6桁チェック
			StringBuilder sbError = new StringBuilder();
			sbError.Append(Validator.CheckHalfwidthNumberError("利用ポイント", strUsePointUse));
			sbError.Append(Validator.CheckLengthMaxError("利用ポイント", strUsePointUse, 6));
			if (sbError.Length == 0)
			{
				// 0以上チェック
				sbError.Append(Validator.CheckNumberMinError("利用ポイント", strUsePointUse, 0));
			}
			this.ErrorMessages.Add(riCart.ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Point, sbError.ToString());
		}
	}

	/// <summary>
	/// Amazonペイメントが使えるかどうか
	/// </summary>
	/// <returns>
	/// True：利用可
	/// False：利用不可
	/// </returns>
	public bool CanUseAmazonPayment()
	{
		// オプションOFFなら不可
		if (Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
		{
			return false;
		}

		// 複数カートなら不可
		if (this.CartList.Items.Count != 1)
		{
			return false;
		}

		// 決済設定で無効の場合は不可
		var amazonPay = new PaymentService().Get(this.ShopId, OrderCommon.GetAmazonPayPaymentId());
		if (amazonPay.ValidFlg == Constants.FLG_PAYMENT_INVALID_FLG_VALID)
		{
			return false;
		}

		// ユーザ管理レベル設定で利用不可になっている場合は不可
		if (this.LoginUserId != null)
		{
			var levels = amazonPay.UserManagementLevelNotUse.Split(',');
			if (levels.Contains(new UserService().Get(this.LoginUserId).UserManagementLevelId))
			{
				return false;
			}
		}

		// グローバルオプションがONの場合
		// ・ログイン
		// ・基軸通貨
		// ・ログインユーザーの国
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			if (Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code != "JPY") return false;

			if (this.IsLoggedIn)
			{
				var user = new UserService().Get(this.LoginUserId);
				if ((user != null)
					&& (IsCountryJp(user.AddrCountryIsoCode)) == false) return false;
			}
		}


		// カートオブジェクト単位での判定
		if (this.CartList.Items[0].CanUseAmazonPayment() == false)
		{
			return false;
		}

		// 上記判定でOKなら利用可能
		return true;
	}

	/// <summary>
	/// 表示サイトがＰＣ・スマフォかの判定
	/// </summary>
	/// <returns>
	/// True：利用可
	/// False：利用不可
	/// </returns>
	public bool CanUseAmazonPaymentForFront()
	{
		//Amazonペイメントが使えるかどうか
		if (CanUseAmazonPayment() == false)
		{
			return false;
		}

		//表示サイトがＰＣ・スマフォでない場合は不可
		var amazonPayCv2 = DataCacheControllerFacade
			.GetPaymentCacheController()
			.GetValidAllWithPrice()
			.FirstOrDefault(pl => pl.IsPaymentIdAmazonPayCv2);
		if ((amazonPayCv2 == null)
			|| (amazonPayCv2.MobileDispFlg != Constants.FLG_PAYMENT_MOBILE_DISP_FLG_BOTH_PC_AND_MOBILE))
		{
			return false;
		}

		return true;
	}

	/// <summary>Is Use Atone Payment And Not Yet Card Tran Id</summary>
	public bool IsUseAtonePaymentAndNotYetCardTranId
	{
		get
		{
			var result = this.CartList.Items.Any(item =>
				(item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& string.IsNullOrEmpty(item.Payment.CardTranId));
			return result;
		}
	}
	/// <summary>Is Use Aftee Payment And Not Yet Card Tran Id</summary>
	public bool IsUseAfteePaymentAndNotYetCardTranId
	{
		get
		{
			var result = this.CartList.Items.Any(item =>
				(item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& string.IsNullOrEmpty(item.Payment.CardTranId));
			return result;
		}
	}
	#endregion

	#region CartListPageより
	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	public virtual void rCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		#region ラップ済みコントロール宣言
		var whfShopId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfShopId", "");
		var whfProductId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductId", "");
		var whfVariationId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfVariationId", "");
		var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(e.Item, "hfIsFixedPurchase", "false");	// 旧バージョンはこちらを見る
		var whfhfAddCartKbn = GetWrappedControl<WrappedHiddenField>(e.Item, "hfAddCartKbn", "Normal");
		var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductSaleId", "");
		var whfProductOptionValue = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductOptionValue", "");
		var whfUnallocatedQuantity = GetWrappedControl<WrappedHiddenField>(e.Item, "hfUnallocatedQuantity", "0");
		var whfAllocatedQuantity = GetWrappedControl<WrappedHiddenField>(e.Item, "hfAllocatedQuantity", "0");
		var whfProductSetId = GetWrappedControl<WrappedHiddenField>(e.Item.Parent.Parent, "hfProductSetId", "");
		var whfProductSetNo = GetWrappedControl<WrappedHiddenField>(e.Item.Parent.Parent, "hfProductSetNo", "");
		var wtbCouponCode = GetWrappedControl<WrappedTextBox>(e.Item, "tbCouponCode", "");
		var whfSubscriptionBoxCourseId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfSubscriptionBoxCourseId", "");
		#endregion

		HiddenField hfCartId = (HiddenField)BasePageHelper.GetParentRepeaterItemControl((Repeater)source, "hfCartId");
		string cartId = (hfCartId != null) ? hfCartId.Value : "";

		//------------------------------------------------------
		// 商品情報削除
		//------------------------------------------------------
		if (e.CommandName == "DeleteProduct")
		{
			string strShopId = whfShopId.Value;
			string strProductId = whfProductId.Value;
			string strVariationId = whfVariationId.Value;
			Constants.AddCartKbn addCartKbn = Constants.AddCartKbn.Normal;
			if (whfhfAddCartKbn.InnerControl != null)
			{
				Enum.TryParse<Constants.AddCartKbn>(whfhfAddCartKbn.Value, out addCartKbn);
			}
			else
			{
				addCartKbn = (Constants.FIXEDPURCHASE_OPTION_ENABLED && bool.Parse(whfIsFixedPurchase.Value)) ? Constants.AddCartKbn.FixedPurchase : Constants.AddCartKbn.Normal;
			}
			string strProductSaleId = whfProductSaleId.Value;
			string strProductOptionValue = whfProductOptionValue.Value;

			// 対象カート取得
			CartObject targetCart = this.CartList.GetCart(cartId);

			// 削除対象商品取得
			if (targetCart != null)
			{
				var targetCartProduct = targetCart.GetProduct(strShopId, strProductId, strVariationId, false, bool.Parse(whfIsFixedPurchase.Value), strProductSaleId, strProductOptionValue, string.Empty, whfSubscriptionBoxCourseId.Value);
				if (targetCartProduct != null)
				{
					// 削除数取得
					int deleteQuantity = 0;
					if (whfUnallocatedQuantity.Value != "0")
					{
						deleteQuantity = int.Parse(whfUnallocatedQuantity.Value);
					}
					else
					{
						deleteQuantity = int.Parse(whfAllocatedQuantity.Value);
					}

					if (targetCartProduct.CountSingle > deleteQuantity)
					{
						// 対象商品のカート内の合計数が削除数量より多ければ数量を減らすだけ
						targetCartProduct.SetProductCount(cartId, targetCartProduct.CountSingle - deleteQuantity);
					}
					else
					{
						// カート商品削除（商品数が0になればカート削除）
						this.CartList.DeleteProduct(cartId, strShopId, strProductId, strVariationId, addCartKbn, strProductSaleId, strProductOptionValue);

						// Delete Product Novelty
						if (string.IsNullOrEmpty(targetCartProduct.NoveltyId) == false)
						{
							this.CartList.NoveltyIdsDelete = this.CartList.NoveltyIdsDelete ?? new List<string>();
							this.CartList.NoveltyIdsDelete.Add(targetCartProduct.NoveltyId);
						}
					}

				}

				UpdateFixedPurchaseMemberDiscountAmount(decimal.Zero);
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(targetCart.ShippingType);
				targetCart.CartShippingMethodUserUnSelected(shopShipping);
				targetCart.Calculate(true, isCartItemChanged: true);

				// カートの内容が変更されたため次へボタンの遷移先再設定
				SetNextPageAndSessionInfo();
			}
		}
		//------------------------------------------------------
		// 商品セット情報削除
		//------------------------------------------------------
		else if (e.CommandName == "DeleteProductSet")
		{
			string strProductSetId = whfProductSetId.Value;
			string strProductSetNo = whfProductSetNo.Value;

			// カート商品削除（商品数が0になればカート削除）
			// HACK: デザイン側のhfCartIdを削除されるとカートIDが取得できない
			this.CartList.DeleteProductSet(cartId, strProductSetId, int.Parse(strProductSetNo));
		}
		//------------------------------------------------------
		// クーポン情報削除
		//------------------------------------------------------
		else if (e.CommandName == "DeleteCoupon")
		{
			// クーポンコードのテキストをクリアし、
			// 再計算処理にてクーポン情報のインスタンスを削除
			wtbCouponCode.Text = "";
		}

		//------------------------------------------------------
		// ノベルティ追加
		//------------------------------------------------------
		else if (e.CommandName == "AddNovelty")
		{
			AddNovelty(e.CommandArgument.ToString(), cartId);
		}

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED
			&& (this.CartList.Items.Count == 0))
		{
			this.AuthenticationCode = string.Empty;
			this.CartList.AuthenticationCode = string.Empty;
			this.CartList.HasAuthenticationCode = false;
		}

		//------------------------------------------------------
		// カートチェック
		//------------------------------------------------------
		CheckCartData();

		//------------------------------------------------------
		// 画面更新
		//------------------------------------------------------
		Reload();
	}

	/// <summary>
	/// ノベルティ追加
	/// </summary>
	/// <param name="commandArgument">付与アイテム情報</param>
	/// <param name="cartId">カートID</param>
	protected void AddNovelty(string commandArgument, string cartId)
	{
		// 付与アイテム取得
		var noveltyListIndex = int.Parse(commandArgument.Split(',')[0]);
		var noveltyGrantItemIndex = int.Parse(commandArgument.Split(',')[1]);
		var noveltyGrantItem = this.CartNoveltyList.GetCartNovelty(cartId)[noveltyListIndex].GrantItemList[noveltyGrantItemIndex];

		// カートに追加していない？
		if (this.CartList.Items.SelectMany(cart => cart.Items.Where(item => (item.NoveltyId == noveltyGrantItem.NoveltyId))).Any() == false)
		{
			// 付与アイテムが商品マスタに存在する?
			var product = GetProduct(noveltyGrantItem.ShopId, noveltyGrantItem.ProductId, noveltyGrantItem.VariationId);
			if (product.Count != 0)
			{
				// カート商品（ノベルティID含む）を作成し、カートに追加
				var cartProduct = new CartProduct(product[0], w2.App.Common.Constants.AddCartKbn.Normal, "", 1, true, new ProductOptionSettingList());
				cartProduct.NoveltyId = noveltyGrantItem.NoveltyId;
				this.CartList.AddProduct(cartProduct);
			}
		}
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	public void Reload()
	{
		// 元のカートリピーターがnullの場合はそもそも画面表示しない
		if (this.WrCartList.InnerControl == null) return;

		// カートノベルティリストをセット
		SetCartNovelty();

		// 頒布会のエラーがあるか
		this.IsSubscriptionBoxError = SubscriptionErrorCheck();

		//------------------------------------------------------
		// カートリストデータバインド
		//------------------------------------------------------
		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();

		//------------------------------------------------------
		// Javascriptセット
		//------------------------------------------------------
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			string usePoint = this.CartList.Items[riCart.ItemIndex].UsePoint.ToString();
			string couponCode = (this.CartList.Items[riCart.ItemIndex].Coupon != null) ? this.CartList.Items[riCart.ItemIndex].Coupon.CouponCode : "";
			var wrCart = GetWrappedControl<WrappedRepeater>(riCart, "rCart");
			var wrCartSetPromotion = GetWrappedControl<WrappedRepeater>(riCart, "rCartSetPromotion");
			var wtbOrderPointUse = GetWrappedControl<WrappedTextBox>(riCart, "tbOrderPointUse", usePoint);
			var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode", couponCode);
			var wlbRecalculateCart = GetWrappedControl<WrappedLinkButton>(riCart, "lbRecalculateCart");

			// テキストボックス向けイベント作成
			TextBoxEventScriptCreator eventCreator = new TextBoxEventScriptCreator(wlbRecalculateCart);
			eventCreator.RegisterInitializeScript(this.Page);

			// Create link button events
			var deleteButtonEventCreator = new TextBoxEventScriptCreator();

			foreach (RepeaterItem riProduct in wrCart.Items)
			{
				// 注文数入力ボックス設定
				var wtbProductCount = GetWrappedControl<WrappedTextBox>(riProduct, "tbProductCount", "1");
				var wrProductSet = GetWrappedControl<WrappedRepeater>(riProduct, "rProductSet");

				eventCreator.AddScriptToControl(wtbProductCount);

				// Add script for link delete of product
				deleteButtonEventCreator.AddScriptToControl(GetWrappedControl<WrappedLinkButton>(riProduct, "lbDeleteProduct"));

				// セット商品も
				if (wrProductSet.DataSource != null)
				{
					foreach (RepeaterItem riProductSet in wrProductSet.Items)
					{
						var wtbProductSetCount = GetWrappedControl<WrappedTextBox>(riProductSet, "tbProductSetCount", "0");
						eventCreator.AddScriptToControl(wtbProductSetCount);

						// Add script for link delete of product set
						deleteButtonEventCreator.AddScriptToControl(GetWrappedControl<WrappedLinkButton>(riProductSet, "lbDeleteProductSet"));

						break;
					}
				}
			}

			InitializeCouponComponents(riCart);

			// セットプロモーション商品の注文数入力テキストボックス設定
			foreach (RepeaterItem riSetPromotion in wrCartSetPromotion.Items)
			{
				var wrCartSetPromotionItem = GetWrappedControl<WrappedRepeater>(riSetPromotion, "rCartSetPromotionItem");
				foreach (RepeaterItem riSetPromotionItem in wrCartSetPromotionItem.Items)
				{
					var wtbSetPromotionItemCount = GetWrappedControl<WrappedTextBox>(riSetPromotionItem, "tbSetPromotionItemCount", "0");
					eventCreator.AddScriptToControl(wtbSetPromotionItemCount);

					// Add script for link delete of product set promotion
					deleteButtonEventCreator.AddScriptToControl(GetWrappedControl<WrappedLinkButton>(riSetPromotionItem, "lbDeleteProduct"));
				}
			}

			// ポイント入力ボックス設定
			eventCreator.AddScriptToControl(wtbOrderPointUse);

			// クーポン入力ボックス設定
			eventCreator.AddScriptToControl(wtbCouponCode);
		}

		// セットプロモーション通知
		if (Constants.SETPROMOTION_OPTION_ENABLED && this.CartList.Items.Any(cart => cart.Items.Count() > Constants.SETPROMOTION_MAXIMUM_NUMBER_OF_TARGET_SKUS))
		{
			this.ErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SETPROMOTION_TARGET_SKUS_OVER).Replace("@@ 1 @@", Constants.SETPROMOTION_MAXIMUM_NUMBER_OF_TARGET_SKUS.ToString()));
		}

		//------------------------------------------------------
		// エラーメッセージ表示
		//------------------------------------------------------
		this.ErrorMessage = this.ErrorMessages.Get();

		// 全体用のエラーメッセージを表示用として設定する
		this.DispErrorMessage = this.ErrorMessages.Get();
		if (string.IsNullOrEmpty((string)Session[Constants.SESSION_KEY_ERROR_MSG_FOR_CHANGE_CART]) == false)
		{
			this.DispErrorMessage = (string)Session[Constants.SESSION_KEY_ERROR_MSG_FOR_CHANGE_CART];
			Session[Constants.SESSION_KEY_ERROR_MSG_FOR_CHANGE_CART] = null;
		}

		// 次へボタンの遷移先設定
		SetNextPageAndSessionInfo();

		//画面遷移先とデフォルト注文方法を設定
		SetNextPageAndDefaultOrderSetting();
	}

	/// <summary>
	/// 画面遷移先とデフォルト注文方法を設定
	/// </summary>
	/// <remarks>ReLoad()から分離</remarks>
	private void SetNextPageAndDefaultOrderSetting()
	{
		// 次の画面が同梱画面ではない場合、デフォルト注文方法の再設定を行う
		if ((this.CartList.CartNextPage != Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST) && Constants.TWOCLICK_OPTION_ENABLE)
		{
			// デフォルト注文方法設定があれば、設定に応じた画面遷移先を設定
			var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
			if (userDefaultOrderSetting != null)
			{
				// 一度購入後セッションが続いていると、デフォルト注文方法が取得できなくなるためnullにしておく
				// 決済方法がAmazonPayの場合は、配送先情報を上書きしてしまうのを防ぐため除く
				if ((this.SelectedShippingMethod != null)
					&& (this.IsPaymentIdAmazonPay == false))
				{
					this.SelectedShippingMethod = null;
				}

				new UserDefaultOrderManager(this.CartList, userDefaultOrderSetting, this.IsAddRecmendItem)
					.SetNextPageAndDefaultOrderSetting(this.SelectedShippingMethod != null);
				this.SelectedShippingMethod = (this.CartList.Items.Count != 0)
					? this.CartList.Items[0].Shippings.Select(x => x.ShippingMethod).ToArray()
					: null;
			}
		}
	}

	/// <summary>
	/// カートノベルティリストをセット
	/// </summary>
	/// <returns>カートに変更があったか</returns>
	protected bool SetCartNovelty()
	{
		var isCartChanged = false;
		if (Constants.NOVELTY_OPTION_ENABLED)
		{
			// カートノベルティリスト作成
			var cartNoveltyList = new CartNoveltyList(this.CartList);

			// Add Product Grant Novelty
			var cartItemCountBefore = this.CartList.Items.Count;
			AddProductGrantNovelty(cartNoveltyList);

			// For case add Novelty to other cart
			if (cartItemCountBefore != this.CartList.Items.Count)
			{
				cartNoveltyList = new CartNoveltyList(this.CartList);
				isCartChanged = true;
			}

			// 付与条件外のカート内の付与アイテムを削除
			this.CartList.RemoveNoveltyGrantItem(cartNoveltyList);
			// カートに追加された付与アイテムを含むカートノベルティを削除
			cartNoveltyList.RemoveCartNovelty(this.CartList);

			this.CartNoveltyList = cartNoveltyList;
		}
		return isCartChanged;
	}

	/// <summary>
	/// 再計算リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public virtual void lbRecalculate_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 入力中カートINDEXセット
		//------------------------------------------------------
		this.CurrentCartIndex = int.Parse(((LinkButton)sender).CommandArgument);

		//------------------------------------------------------
		// 入力チェックし、エラーがなければその他チェック＆オブジェクトへセット
		//------------------------------------------------------
		CheckAndSetInputData();

		//------------------------------------------------------
		// カートチェック（入力チェックNGの場合はカートチェックまではしない）
		//------------------------------------------------------
		if (this.ErrorMessages.Count == 0)
		{
			CheckCartData();
		}

		//------------------------------------------------------
		// 画面更新
		//------------------------------------------------------
		Reload();
	}

	/// <summary>
	/// カートコピーボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbCopyCart_Click(object sender, System.EventArgs e)
	{
		this.CurrentCartIndex = int.Parse(((LinkButton)sender).CommandArgument);

		var currentCart = this.CartList.Items[this.CurrentCartIndex];
		var copyCart = currentCart.Copy();
		var errorMessage = new StringBuilder();
		var errorProducts = new List<CartProduct>();

		// コピー先のカートに商品追加処理
		foreach (var cartProduct in copyCart.Items)
		{
			// 商品系エラーがる場合、スキップ
			if (HasErrorProduct(errorProducts, cartProduct.ShopId, cartProduct.ProductId, cartProduct.VariationId))
			{
				continue;
			}

			// コピー先の商品情報チェック
			var productErrorMessage = CheckProductCopy(currentCart, cartProduct);

			// コピー先の商品情報でエラーがある場合、エラーメッセージを格納してスキップ
			if (string.IsNullOrEmpty(productErrorMessage) == false)
			{
				errorMessage.Append(productErrorMessage);
				errorProducts.Add(cartProduct);

				continue;
			}

			copyCart.Add(cartProduct, true);
		}

		// エラーがない場合、カートコピーを実行する
		if (string.IsNullOrEmpty(copyCart.CheckProductSet(false)) && (errorMessage.Length == 0))
		{
			this.CartList.Items.Insert(this.CurrentCartIndex + 1, copyCart);
		}

		CheckCartData();

		// カートコピー、削除完了メッセージ初期化
		ResetCartCopyAndDeleteCompleteMessage();

		// エラーがない場合、カートコピー完了メッセージを表示
		this.ErrorMessages.Add(errorMessage.ToString());
		if (this.ErrorMessages.Count == 0) this.CartList.Items[this.CurrentCartIndex].IsCartCopyCompleteMesseges = true;

		Reload();
	}

	/// <summary>
	/// カート削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbDeleteCart_Click(object sender, System.EventArgs e)
	{
		this.CurrentCartIndex = int.Parse(((LinkButton)sender).CommandArgument);
		OrderCommon.DeleteCart(this.CartList.Items[this.CurrentCartIndex].CartId);
		this.CartList.Items.RemoveAt(this.CurrentCartIndex);

		// カートコピー、削除完了メッセージ初期化
		ResetCartCopyAndDeleteCompleteMessage();

		// エラーがない場合、カート削除完了メッセージを表示
		if ((this.ErrorMessages.Count == 0) && (this.CartList.Items.Count > 0))
		{
			this.CartList.Items[0].IsCartDeleteCompleteMesseges = true;
		}
	}

	/// <summary>
	/// コピー先の商品情報チェック
	/// </summary>
	/// <param name="cartObj">カート情報</param>
	/// <param name="cartProduct">カート商品</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckProductCopy(CartObject cartObj, CartProduct cartProduct)
	{
		var errorMessage = string.Empty;

		// コピー先の商品情報がない場合、エラーメッセージ表示
		var product = GetProduct(cartProduct.ShopId, cartProduct.ProductId, cartProduct.VariationId);
		if (product.Count == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// カート一覧の商品合計数取得
		var productCount
			= (this.CartList.GetProductCountTotalInCartList(cartProduct) + this.CartList.ProductCountTotalInCart(cartObj, cartProduct));

		// 商品在庫エラーまたは商品販売可能数量エラーの場合、エラーメッセージ表示
		var productError = OrderCommon.CheckProductStatus(product[0], productCount, cartProduct.AddCartKbn, this.LoginUserId);
		if ((productError == OrderErrorcode.ProductNoStockBeforeCart) || (productError == OrderErrorcode.MaxSellQuantityError))
		{
			errorMessage = OrderCommon.GetErrorMessage(
				productError,
				CreateProductJointName(product[0]),
				(cartProduct.ProductMaxSellQuantity + 1).ToString());
		}

		// 商品系エラーがある場合、エラーメッセージ表示
		if ((string.IsNullOrEmpty(errorMessage) && (productError != OrderErrorcode.NoError)))
		{
			errorMessage = OrderCommon.GetErrorMessage(productError, CreateProductJointName(product[0]));
		}

		return errorMessage;
	}

	/// <summary>
	/// カートコピー、削除完了メッセージ初期化
	/// </summary>
	private void ResetCartCopyAndDeleteCompleteMessage()
	{
		this.CartList.Items.ToList().ForEach(cart => cart.IsCartCopyCompleteMesseges = false);
		this.CartList.Items.ToList().ForEach(cart => cart.IsCartDeleteCompleteMesseges = false);
	}

	/// <summary>
	/// 対象商品を含むセットプロモーションを取得
	/// </summary>
	/// <param name="productInfo">カート商品情報</param>
	/// <returns>対象商品を含むセットプロモーション</returns>
	public SetPromotionModel[] GetSetPromotionByProduct(CartProduct productInfo)
	{
		var setPromotionList = DataCacheControllerFacade.GetSetPromotionCacheController().GetSetPromotionByProduct(
			productInfo,
			this.MemberRankId,
			this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
			this.LoginUserHitTargetListIds);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			setPromotionList = NameTranslationCommon.SetSetPromotionTranslationData(
				setPromotionList,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
		}
		return setPromotionList;
	}

	/// <summary>
	/// カートノベルティ取得
	/// </summary>
	/// <param name="cartId">カートID</param>
	/// <returns>カートノベルティ</returns>
	public CartNovelty[] GetCartNovelty(string cartId)
	{
		if (Constants.NOVELTY_OPTION_ENABLED == false) return new CartNovelty[0];

		return this.CartNoveltyList.GetCartNovelty(cartId);
	}

	/// <summary>
	/// 数量変更可能？
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="product">カート商品</param>
	/// <returns>可能：true、不可：false</returns>
	public bool IsChangeProductCount(CartObject cart, CartProduct product)
	{
		// ギフトカート or ノベルティの場合、不可（false）を返す
		if (cart.IsGift) return false;
		if (Constants.NOVELTY_OPTION_ENABLED && (product.IsNovelty)) return false;

		return true;
	}

	/// <summary>
	/// 定期会員割引額更新
	/// </summary>
	/// <param name="updatePrice">更新金額</param>
	private void UpdateFixedPurchaseMemberDiscountAmount(decimal updatePrice)
	{
		// 非定期会員かつカート内に定期商品が存在しなくなった場合、定期会員割引額を0に更新する
		if ((this.LoginUserFixedPurchaseMemberFlg != Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON)
			&& (this.CartList.HasFixedPurchase == false))
		{
			foreach (CartObject cartObj in this.CartList.Cast<CartObject>().ToList())
			{
				cartObj.FixedPurchaseMemberDiscountAmount = updatePrice;
				cartObj.HasFixedPurchaseForCartSeparation = false;
			}
		}
	}

	/// <summary>
	/// レジへ進むリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public virtual void lbNext_Click(object sender, System.EventArgs e)
	{
		if (this.CartList.Items.Count != 0)
		{
			//------------------------------------------------------
			// カートチェック
			//------------------------------------------------------
			if (CheckCart() == false)
			{
				Reload(); // Reloadしないとエラーが画面へ表示されない
				return;
			}

			//------------------------------------------------------
			// ノベルティ関連処理
			//------------------------------------------------------
			if (Constants.NOVELTY_OPTION_ENABLED)
			{
				var beforeNoveltyCount = 0;
				foreach (var cart in this.CartList.Items)
				{
					beforeNoveltyCount += cart.Items.Count(cartProduct => cartProduct.IsNovelty);
				}

				// カートノベルティリスト作成
				var cartNoveltyList = new CartNoveltyList(this.CartList);
				// 付与条件外のカート内の付与アイテムを削除
				this.CartList.RemoveNoveltyGrantItem(cartNoveltyList);
				// カートに追加された付与アイテムを含むカートノベルティを削除
				cartNoveltyList.RemoveCartNovelty(this.CartList);
				// カートノベルティリストをセット
				this.CartNoveltyList = cartNoveltyList;

				var afterNoveltyCount = 0;
				foreach (var cart in this.CartList.Items)
				{
					afterNoveltyCount += cart.Items.Count(cartProduct => cartProduct.IsNovelty);
				}

				// ノベルティの変更があった場合、画面更新して次画面遷移中止
				if (beforeNoveltyCount != afterNoveltyCount)
				{
					Reload();
					return;
				}
			}

			// カートリストで入力された商品数量の保持
			this.CartList.SetProductCountBeforeDivide();
		}

		// 注文同梱選択画面へ遷移する場合、同梱前のカート情報を保管する
		if (this.CartList.CartNextPage == Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST)
		{
			SessionManager.OrderCombineFromAmazonPayButton = false;
			SessionManager.OrderCombineBeforeCartList = this.CartList;
		}

		// かんたん会員の場合、注文者決定画面へ遷移
		if (this.IsEasyUser)
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_OWNER_DECISION;
		}

		if (SessionManager.IsTwoClickButton) SetNextPageAndDefaultOrderSetting();

		//------------------------------------------------------
		// 配送先入力画面へ遷移
		//------------------------------------------------------
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;

		// 画面遷移(会員配送先入力画面)
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + this.CartList.CartNextPage);
	}

	/// <summary>
	/// カートチェック
	/// </summary>
	/// <returns>チェックOKか</returns>
	public bool CheckCart()
	{
		// 入力チェック
		CheckAndSetInputData();

		// カートチェック
		CheckCartData();

		// エラーの場合はエラーメッセージ表示
		if (string.IsNullOrEmpty(this.ErrorMessages.Get()) == false) return false;
		if (this.ErrorMessages.Count != 0) return false;

		// Check subscription box error
		if (this.CartList.Items.Any(cart => string.IsNullOrEmpty(cart.SubscriptionBoxErrorMsg) == false)) return false;

		return true;
	}

	/// <summary>
	/// 次へボタン遷移先変更およびセッション情報変更
	/// </summary>
	private void SetNextPageAndSessionInfo()
	{
		// 注文同梱されていた場合、元のカート情報に戻す
		if (this.IsOrderCombined)
		{
			this.CartList = SessionManager.CartList;
			SessionManager.OrderCombineCartList = null;
			if (this.CartList.HasOrderLimitProduct == false)
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST;
				return;
			}
		}

		var isCombinableAmazonPay = (this.IsAmazonLoggedIn && Constants.AMAZON_PAYMENT_OPTION_ENABLED);
		var isPossibleOrderCombine = OrderCombineUtility.ExistCombinableOrder(this.LoginUserId, this.CartList, true, isCombinableAmazonPay);
		if (Constants.ORDER_COMBINE_OPTION_ENABLED
			&& OrderCombineUtility.ExistCombinableOrder(this.LoginUserId, this.CartList, true, isCombinableAmazonPay))
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST;
			return;
		}

		if (this.CartList.UserId != "")
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_SHIPPING;
		}
		else
		{
			var url = new UrlCreator(Constants.PAGE_FRONT_ORDER_OWNER_DECISION)
				.AddParam(Constants.REQUEST_KEY_NEXT_URL, Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING)
				.CreateUrl();

			this.CartList.CartNextPage = url;
		}
	}

	/// <summary>
	/// カート一覧ページ向けペイパル認証完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void PayPalAuthCompleteForCartList(object sender, EventArgs e)
	{
		// 認証実行（ユーザーが存在した場合リダイレクトを行う）
		lbPayPalAuthComplete_Click(sender, e);

		// 次へボタン処理
		if (CheckCart() == false)
		{
			Reload(); // Reloadしないとエラーが画面へ表示されない
			return;
		}

		// 新しいセッションを生成し、配送先入力画面へ遷移
		RedirectToOrderShippingWithNewSession();
	}

	/// <summary>
	/// 新しいセッションを生成し、配送先入力画面へ遷移
	/// </summary>
	public void RedirectToOrderShippingWithNewSession()
	{
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		this.Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

		// セッション張り直しのためのデータ格納（セッションハイジャック対策）
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(this.Request, this.Response, this.Session);

		// 正しくログインしていたら元の画面へ遷移（遷移先をパラメータで渡す）
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_RESTORE_SESSION)
			.AddParam(Constants.REQUEST_KEY_NEXT_URL, this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING)
			.AddParam(Constants.REQUEST_KEY_LOGIN_FLG, "1")
			.CreateUrl();
		this.Response.Redirect(url);
	}

	/// <summary>
	/// Add Product Grant Novelty When Auto Additional Flag is ON
	/// </summary>
	/// <param name="cartNoveltyList">Cart Novelty List</param>
	public void AddProductGrantNovelty(CartNoveltyList cartNoveltyList)
	{
		var cartList = ObjectUtility.DeepCopy<CartObjectList>(this.CartList);

		foreach (var cart in cartList.Items)
		{
			var noveltyForCart = cartNoveltyList.GetCartNovelty(cart.CartId);
			var cartNoveltyItem = noveltyForCart
				.Where(item => ((item.GrantItemList.Length > 0)
					&& (item.AutoAdditionalFlg == Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_VALID)));

			foreach (var noveltyItem in cartNoveltyItem)
			{
				var noveltyGrantItem = noveltyItem.GrantItemList[0];

				// Exists NoveltyId in Any cart
				if (this.CartList.Items.SelectMany(anyCart => anyCart.Items
						.Where(item => (item.NoveltyId == noveltyGrantItem.NoveltyId))).Any()
					|| ((cartList.NoveltyIdsDelete != null)
						&& cartList.NoveltyIdsDelete.Contains(noveltyGrantItem.NoveltyId))) continue;

				// Exists Product Novelty has added
				if (this.CartList.Items.Any(cartItem => cartItem.Items.Where(cartProduct =>
					(string.IsNullOrEmpty(cartProduct.NoveltyId) == false)
					&& (cartProduct.ProductId == noveltyGrantItem.ProductId)
					&& (cartProduct.VariationId == noveltyGrantItem.VariationId)).Any())) continue;

				if (cart.IsProductNoveltyHasDelete(
					noveltyGrantItem.ProductId,
					noveltyGrantItem.VariationId,
					cartList.NoveltyIdsDelete,
					cartNoveltyItem.ToArray())) continue;

				var product = GetProduct(
					noveltyGrantItem.ShopId,
					noveltyGrantItem.ProductId,
					noveltyGrantItem.VariationId);
				if (product.Count != 0)
				{
					// カート商品（ノベルティID含む）を作成し、カートに追加
					var cartProduct = new CartProduct(
						product[0],
						w2.App.Common.Constants.AddCartKbn.Normal,
						string.Empty,
						1,
						true,
						new ProductOptionSettingList());
					cartProduct.NoveltyId = noveltyGrantItem.NoveltyId;
					this.CartList.AddProduct(cartProduct);
				}
			}
		}
	}

	#region カート投入アクション系
	/// <summary>
	/// リクエストからカートに商品投入
	/// </summary>
	/// <param name="addCartHttpRequest">カート投入URL情報</param>
	public void AddProductToCartFromRequest(AddCartHttpRequest addCartHttpRequest)
	{
		// 先にカート不整合チェック
		// ※カート投入URL利用時に、DBとプロパティとで商品情報に不整合が存在する場合、単一商品が複数明細に分かれる場合があるため
		foreach (var co in this.CartList.Items)
		{
			var cartProducts = co.GetCartProductsEither();

			foreach (var cp in co.Items)
			{
				var product = OrderCommon.GetCartProductFromDataView(cartProducts, cp.ShopId, cp.ProductId, cp.VariationId, cp.IsFixedPurchase, cp.ProductSaleId);
				if (product == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_NO_ADJUSTMENT);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}

		// 通常または複数一括で投入？
		if (addCartHttpRequest.CartAddUrlType == Constants.KBN_REQUEST_CART_ACTION_ADD)
		{
			foreach (var addCartProduct in addCartHttpRequest.AddCartProducts.Values)
			{
				if (addCartProduct.Accurate == false) continue;

				// 商品カート投入を行い、処理後、エラー内容があれば追加
				var errorMessage = AddProductToCart(
					addCartProduct.ShopId,
					addCartProduct.ProductId,
					addCartProduct.VariationId,
					addCartProduct.AddCartType,
					addCartProduct.ProductCount,
					addCartProduct.ProductOptionValue,
					null,
					"",
					true,
					addCartProduct.SubscriptionBoxCourseId);
				AddErrorMessages(errorMessage);

				addCartProduct.Accurate = (errorMessage == "");
			}
		}
		// 商品セット投入？
		else if (addCartHttpRequest.CartAddUrlType == Constants.KBN_REQUEST_CART_ACTION_ADD_SET)
		{
			var shopId = "";
			var productSetId = "";
			var productSetItems = new List<Dictionary<string, string>>();
			foreach (var addCartProduct in addCartHttpRequest.AddCartProducts.Values)
			{
				if (addCartProduct.ProductCount <= 0) continue;

				shopId = addCartProduct.ShopId;
				productSetId = addCartProduct.ProductSetId;
				var productSetItemInfo = new Dictionary<string, string>
				{
					{ Constants.FIELD_CART_PRODUCT_ID, addCartProduct.ProductId },
					{ Constants.FIELD_CART_VARIATION_ID, addCartProduct.VariationId },
					{ Constants.FIELD_CART_PRODUCT_COUNT, addCartProduct.ProductCount.ToString() }
				};
				productSetItems.Add(productSetItemInfo);
			}

			// 商品カート投入を行い、処理後、エラー内容があれば追加
			var errorMessage =
				AddProductSetToCart(
					shopId,
					productSetId,
					productSetItems,
					true);
			AddErrorMessages(errorMessage);

			// HACK:管理側で設定した商品セットの一部がなくてもカート投入できてしまっている & セット商品すべてのAccurateを変更する必要をなくしたい
			foreach (var addCartProduct in addCartHttpRequest.AddCartProducts.Values)
			{
				addCartProduct.Accurate = (errorMessage == "");
			}
		}
		else if (addCartHttpRequest.CartAddUrlType == Constants.KBN_REQUEST_CART_ACTION_ADD_SUBSCRIPTIONBOX)
		{
			foreach (var addCartProduct in addCartHttpRequest.AddCartProducts.Values)
			{
				if (addCartProduct.Accurate == false) continue;

				// 商品カート投入を行い、処理後、エラー内容があれば追加
				var errorMessage = AddProductToCart(
					addCartProduct.ShopId,
					addCartProduct.ProductId,
					addCartProduct.VariationId,
					Constants.AddCartKbn.SubscriptionBox,
					addCartProduct.ProductCount,
					addCartProduct.ProductOptionValue,
					contentsLog: null,
					timeSaleId: string.Empty,
					addCartUrlFlg: true,
					subscriptionBoxCourseId: addCartProduct.SubscriptionBoxCourseId);
				AddErrorMessages(errorMessage);

				var success = string.IsNullOrEmpty(errorMessage);
				addCartProduct.Accurate = success;

				if (success)
				{
					var targetItem = this.CartList.Items.First(
						cart => cart.SubscriptionBoxCourseId == addCartProduct.SubscriptionBoxCourseId);
					targetItem.IsFirstSelectionSubscriptionBox = true;
				}
			}
		}

		// エラーがない場合にのみ定期配送パターンの設定を試行
		if (string.IsNullOrEmpty(this.ErrorMessages.Get()))
		{
			var productNameDelete = string.Empty;
			foreach (var addCartProduct in addCartHttpRequest.AddCartProducts.Values)
			{
				if (string.IsNullOrEmpty(addCartProduct.FixedPurchaseKbn)
					|| string.IsNullOrEmpty(addCartProduct.FixedPurchaseSetting)
					|| (addCartProduct.IsFixedPurchase == false)) continue;

				foreach (var cart in this.CartList.Items)
				{
					foreach (var cartProduct in cart.Items)
					{
						if ((cartProduct.ProductId == addCartProduct.ProductId)
							&& (this.CartList.IsUpdateFixedPurchaseShippingPattern(
								cart,
								addCartProduct.FixedPurchaseKbn,
								addCartProduct.FixedPurchaseSetting) == false))
						{
							productNameDelete = string.IsNullOrEmpty(productNameDelete)
								? cartProduct.ProductName.Replace(Constants.PRODUCT_FIXED_PURCHASE_STRING, string.Empty)
								: string.Format("{0},{1}",
									productNameDelete,
									cartProduct.ProductName.Replace(Constants.PRODUCT_FIXED_PURCHASE_STRING, string.Empty));
						}
					}
				}

				if ((string.IsNullOrEmpty(productNameDelete) == false) && addCartProduct.Accurate)
				{
					this.CartList.DeleteProduct(
						addCartProduct.ShopId,
						addCartProduct.ProductId,
						addCartProduct.VariationId,
						addCartProduct.AddCartType,
						string.Empty);
				}
			}

			if (string.IsNullOrEmpty(productNameDelete) == false)
			{
				AddErrorMessages(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SPECIFIED_DELIVERY_INTERVAL_NOT_AVAILABLE)
					.Replace("@@ 1 @@", productNameDelete));
			}
		}

		this.CartList.CartListShippingMethodUserUnSelected();
	}

	/// <summary>
	/// ページ全体用のエラーメッセージを追加
	/// </summary>
	/// <param name="errorMessage">追加するエラー内容</param>
	protected void AddErrorMessages(string errorMessage)
	{
		// エラー内容に重複がなければ追加する
		if (this.ErrorMessages.Get().Contains(errorMessage) == false) this.ErrorMessages.Add(errorMessage);
	}
	#endregion

	/// <summary>会員登録ポイント</summary>
	public decimal UserRegistPoint
	{
		get { return (decimal)ViewState["UserRegistPoint"]; }
		set { ViewState["UserRegistPoint"] = value; }
	}
	/// <summary>エラーメッセージ表示用</summary>
	public string DispErrorMessage
	{
		get { return (string)ViewState["ErrorMessages"]; }
		set { ViewState["ErrorMessages"] = value; }
	}
	/// <summary>カートエラーメッセージ表示用</summary>
	public string DispCartErrorMessage { get; set; }
	/// <summary>会員ランクによるカートエラーメッセージ</summary>
	public string DispCartErrorMessageForMemberRank { get; set; }
	/// <summary>カートノベルティリスト</summary>
	public CartNoveltyList CartNoveltyList
	{
		get { return (CartNoveltyList)ViewState["CartNoveltyList"]; }
		set { ViewState["CartNoveltyList"] = value; }
	}
	/// <summary>PayPalショートカット表示するか</summary>
	public bool DispPayPalShortCut
	{
		get { return ((this.IsLoggedIn == false) && (SessionManager.PayPalLoginResult == null)); }
	}
	/// <summary>エラーメッセージ</summary>
	public string ErrorMessage { get; set; }
	/// <summary>カートエラーメッセージ</summary>
	public OrderPage.CartErrorMessages ErrorMessages
	{
		get { return m_cemErrorMessages; }
	}
	private readonly OrderPage.CartErrorMessages m_cemErrorMessages = new OrderPage.CartErrorMessages();
	/// <summary>頒布会商品のエラーが出ているか</summary>
	public bool IsSubscriptionBoxError { get; set; }
	/// <summary>決済種別IDがAmazonPayか</summary>
	public bool IsPaymentIdAmazonPay
	{
		get
		{
			foreach (var item in this.CartList.Items)
			{
				if ((item.Payment != null)
					&& ((Constants.AMAZON_PAYMENT_CV2_ENABLED
						&& (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
					|| (Constants.AMAZON_PAYMENT_OPTION_ENABLED
						&& (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))))
				{
					return true;
				}
			}
			return false;
		}
	}
	#endregion
}
