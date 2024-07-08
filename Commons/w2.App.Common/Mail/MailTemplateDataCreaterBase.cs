/*
=========================================================================================================
  Module      : 注文メールテンプレートデータ作成基底クラス(MailTemplateDataCreaterBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Cart;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Common.Extensions;
using w2.Common.Net.Mail;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Coupon.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.User.Helper;

namespace w2.App.Common.Mail
{
	/// <summary>
	/// 注文メールテンプレートデータ作成基底クラス
	/// </summary>
	public class MailTemplateDataCreaterBase
	{
		/// <summary> オペレータ文字列の定義</summary>
		public const string STRING_FOR_OPERATOR = "_for_operator";
		/// <summary>ネクストエンジン用文字列の定義</summary>
		public const string STRING_FOR_NEXTENGINE = "_for_nextengine";
		/// <summary>注文拡張項目 内容置換文字列</summary>
		private const string ORDER_EXTEND_REPLACE_TEXT = "order_extend_attributes";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isPc">注文区分がPCか</param>
		protected MailTemplateDataCreaterBase(bool isPc)
		{
			this.IsPc = isPc;
			Initialize(this.IsPc);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="isPc">PCか</param>
		protected void Initialize(bool isPc)
		{
			this.BorderString = isPc ? "-----------" : "";
		}

		/// <summary>
		/// 通貨数値をメールテンプレ用の通貨フォーマットにして返す。
		/// ※ 日本国内の場合は通貨記号なし、 Project.configの「SETTING_MAIL_PRICE_FORMAT」は非適用
		/// </summary>
		/// <param name="price">通貨数値</param>
		/// <returns>適用後の通貨数値</returns>
		protected string ConvertPriceForMail(object price)
		{
			var result = (CurrencyManager.IsJapanKeyCurrencyCode)
				? StringUtility.ToPrice(price, format: "{0:#,##0}")
				: price.ToPriceString(true);
			return result;
		}

		/// <summary>
		/// 通貨数値をメールテンプレ用の通貨フォーマットにして返す。
		/// ※ 日本国内の場合は通貨記号なし、Project.configの「SETTING_MAIL_PRICE_FORMAT」を適用
		/// </summary>
		/// <param name="price">通貨数値</param>
		/// <returns>適用後の通貨数値</returns>
		protected string ConvertPriceForMailByConfig(object price)
		{
			var result = (CurrencyManager.IsJapanKeyCurrencyCode)
				? String.Format(Constants.SETTING_MAIL_PRICE_FORMAT, StringUtility.ToPrice(price, format: "{0:#,##0}"))
				: price.ToPriceString(true);
			return result;
		}

		/// <summary>
		/// 発行したクーポン情報作成
		/// </summary>
		/// <param name="publishMessages">発行メッセージ</param>
		/// <param name="coupon">クーポン情報</param>
		/// <param name="itemNo">項目番</param>
		/// <param name="localeId">ロケールID</param>
		public void CreatePublishCouponInfo(StringBuilder publishMessages, UserCouponDetailInfo coupon, int itemNo, string localeId)
		{
			publishMessages.Append("-(").Append(itemNo.ToString()).Append(")----------" + this.BorderString).Append("\r\n");
			publishMessages.Append(CommonPage.ReplaceTagByLocaleId("@@MailTemplate.coupon_code.name@@", localeId)).Append(coupon.CouponCode).Append("\r\n");
			publishMessages.Append(CommonPage.ReplaceTagByLocaleId("@@MailTemplate.coupon_name.name@@", localeId)).Append(coupon.CouponDispName).Append("\r\n");
			publishMessages.Append(CommonPage.ReplaceTagByLocaleId("@@MailTemplate.expire_end.name@@", localeId))
				.Append(DateTimeUtility.ToString(
					coupon.ExpireEnd.GetValueOrDefault(),
					DateTimeUtility.FormatType.ShortDate2Letter,
					localeId))
				.Append("\r\n");
		}

		/// <summary>
		/// エンドユーザー向けの日付タグ追加
		/// </summary>
		/// <param name="mailDatas">メール用のデータ</param>
		/// <param name="tagName">タグ名</param>
		/// <param name="dateValue">日付値</param>
		/// <param name="languageLocaleId">言語ロケールコード</param>
		public void AddLongDateTagsForUser(
			Hashtable mailDatas,
			string tagName,
			object dateValue,
			string languageLocaleId)
		{
			var oneLetterDate = FormatDateForUserDateTag(
				dateValue,
				languageLocaleId,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
			mailDatas[tagName + "_yyyymd(d)"] = oneLetterDate;

			var twoLetterDate = FormatDateForUserDateTag(
				dateValue,
				languageLocaleId,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
			mailDatas[tagName + "_yyyymmdd(d)"] = twoLetterDate;
		}

		/// <summary>
		/// エンドユーザー向け日付タグの日付値をフォーマットする
		/// </summary>
		/// <param name="dateValue">日付値</param>
		/// <param name="languageLocaleId">言語ロケールコード</param>
		/// <param name="formatType">フォーマットタイプ</param>
		/// <returns>フォーマットした日付値</returns>
		private string FormatDateForUserDateTag(
			object dateValue,
			string languageLocaleId,
			DateTimeUtility.FormatType formatType)
		{
			var formattedDate = (IsDateTimeInThePast(dateValue) == false)
				? DateTimeUtility.ToString(dateValue, formatType, languageLocaleId)
				: Constants.CONST_INVALID_FIRST_SHIPPING_DATE_VALUE;
			return formattedDate;
		}

		/// <summary>
		/// 管理者向けの日付タグ追加
		/// </summary>
		/// <param name="mailDatas">メール用のデータ</param>
		/// <param name="tagName">タグ名</param>
		/// <param name="dateValue">日付値</param>
		public void AddLongDateTagsForManager(Hashtable mailDatas, string tagName, object dateValue)
		{
			var oneLetterDate = FormatDateForManagerDateTag(
				dateValue,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
			mailDatas[tagName + "_yyyymd(d)" + STRING_FOR_OPERATOR] = oneLetterDate;

			var twoLetterDate = FormatDateForManagerDateTag(
				dateValue,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
			mailDatas[tagName + "_yyyymmdd(d)" + STRING_FOR_OPERATOR] = twoLetterDate;
		}

		/// <summary>
		/// お知らせメール登録解除リンクのタグを追加
		/// </summary>
		/// <param name="mailDatas">メール用のデータ</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailAddress">メールアドレス</param>
		public void AddMailUnsubscribeTags(
			Hashtable mailDatas,
			string userId,
			string mailAddress)
		{
			var unsubscribeUrl = new UrlCreator($"{Constants.PROTOCOL_HTTPS}{Constants.SITE_DOMAIN}{Constants.PATH_ROOT_FRONT_PC}{Constants.MAIL_LISTUNSUBSCRIBE_URL}")
				.AddParam(Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_USER_ID, userId)
				.AddParam(Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_VERIFICATION_KEY, UnsubscribeVarificationHelper.Hash(userId, mailAddress))
				.CreateUrl();
			mailDatas["mail_unsubscribe_link"] = unsubscribeUrl;
		}

		/// <summary>
		/// 管理者向け日付タグの日付値をフォーマットする
		/// </summary>
		/// <param name="dateValue">日付値</param>
		/// <param name="formatType">フォーマットタイプ</param>
		/// <returns>フォーマットした日付値</returns>
		private string FormatDateForManagerDateTag(
			object dateValue,
			DateTimeUtility.FormatType formatType)
		{
			var formattedDate = (IsDateTimeInThePast(dateValue) == false)
				? DateTimeUtility.ToStringForManager(dateValue, formatType)
				: Constants.CONST_INVALID_FIRST_SHIPPING_DATE_VALUE;
			return formattedDate;
		}

		/// <summary>
		/// 引数の値が過去の日時のDateTime値かどうか確認する
		/// </summary>
		/// <param name="dateValue">日付値</param>
		/// <returns>引数の値が過去の日時のDateTime値であればTRUE</returns>
		private bool IsDateTimeInThePast(object dateValue)
		{
			var result = ((dateValue is DateTime)
				&& ((DateTime)dateValue == DateTime.Parse(Constants.CONST_DEFAULT_DATETIME_VALUE)));
			return result;
		}

		/// <summary>
		/// 商品金額 税込み・税抜き表記の文言取得
		/// </summary>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>税込み・税抜き表記</returns>
		protected string GetProductTaxText(string languageLocaleId = null)
		{
			var result = (Constants.SETTING_MAIL_PRODUCT_PRICE_TAX_TEXT_DISPLAY)
				? string.Format("({0})", TaxCalculationUtility.GetTaxTypeText(languageLocaleId))
				: "";
			return result;
		}

		/// <summary>
		/// ネクストエンジン用注文メールテンプレートフォーマット作成
		/// </summary>
		/// <param name="variationId">商品ID</param>
		/// <param name="productName">商品名</param>
		/// <param name="productOptionSettingSelectValues">商品付帯情報一覧</param>
		/// <param name="price">価格</param>
		/// <param name="quantity">数量</param>
		/// <param name="itemPrice">商品小計</param>
		/// <returns>ネクストエンジン用フォーマット</returns>
		protected string CreateNextEngineOrderMailTempleteFormat(
			string variationId,
			string productName,
			string productOptionSettingSelectValues,
			decimal price,
			int quantity,
			decimal itemPrice)
		{
			var orderItemText = new StringBuilder();
			orderItemText
				.Append("------------------------------------------------------------")
				.Append("\r\n")
				.Append("商品番号：")
				.Append(variationId)
				.Append("\r\n")
				.Append("注文商品名：")
				.Append(productName);

			if (string.IsNullOrEmpty(productOptionSettingSelectValues) == false)
			{
				orderItemText
					.Append("\r\n")
					.Append("商品オプション：")
					.Append(ProductOptionSettingHelper.GetDisplayProductOptionTexts(productOptionSettingSelectValues));
			}

			orderItemText
				.Append("\r\n")
				.Append("単価：")
				.Append(@"￥")
				.Append(StringUtility.ToNumeric(decimal.ToInt32(price)))
				.Append("\r\n")
				.Append("数量：")
				.Append(StringUtility.ToNumeric(quantity))
				.Append("\r\n")
				.Append("小計：")
				.Append(@"￥")
				.Append(StringUtility.ToNumeric(decimal.ToInt32(itemPrice)))
				.Append("\r\n");

			return orderItemText.ToString();
		}

		/// <summary>
		/// 領収書出力URL追加
		/// </summary>
		/// <param name="mailDatas">メール用のデータ</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="receiptFlg">領収書希望フラグ</param>
		/// <param name="receiptAddress">宛名</param>
		/// <param name="receiptProviso">但し書き</param>
		protected void AddReceiptDownloadUrl(
			Hashtable mailDatas,
			string orderId,
			string receiptFlg,
			string receiptAddress,
			string receiptProviso)
		{
			// 領収書対応OPが無効の場合、何もしない
			if (Constants.RECEIPT_OPTION_ENABLED == false) return;

			// 領収書ダウンロードＵＲＬと領収書情報をメール用のデータに追加
			var encryptOrderId = UserPassowordCryptor.PasswordEncrypt(orderId);
			var url = new UrlCreator(
				Constants.URL_FRONT_PC_SECURE + Constants.PAGE_FRONT_RECEIPTDOWNLOAD)
				.AddParam(Constants.REQUEST_KEY_ORDER_ID_FOR_RECEIPT, encryptOrderId)
				.CreateUrl();
			mailDatas[Constants.TAG_RECEIPT_URL] = (receiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON)
				? url
				: string.Empty;
			mailDatas[Constants.FIELD_ORDER_RECEIPT_ADDRESS] = receiptAddress;
			mailDatas[Constants.FIELD_ORDER_RECEIPT_PROVISO] = receiptProviso;
		}

		/// <summary>
		/// 注文拡張項目設定
		/// </summary>
		/// <param name="target">設定の対象</param>
		/// <param name="model">注文モデル</param>
		protected void SettingOrderExtend(Hashtable target, OrderModel model)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false)
			{
				SettingOrderExtendSetEmpty(target);
				return;
			}

			var temp = OrderExtendCommon.ConvertOrderExtend(model);
			SettingOrderExtend(target, temp);
		}
		/// <summary>
		/// 注文拡張項目設定
		/// </summary>
		/// <param name="target">設定の対象</param>
		/// <param name="model">定期モデル</param>
		protected void SettingOrderExtend(Hashtable target, FixedPurchaseModel model)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false)
			{
				SettingOrderExtendSetEmpty(target);
				return;
			}

			var temp = OrderExtendCommon.ConvertOrderExtend(model);
			SettingOrderExtend(target, temp);
		}
		/// <summary>
		/// 注文拡張項目設定
		/// </summary>
		/// <param name="target">設定の対象</param>
		/// <param name="value">注文データ</param>
		protected void SettingOrderExtend(Hashtable target, Hashtable value)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false)
			{
				SettingOrderExtendSetEmpty(target);
				return;
			}

			var temp = OrderExtendCommon.ConvertOrderExtend(value);
			SettingOrderExtend(target, temp);
		}
		/// <summary>
		/// 注文拡張項目設定
		/// </summary>
		/// <param name="target">設定の対象</param>
		/// <param name="cart">カート</param>
		protected void SettingOrderExtend(Hashtable target, CartObject cart)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false)
			{
				SettingOrderExtendSetEmpty(target);
				return;
			}

			SettingOrderExtend(target, cart.OrderExtend);
		}
		/// <summary>
		/// 注文拡張項目設定
		/// </summary>
		/// <param name="target">設定の対象</param>
		/// <param name="orderExtend">注文拡張項目入力内容</param>
		protected void SettingOrderExtend(Hashtable target, Dictionary<string, CartOrderExtendItem> orderExtend)
		{
			foreach (var settingModel in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				var value = (orderExtend.ContainsKey(settingModel.SettingId))
					? OrderExtendCommon.GetValueDisplayName(settingModel.InputType, settingModel.InputDefault, orderExtend[settingModel.SettingId].Value)
					: string.Empty;

				value = (settingModel.IsInputTypeCheckBox)
					? value.Replace(",", Constants.ORDER_EXTEND_MAIL_CHECKBOX_SEPARATOR)
					: value;

				if (target.ContainsKey(settingModel.SettingId))
				{
					target[settingModel.SettingId] = value;
				}
				else
				{
					target.Add(settingModel.SettingId, value);
				}
			}

			var orderExtendText = GetOrderExtendAttributsText(orderExtend);
			if (target.ContainsKey(ORDER_EXTEND_REPLACE_TEXT))
			{
				target[ORDER_EXTEND_REPLACE_TEXT] = orderExtendText;
			}
			else
			{
				target.Add(ORDER_EXTEND_REPLACE_TEXT, orderExtendText);
			}
		}

		/// <summary>
		/// 注文拡張項目 空文字設定
		/// </summary>
		/// <param name="target">設定の対象</param>
		private void SettingOrderExtendSetEmpty(Hashtable target)
		{
			foreach (var field in Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST)
			{
				if (target.ContainsKey(field))
				{
					target[field] = string.Empty;
				}
				else
				{
					target.Add(field, string.Empty);
				}
			}

			if (target.ContainsKey(ORDER_EXTEND_REPLACE_TEXT))
			{
				target[ORDER_EXTEND_REPLACE_TEXT] = string.Empty;
			}
			else
			{
				target.Add(ORDER_EXTEND_REPLACE_TEXT, string.Empty);
			}
		}

		/// <summary>
		/// 注文拡張項目の記載内容を取得
		/// </summary>
		/// <param name="orderExtend">注文拡張項目入力内容</param>
		/// <returns>注文拡張項目の記載内容</returns>
		public static string GetOrderExtendAttributsText(Dictionary<string, CartOrderExtendItem> orderExtend)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return string.Empty;

			var orderExtendFrontDisplay = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
				.SettingModelsForFront.Select(
					s =>
					{
						var value = (orderExtend.ContainsKey(s.SettingId))
							? OrderExtendCommon.GetValueDisplayName(
								s.InputType,
								s.InputDefault,
								orderExtend[s.SettingId].Value)
							: string.Empty;

						value = (s.IsInputTypeCheckBox)
							? value.Replace(",", Constants.ORDER_EXTEND_MAIL_CHECKBOX_SEPARATOR)
							: value;

						value = (string.IsNullOrEmpty(value)) ? ValueText.GetValueText(Constants.TABLE_ORDEREXTENDSETTING, "empty_text", value) : value;
						var result = s.SettingName + " :" + value;
						return result;
					}).ToArray();

			var orderExtendText = string.Join("\r\n", orderExtendFrontDisplay);
			return orderExtendText;
		}

		/// <summary>
		/// 注文商品の情報を設定
		/// </summary>
		/// <param name="target">設定の対象</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderItemsProductId">注文商品ID</param>
		protected void SetOrderItemsInfo(Hashtable target, string shopId, string[] orderItemsProductId)
		{
			var orderItems = new ProductService().GetProducts(shopId, orderItemsProductId);
			if (orderItems.Any() == false)
			{
				target[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_PRODUCT_ID] = string.Empty;
				target[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_BRAND_ID] = string.Empty;
				return;
			}

			target[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_PRODUCT_ID] = string.Join(",", orderItemsProductId);

			var products = orderItems.GroupBy(v => v.ProductId).Select(v => v.First()).ToArray();
			target[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_BRAND_ID] = products.Select(
				v => new[]
				{
					v.BrandId1,
					v.BrandId2,
					v.BrandId3,
					v.BrandId4,
					v.BrandId5
				}.Where(s => (string.IsNullOrEmpty(s) == false)).Distinct().JoinToString(",")).JoinToString(",");
			target[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_CATEGORY_ID] = products.Select(
				v => new[]
				{
					v.CategoryId1,
					v.CategoryId2,
					v.CategoryId3,
					v.CategoryId4,
					v.CategoryId5
				}.Where(s => (string.IsNullOrEmpty(s) == false)).Distinct().JoinToString(",")).JoinToString(",");
		}

		/// <summary>注文区分がPCか</summary>
		public bool IsPc { get; protected set; }
		/// <summary>境界文字列</summary>
		public string BorderString { get; private set; }
	}
}
