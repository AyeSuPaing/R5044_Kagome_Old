/*
=========================================================================================================
  Module      : クーポンオプション共通処理クラス(CouponOptionUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Web.Page;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Wrapper;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;

namespace w2.App.Common.Option
{
	///*********************************************************************************************
	/// <summary>クーポンエラーコード</summary>
	///*********************************************************************************************
	public enum CouponErrorcode
	{
		/// <summary>エラーなし</summary>
		NoError,
		/// <summary>利用クーポンなしエラー</summary>
		NoCouponError,
		/// <summary>クーポン重複エラー</summary>
		CouponDuplicationError,
		/// <summary>クーポン利用済エラー</summary>
		CouponUsedError,
		/// <summary>クーポン無効エラー</summary>
		CouponInvalidError,
		/// <summary>クーポン有効期限チェックエラー</summary>
		CouponExpiredError,
		/// <summary>クーポン対象商品チェックエラー</summary>
		CouponTargetProductError,
		/// <summary>クーポン対象外商品チェックエラー</summary>
		CouponExceptionalProductError,
		/// <summary>クーポン購入金額チェックエラー</summary>
		CouponUsablePriceError,
		/// <summary>クーポン割引額変更エラー</summary>
		CouponDiscountPriceChangedError,
		/// <summary>クーポン利用可能回数エラー</summary>
		CouponUsableCountError,
		/// <summary>ブラックリスト型クーポン利用対象チェックエラー</summary>
		BlacklistCouponUseTargetError,
		/// <summary>注文同梱時クーポン適応外の商品あり</summary>
		CouponNotApplicableByOrderCombined
	}

	///*********************************************************************************************
	/// <summary>
	/// クーポンオプションユーティリティ
	/// </summary>
	///*********************************************************************************************
	public class CouponOptionUtility
	{
		/// <summary>会員クーポンチェック用ダミーユーザーID</summary>
		public const string DUMMY_USER_ID_FOR_USER_COUPON_CHECK = "DummyUserId";

		/// <summary>クーポン入力方法 選択</summary>
		public const string COUPON_INPUT_METHOD_SELECT = "Select";
		/// <summary>クーポン入力方法 入力</summary>
		public const string COUPON_INPUT_METHOD_MANUAL_INPUT = "ManualInput";

		#region "共通（妥当性チェック、表示文言ビルド等）"
		/// <summary>
		/// クーポン有効性チェック
		/// </summary>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckCouponValid(UserCouponDetailInfo coupon)
		{
			StringBuilder result = new StringBuilder();

			//------------------------------------------------------
			// 有効フラグチェック
			//------------------------------------------------------
			if (coupon.ValidFlg == Constants.FLG_COUPON_VALID_FLG_INVALID)
			{
				result.Append(GetErrorMessage(CouponErrorcode.CouponInvalidError));
			}

			//------------------------------------------------------
			// 有効期限・期間内チェック
			//------------------------------------------------------
			if (result.Length == 0)
			{
				DateTime now = DateTimeWrapper.Instance.Now;
				if ((coupon.ExpireBgn.GetValueOrDefault() > now) || (now > coupon.ExpireEnd.GetValueOrDefault()))
				{
					result.Append(GetErrorMessage(CouponErrorcode.CouponExpiredError));
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// クーポン利用条件チェック
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="coupon">クーポン情報</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckCouponUseConditions(CartObject cart, UserCouponDetailInfo coupon)
		{
			var errorMessages = new StringBuilder();

			// 対象商品チェック（チラシ・ノベルティは除外して判定）
			if (cart.Items.Where(item => item.ProductType == Constants.FLG_PRODUCT_PRODUCT_TYPE_PRODUCT)
				.Any(cartProduct => IsCouponApplyCartProduct(coupon, cartProduct)) == false)
			{
				// 対象商品が１つも存在しない場合はエラー
				errorMessages.Append(GetErrorMessage(CouponErrorcode.CouponTargetProductError));
			}

			// 利用時の最低購入金額チェック
			if (coupon.UsablePrice.HasValue)
			{
				// 商品合計（割引前の全商品の合計金額）がクーポン利用最低購入金額より小さい場合
				if (cart.PriceSubtotal < coupon.UsablePrice.Value)
				{
					errorMessages.Append(GetErrorMessage(CouponErrorcode.CouponUsablePriceError).Replace("@@ 2 @@", CurrencyManager.ToPrice(coupon.UsablePrice.Value)));
				}
			}

			return errorMessages.ToString();
		}

		/// <summary>
		/// クーポン有効性チェック（カート内容に対するチェックも含む）
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckCouponValidWithCart(CartObject coCart, UserCouponDetailInfo coupon)
		{
			StringBuilder errorMessages = new StringBuilder();

			//------------------------------------------------------
			// クーポン自体の有効性チェック
			//------------------------------------------------------
			errorMessages.Append(CheckCouponValid(coupon));
			if (errorMessages.Length != 0)
			{
				return errorMessages.ToString();
			}

			//------------------------------------------------------
			// 配送無料クーポンの場合
			//------------------------------------------------------
			if (IsFreeShipping(coupon.CouponType))
			{
				//------------------------------------------------------
				// 対象商品チェック
				//------------------------------------------------------
				foreach (CartProduct cp in coCart.Items)
				{
					// 対象商品？
					if (IsCouponApplyCartProduct(coupon, cp) == false)
					{
						errorMessages.Append(GetErrorMessage(CouponErrorcode.CouponExceptionalProductError).Replace("@@ 2 @@", cp.ProductJointName));
					}
				}

				//------------------------------------------------------
				// 利用時の最低購入金額チェック
				//------------------------------------------------------
				// クーポン利用最低購入金額NULL以外の場合
				if (coupon.UsablePrice != null)
				{
					// 商品合計がクーポン利用最低購入金額より小さい場合※会員ランク割引を按分した金額は利用しない。
					if (coCart.PriceSubtotal < coupon.UsablePrice.GetValueOrDefault())
					{
						errorMessages.Append(GetErrorMessage(CouponErrorcode.CouponUsablePriceError).Replace("@@ 2 @@", CurrencyManager.ToPrice(coupon.UsablePrice)));
					}
				}
			}
			//------------------------------------------------------
			// 商品小計への値引き場合
			//------------------------------------------------------
			else
			{
				//------------------------------------------------------
				// 対象商品チェック
				//------------------------------------------------------
				decimal priceSubTotalCoupon = 0;
				bool exceptionalFlg = false;

				// 個別の商品チェック
				var individualProducts = coCart.Items
					.Where(item => (item.IsSubscriptionBoxFixedAmount() == false)
						&& IsCouponApplyCartProduct(coupon, item));

				if (individualProducts.Any())
				{
					exceptionalFlg = true;
					priceSubTotalCoupon += individualProducts.Sum(item => item.PriceSubtotalAfterDistributionForCampaign);
				}

				// 定額頒布会の場合は、すべての商品が対象の場合のみ適用可能
				var subscriptionBoxFixedItems = coCart.Items
					.Where(item => item.IsSubscriptionBoxFixedAmount())
					.GroupBy(item => item.SubscriptionBoxCourseId)
					.Where(sb => sb.All(item => IsCouponApplyCartProduct(coupon, item)));

				if (subscriptionBoxFixedItems.Any())
				{
					exceptionalFlg = true;
					priceSubTotalCoupon += subscriptionBoxFixedItems.Sum(items => items.FirstOrDefault()?.SubscriptionBoxFixedAmount ?? 0m);
				}

				// 対象商品が１つも存在しない場合はエラー
				if (exceptionalFlg == false)
				{
					errorMessages.Append(GetErrorMessage(CouponErrorcode.CouponTargetProductError));
				}

				if (priceSubTotalCoupon < 0)
				{
					priceSubTotalCoupon = 0;
				}

				//------------------------------------------------------
				// 利用時の最低購入金額チェック
				//------------------------------------------------------
				if (errorMessages.Length == 0)
				{
					// クーポン利用最低購入金額NULL以外の場合
					if (coupon.UsablePrice != null)
					{
						// 商品合計(クーポン割引、会員ランク割引適用対象分)がクーポン利用最低購入金額より小さい場合
						if (priceSubTotalCoupon < coupon.UsablePrice.GetValueOrDefault())
						{
							errorMessages.Append(GetErrorMessage(CouponErrorcode.CouponUsablePriceError).Replace("@@ 2 @@", CurrencyManager.ToPrice(coupon.UsablePrice)));
						}
					}
				}
			}

			return errorMessages.ToString();
		}

		/// <summary>
		/// クーポン未使用かチェック
		/// </summary>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <returns>クーポンエラーコード</returns>
		public static CouponErrorcode CheckUseCoupon(UserCouponDetailInfo coupon, string userId, string mailAddress)
		{
			var errorcode = CouponErrorcode.NoError;
			switch (coupon.CouponType)
			{
				case Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING:
					errorcode = (coupon.CouponCount == 0) ? CouponErrorcode.CouponUsableCountError : CouponErrorcode.NoError;
					break;

				case Constants.FLG_COUPONCOUPON_TYPE_USERREGIST:
				case Constants.FLG_COUPONCOUPON_TYPE_BUY:
				case Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY:
				case Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON:
				case Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION:
					errorcode = (coupon.UseFlg == Constants.FLG_USERCOUPON_USE_FLG_USE) ? CouponErrorcode.CouponUsedError : CouponErrorcode.NoError;
					break;

				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL:
				case Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON:
					var isNotRequiredRegistration = 
						((coupon.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL)
						|| (coupon.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL)
						|| (coupon.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON));
					var isUseTarget =
						((Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE != Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
						|| (string.IsNullOrEmpty(mailAddress) == false)
						|| ((isNotRequiredRegistration)
							&& (string.IsNullOrEmpty(mailAddress))
							&& (string.IsNullOrEmpty(userId))));
					if (isUseTarget == false)
					{
						errorcode = CouponErrorcode.BlacklistCouponUseTargetError;
						break;
					}
					var isUsed = DomainFacade.Instance.CouponService.CheckUsedCoupon(coupon.CouponId,
						(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
							? mailAddress
							: userId);
					errorcode = isUsed ? CouponErrorcode.CouponUsedError : CouponErrorcode.NoError;
					break;

				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER:
					errorcode = ((coupon.UseFlg == Constants.FLG_USERCOUPON_USE_FLG_USE)
						|| (coupon.UserCouponCount == null)
						|| (coupon.UserCouponCount == 0)) ? CouponErrorcode.CouponUsedError : CouponErrorcode.NoError;
					break;
			}

			return errorcode;
		}

		/// <summary>
		/// クーポン割引額変更チェック
		/// </summary>
		/// <param name="ccCoupon">カートクーポン情報</param>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <returns>クーポンエラーコード</returns>
		public static CouponErrorcode CheckDiscount(CartCoupon ccCoupon, UserCouponDetailInfo coupon)
		{
			//------------------------------------------------------
			// クーポン割引額の変更があったかチェック
			//------------------------------------------------------
			if ((ccCoupon.DiscountKbn == CartCoupon.CouponDiscountKbn.Price)
				&& ((ccCoupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_INVALID)
					&& ((coupon.DiscountPrice == null)
						|| (ccCoupon.DiscountPrice != coupon.DiscountPrice.GetValueOrDefault()))))
			{
				return CouponErrorcode.CouponDiscountPriceChangedError;
			}

			//------------------------------------------------------
			// クーポン割引率の変更があったかチェック
			//------------------------------------------------------
			if ((ccCoupon.DiscountKbn == CartCoupon.CouponDiscountKbn.Rate)
				&& ((coupon.DiscountRate == null)
					|| (ccCoupon.DiscountRate != coupon.DiscountRate.GetValueOrDefault())))
			{
				return CouponErrorcode.CouponDiscountPriceChangedError;
			}

			return CouponErrorcode.NoError;
		}

		/// <summary>
		/// CartProductを対象にクーポン適用対象商品かチェック
		/// </summary>
		/// <param name="coupon">クーポン情報</param>
		/// <param name="cartProduct">カート商品情報</param>
		/// <returns>クーポン適用対象商品か</returns>
		public static bool IsCouponApplyCartProduct(UserCouponDetailInfo coupon, CartProduct cartProduct)
		{
			return IsCouponApplyCartProduct((CouponModel)coupon, cartProduct);
		}
		/// <summary>
		/// CarProcuctを対象にクーポン適用対象商品かチェック
		/// </summary>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <param name="cartProduct">カート商品情報</param>
		/// <returns>クーポン適用対象商品</returns>
		public static bool IsCouponApplyCartProduct(CouponModel coupon, CartProduct cartProduct)
		{
			var product = new Hashtable()
			{
				{Constants.FIELD_PRODUCT_PRODUCT_ID, cartProduct.ProductId},
				{Constants.FIELD_PRODUCT_ICON_FLG1, cartProduct.IconFlg[0]},
				{Constants.FIELD_PRODUCT_ICON_FLG2, cartProduct.IconFlg[1]},
				{Constants.FIELD_PRODUCT_ICON_FLG3, cartProduct.IconFlg[2]},
				{Constants.FIELD_PRODUCT_ICON_FLG4, cartProduct.IconFlg[3]},
				{Constants.FIELD_PRODUCT_ICON_FLG5, cartProduct.IconFlg[4]},
				{Constants.FIELD_PRODUCT_ICON_FLG6, cartProduct.IconFlg[5]},
				{Constants.FIELD_PRODUCT_ICON_FLG7, cartProduct.IconFlg[6]},
				{Constants.FIELD_PRODUCT_ICON_FLG8, cartProduct.IconFlg[7]},
				{Constants.FIELD_PRODUCT_ICON_FLG9, cartProduct.IconFlg[8]},
				{Constants.FIELD_PRODUCT_ICON_FLG10, cartProduct.IconFlg[9]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END1, cartProduct.IconTermEnd[0]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END2, cartProduct.IconTermEnd[1]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END3, cartProduct.IconTermEnd[2]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END4, cartProduct.IconTermEnd[3]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END5, cartProduct.IconTermEnd[4]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END6, cartProduct.IconTermEnd[5]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END7, cartProduct.IconTermEnd[6]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END8, cartProduct.IconTermEnd[7]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END9, cartProduct.IconTermEnd[8]},
				{Constants.FIELD_PRODUCT_ICON_TERM_END10, cartProduct.IconTermEnd[9]},
				{Constants.FIELD_PRODUCT_BRAND_ID1, cartProduct.BrandId},
				{Constants.FIELD_PRODUCT_BRAND_ID2, cartProduct.BrandId2},
				{Constants.FIELD_PRODUCT_BRAND_ID3, cartProduct.BrandId3},
				{Constants.FIELD_PRODUCT_BRAND_ID4, cartProduct.BrandId4},
				{Constants.FIELD_PRODUCT_BRAND_ID5, cartProduct.BrandId5},
				{Constants.FIELD_PRODUCT_CATEGORY_ID1, cartProduct.CategoryId1},
				{Constants.FIELD_PRODUCT_CATEGORY_ID2, cartProduct.CategoryId2},
				{Constants.FIELD_PRODUCT_CATEGORY_ID3, cartProduct.CategoryId3},
				{Constants.FIELD_PRODUCT_CATEGORY_ID4, cartProduct.CategoryId4},
				{Constants.FIELD_PRODUCT_CATEGORY_ID5, cartProduct.CategoryId5},
			};
			var result = IsCouponApplyProduct(coupon, product);
			return result;
		}

		/// <summary>
		/// クーポン適用対象商品かチェック
		/// </summary>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <param name="product">商品情報</param>
		/// <returns>クーポン適用対象商品</returns>
		public static bool IsCouponApplyProduct(UserCouponDetailInfo coupon, Object product)
		{
			return IsCouponApplyProduct((CouponModel)coupon, product);
		}
		/// <summary>
		/// クーポン適用対象商品かチェック
		/// </summary>
		/// <param name="coupon">クーポン情報</param>
		/// <param name="product">商品情報</param>
		/// <returns>クーポン適用対象商品か</returns>
		public static bool IsCouponApplyProduct(CouponModel coupon, object product)
		{
			// 商品情報が取得できなかった場合は処理を抜ける
			if (product == null) return false;

			if (coupon.ProductKbn != Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND)
			{
				// クーポン例外商品アイコンに含まれているかチェック
				var result = CheckProductExistExceptionalIcon(coupon, product);

				// クーポン例外商品に含まれているかチェック
				if (result == false)
				{
					result = CheckProductExistExceptionalProductId(
						(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID),
						coupon.ExceptionalProduct);
				}

				// クーポン対象商品区分が「全ての商品を対象」の場合は、対象外商品
				if (coupon.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_TARGET)
				{
					result = (result == false);
				}

				return result;
			}
			else
			{
				// クーポン例外商品に含まれているかチェック
				var result = (CheckProductExistExceptionalProductId(
						(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID),
						coupon.ExceptionalProduct)
					== false);

				// クーポン例外商品アイコンに含まれているかチェック
				if (result && (coupon.ExceptionalIcon > 0))
				{
					result = CheckProductExistExceptionalIcon(coupon, product);
				}

				// クーポン例外カテゴリ/クーポン例外ブランドに含まれているかチェック
				if (result)
				{
					if ((string.IsNullOrEmpty(coupon.ExceptionalBrandIds)) == false)
					{
						result = CheckProductExistExceptionalBrand(product, coupon.ExceptionalBrandIds);
					}
					if (result && (string.IsNullOrEmpty(coupon.ExceptionalProductCategoryIds)) == false)
					{
						result = CheckProductExistExceptionalProductCategory(product, coupon.ExceptionalProductCategoryIds);
					}
				}

				return result;
			}
		}

		/// <summary>
		/// 商品が例外キャンペーンアイコンに存在することを確認
		/// </summary>
		/// <param name="coupon">クーポン</param>
		/// <param name="product">商品</param>
		/// <returns>存在するか</returns>
		private static bool CheckProductExistExceptionalIcon(CouponModel coupon, object product)
		{
			var result = (((coupon.ExceptionalIcon1 == Constants.FLG_COUPON_EXCEPTIONAL_ICON1)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG1) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END1))))
				||
				((coupon.ExceptionalIcon2 == Constants.FLG_COUPON_EXCEPTIONAL_ICON2)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG2) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END2))))
				||
				((coupon.ExceptionalIcon3 == Constants.FLG_COUPON_EXCEPTIONAL_ICON3)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG3) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END3))))
				||
				((coupon.ExceptionalIcon4 == Constants.FLG_COUPON_EXCEPTIONAL_ICON4)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG4) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END4))))
				||
				((coupon.ExceptionalIcon5 == Constants.FLG_COUPON_EXCEPTIONAL_ICON5)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG5) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END5))))
				||
				((coupon.ExceptionalIcon6 == Constants.FLG_COUPON_EXCEPTIONAL_ICON6)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG6) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END6))))
				||
				((coupon.ExceptionalIcon7 == Constants.FLG_COUPON_EXCEPTIONAL_ICON7)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG7) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END7))))
				||
				((coupon.ExceptionalIcon8 == Constants.FLG_COUPON_EXCEPTIONAL_ICON8)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG8) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END8))))
				||
				((coupon.ExceptionalIcon9 == Constants.FLG_COUPON_EXCEPTIONAL_ICON9)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG9) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END9))))
				||
				((coupon.ExceptionalIcon10 == Constants.FLG_COUPON_EXCEPTIONAL_ICON10)
					&& (((string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_FLG10) == Constants.FLG_PRODUCT_ICON_ON)
						&& (DateTimeWrapper.Instance.Now < (DateTime)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_ICON_TERM_END10)))));
			return result;
		}

		/// <summary>
		/// 商品が例外商品であるかかを確認
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="exceptionalProductIds">例外商品ID</param>
		/// <returns>例外商品か</returns>
		private static bool CheckProductExistExceptionalProductId(string productId, string exceptionalProductIds)
		{
			foreach (string exceptProductId in exceptionalProductIds.Split(','))
			{
				if (productId == exceptProductId)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 商品が例外カテゴリに存在することを確認
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="exceptionalProductCategoryIds">例外カテゴリID</param>
		/// <returns>存在するか</returns>
		private static bool CheckProductExistExceptionalProductCategory(object product, string exceptionalProductCategoryIds)
		{
			var targetProductCategoryIds = exceptionalProductCategoryIds.Trim().Split(',');
			var productCategoryIds = new[]
			{
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_CATEGORY_ID1),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_CATEGORY_ID2),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_CATEGORY_ID3),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_CATEGORY_ID4),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_CATEGORY_ID5)
			};

			var result = targetProductCategoryIds.Any(
				targetProductCategoryId => productCategoryIds.Any(
					productCategoryId => IsTargetCategory(productCategoryId, targetProductCategoryId)));
			return result;
		}

		/// <summary>
		/// 商品がカテゴリIDに該当するか
		/// </summary>
		/// <param name="productCategoryId">商品カテゴリID</param>
		/// <param name="targetProductCategoryIds">指定カテゴリID</param>
		/// <returns>該当するか</returns>
		private static bool IsTargetCategory(string productCategoryId, string targetProductCategoryIds)
		{
			// 商品にカテゴリID指定がない？
			if (string.IsNullOrEmpty(productCategoryId)) return false;

			// カテゴリIDが同じ？
			if (targetProductCategoryIds == productCategoryId) return true;

			// カート商品のカテゴリIDが子カテゴリである？
			if (productCategoryId.Length > targetProductCategoryIds.Length)
			{
				return productCategoryId.StartsWith(targetProductCategoryIds);
			}

			return false;
		}

		/// <summary>
		/// 商品が例外ブランドに存在することを確認
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="exceptionalBrandIds">例外ブランドID</param>
		/// <returns>存在するか</returns>
		private static bool CheckProductExistExceptionalBrand(object product, string exceptionalBrandIds)
		{
			var targetBrandIds = exceptionalBrandIds.Trim().Split(',');
			var productBrandIds = new[]
			{
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_BRAND_ID1),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_BRAND_ID2),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_BRAND_ID3),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_BRAND_ID4),
				(string)ProductCommon.GetKeyValue(product, Constants.FIELD_PRODUCT_BRAND_ID5)
			};

			var result = targetBrandIds.Any(targetBrandId => productBrandIds.Contains(targetBrandId));
			return result;
		}

		/// <summary>
		/// 指定されたエラーコードからエラーメッセージ取得
		/// </summary>
		/// <param name="errorCode">クーポンエラーコード</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(CouponErrorcode errorCode)
		{
			string messageKey = "";
			switch (errorCode)
			{
				// 利用クーポンなしエラー
				case CouponErrorcode.NoCouponError:
					messageKey = CommerceMessages.ERRMSG_COUPON_NO_COUPON_ERROR;
					break;

				// クーポン重複エラー
				case CouponErrorcode.CouponDuplicationError:
					messageKey = CommerceMessages.ERRMSG_COUPON_DUPLICATION_ERROR;
					break;

				// クーポン利用済エラー
				case CouponErrorcode.CouponUsedError:
					messageKey = CommerceMessages.ERRMSG_COUPON_USED_ERROR;
					break;

				// クーポン無効エラー
				case CouponErrorcode.CouponInvalidError:
					messageKey = CommerceMessages.ERRMSG_COUPON_INVALID_ERROR;
					break;

				// クーポン有効期限チェックエラー
				case CouponErrorcode.CouponExpiredError:
					messageKey = CommerceMessages.ERRMSG_COUPON_EXPIRED_ERROR;
					break;

				// クーポン対象商品チェックエラー
				case CouponErrorcode.CouponTargetProductError:
					messageKey = CommerceMessages.ERRMSG_COUPON_TARGET_PRODUCT_ERROR;
					break;

				// クーポン対象外商品チェックエラー
				case CouponErrorcode.CouponExceptionalProductError:
					messageKey = CommerceMessages.ERRMSG_COUPON_EXCEPTIONAL_PRODUCT_ERROR;
					break;

				// クーポン購入金額チェックエラー
				case CouponErrorcode.CouponUsablePriceError:
					messageKey = CommerceMessages.ERRMSG_COUPON_USABLE_PRICE_ERROR;
					break;

				// クーポン割引額変更エラー
				case CouponErrorcode.CouponDiscountPriceChangedError:
					messageKey = CommerceMessages.ERRMSG_COUPON_DISCOUNT_PRICE_CHANGED_ERROR;
					break;

				// クーポン利用可能回数エラー
				case CouponErrorcode.CouponUsableCountError:
					messageKey = CommerceMessages.ERRMSG_COUPON_USABLE_COUNT_ERROR;
					break;

				// ブラックリスト型クーポン利用対象チェックエラー
				case CouponErrorcode.BlacklistCouponUseTargetError:
					messageKey = CommerceMessages.ERRMSG_BLACKLIST_COUPON_USE_TARGET_ERROR;
					break;

				// 注文同梱時クーポン適応外の商品あり
				case CouponErrorcode.CouponNotApplicableByOrderCombined:
					messageKey = CommerceMessages.ERRMSG_COUPON_NOT_APPLICABLE_BY_ORDER_COMBINED;
					break;
			}

			return CommerceMessages.GetMessages(messageKey);
		}

		/// <summary>
		/// クーポン割引文字列取得
		/// </summary>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <returns>クーポン割引文字列</returns>
		public static string GetCouponDiscountString(UserCouponDetailInfo coupon)
		{
			var freeShippingMessage = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_COUPON_LIST,
				Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
				Constants.VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING);
			var discountPriceMessage = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_COUPON_LIST,
				Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
				Constants.VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE);
			if (coupon.DiscountPrice != null)
			{
				if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
					return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToPrice(coupon.DiscountPrice);
				return StringUtility.ToPrice(coupon.DiscountPrice);
			}
			if (coupon.DiscountRate != null)
			{
				if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
					return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(coupon.DiscountRate) + "%";
				return StringUtility.ToNumeric(coupon.DiscountRate) + "%";
			}
			if (IsFreeShipping(coupon.CouponType) || (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
			{
				return freeShippingMessage;
			}
			return "-";
		}
		/// <summary>
		/// クーポン割引文字列取得
		/// </summary>
		/// <param name="cartCoupon">カートクーポン</param>
		/// <returns>クーポン割引文字列</returns>
		public static string GetCouponDiscountString(CartCoupon cartCoupon)
		{
			if (cartCoupon == null) return "-";
			var freeShippingMessage = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_COUPON_LIST,
				Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
				Constants.VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING);
			var discountPriceMessage = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_COUPON_LIST,
				Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
				Constants.VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE);
			switch (cartCoupon.DiscountKbn)
			{
				case CartCoupon.CouponDiscountKbn.FreeShipping:
					return freeShippingMessage;

				case CartCoupon.CouponDiscountKbn.Price:
					if (cartCoupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
						return freeShippingMessage + "\n" + discountPriceMessage + cartCoupon.DiscountPrice.ToPriceString(true);
					return cartCoupon.DiscountPrice.ToPriceString(true);

				case CartCoupon.CouponDiscountKbn.Rate:
					if (cartCoupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
						return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(cartCoupon.DiscountRate) + "%";
					return StringUtility.ToNumeric(cartCoupon.DiscountRate) + "%";
			}
			return "-";
		}

		/// <summary>
		/// 新規クーポンID生成
		/// </summary>
		/// <returns>新規クーポンID</returns>
		public static string CreateNewCouponId()
		{
			return NumberingUtility.CreateNewNumber(Constants.CONST_DEFAULT_SHOP_ID, Constants.NUMBER_KEY_COUPON_ID).ToString().PadLeft(Constants.CONST_COUPON_ID_LENGTH, '0');
		}
		#endregion

		/// <summary>
		/// 有効期限開始日が指定日時以降・有効期限終了日が指定日時以降のクーポン取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="expireDateTimeBegin">有効期限開始日時</param>
		/// <param name="expireDateTimeEnd">有効期限終了日時</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="referralCode">Referral code</param>
		/// <returns>クーポン情報</returns>
		public static UserCouponDetailInfo[] GetUserUsableCouponsSpecificExpireDate(
			string userId,
			string mailAddress,
			DateTime expireDateTimeBegin,
			DateTime expireDateTimeEnd,
			string languageCode = null,
			string languageLocaleId = null,
			string referralCode = "")
		{
			var userCoupons = new CouponService().GetAllUserCouponsSpecificExpireDate(userId, Constants.W2MP_DEPT_ID, expireDateTimeBegin, expireDateTimeEnd);
			var userUsableCoupons = userCoupons
				.Where(cp => ((StringUtility.ToEmpty(cp.ValidFlg) == Constants.FLG_COUPON_VALID_FLG_VALID) && (CheckUseCoupon(cp, userId, mailAddress) == CouponErrorcode.NoError)))
				.ToArray();

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報を設定
				userUsableCoupons = NameTranslationCommon.SetCouponTranslationData(userUsableCoupons, languageCode, languageLocaleId);
			}

			var hasUserIdReferred = DomainFacade.Instance.UserService.GetReferredUserId(userId);

			if (string.IsNullOrEmpty(hasUserIdReferred)
				&& string.IsNullOrEmpty(referralCode))
			{
				userUsableCoupons = userUsableCoupons
					.Where(userUsableCoupon => (userUsableCoupon.CouponType != Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON))
					.ToArray();
			}

			return userUsableCoupons;
		}

		/// <summary>
		/// カートに対し利用可能なクーポンのリスト取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="cart">カート情報</param>
		/// <param name="referralCode">Referral code</param>
		/// <returns>クーポンのリスト</returns>
		public static ListItemCollection GetUsableCouponList(
			string userId,
			string mailAddress,
			CartObject cart,
			string languageCode = null,
			string languageLocaleId = null,
			string referralCode = "")
		{
			var ownerMailAddress = (cart.Owner != null) ? cart.Owner.MailAddr : mailAddress;
			var now = DateTimeWrapper.Instance.Now;
			var coupons = GetUserUsableCouponsSpecificExpireDate(
				userId,
				ownerMailAddress,
				now,
				now,
				referralCode: referralCode);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				coupons = NameTranslationCommon.SetCouponTranslationData(
					coupons,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId);
			}

			var couponList = new ListItemCollection();
			foreach (var coupon in coupons)
			{
				if (CheckCouponValidWithCart(cart, coupon) == "")
				{
					var couponName = string.IsNullOrEmpty(coupon.CouponDispName) ? coupon.CouponName : coupon.CouponDispName;
					couponList.Add(new ListItem(couponName, coupon.CouponCode));
				}
			}

			return couponList;
		}

		/// <summary>
		/// カートに対し利用可能なクーポンリスト取得(先頭にブランクあり)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="cart">カート情報</param>
		/// <param name="referralCode">Referral code</param>
		/// <returns>クーポンリスト(先頭ブランクあり)</returns>
		public static ListItemCollection GetUsableCouponListWithBlank(
			string userId,
			string mailAddress,
			CartObject cart,
			string languageCode,
			string languageLocaleId,
			string referralCode = "")
		{
			var couponList = GetUsableCouponList(
				userId,
				mailAddress,
				cart,
				languageCode,
				languageLocaleId,
				referralCode);

			couponList.Insert(0, new ListItem("", ""));

			return couponList;
		}

		/// <summary>
		/// カートに対して利用できるクーポンの有無判定
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="cart">カート</param>
		/// <param name="referralCode">Referral code</param>
		/// <returns>利用できるクーポンがある場合true、ない場合false</returns>
		public static bool ExistsUsableCoupon(
			string userId,
			string mailAddress,
			CartObject cart,
			string referralCode = "")
		{
			var ownerMailAddress = (cart.Owner != null) ? cart.Owner.MailAddr : mailAddress;
			var usableCoupons = GetUsableCouponList(
				userId,
				ownerMailAddress,
				cart,
				referralCode: referralCode);

			return (usableCoupons.Count > 0);
		}

		/// <summary>
		/// 選択されたクーポン取得
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="usableCouponListWithBlankList">カートに対し利用可能なクーポンリスト(先頭にブランクあり)</param>
		/// <returns>クーポン</returns>
		public static string GetSelectedCoupon(CartObject cart, ListItemCollection usableCouponListWithBlankList)
		{
			// 有効期限切れなどにより選択されていたクーポンが選択肢から消えた場合、選択肢の先頭(ブランク)を選択された状態にする
			if (usableCouponListWithBlankList.FindByValue(cart.SelectedCoupon) == null)
			{
				cart.SelectedCoupon = usableCouponListWithBlankList[0].Value;
			}
			return cart.SelectedCoupon;
		}

		/// <summary>
		/// カートに対し利用可能なクーポン情報取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="cart">カート情報</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="referralCode">Referral code</param>
		/// <returns>クーポン情報</returns>
		public static UserCouponDetailInfo[] GetUsableCoupons(
			string userId,
			string mailAddress,
			CartObject cart,
			string languageCode = null,
			string languageLocaleId = null,
			string referralCode = "")
		{
			var ownerMailAddress = (cart.Owner != null) ? cart.Owner.MailAddr : mailAddress;
			var now = DateTimeWrapper.Instance.Now;
			var coupons = GetUserUsableCouponsSpecificExpireDate(
				userId,
				ownerMailAddress,
				now,
				now,
				referralCode: referralCode);

			var usableCoupons = coupons.Where(cp => CheckCouponValidWithCart(cart, cp) == "").ToArray<UserCouponDetailInfo>();

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				usableCoupons = NameTranslationCommon.SetCouponTranslationData(usableCoupons, languageCode, languageLocaleId);
			}
			return usableCoupons;
		}

		/// <summary>
		/// クーポン入力方法取得
		/// </summary>
		/// <returns>クーポン入力方法</returns>
		public static ListItemCollection GetCouponInputMethod()
		{
			var listItem = new ListItemCollection();
			listItem.Add(
				new ListItem(
					CommonPage.ReplaceTag("@@DispText.coupon_input_method.select@@"),
					COUPON_INPUT_METHOD_SELECT));
			listItem.Add(
				new ListItem(
					CommonPage.ReplaceTag("@@DispText.coupon_input_method.manualInput@@"),
					COUPON_INPUT_METHOD_MANUAL_INPUT));

			return listItem;
		}

		/// <summary>
		/// 利用可能回数表示文字列取得
		/// </summary>
		/// <param name="coupon">ユーザークーポン詳細情報</param>
		/// <returns>利用可能回数表示</returns>
		public static string GetCouponCount(UserCouponDetailInfo coupon)
		{
			var couponCount = string.Empty;
			switch (StringUtility.ToEmpty(coupon.CouponType))
			{
				case Constants.FLG_COUPONCOUPON_TYPE_USERREGIST:
				case Constants.FLG_COUPONCOUPON_TYPE_BUY:
				case Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL:
				case Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON:
				case Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON:
				case Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION:
					couponCount = "1";
					break;

				case Constants.FLG_COUPONCOUPON_TYPE_UNLIMIT:
				case Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT:
					couponCount = "無制限";
					break;

				case Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING:
					couponCount = coupon.CouponCount.ToString();
					break;

				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER:
					couponCount = coupon.UserCouponCount.ToString();
					break;
			}
			return couponCount;
		}

		/// <summary>
		/// 会員限定利用回数制限付きクーポン利用回数更新(汎用クーポン)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="coupoId">クーポンID</param>
		/// <param name="couponNo">クーポン枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="isCountUp">利用回数を+1するのか？(True:プラス１ False:マイナス1)</param>
		/// <returns>処理結果</returns>
		public static bool UpdateUserCouponCount(string deptId, string userId, string coupoId, int couponNo, SqlAccessor accessor, bool isCountUp)
		{
			var couponService = new CouponService();
			var useCoupon = couponService.GetUserCoupons(deptId, userId, accessor)
				.FirstOrDefault(
					uc => (uc.CouponId == coupoId)
					&& (uc.CouponNo == couponNo));

			if (useCoupon == null) return false;
			var couponModel = new UserCouponModel
			{
				UserId = useCoupon.UserId,
				DeptId = useCoupon.DeptId,
				CouponId = useCoupon.CouponId,
				CouponNo = useCoupon.CouponNo.Value,
				UserCouponCount = useCoupon.UserCouponCount,
				LastChanged = Constants.FLG_LASTCHANGED_USER
			};
			couponModel.UserCouponCount = isCountUp
				? couponModel.UserCouponCount + 1
				: couponModel.UserCouponCount - 1;

			var result = couponService.UpdateUseCouponCountUserCoupon(couponModel, accessor);
			return result;
		}

		/// <summary>
		/// 発行者向け回数制限ありクーポン？
		/// </summary>
		/// <param name="couponType">クーポン種別</param>
		/// <returns>発行者向け回数制限ありクーポンならtrue</returns>
		public static bool IsCouponLimit(string couponType)
		{
			var couponLimit = new[]
			{
				Constants.FLG_COUPONCOUPON_TYPE_USERREGIST,
				Constants.FLG_COUPONCOUPON_TYPE_BUY,
				Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY,
				Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON,
				Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION,
			};
			var isCouponLimit = couponLimit.Any(coupon => coupon == couponType);
			return isCouponLimit;
		}

		/// <summary>
		/// 会員限定回数制限付きクーポン？
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <returns>会員限定回数制限付きクーポンならTrue</returns>
		public static bool IsCouponLimitedForRegisteredUserByCouponId(string couponId)
		{
			var couponType =  new CouponService().GetCoupon(Constants.CONST_DEFAULT_DEPT_ID, couponId).CouponType;
			return IsCouponLimitedForRegisteredUser(couponType);
		}

		/// <summary>
		/// 会員限定回数制限付きクーポン？
		/// </summary>
		/// <param name="couponType">クーポン種別</param>
		/// <returns>会員限定回数制限付きクーポンならTrue</returns>
		public static bool IsCouponLimitedForRegisteredUser(string couponType)
		{
			var couponLimitedForRegisteredUser = new []
			{
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER,
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER,
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER,
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER
			};
			var isCouponLimitedForRegisteredUser = couponLimitedForRegisteredUser.Any(coupon => coupon == couponType);
			return isCouponLimitedForRegisteredUser;
		}

		/// <summary>
		/// 全員向け回数制限ありクーポン？
		/// </summary>
		/// <param name="couponType">クーポン種別</param>
		/// <returns>全員向け回数制限ありクーポンならtrue</returns>
		public static bool IsCouponAllLimit(string couponType)
		{
			var couponAllLimit = new[]
			{
				Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING
			};
			var isCouponAllLimit = couponAllLimit.Any(coupon => coupon == couponType);
			return isCouponAllLimit;
		}

		/// <summary>
		/// 無制限クーポン？
		/// </summary>
		/// <param name="couponType">クーポン種別</param>
		/// <returns>無制限クーポンならtrue</returns>
		public static bool IsCouponUnlimit(string couponType)
		{
			var couponUnlimit = new[]
			{
				Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
				Constants.FLG_COUPONCOUPON_TYPE_UNLIMIT
			};
			var isCouponUnlimit = couponUnlimit.Any(coupon => coupon == couponType);
			return isCouponUnlimit;
		}

		/// <summary>
		/// 配送料無料クーポン？
		/// </summary>
		/// <param name="couponType">クーポン種別</param>
		/// <returns>配送料無料クーポンならtrue</returns>
		public static bool IsFreeShipping(string couponType)
		{
			var freeShippingCoupon = new[]
			{
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING,
				Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER,
				Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL,
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER,
				Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER
			};
			var isFreeShipping = freeShippingCoupon.Any(coupon => coupon == couponType);
			return isFreeShipping;
		}

		/// <summary>
		/// 配送料無料金額割引クーポン利用判定
		/// </summary>
		/// <returns>配送料無料利用フラグ</returns>
		public static bool IsFreeShippingDiscountMoney(string freeShipping, decimal discountPrice, decimal discountRate)
		{
			return ((freeShipping == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				&& (discountPrice != 0)
				&& (discountRate != 0));
		}

		/// <summary>
		/// ブラックリスト型クーポン？
		/// </summary>
		/// <param name="couponType">クーポン種別</param>
		/// <returns>ブラックリスト型クーポンならtrue</returns>
		public static bool IsBlacklistCoupon(string couponType)
		{
			var blacklistCoupon = new[]
			{
				Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER,
				Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER,
				Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL,
				Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL,
				Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON,
			};
			var isBlacklistCoupon = blacklistCoupon.Any(coupon => coupon == couponType);
			return isBlacklistCoupon;
		}
	}
}
