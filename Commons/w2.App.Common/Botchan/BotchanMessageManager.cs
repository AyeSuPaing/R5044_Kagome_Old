/*
=========================================================================================================
  Module      : Botchan Message Manager(BotchanMessageManager.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Data;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Util;

namespace w2.App.Common.Botchan
{
	/// <summary>
	/// Botchan Message Manager
	/// </summary>
	public class BotchanMessageManager
	{
		/// <summary>
		/// Messages Code
		/// </summary>
		public enum MessagesCode
		{
			/// <summary>共通_システムエラー</summary>
			SYSTEM_ERROR,
			/// <summary>001ログインAPI_該当のユーザーは存在しませんでした</summary>
			USER_NOT_EXISTS,
			/// <summary>001ログインAPI_メールアドレスまたはパスワードが違います</summary>
			FRONT_USER_LOGIN_IN_MAILADDR_ERROR,
			/// <summary>001ログインAPI_誤ったユーザIDとパスワードで一定回数以上ログインが行われました</summary>
			FRONT_USER_LOGIN_ACCOUNT_LOCK,
			/// <summary>002再計算API_カートIDが不一致</summary>
			CART_VARIANCE,
			/// <summary>002再計算API_定期データ不備</summary>
			FRONT_PRODUCT_FIXED_PURCHASE_INVALID,
			/// <summary>002再計算API_商品配送種別不一致</summary>
			PRODUCT_SHIPPING_KBN_DIFF,
			/// <summary>002再計算API_利用範囲以外ポイント</summary>
			FRONT_POINT_USE_MAX_ERROR,
			/// <summary>002再計算API_該当する住所が取得できませんでした</summary>
			FRONT_ZIPCODE_NO_ADDR,
			/// <summary>002再計算API_該当商品現在販売を行っておりません</summary>
			FRONT_PRODUCT_NO_SELL,
			/// <summary>002再計算API_該当商品在庫がありません</summary>
			FRONT_PRODUCT_NO_STOCK_BEFORE_CART,
			/// <summary>002再計算API_特定ランク以上で購入可能な商品です</summary>
			FRONT_PRODUCT_BUYABLE_MEMBER_RANK,
			/// <summary>002再計算API_商品は一回のお買い物で購入できる数量を超えました</summary>
			FRONT_PRODUCT_OVER_MAXSELLQUANTITY,
			/// <summary>002再計算API_該当商品は削除されました</summary>
			FRONT_PRODUCT_DELETE,
			/// <summary>002再計算API_セール期間が終了しました</summary>
			FRONT_PRODUCT_SALES_INVALID,
			/// <summary>002再計算API_セールの設定に変更がありました</summary>
			FRONT_PRODUCT_SALES_CHANGE,
			/// <summary>002再計算API_価格の設定に変更がありました</summary>
			FRONT_PRODUCT_PRICE_CHANGE,
			/// <summary>002再計算API_ポイント単位のみ指定可能</summary>
			FRONT_POINT_USABLE_UNIT_ERROR,
			/// <summary>002再計算API_指定利用ポイントが商品合計金額を超えました</summary>
			FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR,
			/// <summary>002再計算API_該当する商品がありません</summary>
			FRONT_PRODUCT_NO_ITEM,
			/// <summary>002再計算API_ポイントオプションがOFF</summary>
			POINT_OPTION_OFF,
			/// <summary>002再計算API_クーポンオプションがOFF</summary>
			COUPON_OPTION_OFF,
			/// <summary>002再計算API_指定不可配送希望日</summary>
			NOT_ALLOWED_SHIPPING_DATE,
			/// <summary>002再計算API_指定不可配送パターン</summary>
			NOT_ALLOWED_FIXEDPURCHASE_KBN,
			/// <summary>002再計算API_指定不可月間隔</summary>
			NOT_ALLOWED_FIXEDPURCHASE_MONTHLY_INTERVAL,
			/// <summary>002再計算API_指定不可日間隔</summary>
			NOT_ALLOWED_FIXEDPURCHASE_DAY_INTERVAL,
			/// <summary>002再計算API_指定利用クーポンにより商品合計金額を超えた</summary>
			COUPON_PRICE_SUBTOTAL_MINUS_COUPON_ERROR,
			/// <summary>002再計算API_クーポンコード現在無効です</summary>
			COUPON_INVALID_ERROR,
			/// <summary>002再計算API_クーポンコード利用済みです</summary>
			COUPON_USED_ERROR,
			/// <summary>002再計算API_クーポンコードは利用可能回数を超えた</summary>
			COUPON_USABLE_COUNT_ERROR,
			/// <summary>002再計算API_適用できる商品が存在しません</summary>
			COUPON_TARGET_PRODUCT_ERROR,
			/// <summary>002再計算API_カート内にクーポンを適用できない商品が存在します</summary>
			COUPON_EXCEPTIONAL_PRODUCT_ERROR,
			/// <summary>002再計算API_クーポンコードを適用する場合、クーポン対象商品を〇〇以上でご購入して下さい</summary>
			COUPON_USABLE_PRICE_ERROR,
			/// <summary>セット商品内の配送種別に変更がありました。セットを削除しました。</summary>
			FRONT_PRODUCTSET_SHIPPING_TYPE_CHANGE,
			/// <summary>003注文登録API_注文エラー、具体的なエラー内容はメッセージ内容を参考してください</summary>
			REGIST_ORDER_FAILURE,
			/// <summary>003注文登録API_レコメンドエラー、すでに出荷作業に入っているため、レコメンドに失敗しました。</summary>
			INVALID_RECOMMEND_ORDER_STATUS,
			/// <summary>共通_認証キー不正</summary>
			INVALID_AUTHENTICATION_KEY,
			/// <summary>002再計算API_「商品名」はただいま購入できません</summary>
			FRONT_PRODUCT_INVALID,
			/// <summary>002再計算API_「商品名」の商品はお支払い方法が一部制限されております。</summary>
			FRONT_PRODUCT_LIMITED_PAYMENT,
			/// <summary>002再計算API_メール便はご利用できません。</summary>
			NOT_ALLOWED_USE_SHIPPING_KBN_MAIL,
			/// <summary>002再計算API_配送方法がメール便のため既定のお支払方法に代引きはご利用できません。他のお支払方法を選択してください。</summary>
			FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_COLLECT,
			/// <summary>002再計算API_レコメンドオプションがOFF</summary>
			RECOMMEND_OPTION_OFF,
			/// <summary>002再計算API_利用できるクーポンコードが存在しません。</summary>
			COUPON_NO_COUPON_ERROR,
			/// <summary>002再計算API_今すぐ注文は利用できません。</summary>
			FIXEDPURCHASEORDER_ERROR,
			/// <summary>入力された配送先住所は配送不可エリアです。</summary>
			UNAVAILABLE_SHIPPING_AREA_ERROR,
			/// <summary>レコメンドの元注文が存在しない</summary>
			NONE_ORIGINAL_ORDER,
			/// <summary>レコメンド対象商品は存在しない</summary>
			NONE_RECOMMEND_TARGET,
			/// <summary>注文内容をおすすめ商品の購入に変更する際にエラーが発生しました</summary>
			FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER,
			/// <summary>在庫不足のため、注文情報の変更に失敗しました</summary>
			PRODUCTSTOCK_OUT_OF_STOCK_ERROR,
			/// <summary>クレジット情報の登録(表示フラグ更新)に失敗</summary>
			USERCREDITCARD_CANNOT_UPDATE_DISP_FLG_ERROR,
			/// <summary>各決済管理画面で処理をしてください</summary>
			EXTERNAL_PAYMENT_CANCEL_FAILED,
			/// <summary>共通_@@1@@は入力必須項目です。</summary>
			INPUTCHECK_NECESSARY,
			/// <summary>共通_@@1@@は@@2@@文字で入力して下さい。</summary>
			INPUTCHECK_LENGTH,
			/// <summary>共通_@@1@@は@@2@@文字以下で入力して下さい。</summary>
			INPUTCHECK_LENGTH_MAX,
			/// <summary>共通_@@1@@は@@2@@文字以上で入力して下さい。</summary>
			INPUTCHECK_LENGTH_MIN,
			/// <summary>共通_@@1@@は半角@@2@@文字(全角@@3@@文字)で入力して下さい。</summary>
			INPUTCHECK_BYTE_LENGTH,
			/// <summary>共通_@@1@@は半角@@2@@文字(全角@@3@@文字)以下で入力して下さい。</summary>
			INPUTCHECK_BYTE_LENGTH_MAX,
			/// <summary>共通_@@1@@は半角@@2@@文字(全角@@3@@文字)以上で入力して下さい。</summary>
			INPUTCHECK_BYTE_LENGTH_MIN,
			/// <summary>共通_@@1@@は@@2@@以下で入力して下さい。</summary>
			INPUTCHECK_NUMBER_MAX,
			/// <summary>共通_@@1@@は@@2@@以上で入力して下さい。</summary>
			INPUTCHECK_NUMBER_MIN,
			/// <summary>共通_@@1@@は全角文字で入力して下さい。</summary>
			INPUTCHECK_FULLWIDTH,
			/// <summary>共通_@@1@@は全角ひらがなで入力して下さい。</summary>
			INPUTCHECK_FULLWIDTH_HIRAGANA,
			/// <summary>_@@1@@は全角カタカナで入力して下さい。</summary>
			INPUTCHECK_FULLWIDTH_KATAKANA,
			/// <summary>共通_@@1@@は半角文字で入力して下さい。</summary>
			INPUTCHECK_HALFWIDTH,
			/// <summary>共通_@@1@@は半角英数字で入力して下さい。</summary>
			INPUTCHECK_HALFWIDTH_ALPHNUM,
			/// <summary>共通_@@1@@は半角英数記号で入力して下さい。</summary>
			INPUTCHECK_HALFWIDTH_ALPHNUMSYMBOL,
			/// <summary>共通_@@1@@は半角数字で入力して下さい。</summary>
			INPUTCHECK_HALFWIDTH_NUMBER,
			/// <summary>共通_@@1@@は半角数値で入力して下さい。</summary>
			INPUTCHECK_HALFWIDTH_DECIMAL,
			/// <summary>共通_@@1@@は半角の正しい日付形式で入力して下さい。</summary>
			INPUTCHECK_HALFWIDTH_DATE,
			/// <summary>共通_@@1@@は正しい日付で入力して下さい。</summary>
			INPUTCHECK_DATE,
			/// <summary>共通_@@1@@は未来の正しい日付で入力して下さい。</summary>
			INPUTCHECK_DATE_FUTURE,
			/// <summary>共通_@@1@@は過去の正しい日付で入力して下さい。</summary>
			INPUTCHECK_DATE_PAST,
			/// <summary>共通_@@1@@は正しいメールアドレスの形式で入力して下さい。</summary>
			INPUTCHECK_MAILADDRESS,
			/// <summary>共通_@@1@@は正しい形式で入力して下さい。(例：東京都、沖縄県など)</summary>
			INPUTCHECK_PREFECTURE,
			/// <summary>共通_@@1@@に「@@2@@」は入力できません。</summary>
			INPUTCHECK_PROHIBITED_CHAR,
			/// <summary>共通_@@1@@に「@@3@@」（@@2@@）は入力できません。</summary>
			INPUTCHECK_OUTOFCHARCODE,
			/// <summary>共通_@@1@@は@@2@@で入力して下さい。</summary>
			INPUTCHECK_REGEXP,
			/// <summary>共通_@@1@@は正しい形式で入力して下さい。</summary>
			INPUTCHECK_REGEXP2,
			/// <summary>共通_@@1@@に「@@2@@」は入力できません。</summary>
			INPUTCHECK_EXCEPT_REGEXP,
			/// <summary>共通_@@1@@と@@1@@(確認用)が一致しません。</summary>
			INPUTCHECK_CONFIRM,
			/// <summary>共通_@@2@@と同じ値を@@1@@に使用出来ません。</summary>
			INPUTCHECK_EQUIVALENCE,
			/// <summary>共通_@@1@@と@@2@@が一致しません。</summary>
			INPUTCHECK_DIFFERENT_VALUE,
			/// <summary>共通_入力された@@1@@は既に使用されています。</summary>
			INPUTCHECK_DUPLICATION,
			/// <summary>共通_IP制御</summary>
			NOT_ALLOWED_IP_ADDRESS,
			/// <summary>共通_BOTCHANオプションがOFF</summary>
			BOTCHAN_OPTION_OFF,
			/// <summary>共通_ RequestParamとResponseをログに出力するかどうかがOFF</summary>
			BOTCHAN_OUTPUT_REQUEST_PARAM_AMD_RESPONSE_TO_THE_LOG_OFF,
			/// <summary>共通_ Botchan再与信後エラー時メールタイトル</summary>
			BOTCHAN_AFTER_REAUTH_ERROR_MAIL_TITLE,
			/// <summary>共通_ Botchan再与信後エラー時メール本文</summary>
			BOTCHAN_AFTER_REAUTH_ERROR_MAIL_BODY,
			/// <summary>エラーなし</summary>
			NONE
		}

		/// <summary>
		/// BOTCHANエラーコード取得
		/// </summary>
		/// <param name="oeErrorCode">エラーコード</param>
		/// <returns>エラーメッセージ</returns>
		public static MessagesCode GetMessagesCode(OrderErrorcode oeErrorCode)
		{
			var strMessageKey = new MessagesCode();

			switch (oeErrorCode)
			{
				// 商品無効エラー
				case OrderErrorcode.ProductInvalid:
					strMessageKey = MessagesCode.FRONT_PRODUCT_INVALID;
					break;

				// 商品販売期間エラー
				case OrderErrorcode.ProductOutOfSellTerm:
					strMessageKey = MessagesCode.FRONT_PRODUCT_NO_SELL;
					break;

				// 商品在庫エラー（カート投入前）
				case OrderErrorcode.ProductNoStockBeforeCart:
					strMessageKey = MessagesCode.FRONT_PRODUCT_NO_STOCK_BEFORE_CART;
					break;

				// 商品販売可能数量エラー
				case OrderErrorcode.MaxSellQuantityError:
					strMessageKey = MessagesCode.FRONT_PRODUCT_OVER_MAXSELLQUANTITY;
					break;

				// 販売可能会員ランク外エラー
				case OrderErrorcode.SellMemberRankError:
					strMessageKey = MessagesCode.FRONT_PRODUCT_BUYABLE_MEMBER_RANK;
					break;

				// セット商品配送種別変更エラー
				case OrderErrorcode.ProductShippingTypeChanged:
					strMessageKey = MessagesCode.FRONT_PRODUCTSET_SHIPPING_TYPE_CHANGE;
					break;

				// 定期購入無効エラー
				case OrderErrorcode.ProducFixedPurchaseInvalidError:
					strMessageKey = MessagesCode.FRONT_PRODUCT_FIXED_PURCHASE_INVALID;
					break;

				// 配送不可エリアエラー
				case OrderErrorcode.UnavailableShippingAreaError:
					strMessageKey = MessagesCode.UNAVAILABLE_SHIPPING_AREA_ERROR;
					break;
			}

			return strMessageKey;
		}

		/// <summary>
		/// 指定されたエラーコードからエラーメッセージ取得
		/// </summary>
		/// <param name="errorCode">クーポンエラーコード</param>
		/// <returns>エラーメッセージ</returns>
		public static MessagesCode GetErrorMessage(CouponErrorcode errorCode)
		{
			var messageKey = new MessagesCode();
			switch (errorCode)
			{
				// クーポン利用済エラー
				case CouponErrorcode.CouponUsedError:
					messageKey = MessagesCode.COUPON_USED_ERROR;
					break;

				// クーポン利用可能回数エラー
				case CouponErrorcode.CouponUsableCountError:
					messageKey = MessagesCode.COUPON_USABLE_COUNT_ERROR;
					break;

				// 上記以外の場合、エラーなし
				default:
					messageKey = MessagesCode.NONE;
					break;
			}
			return messageKey;
		}

		/// <summary>
		/// エラー詳細内容作成
		/// </summary>
		/// <param name="dvProduct">商品情報</param>
		/// <param name="messagesCode">エラーコード</param>
		/// <param name="coupon"></param>
		/// <param name="cartProduct">カート内商品情報</param>
		/// <returns>エラー詳細内容</returns>
		public static string ConversionErrorMessage(
			MessagesCode messagesCode,
			CartCoupon coupon = null,
			CartProduct cartProduct = null,
			DataView dvProduct = null)
		{
			var memo = string.Empty;

			var errorMessage = MessageManager.GetMessages(messagesCode.ToString());
			switch (messagesCode)
			{
				case MessagesCode.FRONT_PRODUCT_NO_SELL:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName);
					break;

				case MessagesCode.FRONT_PRODUCT_NO_STOCK_BEFORE_CART:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName);
					break;

				case MessagesCode.FRONT_PRODUCT_BUYABLE_MEMBER_RANK:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName).Replace(
							"@@ 2 @@",
							MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Database.Common.Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
					break;

				case MessagesCode.FRONT_PRODUCT_OVER_MAXSELLQUANTITY:
					memo = errorMessage;
					break;

				case MessagesCode.FRONT_PRODUCT_DELETE:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductId);
					break;

				case MessagesCode.FRONT_PRODUCT_SALES_INVALID:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName);
					break;

				case MessagesCode.FRONT_PRODUCT_SALES_CHANGE:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName);
					break;

				case MessagesCode.FRONT_PRODUCT_PRICE_CHANGE:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName);
					break;

				case MessagesCode.FRONT_PRODUCT_INVALID:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName);
					break;

				case MessagesCode.FRONT_PRODUCTSET_SHIPPING_TYPE_CHANGE:
					memo = errorMessage;
					break;

				case MessagesCode.FRONT_PRODUCT_FIXED_PURCHASE_INVALID:
					memo = errorMessage.Replace("@@ 1 @@", cartProduct.ProductJointName);
					break;

				case MessagesCode.COUPON_USABLE_COUNT_ERROR:
					memo = errorMessage.Replace("@@ 1 @@", coupon.CouponCode);
					break;

				case MessagesCode.COUPON_USED_ERROR:
					memo = errorMessage.Replace("@@ 1 @@", coupon.CouponCode);
					break;
			}
			return memo;
		}
	}
}
