/*
=========================================================================================================
  Module      : 定期台帳から注文メールテンプレートデータを作成(MailTemplateDataCreaterForFixedPurchase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Common.Util;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Mail
{
	/// <summary>
	/// 定期台帳から注文メールテンプレートデータを作成
	/// </summary>
	public class MailTemplateDataCreaterForFixedPurchase : MailTemplateDataCreaterBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isPc">注文区分がPCか</param>
		public MailTemplateDataCreaterForFixedPurchase(bool isPc)
			: base(isPc)
		{
		}

		/// <summary>
		/// メール送信用の定期購入情報取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>メールテンプレート用ハッシュテーブル</returns>
		public Hashtable GetFixedPurchaseMailDatas(string fixedPurchaseId)
		{
			// 定期購入情報取得
			var fixedPurchaseContainer = new FixedPurchaseService().GetContainer(fixedPurchaseId, true);
			var dataSendMail = new Hashtable();

			if (fixedPurchaseContainer == null) return dataSendMail;
			dataSendMail = fixedPurchaseContainer.DataSource;
			if (dataSendMail.Count == 0) return dataSendMail;

			// PC・モバイル判定
			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED == false)
			{
				this.IsPc = ((string)dataSendMail[Constants.FIELD_FIXEDPURCHASE_ORDER_KBN] != Constants.FLG_FIXEDPURCHASE_ORDER_KBN_MOBILE);
			}
			// 初期化し直す
			Initialize(this.IsPc);

			dataSendMail["is_pc"] = this.IsPc;

			// 注文者情報　区分値
			dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_KBN + "_text"]
				= ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_KBN, (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_KBN]);
			dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_SEX + "_text"]
				= ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_SEX, (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_SEX]);

			// 該当な日付形式へ変換のため、注文者の言語ロケールIDを取得
			var languageLocaleId = (string)dataSendMail[Constants.FIELD_FIXEDPURCHASE_DISP_LANGUAGE_LOCALE_ID];

			// 生年月日の時分秒削除
			dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]
				= DateTimeUtility.ToString(
					dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
					DateTimeUtility.FormatType.ShortDate2Letter,
					languageLocaleId);
			dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_BIRTH + STRING_FOR_OPERATOR] =
				DateTimeUtility.ToStringForManager(
					dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
					DateTimeUtility.FormatType.ShortDate2Letter);

			// 定期購入情報一覧URL
			dataSendMail["fixed_purchase_history_list_url"]
				= (string.IsNullOrEmpty(Constants.URL_FRONT_PC)) ? "" : Constants.URL_FRONT_PC.Replace(Constants.PROTOCOL_HTTP, Constants.PROTOCOL_HTTPS) + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST;

			// 購入履歴一覧画面URL
			dataSendMail["order_history_list_url"] 
				= (string.IsNullOrEmpty(Constants.URL_FRONT_PC))
					? ""
					: Constants.URL_FRONT_PC.Replace(Constants.PROTOCOL_HTTP, Constants.PROTOCOL_HTTPS) + Constants.PAGE_FRONT_ORDER_HISTORY_LIST;

			// 定期購入商品文字列作成
			dataSendMail["fixed_purchase_items"] = GetFixedPurchaseItemsStringForMailTemplete(fixedPurchaseContainer);
			dataSendMail["fixed_purchase_variation_items"] = GetFixedPurchaseItemsStringForMailTemplete(fixedPurchaseContainer, true);

			// 配送希望時間帯セット
			var deliveryCompany = new DeliveryCompanyService().Get(fixedPurchaseContainer.Shippings[0].DeliveryCompanyId);
			if (deliveryCompany != null)
			{
				var shippingTime = deliveryCompany.GetShippingTimeMessage(fixedPurchaseContainer.Shippings[0].ShippingTime);

				dataSendMail[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TIME]
					= (string.IsNullOrEmpty(shippingTime))
					? CommonPage.ReplaceTag("@@DispText.shipping_time_list.none@@")
					: shippingTime;
				// 配送会社名セット
				dataSendMail[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME] = deliveryCompany.DeliveryCompanyName;
				// 配送種別名セット
				dataSendMail["shipping_method_name"] =
					ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, (string)dataSendMail[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]);
			}

			// 配送パターンセット
			dataSendMail.Add("fixed_purchase_pattern", OrderCommon.CreateFixedPurchaseSettingMessage(fixedPurchaseContainer));

			// 次回・次々回配送日セット
			AddLongDateTagsForUser(
				dataSendMail,
				Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE,
				dataSendMail[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE],
				languageLocaleId);
			AddLongDateTagsForUser(
				dataSendMail,
				Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE,
				dataSendMail[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE],
				languageLocaleId);

			// 管理者向けの次回・次々回配送日セット
			AddLongDateTagsForManager(
				dataSendMail,
				Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE,
				dataSendMail[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE]);
			AddLongDateTagsForManager(
				dataSendMail,
				Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE,
				dataSendMail[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE]);

			// 最終購入日セット
			AddLongDateTagsForUser(
				dataSendMail,
				Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE,
				dataSendMail[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE],
				languageLocaleId);

			// 管理者向けの最終購入日セット
			AddLongDateTagsForManager(
				dataSendMail,
				Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE,
				dataSendMail[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE]);
			dataSendMail[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE]
				= DateTimeUtility.ToString(
					dataSendMail[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE],
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					languageLocaleId);
			dataSendMail[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE + STRING_FOR_OPERATOR]
				= DateTimeUtility.ToStringForManager(
					dataSendMail[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE],
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);

			// 定期購入配送キャンセル期限日セット（次回配信日 - 配送キャンセル期限）
			AddLongDateTagsForUser(
				dataSendMail,
				Constants.FIELD_MAIL_FIELD_PURCHASE_CANCEL_DEADLINE_DATE,
				fixedPurchaseContainer.NextShippingDate.Value.AddDays(fixedPurchaseContainer.CancelDeadline.Value * -1),
				languageLocaleId);

			// 購入回数(注文基準/出荷基準)セット
			dataSendMail[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] = fixedPurchaseContainer.OrderCount.ToString();
			dataSendMail[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT] = fixedPurchaseContainer.ShippedCount.ToString();

			// 商品小計を計算しセット
			var totalPrice = 0m;
			foreach (var currentItem in fixedPurchaseContainer.Shippings[0].Items)
			{
				totalPrice += currentItem.GetItemPrice();
			}
			dataSendMail.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, ConvertPriceForMail(totalPrice));

			// 表示通貨価格(グローバルオプションがONの場合 申し込み時の通貨コードでレート計算)
			var globalPrice = CurrencyManager.ToPrice(
				fixedPurchaseContainer.DispCurrencyCode,
				fixedPurchaseContainer.DispCurrencyLocaleId,
				totalPrice);
			dataSendMail.Add("order_price_total_global", globalPrice);

			//-------------------------
			// 次回配送予定日セット
			//-------------------------
			var fixedPurchaseHistoryList = FixedPurchaseService.GetFixedPurchaseHistoryListForMailTemplate(fixedPurchaseId);
			var nextShippingEstimatedDay = string.Empty;
			string nextShippingEstimatedDayForOperator;
			if (fixedPurchaseHistoryList.Length != 0)
			{
				// 当該定期購入情報に未出荷の受注情報が含まれており、かつ配送希望日が指定されていない受注情報が存在する場合は「ご指定なし」をセット
				if (fixedPurchaseHistoryList.Any(item => (item.ShippingDate == null)))
				{
					nextShippingEstimatedDay = CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@");
					nextShippingEstimatedDayForOperator = CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@");
				}
				// 当該定期購入情報に未出荷の受注情報が含まれており、かつ全てに配送希望日が指定されている場合は最短の配送希望日をセット
				else
				{
					nextShippingEstimatedDay = DateTimeUtility.ToString(
						fixedPurchaseHistoryList.Select(item => item.ShippingDate).Cast<DateTime>().OrderBy(x => x).First(),
						DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
						languageLocaleId);
					nextShippingEstimatedDayForOperator = DateTimeUtility.ToStringForManager(
						fixedPurchaseHistoryList.Select(item => item.ShippingDate).Cast<DateTime>().OrderBy(x => x).First(),
						DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
				}
			}
			// 当該定期購入情報に未出荷の受注情報が含まれていない場合、次回配送日をセット
			else
			{
				nextShippingEstimatedDay = DateTimeUtility.ToString(
					dataSendMail[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE],
					DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
					languageLocaleId);
				nextShippingEstimatedDayForOperator = DateTimeUtility.ToStringForManager(
					dataSendMail[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE],
					DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
			}
			dataSendMail.Add("next_shipping_estimated_date", nextShippingEstimatedDay);
			dataSendMail["next_shipping_estimated_date" + STRING_FOR_OPERATOR] = nextShippingEstimatedDayForOperator;

			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// ポイント取得
				var userPoint = PointOptionUtility.GetUserPoint((string)dataSendMail[Constants.FIELD_USERPOINT_USER_ID], Constants.FLG_USERPOINT_POINT_KBN_BASE);
				// 現在の保有ポイント
				dataSendMail.Add("current_user_point", StringUtility.ToNumeric(userPoint.PointUsable));

				if (Constants.FIXEDPURCHASE_OPTION_ENABLED
					&& Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE
					&& (fixedPurchaseContainer.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON))
				{
					dataSendMail["next_shipping_use_point"] = CommonPage.ReplaceTag("@@DispText.fixed_purchase.UseAllPointFlg@@");
				}
			}

			dataSendMail.Add("default_shipping_date_text", CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@"));

			// 定期再開予定日セット
			if (dataSendMail[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE] == DBNull.Value)
			{
				dataSendMail[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_yyyymd(d)"]
					= dataSendMail[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_yyyymmdd(d)"]
					= "指定なし";
			}
			else
			{
				AddLongDateTagsForUser(
					dataSendMail,
					Constants.FIELD_FIXEDPURCHASE_RESUME_DATE,
					dataSendMail[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE],
					languageLocaleId);
			}

			// 休止理由
			dataSendMail[Constants.FIELD_FIXEDPURCHASE_SUSPEND_REASON] = fixedPurchaseContainer.SuspendReason;

			dataSendMail[Constants.TAG_FIXED_PURCHASE_MEMO] = fixedPurchaseContainer.Memo;

			var subscriptionBox = new DataCacheController.SubscriptionBoxCacheController().CacheData
				.FirstOrDefault(sb => (sb.CourseId == fixedPurchaseContainer.SubscriptionBoxCourseId));
			var subscriptionBoxDisplayName = (subscriptionBox != null) ? subscriptionBox.DisplayName : "";
			dataSendMail.Add(Constants.MAILTAG_SUBSCRIPTION_BOX_DISPLAY_NAME, subscriptionBoxDisplayName);

			SettingOrderExtend(dataSendMail, fixedPurchaseContainer.DataSource);

			// 購入回数（注文基準）
			var currentOrder = new OrderService().GetLastFixedPurchaseOrder(fixedPurchaseId);
			dataSendMail[Constants.FIELD_ORDER_ORDER_COUNT_ORDER] = currentOrder.OrderCountOrder.HasValue
				? currentOrder.OrderCountOrder.Value.ToString()
				: string.Empty;

			var orderItemsProductId = fixedPurchaseContainer.Shippings[0].Items.Select(v => v.ProductId)
				.ToArray();
			SetOrderItemsInfo(dataSendMail, fixedPurchaseContainer.ShopId, orderItemsProductId);

			// 広告コード情報をセット
			var user = new UserService().Get(fixedPurchaseContainer.UserId);
			dataSendMail[Constants.FIELD_ORDER_ADVCODE_FIRST] = user.AdvcodeFirst ?? string.Empty;

			AddMailUnsubscribeTags(dataSendMail, fixedPurchaseContainer.UserId, fixedPurchaseContainer.OwnerMailAddr);

			return dataSendMail;
		}

		/// <summary>
		/// 定期購入メールテンプレート用に、定期購入商品情報の文字列を取得する。
		/// </summary>
		/// <param name="fixedPurchaseContainer">定期購入情報</param>
		/// <param name="variationDisplayFlg">バリエーション商品名を表示するか</param>
		/// <returns>定期購入商品文字列</returns>
		private string GetFixedPurchaseItemsStringForMailTemplete(FixedPurchaseContainer fixedPurchaseContainer, bool variationDisplayFlg = false)
		{
			var fixedPurchaseItems = fixedPurchaseContainer.Shippings[0].Items;
			var fixedPurchaseItemsString = new StringBuilder();
			var itemNo = 1;

			foreach (var currentItem in fixedPurchaseItems)
			{
				fixedPurchaseItemsString.Append("-(").Append(itemNo).Append(")----------" + this.BorderString).Append("\r\n");
				fixedPurchaseItemsString.Append("商品ID    ：").Append(currentItem.VariationId).Append("\r\n");
				fixedPurchaseItemsString.Append("商品名    ：");
				if (variationDisplayFlg)
				{
					fixedPurchaseItemsString
					.Append(
						ProductCommon.CreateProductJointName(
							currentItem.Name,
							currentItem.VariationName1,
							currentItem.VariationName2,
							currentItem.VariationName3))
					.Append("\r\n");
				}
				else
				{
					fixedPurchaseItemsString.Append(currentItem.Name).Append("\r\n");
				}

				if (currentItem.ProductOptionTexts != string.Empty)
				{
					// 付帯情報を改行するためにReplaceを使用
					var prductOptionTexts = ProductOptionSettingHelper.GetDisplayProductOptionTexts(currentItem.ProductOptionTexts)
						.Replace("　", "\n            ");
					fixedPurchaseItemsString.Append("            ").Append(prductOptionTexts).Append("\r\n");
				}
				var productPrice = currentItem.GetValidPrice();
				fixedPurchaseItemsString.Append("商品単価  ：").Append(ConvertPriceForMailByConfig(productPrice)).Append(GetProductTaxText()).Append("\r\n");
				fixedPurchaseItemsString.Append("数量      ：").Append(StringUtility.ToNumeric(currentItem.ItemQuantity)).Append("\r\n");

				itemNo++;
			}

			var stringResult = fixedPurchaseItemsString.ToString();

			return stringResult.Remove(stringResult.LastIndexOf("\r\n"));
		}
	}
}
