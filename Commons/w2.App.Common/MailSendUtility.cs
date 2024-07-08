/*
=========================================================================================================
  Module      : メール送信共通モジュール(MailSendUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Line.LineDirectConnect;
using w2.App.Common.Line.LineDirectMessage.MessageType;
using w2.App.Common.Mail;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.ShopMessage;
using w2.App.Common.SMS;
using w2.Common.Net.Mail;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.GlobalSMS;
using w2.Domain.MailSendLog;
using w2.Domain.MailTemplate;
using w2.Domain.MessagingAppContents;
using w2.Domain.User;
using w2.Domain.User.Helper;

namespace w2.App.Common
{
	///*********************************************************************************************
	/// <summary>
	/// メール送信ユーティリティ
	/// </summary>
	///*********************************************************************************************
	public class MailSendUtility : SmtpMailSender
	{
		#region 定数
		/// <summary>振り分けタグ：否定</summary>
		private const string TAG_NOT_USE = "Un";
		/// <summary>振り分けタグ：注文者区分(会員)</summary>
		private const string TAG_ORDEROWNER_OWNER_KBN_USER = "OwnerKbnUser";
		/// <summary>振り分けタグ：注文者区分(ゲスト)</summary>
		private const string TAG_ORDEROWNER_OWNER_KBN_GUEST = "OwnerKbnGuest";
		/// <summary>振り分けタグ：配送種別ID</summary>
		private const string TAG_ORDER_SHIPPING_ID = "ShippingId";
		/// <summary>振り分けタグ：運送会社ID</summary>
		private const string TAG_ORDER_DELIVERY_COMPANY_ID = "DeliveryCompanyId";
		/// <summary>振り分けタグ：配送方法(宅配便)</summary>
		private const string TAG_ORDER_SHIPPING_METHOD_EXPRESS = "UseExpressDelivery";
		/// <summary>振り分けタグ：配送方法(メール便)</summary>
		private const string TAG_ORDER_SHIPPING_METHOD_MAIL = "UseMailDelivery";
		/// <summary>振り分けタグ：決済種別ID</summary>
		private const string TAG_ORDER_ORDER_PAYMENT_KBN = "OrderPaymentKbn";
		/// <summary>振り分けタグ：モールID</summary>
		private const string TAG_ORDER_MALL_ID = "MallId";
		/// <summary>振り分けタグ：発行ポイント</summary>
		private const string TAG_PUBLISH_POINT = "PublishPoint";
		/// <summary>振り分けタグ：メンバーランクOP</summary>
		private const string TAG_MEMBER_RANK = "MemberRank";
		/// <summary>振り分けタグ：利用ポイント</summary>
		private const string TAG_USE_POINT = "UsePoint";
		/// <summary>振り分けタグ：適用されなかった戻しポイント分</summary>
		private const string TAG_NOT_USE_POINT = "NotUsePoint";
		/// <summary>振り分けタグ：ポイントOP利用有無</summary>
		private const string TAG_POINT_OP = "PointOption";
		/// <summary>振り分けタグ：発行クーポン</summary>
		private const string TAG_PUBLISH_COUPON = "PublishCoupon";
		/// <summary>振り分けタグ：利用クーポン</summary>
		private const string TAG_USE_COUPON = "UseCoupon";
		/// <summary>振り分けタグ：定期購入ステータス</summary>
		private const string TAG_FIXEDPURCHASE_STATUS = "FixedPurchaseStatus";
		/// <summary>振り分けタグ：配送希望日</summary>
		private const string TAG_USE_SHIPPING_DATE = "UseShippingDate";
		/// <summary>振り分けタグ：配送希望時間帯</summary>
		private const string TAG_USE_SHIPPING_TIME = "UseShippingTime";
		/// <summary>振り分けタグ：定期購入情報</summary>
		private const string TAG_USE_FIXED_PURCHASE = "UseFixedPurchase";
		/// <summary>振り分けタグ：のし情報</summary>
		private const string TAG_USE_WRAPPING_PAPER = "UseWrappingPaper";
		/// <summary>振り分けタグ：包装情報</summary>
		private const string TAG_USE_WRAPPING_BAG = "UseWrappingBag";
		/// <summary>振り分けタグ：配送料別途見積もり</summary>
		private const string TAG_USE_SHIPPINGPRICE_SEPARATE_ESTIMATES = "UseSeparateEstimates";
		/// <summary>振り分けタグ：デジタルコンテンツ商品含</summary>
		private const string TAG_USE_DIGITAL_CONTENTS = "UseDigitalContents";
		/// <summary>振り分けタグ：入荷通知受付区分(会員)</summary>
		private const string TAG_PRODUCTARRIVAL_KBN_USER = "IsUser";
		/// <summary>振り分けタグ：入荷通知受付区分(ゲスト)</summary>
		private const string TAG_PRODUCTARRIVAL_KBN_GUEST = "IsGuest";
		/// <summary>振り分けタグ：ユーザ拡張項目設定ID</summary>
		private const string TAG_USEREXTEND_ID = "UserExtendId";
		/// <summary>振り分けタグ：注文セットプロモーション(商品割引分)</summary>
		private const string TAG_SETPROMOTION_PRODUCT_DISCOUNT = "SetPromotionProductDiscount";
		/// <summary>振り分けタグ：注文セットプロモーション(配送料割引分)</summary>
		private const string TAG_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT = "SetPromotionShippingChargeDiscount";
		/// <summary>振り分けタグ：注文セットプロモーション(決済手数料割引分)</summary>
		private const string TAG_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT = "SetPromotionPaymentChargeDiscount";
		/// <summary>振り分けタグ：注文セットプロモーション割引有無</summary>
		private const string TAG_HAS_ANY_SETPROMOTION_DISCOUNT = "HasAnySetPromotionDiscount";
		/// <summary>振り分けタグ：ユーザー管理レベルID</summary>
		private const string TAG_USER_USER_MANAGEMENT_LEVEL = "UserManagementLevel";
		/// <summary>振り分けタグ：拡張ステータス</summary>
		private const string TAG_ORDER_EXTEND_STATUS = "ExtendStatus";
		/// <summary>振り分けタグ：定期購入回数(注文時点)</summary>
		private const string TAG_ORDER_FIXED_PURCHASE_ORDER_COUNT = "FixedPurchaseOrderCount";
		/// <summary>振り分けタグ：定期購入回数(出荷時点)</summary>
		private const string TAG_ORDER_FIXED_PURCHASE_SHIPPED_COUNT = "FixedPurchaseShippedCount";
		/// <summary>振り分けタグ：商品税別設定</summary>
		private const string TAG_PRODUCT_TAX_EXCLUDED = "ProductTaxExcluded";
		/// <summary>振り分けタグ：領収書希望あり</summary>
		private const string TAG_HAS_RECEIPT = "HasReceipt";
		/// <summary>振り分けタグ：商品ID</summary>
		private const string TAG_PRODUCT_ID = "ProductId";
		/// <summary>振り分けタグ：カテゴリーID</summary>
		private const string TAG_CATEGORY_ID = "CategoryId";
		/// <summary>振り分けタグ：ブランドID</summary>
		private const string TAG_BRAND_ID = "BrandId";
		/// <summary>振り分けタグ：メンバーランクID</summary>
		private const string TAG_MEMBER_RANK_ID = "MemberRankId";
		/// <summary>振り分けタグ：購入回数（注文基準）</summary>
		private const string TAG_ORDER_COUNT_ORDER = "OrderCountOrder";
		/// <summary>振り分けタグ：初回広告コード</summary>
		private const string TAG_ADV_CODE_FIRST = "AdvCodeFirst";
		/// <summary>振り分けタグ：最新広告コード</summary>
		private const string TAG_ADV_CODE_NEW = "AdvCodeNew";
		/// <summary>振り分けタグ：頒布会購入情報</summary>
		private const string TAG_USE_SUBSCRIPTION_BOX = "UseSubscriptionBox";
		/// <summary>振り分けタグ：Gift shipping loop</summary>
		private const string TAG_GIFT_SHIPPING_LOOP = "GiftShippingLoop";
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="userId">ユーザーID（メール送信ログ保存用）</param>
		/// <param name="inputParam">入力パラメタ</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="userMailAddress">ユーザーメールアドレス</param>
		/// <remarks>ユーザーIDの指定がある場合、メール送信ログを登録</remarks>
		public MailSendUtility(
			string shopId,
			string mailId,
			string userId,
			Hashtable inputParam,
			string languageCode = null,
			string languageLocaleId = null,
			string userMailAddress = null)
			: this(shopId, mailId, userId, inputParam, true, Constants.MailSendMethod.Auto, languageCode, languageLocaleId, userMailAddress)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="userId">ユーザーID（メール送信ログ保存用）</param>
		/// <param name="inputParam">入力パラメタ</param>
		/// <param name="isPC">PCフラグ</param>
		/// <param name="mailSendMethod">メール送信方法</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="userMailAddress">ユーザーメールアドレス</param>
		/// <remarks>ユーザーIDの指定がある場合、メール送信ログを登録</remarks>
		public MailSendUtility(
			string shopId,
			string mailId,
			string userId,
			Hashtable inputParam,
			bool isPC,
			Constants.MailSendMethod mailSendMethod,
			string languageCode = null,
			string languageLocaleId = null,
			string userMailAddress = null)
			: base()
		{
			//------------------------------------------------------
			// メールテンプレート情報取得
			//------------------------------------------------------
			var mailTemplate = GetMailTemplateInfo(shopId, mailId, languageCode, languageLocaleId);

			//------------------------------------------------------
			// 各変数へセット
			//------------------------------------------------------
			this.ShopId = shopId;
			this.MailId = mailId;
			this.UserId = userId;
			this.UserMailAddress = userMailAddress;
			this.TmpName = mailTemplate.MailName;
			this.TmpFrom = mailTemplate.MailFrom;
			this.TmpTo = mailTemplate.MailTo;
			this.TmpCC = mailTemplate.MailCc;
			this.TmpBcc = mailTemplate.MailBcc;
			this.TmpSubject = isPC ? mailTemplate.MailSubject : mailTemplate.MailSubjectMobile;
			this.TmpBody = isPC ? mailTemplate.MailBody : mailTemplate.MailBodyMobile;
			this.MailSendMethod = mailSendMethod;
			this.AutoSendFlgCheck = mailTemplate.AutoSendFlgCheck;

			// Body HTML setting
			this.CanSendHtml = (mailTemplate.SendHtmlFlg == Constants.FLG_MAILTEMPLATE_SEND_HTML_FLG_SEND);
			this.TmpBodyHtml = this.CanSendHtml
				? mailTemplate.MailTextHtml
				: string.Empty;

			// PC・MBメール別エンコーディング設定
			SetEncoding(
				isPC ? Constants.PC_MAIL_DEFAULT_ENCODING : Constants.MOBILE_MAIL_ENCODING,
				isPC ? Constants.PC_MAIL_DEFAULT_TRANSFER_ENCODING : Constants.MOBILE_MAIL_TRANSFER_ENCODING);

			// SMS情報
			// SMSオプションが有効 かつ メールテンプレートのSMS利用フラグがON かつ ユーザーが特定できる場合のみ
			this.UseSms = (Constants.GLOBAL_SMS_OPTION_ENABLED
				&& (mailTemplate.SmsUseFlg == MailTemplateModel.SMS_USE_FLG_ON)
				&& (string.IsNullOrEmpty(userId) == false));

			if (this.UseSms)
			{
				var carrier = SMSHelper.GetSMSPhoneCarrier(userId);
				var sv = new GlobalSMSService();
				var smsTemplate = sv.GetSmsTemplate(shopId, mailId, carrier);
				this.TmpSmsText = smsTemplate.SmsText;
			}
			else
			{
				this.TmpSmsText = string.Empty;
			}

			// LINE情報
			this.UseLine = (Line.Constants.LINE_DIRECT_OPTION_ENABLED
				&& (mailTemplate.LineUseFlg == MailTemplateModel.LINE_USE_FLG_ON)
				&& (string.IsNullOrEmpty(userId) == false));
			if (this.UseLine)
			{
				var lineTemplats = new MessagingAppContentsService().GetAllContentsEachMessagingAppKbn(
					shopId,
					MessagingAppContentsModel.MASTER_KBN_MAILTEMPLATE,
					mailId,
					MessagingAppContentsModel.MESSAGING_APP_KBN_LINE);
				this.TmpLineText = lineTemplats.Select(t => (t.IsText)
					? ShopMessageUtil.ConvertShopMessage(t.Contents, languageCode, languageLocaleId, false)
					: t.Contents).ToArray();
			}
			else
			{
				this.TmpLineText = new string[0];
			}

			// サイト基本情報設定置換
			this.TmpFrom = ShopMessageUtil.ConvertShopMessage(
				AdjustShopMessageTagWithHavingSpace(this.TmpFrom),
				languageCode,
				languageLocaleId,
				false).TrimAllSpaces();
			mailTemplate.MailFromName = ShopMessageUtil.ConvertShopMessage(
				mailTemplate.MailFromName,
				languageCode,
				languageLocaleId,
				false);
			this.TmpBcc = ShopMessageUtil.ConvertShopMessage(
				AdjustShopMessageTagWithHavingSpace(this.TmpBcc),
				languageCode,
				languageLocaleId,
				false).TrimAllSpaces();
			this.TmpSubject = ShopMessageUtil.ConvertShopMessage(this.TmpSubject, languageCode, languageLocaleId, false);
			this.TmpBody = ShopMessageUtil.ConvertShopMessage(this.TmpBody, languageCode, languageLocaleId, false);
			if (this.CanSendHtml)
			{
				this.TmpBodyHtml = ShopMessageUtil.ConvertShopMessage(
					this.TmpBodyHtml,
					languageCode,
					languageLocaleId,
					doHtmlEncodeChangeToBr: true);
			}

			// サイトのルートパス情報を置換
			var siteRootPath = new MailTemplateSiteRootPath();
			this.TmpSubject = siteRootPath.ConvertSiteRootPath(this.TmpSubject, false);
			this.TmpBody = siteRootPath.ConvertSiteRootPath(this.TmpBody, false);
			this.TmpLineText = this.TmpLineText.Select(v => siteRootPath.ConvertSiteRootPath(v, false)).ToArray();
			if (this.CanSendHtml)
			{
				this.TmpBodyHtml = siteRootPath.ConvertSiteRootPath(this.TmpBodyHtml, doHtmlEncodeChangeToBr: true);
			}
			
			// 登録解除リンクを置換
			var unsubscribeUrl = new UrlCreator($"{Constants.PROTOCOL_HTTPS}{Constants.SITE_DOMAIN}{Constants.PATH_ROOT_FRONT_PC}{Constants.MAIL_LISTUNSUBSCRIBE_URL}")
				.AddParam(Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_USER_ID, userId)
				.AddParam(Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_VERIFICATION_KEY, UnsubscribeVarificationHelper.Hash(userId, userMailAddress ?? ""))
				.CreateUrl();
			this.TmpBody = this.TmpBody.Replace("@@ mail_unsubscribe_link @@", unsubscribeUrl);
			this.TmpBodyHtml = this.TmpBodyHtml.Replace("@@ mail_unsubscribe_link @@", unsubscribeUrl);

			//------------------------------------------------------
			// パラメタ置換
			//------------------------------------------------------
			ConvertParameter(inputParam);

			//------------------------------------------------------
			// モールIDを取得
			//------------------------------------------------------
			if (inputParam.Contains(Constants.FIELD_ORDER_MALL_ID))
			{
				this.MallId = (string)inputParam[Constants.FIELD_ORDER_MALL_ID];
			}

			//------------------------------------------------------
			// オブジェクトにセット
			//------------------------------------------------------
			SetFrom(this.TmpFrom.Trim(), mailTemplate.MailFromName);
			foreach (string strMailAddr in this.TmpTo.Split(','))
			{
				AddTo(strMailAddr.Trim());
			}
			foreach (string strMailAddr in this.TmpCC.Split(','))
			{
				AddCC(strMailAddr.Trim());
			}
			foreach (string strMailAddr in this.TmpBcc.Split(','))
			{
				AddBcc(strMailAddr.Trim());
			}
			SetSubject(this.TmpSubject);
			SetBody(this.TmpBody);
			SetBodyHtml(this.TmpBodyHtml);
			// Return-Pathセット
			SetReturnPath(Constants.ERROR_MAILADDRESS);
		}
		/// <summary>
		/// コンストラクタ(タグ置換済みの本文等が既にある場合に使用する）
		/// </summary>
		/// <param name="mailSendMethod">メール送信方法</param>
		public MailSendUtility(Constants.MailSendMethod mailSendMethod)
			: base()
		{
			this.MailSendMethod = mailSendMethod;

			// Return-Pathセット
			SetReturnPath(Constants.ERROR_MAILADDRESS);
		}

		/// <summary>
		/// メールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID></param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>メールテンプレート情報</returns>
		public static MailTemplateModel GetMailTemplateInfo(string shopId, string mailId, string languageCode = null, string languageLocaleId = null)
		{
			// グローバルOP：ONの場合は、言語コード／言語ロケールIDを指定してテンプレートを取得する
			var mailTemplate = (Constants.GLOBAL_OPTION_ENABLE)
				? new MailTemplateService().Get(shopId, mailId, languageCode, languageLocaleId)
				: new MailTemplateService().Get(shopId, mailId);
			if (mailTemplate == null)
			{
				throw new ApplicationException("メールテンプレートが取得できませんでした。（店舗ID:" + shopId + "、メールテンプレートID:" + mailId + "）");
			}
			return mailTemplate;
		}

		/// <summary>
		/// 埋め込みタグ空白入れる
		/// </summary>
		/// <param name="mailTag">埋め込みタグ</param>
		/// <returns>空白入れた埋め込みタグ</returns>
		public static string AdjustShopMessageTagWithHavingSpace(string mailTag)
		{
			if (Regex.IsMatch(mailTag, ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_WITHOUT_SPACE))
			{
				mailTag = mailTag.Replace(ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_FIRST_PART_WITHOUT_SPACE, ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_FIRST_PART_WITH_SPACE);
				mailTag = Regex.Replace(mailTag, ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_WITHOUT_LAST_TAG, "$0 ");
			}

			return mailTag;
		}

		/// <summary>
		/// パラメタ置換
		/// </summary>
		/// <param name="inputParam">入力パラメタ</param>
		private void ConvertParameter(Hashtable inputParam)
		{
			// 入力パラメタが無ければ何もしない
			if (inputParam == null) return;

			// ValueTextから取得できる項目は先に変換
			ReplaceMailWithValueText(inputParam, Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN);					// ユーザー区分
			ReplaceMailWithValueText(inputParam, Constants.TABLE_USER, Constants.FIELD_USER_SEX);						// 性別
			ReplaceMailWithValueText(inputParam, Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG);					// メール配信フラグ
			ReplaceMailWithValueText(inputParam, Constants.TABLE_MOBILEGROUP, Constants.FIELD_MOBILEGROUP_CAREER_ID);	// モバイルキャリアID

			//------------------------------------------------------
			// ギフトの場合のループ置換処理（先にループを置換）
			//------------------------------------------------------
			if ((inputParam.ContainsKey(Constants.FIELD_ORDER_GIFT_FLG))
				&& ((string)inputParam[Constants.FIELD_ORDER_GIFT_FLG] == Constants.FLG_ORDER_GIFT_FLG_ON))
			{
				SetTagEnabled("IsGift");
				SetTagDisabled("IsNotGift");

				this.TmpSubject = ReplaceLoop(this.TmpSubject, TAG_GIFT_SHIPPING_LOOP, inputParam);
				this.TmpBody = ReplaceLoop(this.TmpBody, TAG_GIFT_SHIPPING_LOOP, inputParam);
				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = ReplaceLoop(this.TmpBodyHtml, TAG_GIFT_SHIPPING_LOOP, inputParam, changeToBrTag: true);
				}
				if (this.UseSms) { this.TmpSmsText = ReplaceLoop(this.TmpSmsText, TAG_GIFT_SHIPPING_LOOP, inputParam); }
				if (this.UseLine && (this.TmpLineText.Length > 0))
				{
					this.TmpLineText = this.TmpLineText.Select(text => ReplaceLoop(text, TAG_GIFT_SHIPPING_LOOP, inputParam)).ToArray();
				}
			}
			else
			{
				SetTagEnabled("IsNotGift");
				SetTagDisabled("IsGift");
			}

			// 別出荷フラグを設定
			if ((inputParam.ContainsKey(Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG))
				&& ((string)inputParam[Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG] == Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID))
			{
				SetTagEnabled("IsAnotherShipping");
				SetTagDisabled("IsNotAnotherShipping");
			}
			else
			{
				SetTagEnabled("IsNotAnotherShipping");
				SetTagDisabled("IsAnotherShipping");
			}

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
			{
				// Set visible for address convenience store
				if ((inputParam.ContainsKey(Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG))
					&& ((string)inputParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
				{
					SetTagEnabled("IsStoreConvenience");
					SetTagDisabled("IsNotStoreConvenience");
				}
				else
				{
					SetTagEnabled("IsStoreConvenience");
					SetTagDisabled("IsNotStoreConvenience");
				}
			}

			// User Management Level
			if (inputParam.Contains(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID))
			{
				// Show User Management Level
				SetTagEnabled(string.Format("{0}:{1}", TAG_USER_USER_MANAGEMENT_LEVEL, inputParam[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID].ToString()));

				// Hide User Management Level
				SetTagDisabled(TAG_USER_USER_MANAGEMENT_LEVEL + ":((?!@@>).)*");
			}

			// 領収書出力ＵＲＬ設定／定期台帳の領収書情報表示
			if (Constants.RECEIPT_OPTION_ENABLED
				&& ((inputParam.ContainsKey(Constants.TAG_RECEIPT_URL)
						&& (string.IsNullOrEmpty((string)inputParam[Constants.TAG_RECEIPT_URL]) == false))
					|| (inputParam.ContainsKey(Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG)
						&& ((string)inputParam[Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG] == Constants.FLG_ORDER_RECEIPT_FLG_ON))))
			{
				SetTagEnabled(TAG_HAS_RECEIPT);
			}
			else
			{
				SetTagDisabled(TAG_HAS_RECEIPT);
			}

			//------------------------------------------------------
			// Convert To Order Extend Status
			//------------------------------------------------------
			ConvertToOrderExtendStatus(inputParam);

			//------------------------------------------------------
			// ユーザ拡張項目のループ置換処理
			//------------------------------------------------------
			if (Constants.BOTCHAN_OPTION == false) ConvertToUserExtend(inputParam);

			//------------------------------------------------------
			// 注文セットプロモーションのループ置換処理
			//------------------------------------------------------
			if (inputParam.ContainsKey("SetPromotionProductDiscountLoop"))
			{
				this.TmpBody = ReplaceLoop(this.TmpBody, "SetPromotionProductDiscountLoop", inputParam);
				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = ReplaceLoop(
						this.TmpBodyHtml,
						"SetPromotionProductDiscountLoop",
						inputParam,
						changeToBrTag: true);
				}
				if (this.UseSms) { this.TmpSmsText = ReplaceLoop(this.TmpSmsText, "SetPromotionProductDiscountLoop", inputParam); }
				if (this.UseLine && (this.TmpLineText.Length > 0))
				{
					this.TmpLineText = this.TmpLineText.Select(text => ReplaceLoop(text, "SetPromotionProductDiscountLoop", inputParam)).ToArray();
				}
			}
			if (inputParam.ContainsKey("SetPromotionShippingChargeDiscountLoop"))
			{
				this.TmpBody = ReplaceLoop(this.TmpBody, "SetPromotionShippingChargeDiscountLoop", inputParam);
				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = ReplaceLoop(
						this.TmpBodyHtml,
						"SetPromotionShippingChargeDiscountLoop",
						inputParam,
						changeToBrTag: true);
				}
				if (this.UseSms) { this.TmpSmsText = ReplaceLoop(this.TmpSmsText, "SetPromotionShippingChargeDiscountLoop", inputParam); }
				if (this.UseLine && (this.TmpLineText.Length > 0))
				{
					this.TmpLineText = this.TmpLineText.Select(text => ReplaceLoop(text, "SetPromotionShippingChargeDiscountLoop", inputParam)).ToArray();
				}
			}
			if (inputParam.ContainsKey("SetPromotionPaymentChargeDiscountLoop"))
			{
				this.TmpBody = ReplaceLoop(this.TmpBody, "SetPromotionPaymentChargeDiscountLoop", inputParam);
				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = ReplaceLoop(
						this.TmpBodyHtml,
						"SetPromotionPaymentChargeDiscountLoop",
						inputParam,
						changeToBrTag: true);
				}
				if (this.UseSms) { this.TmpSmsText = ReplaceLoop(this.TmpSmsText, "SetPromotionPaymentChargeDiscountLoop", inputParam); }
				if (this.UseLine && (this.TmpLineText.Length > 0))
				{
					this.TmpLineText = this.TmpLineText.Select(text => ReplaceLoop(text, "SetPromotionPaymentChargeDiscountLoop", inputParam)).ToArray();
				}
			}
			if (inputParam.ContainsKey(Constants.MAILTAG_ORDER_ITEMS_LOOP))
			{
				this.TmpBody = ReplaceOrderItemsLoop(this.TmpBody, (Hashtable[])inputParam[Constants.MAILTAG_ORDER_ITEMS_LOOP]);
				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = ReplaceOrderItemsLoop(
						this.TmpBodyHtml,
						(Hashtable[])inputParam[Constants.MAILTAG_ORDER_ITEMS_LOOP],
						changeToBrTag: true);
				}
				if (this.UseSms)
				{
					this.TmpSmsText = ReplaceOrderItemsLoop(this.TmpSmsText, (Hashtable[])inputParam[Constants.MAILTAG_ORDER_ITEMS_LOOP]);
				}
				if (this.UseLine && (this.TmpLineText.Length > 0))
				{
					this.TmpLineText = this.TmpLineText.Select(text => ReplaceOrderItemsLoop(text, (Hashtable[])inputParam[Constants.MAILTAG_ORDER_ITEMS_LOOP])).ToArray();
				}
			}
			if (inputParam.ContainsKey(Constants.CONST_AUTHENTICATION_DEADLINE))
			{
				var deadLine = (this.MailId == Constants.CONST_MAIL_ID_SEND_2_STEP_AUTHENTICATION_CODE)
					? StringUtility.ToEmpty(inputParam[Constants.CONST_AUTHENTICATION_DEADLINE])
					: Constants.CONST_AUTHENTICATION_DEADLINE;
				this.TmpBody = this.TmpBody.Replace("@@ " + Constants.CONST_AUTHENTICATION_DEADLINE + " @@", deadLine);
			}

			ReplaceStorePickupTags(inputParam);

			//------------------------------------------------------
			// データの置換（ループ以外）
			//------------------------------------------------------
			foreach (string strKey in inputParam.Keys)
			{
				if ((inputParam[strKey] is IList) == false)
				{
					this.TmpSubject = this.TmpSubject.Replace("@@ " + strKey + " @@", StringUtility.ToEmpty(inputParam[strKey]));
					this.TmpBody = this.TmpBody.Replace("@@ " + strKey + " @@", StringUtility.ToEmpty(inputParam[strKey]));
					if (this.CanSendHtml)
					{
						this.TmpBodyHtml = this.TmpBodyHtml.Replace(
							"@@ " + strKey + " @@",
							StringUtility.ChangeToBrTag(StringUtility.ToEmpty(inputParam[strKey])));
					}
					if (this.UseSms) { this.TmpSmsText = this.TmpSmsText.Replace("@@ " + strKey + " @@", StringUtility.ToEmpty(inputParam[strKey])); }
					if (this.UseLine && (this.TmpLineText.Length > 0))
					{
						this.TmpLineText = this.TmpLineText.Select(text => text.Replace("@@ " + strKey + " @@", StringUtility.ToEmpty(inputParam[strKey]))).ToArray();
					}
				}
			}

			//------------------------------------------------------
			// 注文系の切替タグ
			//------------------------------------------------------
			// 注文者区分が取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_ORDEROWNER_OWNER_KBN))
			{
				if (UserService.IsUser((string)inputParam[Constants.FIELD_ORDEROWNER_OWNER_KBN]))
				{
					SetTagEnabled(TAG_ORDEROWNER_OWNER_KBN_USER);
					SetTagDisabled(TAG_ORDEROWNER_OWNER_KBN_GUEST);
				}
				else if (UserService.IsGuest((string)inputParam[Constants.FIELD_ORDEROWNER_OWNER_KBN]))
				{
					SetTagDisabled(TAG_ORDEROWNER_OWNER_KBN_USER);
					SetTagEnabled(TAG_ORDEROWNER_OWNER_KBN_GUEST);
				}
			}

			// 配送種別IDが取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_ORDER_SHIPPING_ID))
			{
				// 該当配送種別ID有効
				SetTagEnabled(TAG_ORDER_SHIPPING_ID + ":" + (string)inputParam[Constants.FIELD_ORDER_SHIPPING_ID]);

				// その他の配送種別ID無効
				SetTagDisabled(TAG_ORDER_SHIPPING_ID + ":" + "((?!@@>).)*");
			}

			// 運送会社IDが取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID))
			{
				// 該当運送会社ID有効
				SetTagEnabled(TAG_ORDER_DELIVERY_COMPANY_ID + ":" + (string)inputParam[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID]);

				// その他の運送会社ID無効
				SetTagDisabled(TAG_ORDER_DELIVERY_COMPANY_ID + ":" + "((?!@@>).)*");
			}

			// 配送方法が取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD))
			{
				string shippingMethod = inputParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD].ToString();
				// 配送方法が宅配便の場合
				if (shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				{
					// 宅配便有効
					SetTagEnabled(TAG_ORDER_SHIPPING_METHOD_EXPRESS);
					// メール便無効
					SetTagDisabled(TAG_ORDER_SHIPPING_METHOD_MAIL);
				}
				else if (shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				{
					// メール便有効
					SetTagEnabled(TAG_ORDER_SHIPPING_METHOD_MAIL);
					// 宅配便無効
					SetTagDisabled(TAG_ORDER_SHIPPING_METHOD_EXPRESS);
				}
			}

			// 決済種別が取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN))
			{
				string strPaymentId = inputParam[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN].ToString();

				// 該当決済種別ID有効
				SetTagEnabled(TAG_ORDER_ORDER_PAYMENT_KBN + ":" + strPaymentId);

				// その他の決済種別ID無効
				SetTagDisabled(TAG_ORDER_ORDER_PAYMENT_KBN + ":" + "((?!@@>).)*");
			}

			// モールIDが取れれば置換処理実行 ※通常注文時にも適用したいため判定なし
			//if (htInput.Contains(Constants.FIELD_ORDER_MALL_ID))
			{
				string strMallId = StringUtility.ToEmpty(inputParam[Constants.FIELD_ORDER_MALL_ID]);

				// 該当ID有効
				SetTagEnabled(TAG_ORDER_MALL_ID + ":" + strMallId);

				// その他のID無効
				SetTagDisabled(TAG_ORDER_MALL_ID + ":" + "((?!@@>).)*");
			}

			// 会員ランクOP
			SetTagEnabled(TAG_MEMBER_RANK, Constants.MEMBER_RANK_OPTION_ENABLED);

			// 発行ポイントが取れれば置換処理実行
			decimal pointAddForUserRegister = 0;
			decimal pointAddForOrder = 0;
			if (inputParam.Contains(Constants.FIELD_USERPOINT_POINT) || inputParam.Contains(Constants.FIELD_ORDER_ORDER_POINT_ADD))
			{
				// 会員登録メール用
				decimal.TryParse(StringUtility.ToEmpty(inputParam[Constants.FIELD_USERPOINT_POINT]), out pointAddForUserRegister);
				// 注文完了メール用
				decimal.TryParse(StringUtility.ToEmpty(inputParam[Constants.FIELD_ORDER_ORDER_POINT_ADD]), out pointAddForOrder);
			}
			// 発行ポイント有効
			SetTagEnabled(TAG_PUBLISH_POINT, ((pointAddForUserRegister + pointAddForOrder) > 0));

			// 利用ポイントが取れれば置換処理実行
			decimal pointUse = 0;
			if (inputParam.Contains(Constants.FIELD_ORDER_ORDER_POINT_USE))
			{
				decimal.TryParse(StringUtility.ToEmpty(inputParam[Constants.FIELD_ORDER_ORDER_POINT_USE]), out pointUse);
			}
			decimal nextShippingUsePoint = 0;
			if (inputParam.Contains(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT))
			{
				decimal.TryParse(StringUtility.ToEmpty(inputParam[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT]), out nextShippingUsePoint);
			}
			// ポイント有効
			SetTagEnabled(TAG_USE_POINT, ((pointUse > 0) || (nextShippingUsePoint > 0)));
			// 適用されなかった戻しポイントタグの有効
			SetTagEnabled(TAG_NOT_USE_POINT, (nextShippingUsePoint > pointUse));
			if (nextShippingUsePoint > pointUse)
			{
				this.TmpSubject = this.TmpSubject.Replace("@@ " + "not_use_point" + " @@", StringUtility.ToEmpty(nextShippingUsePoint - pointUse));
				this.TmpBody = this.TmpBody.Replace("@@ " + "not_use_point" + " @@", StringUtility.ToEmpty(nextShippingUsePoint - pointUse));
				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = this.TmpBodyHtml.Replace("@@ " + "not_use_point" + " @@", StringUtility.ToEmpty(nextShippingUsePoint - pointUse));
				}
				if (this.UseSms) { this.TmpSmsText = this.TmpSmsText.Replace("@@ " + "not_use_point" + " @@", StringUtility.ToEmpty(nextShippingUsePoint - pointUse)); }
				if (this.UseLine && (this.TmpLineText.Length > 0))
				{
					this.TmpLineText = this.TmpLineText.Select(text => text.Replace("@@ " + "not_use_point" + " @@", StringUtility.ToEmpty(nextShippingUsePoint - pointUse))).ToArray();
				}
			}

			// 発行クーポンが取れれば置換処理実行
			SetTagEnabled(TAG_PUBLISH_COUPON, (StringUtility.ToEmpty(inputParam["publish_coupons"]) != ""));

			// 利用クーポンが取れれば置換処理実行
			SetTagEnabled(TAG_USE_COUPON, (StringUtility.ToEmpty(inputParam[Constants.FIELD_ORDERCOUPON_COUPON_CODE]) != ""));

			// ポイントOPが利用できれば置換処理実行
			SetTagEnabled(TAG_POINT_OP, Constants.W2MP_POINT_OPTION_ENABLED);

			// 定期購入ステータスが取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS))
			{
				string strFixedPurchaseStatus = (string)inputParam[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS];

				// 該当決済種別ID有効
				SetTagEnabled(TAG_FIXEDPURCHASE_STATUS + ":" + strFixedPurchaseStatus);

				// その他の決済種別ID無効
				SetTagDisabled(TAG_FIXEDPURCHASE_STATUS + ":" + "((?!@@>).)*");
			}

			// 配送希望日が取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG))
			{
				if ((string)inputParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG] == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID)
				{
					if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
						&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
						&& inputParam.ContainsKey(Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)
						&& ((string)inputParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
					{
						SetTagDisabled(TAG_USE_SHIPPING_DATE);
					}
					else
					{
						SetTagEnabled(TAG_USE_SHIPPING_DATE);
					}
				}
				else
				{
					SetTagDisabled(TAG_USE_SHIPPING_DATE);
				}
			}

			// 配送希望時間帯が取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG))
			{
				if ((string)inputParam[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG] == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID)
				{
					if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
						&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
						&& inputParam.ContainsKey(Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)
						&& ((string)inputParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
					{
						SetTagDisabled(TAG_USE_SHIPPING_TIME);
					}
					else
					{
						SetTagEnabled(TAG_USE_SHIPPING_TIME);
					}
				}
				else
				{
					SetTagDisabled(TAG_USE_SHIPPING_TIME);
				}
			}

			// 定期購入回数（注文・出荷時点）置換処理実行
			ConvertToFixedPurchaseCount(TAG_ORDER_FIXED_PURCHASE_ORDER_COUNT, Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT, inputParam);
			ConvertToFixedPurchaseCount(TAG_ORDER_FIXED_PURCHASE_SHIPPED_COUNT, Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT, inputParam);

			// 定期購入情報が取れれば置換処理実行
			if (inputParam.Contains("fixed_purchase_pattern"))
			{
				SetTagEnabled(TAG_USE_FIXED_PURCHASE);
				SetTagDisabled(TAG_NOT_USE + TAG_USE_FIXED_PURCHASE);
			}
			else
			{
				SetTagDisabled(TAG_USE_FIXED_PURCHASE);
				SetTagEnabled(TAG_NOT_USE + TAG_USE_FIXED_PURCHASE);
			}

			// 頒布会コースIDがあれば置換処理実行
			if (inputParam.Contains("subscription_box_course_id"))
			{
				SetTagEnabled(TAG_USE_SUBSCRIPTION_BOX);
			}
			else
			{
				SetTagDisabled(TAG_USE_SUBSCRIPTION_BOX);
			}

			// ギフト判定
			bool blIsGift = (inputParam.ContainsKey(Constants.FIELD_ORDER_GIFT_FLG))
				&& ((string)inputParam[Constants.FIELD_ORDER_GIFT_FLG] == Constants.FLG_ORDER_GIFT_FLG_ON);

			// のし情報が取れれば置換処理実行
			if (blIsGift
				&& (inputParam.Contains(Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG))
				&& ((string)inputParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID))
			{
				SetTagEnabled(TAG_USE_WRAPPING_PAPER);
			}
			else
			{
				SetTagDisabled(TAG_USE_WRAPPING_PAPER);
			}

			// 包装情報が取れれば置換処理実行
			if (blIsGift
				&& (inputParam.Contains(Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG))
				&& ((string)inputParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID))
			{
				SetTagEnabled(TAG_USE_WRAPPING_BAG);
			}
			else
			{
				SetTagDisabled(TAG_USE_WRAPPING_BAG);
			}

			// 配送料別途見積もりフラグが取れれば置換処理実行
			if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED
				&& (inputParam.ContainsKey(Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG))
				&& ((string)inputParam[Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] == Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID))
			{
				SetTagEnabled(TAG_USE_SHIPPINGPRICE_SEPARATE_ESTIMATES);
				SetTagDisabled(TAG_NOT_USE + TAG_USE_SHIPPINGPRICE_SEPARATE_ESTIMATES);
			}
			else
			{
				SetTagDisabled(TAG_USE_SHIPPINGPRICE_SEPARATE_ESTIMATES);
				SetTagEnabled(TAG_NOT_USE + TAG_USE_SHIPPINGPRICE_SEPARATE_ESTIMATES);
			}

			// デジタルコンテンツ購入フラグが取れれば置換処理実行
			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED
				&& (inputParam.ContainsKey(Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG))
				&& ((string)inputParam[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG] == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON))
			{
				SetTagEnabled(TAG_USE_DIGITAL_CONTENTS);
				SetTagDisabled(TAG_NOT_USE + TAG_USE_DIGITAL_CONTENTS);
			}
			else
			{
				SetTagDisabled(TAG_USE_DIGITAL_CONTENTS);
				SetTagEnabled(TAG_NOT_USE + TAG_USE_DIGITAL_CONTENTS);
			}

			// ユーザIDに応じて入荷通知受付区分置換処理実行
			if ((string)inputParam[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] == Constants.FLG_USERPRODUCTARRIVALMAIL_USER_ID_GUEST)
			{
				SetTagDisabled(TAG_PRODUCTARRIVAL_KBN_USER);
				SetTagEnabled(TAG_PRODUCTARRIVAL_KBN_GUEST);
			}
			else
			{
				SetTagEnabled(TAG_PRODUCTARRIVAL_KBN_USER);
				SetTagDisabled(TAG_PRODUCTARRIVAL_KBN_GUEST);
			}

			// 注文セットプロモーション情報があれば置換処理実行
			if (inputParam.ContainsKey("SetPromotionProductDiscountLoop"))
			{
				SetTagEnabled(TAG_SETPROMOTION_PRODUCT_DISCOUNT);
			}
			else
			{
				SetTagDisabled(TAG_SETPROMOTION_PRODUCT_DISCOUNT);
			}

			if (inputParam.ContainsKey("SetPromotionShippingChargeDiscountLoop"))
			{
				SetTagEnabled(TAG_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT);
			}
			else
			{
				SetTagDisabled(TAG_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT);
			}

			if (inputParam.ContainsKey("SetPromotionPaymentChargeDiscountLoop"))
			{
				SetTagEnabled(TAG_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT);
			}
			else
			{
				SetTagDisabled(TAG_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT);
			}

			SetTagEnabled(TAG_HAS_ANY_SETPROMOTION_DISCOUNT, inputParam.ContainsKey("SetPromotionProductDiscountLoop") || inputParam.ContainsKey("SetPromotionShippingChargeDiscountLoop") || inputParam.ContainsKey("SetPromotionPaymentChargeDiscountLoop"));

			// 商品税別設定
			SetTagEnabled(TAG_PRODUCT_TAX_EXCLUDED, Constants.MANAGEMENT_INCLUDED_TAX_FLAG == false);

			// 商品IDが取れれば置換処理実行
			if (inputParam.Contains(Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_PRODUCT_ID))
			{
				foreach (var productId in ((string)inputParam[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_PRODUCT_ID]).Split(','))
				{
					// 該当商品ID有効
					SetTagEnabled(TAG_PRODUCT_ID + ":" + productId);
				}
				// その他の商品ID無効
				SetTagDisabled(TAG_PRODUCT_ID + ":" + "((?!@@>).)*");
			}

			// ブランドIDが取れれば置換処理実行
			if (inputParam.Contains(Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_BRAND_ID))
			{
				foreach (var brandId in ((string)inputParam[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_BRAND_ID]).Split(','))
				{
					// 該当商品ID有効
					SetTagEnabled(TAG_BRAND_ID + ":" + brandId);
				}
				// その他の商品ID無効
				SetTagDisabled(TAG_BRAND_ID + ":" + "((?!@@>).)*");
			}

			// カテゴリIDが取れれば置換処理実行
			if (inputParam.Contains(Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_CATEGORY_ID))
			{
				foreach (var categoryId in ((string)inputParam[Constants.MAILTEMPLATE_TAG_ORDER_ITEMS_CATEGORY_ID]).Split(','))
				{
					// 該当商品ID有効
					SetTagEnabled(TAG_CATEGORY_ID + ":" + categoryId);
				}
				// その他の商品ID無効
				SetTagDisabled(TAG_CATEGORY_ID + ":" + "((?!@@>).)*");
			}

			// 会員ランクIDが取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_ORDER_MEMBER_RANK_ID))
			{
				string menberRankId = inputParam[Constants.FIELD_ORDER_MEMBER_RANK_ID].ToString();

				// 該当商品ID有効
				SetTagEnabled(TAG_MEMBER_RANK_ID + ":" + menberRankId);

				// その他の商品ID無効
				SetTagDisabled(TAG_MEMBER_RANK_ID + ":" + "((?!@@>).)*");
			}

			// 購入回数（注文基準）置換処理実行
			ConvertToOrderCountOrder(TAG_ORDER_COUNT_ORDER, Constants.FIELD_ORDER_ORDER_COUNT_ORDER, inputParam);

			// 初回広告コードが取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_ORDER_ADVCODE_FIRST))
			{
				var advCodeFirst = inputParam[Constants.FIELD_ORDER_ADVCODE_FIRST].ToString();
				SetTagEnabled(TAG_ADV_CODE_FIRST + ":" + advCodeFirst);
				SetTagDisabled(TAG_ADV_CODE_FIRST + ":" + "((?!@@>).)*");
			}
			// 最新広告コードが取れれば置換処理実行
			if (inputParam.Contains(Constants.FIELD_ORDER_ADVCODE_NEW))
			{
				var advCodeNew = inputParam[Constants.FIELD_ORDER_ADVCODE_NEW].ToString();
				SetTagEnabled(TAG_ADV_CODE_NEW + ":" + advCodeNew);
				SetTagDisabled(TAG_ADV_CODE_NEW + ":" + "((?!@@>).)*");
			}
		}

		/// <summary>
		/// Convert to Order Extend Status
		/// </summary>
		/// <param name="inputParam">入力パラメタ</param>
		private void ConvertToOrderExtendStatus(Hashtable inputParam)
		{
			string extendStatus = string.Empty;
			for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
			{
				extendStatus = Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i;

				if (inputParam.Contains(extendStatus))
				{
					// Show Order Extend Status
					SetTagEnabled(string.Format("{0}{1}:{2}", TAG_ORDER_EXTEND_STATUS, i, inputParam[extendStatus].ToString()));

					// Hide Order Extend Status
					SetTagDisabled(string.Format("{0}{1}:((?!@@>).)*", TAG_ORDER_EXTEND_STATUS, i));
				}
			}
		}

		/// <summary>
		/// ユーザ拡張項目のループ置換処理
		/// </summary>
		/// <param name="inputParam">入力パラメタ</param>
		private void ConvertToUserExtend(Hashtable inputParam)
		{
			if (inputParam.Contains(Constants.TABLE_USEREXTENDSETTING) && inputParam.Contains(Constants.TABLE_USEREXTEND))
			{
				var userExtend = (UserExtendModel)inputParam[Constants.TABLE_USEREXTEND];
				foreach (var userExtendSetting in ((UserExtendSettingList)inputParam[Constants.TABLE_USEREXTENDSETTING]).Items)
				{
					Hashtable keyValue = new Hashtable();
					keyValue.Add("userextend_name", userExtendSetting.SettingName);
					keyValue.Add("userextend_text", userExtend.UserExtendDataText[userExtendSetting.SettingId]);
					keyValue.Add("userextend_value", userExtend.UserExtendDataValue[userExtendSetting.SettingId]);

					string tag = TAG_USEREXTEND_ID + ":" + userExtendSetting.SettingId;
					List<Hashtable> list = new List<Hashtable>();
					list.Add(keyValue);

					Hashtable userExtendInfo = new Hashtable();
					userExtendInfo.Add(tag, list);

					this.TmpFrom = ReplaceLoop(this.TmpFrom, tag, userExtendInfo);
					this.TmpTo = ReplaceLoop(this.TmpTo, tag, userExtendInfo);
					this.TmpCC = ReplaceLoop(this.TmpCC, tag, userExtendInfo);
					this.TmpBcc = ReplaceLoop(this.TmpBcc, tag, userExtendInfo);
					this.TmpSubject = ReplaceLoop(this.TmpSubject, tag, userExtendInfo);
					this.TmpBody = ReplaceLoop(this.TmpBody, tag, userExtendInfo);
					if (this.CanSendHtml)
					{
						this.TmpBodyHtml = ReplaceLoop(this.TmpBodyHtml, tag, userExtendInfo);
					}
					if (this.UseSms) { this.TmpSmsText = ReplaceLoop(this.TmpSmsText, tag, userExtendInfo); }
					if (this.UseLine && (this.TmpLineText.Length > 0))
					{
						this.TmpLineText = this.TmpLineText.Select(text => ReplaceLoop(text, tag, userExtendInfo)).ToArray();
					}
				}
			}

			// 存在しない設定IDの切替タグは本文やタイトルなどすべて無効化
			SetTagDisabled(TAG_USEREXTEND_ID + ":" + "((?!@@>).)*");
		}

		/// <summary>
		/// ループ置換
		/// </summary>
		/// <param name="src">置換対象</param>
		/// <param name="tagName">ループタグ名</param>
		/// <param name="input">Input</param>
		/// <param name="changeToBrTag">改行コードをbrタグに変換するか</param>
		/// <returns>置換後文字列</returns>
		private string ReplaceLoop(string src, string tagName, Hashtable input, bool changeToBrTag = false)
		{
			// HACK: ReplaceLoopメソッド中身がすごい入れ子になってよくわからない
			foreach (string key in input.Keys)
			{
				if ((key == tagName)
					&& (input[key] is IList))
				{
					var tagBegin = "<@@" + tagName + "@@>";
					var tagEnd = "</@@" + tagName + "@@>";

					foreach (Match match in Regex.Matches(src, tagBegin + ".*?" + tagEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase))
					{
						var replaced = new StringBuilder();
						foreach (Hashtable inner in (IList)input[key])
						{
							// ギフトループ処理の場合、配送希望時間帯の有効化・無効化処理を行う
							var matchedValue = ((tagName == TAG_GIFT_SHIPPING_LOOP) && (inner.Contains(Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG)))
								? ReplaceUseShippingTimeInGiftShippingLoop(
									match.Value,
									StringUtility.ToEmpty(inner[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]),
									StringUtility.ToEmpty(inner[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG]))
								: match.Value;

							// For the case, tag name is gift shipping loop and this input has order items loop
							if ((tagName == TAG_GIFT_SHIPPING_LOOP)
								&& inner.ContainsKey(Constants.MAILTAG_ORDER_ITEMS_LOOP))
							{
								// Replace tag values for order items loop
								matchedValue = ReplaceOrderItemsLoop(
									matchedValue,
									(Hashtable[])inner[Constants.MAILTAG_ORDER_ITEMS_LOOP],
									changeToBrTag);
							}
							else
							{
								// For the case, tag name is set product loop or set promotion product loop
								if ((tagName == Constants.MAILTAG_ORDER_SET_PRODUCTS_LOOP)
									|| (tagName == Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCTS_LOOP))
								{
									// Replace tag values for this loop
									matchedValue = ReplaceSetPromotionAndProductSetLoop(inner, matchedValue);
								}
								// Replace product link image tag values
								matchedValue = ReplaceProductImageLink(matchedValue, inner);
							}

							if ((tagName == Constants.MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP)
								&& inner.ContainsKey(Constants.MAILTAG_ORDER_ITEMS_LOOP))
							{
								// Replace tag values for order items loop
								matchedValue = ReplaceOrderItemsLoop(
									matchedValue,
									(Hashtable[])inner[Constants.MAILTAG_ORDER_ITEMS_LOOP],
									changeToBrTag);
							}

							replaced.Append(matchedValue);
							foreach (string keyInner in inner.Keys)
							{
								var value = StringUtility.ToEmpty(inner[keyInner]);
								replaced.Replace(
									"@@ " + keyInner + " @@",
									changeToBrTag ? StringUtility.ChangeToBrTag(value) : value);
							}
						}
						replaced.Replace(tagBegin + "\r\n", "").Replace(tagBegin, "");
						replaced.Replace(tagEnd + "\r\n", "").Replace(tagEnd, "");
						src = src.Replace(match.Value, replaced.ToString());
					}
					break;
				}
			}

			return src;
		}

		/// <summary>
		/// ギフトループ処理内の配送希望時間帯の有効化・無効化処理
		/// </summary>
		/// <param name="replaceTarget">ギフトループ処理内の文字列</param>
		/// <param name="shippingMethod">配送方法</param>
		/// <param name="shippingTimeSetFlg">配送希望時間帯設定可能フラグ</param>
		/// <returns>配送希望時間帯を有効化・無効化後のギフトループ処理内の文字列</returns>
		private string ReplaceUseShippingTimeInGiftShippingLoop(string replaceTarget, string shippingMethod, string shippingTimeSetFlg)
		{
			var tagBgnUseShippingTime = "<@@" + TAG_USE_SHIPPING_TIME + "@@>";
			var tagEndUseShippingTime = "</@@" + TAG_USE_SHIPPING_TIME + "@@>";

			// 宅配便の場合、配送希望時間帯を有効化、メール便の場合は無効化
			var replacedTarget = replaceTarget;
			if ((shippingTimeSetFlg == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID)
				&& (shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS))
			{
				replacedTarget = Regex.Replace(replacedTarget, tagBgnUseShippingTime, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				replacedTarget = Regex.Replace(replacedTarget, tagEndUseShippingTime, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}
			else
			{
				replacedTarget = Regex.Replace(replacedTarget, tagBgnUseShippingTime + ".*?" + tagEndUseShippingTime, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			return replacedTarget;
		}

		/// <summary>
		/// メール文章ValuTextリプレース処理
		/// </summary>
		/// <param name="htInput">ハッシュテーブル</param>
		/// <param name="strTable">テーブル名</param>
		/// <param name="strField">フィールド名</param>
		private void ReplaceMailWithValueText(Hashtable htInput, string strTable, string strField)
		{
			// フィールドが有効且つ、データが設定されているなら
			if (htInput.Contains(strField) && ValueText.Exists(strTable, strField))
			{
				this.TmpSubject = this.TmpSubject.Replace("@@ " + strField + " @@", ValueText.GetValueText(strTable, strField, htInput[strField]));
				this.TmpBody = this.TmpBody.Replace("@@ " + strField + " @@", ValueText.GetValueText(strTable, strField, htInput[strField]));
				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = this.TmpBodyHtml.Replace("@@ " + strField + " @@", ValueText.GetValueText(strTable, strField, htInput[strField]));
				}
				if (this.UseSms) { this.TmpSmsText = this.TmpSmsText.Replace("@@ " + strField + " @@", ValueText.GetValueText(strTable, strField, htInput[strField])); }
				if (this.UseLine && (this.TmpLineText.Length > 0))
				{
					this.TmpLineText = this.TmpLineText.Select(text => text.Replace("@@ " + strField + " @@", ValueText.GetValueText(strTable, strField, htInput[strField]))).ToArray();
				}
			}
		}

		/// <summary>
		/// 定期購入回数（注文・出荷時点）置換
		/// </summary>
		/// <param name="tagName">タグ名</param>
		/// <param name="fieldName">フィールド名（定期購入回数（注文 or 出荷時点））</param>
		/// <param name="inputParam">入力パラメタ</param>
		private void ConvertToFixedPurchaseCount(string tagName, string fieldName, Hashtable inputParam)
		{
			int count;
			if (int.TryParse((string)inputParam[fieldName], out count))
			{
				var tagHead = "<@@" + tagName + ":";
				var tagFoot = "@@>";

				// 件名、本文文字列を結合し、置換対象検索
				var targetString = this.TmpSubject + this.TmpBody + this.TmpSmsText
					+ ((this.TmpLineText.Length > 0)
						? string.Join(null, this.TmpLineText)
						: string.Empty);
				foreach (Match matchFind in GetTagMatches(targetString, tagHead, tagFoot))
				{
					// 属性取得
					var tagInner = GetTagInner(matchFind.Value, tagHead, tagFoot);
					var attributes = tagInner.Split('-');

					var validTag = false;
					var valueIndex = 0;
					foreach (string attribute in attributes)
					{
						valueIndex++;

						// 指定が正しくない？
						if (attribute == "" || (Validator.IsHalfwidthNumber(attribute) == false)) continue;
						var value = int.Parse(attribute);

						// 等価指定?（ハイフン有無で判断する）
						if (tagInner.Contains("-") == false)
						{
							validTag = (count == value);
							break;
						}
						// 範囲指定(From)?
						else if (valueIndex == 1)
						{
							validTag = (count >= value);
							if (validTag == false) break;
						}
						// 範囲指定(To)?
						else if (valueIndex == 2)
						{
							validTag = (count <= value);
							break;
						}
					}
					if (validTag)
					{
						SetTagEnabled(matchFind.Value.Replace("<@@", "").Replace("@@>", ""));
					}
				}
			}
			SetTagDisabled(tagName + ":((?!@@>).)*");
		}

		/// <summary>
		/// 購入回数置換
		/// </summary>
		/// <param name="tagName">タグ名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="inputParam">入力パラメタ</param>
		private void ConvertToOrderCountOrder(string tagName, string fieldName, Hashtable inputParam)
		{
			ConvertToFixedPurchaseCount(tagName, fieldName, inputParam);
		}

		/// <summary>
		/// タグパターンマッチコレクション取得
		/// </summary>
		/// <param name="TargetString">対象文字列</param>
		/// <param name="tagHead">タグ先頭</param>
		/// <param name="tagFoot">タグ末尾</param>
		/// <returns>タグパターンマッチコレクション</returns>
		private MatchCollection GetTagMatches(string TargetString, string tagHead, string tagFoot)
		{
			return Regex.Matches(TargetString.ToString(), tagHead + ".*?" + tagFoot, RegexOptions.Singleline | RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// タグ内部属性文字取得
		/// </summary>
		/// <param name="tagData">対象タグデータ</param>
		/// <param name="tagHead">タグ先頭</param>
		/// <param name="tagFoot">タグ末尾</param>
		/// <returns>タグ内部属性</returns>
		private string GetTagInner(string tagData, string tagHead, string tagFoot)
		{
			// 先頭 or 終端にマッチするパターン
			var pattern = "(" + tagHead + ")|(" + tagFoot + ")";

			// タグ部分を削除（大文字小文字区別しない）
			return Regex.Replace(tagData, pattern, "", RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// タグ有効/無効化
		/// </summary>
		/// <param name="strTagName">タグ名</param>
		/// <param name="blEnabled">有効</param>
		private void SetTagEnabled(string strTagName, bool blEnabled)
		{
			if (blEnabled)
			{
				SetTagEnabled(strTagName);
			}
			else
			{
				SetTagDisabled(strTagName);
			}
		}

		/// <summary>
		/// タグ有効化
		/// </summary>
		/// <param name="strTagName">タグ名</param>
		private void SetTagEnabled(string strTagName)
		{
			var strTagBgn = "<@@" + Regex.Escape(strTagName) + "@@>";
			var strTagEnd = "</@@" + Regex.Escape(strTagName) + "@@>";

			this.TmpFrom = Regex.Replace(this.TmpFrom, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpFrom = Regex.Replace(this.TmpFrom, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpTo = Regex.Replace(this.TmpTo, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpTo = Regex.Replace(this.TmpTo, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpCC = Regex.Replace(this.TmpCC, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpCC = Regex.Replace(this.TmpCC, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpBcc = Regex.Replace(this.TmpBcc, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpBcc = Regex.Replace(this.TmpBcc, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpSubject = Regex.Replace(this.TmpSubject, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpSubject = Regex.Replace(this.TmpSubject, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpBody = Regex.Replace(this.TmpBody, strTagBgn + "\r\n", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpBody = Regex.Replace(this.TmpBody, strTagEnd + "\r\n", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpBody = Regex.Replace(this.TmpBody, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpBody = Regex.Replace(this.TmpBody, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			if (this.CanSendHtml)
			{
				this.TmpBodyHtml = Regex.Replace(this.TmpBodyHtml, strTagBgn + "\r\n", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpBodyHtml = Regex.Replace(this.TmpBodyHtml, strTagEnd + "\r\n", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpBodyHtml = Regex.Replace(this.TmpBodyHtml, strTagBgn, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpBodyHtml = Regex.Replace(this.TmpBodyHtml, strTagEnd, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			if (this.UseSms)
			{
				this.TmpSmsText = Regex.Replace(this.TmpSmsText, strTagBgn + "\r\n", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpSmsText = Regex.Replace(this.TmpSmsText, strTagEnd + "\r\n", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpSmsText = Regex.Replace(this.TmpSmsText, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpSmsText = Regex.Replace(this.TmpSmsText, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			if (this.UseLine && (this.TmpLineText.Length > 0))
			{
				this.TmpLineText = this.TmpLineText.Select(
					text =>
					{
						var tempText = text;
						tempText = Regex.Replace(tempText, strTagBgn + VeriTransConst.NEWLINE_CHARACTER, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
						tempText = Regex.Replace(tempText, strTagEnd + VeriTransConst.NEWLINE_CHARACTER, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
						tempText = Regex.Replace(tempText, strTagBgn, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
						tempText = Regex.Replace(tempText, strTagEnd, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
						return tempText;
					}).ToArray();
			}
		}

		/// <summary>
		/// Set tag enabled
		/// </summary>
		/// <param name="tagName">Tag name</param>
		/// <param name="source">Source</param>
		/// <returns>Source</returns>
		private string SetTagEnabled(string tagName, string source)
		{
			var tagBegin = "<@@" + tagName + "@@>";
			var tagEnd = "</@@" + tagName + "@@>";
			source = Regex.Replace(source, tagBegin + "\r\n", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			source = Regex.Replace(source, tagEnd + "\r\n", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			source = Regex.Replace(source, tagBegin, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			source = Regex.Replace(source, tagEnd, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			return source;
		}

		/// <summary>
		/// タグ無効化
		/// </summary>
		/// <param name="strTagName"></param>
		private void SetTagDisabled(string strTagName)
		{
			string strTagBgn = "<@@" + strTagName + "@@>";
			string strTagEnd = "</@@" + strTagName + "@@>";

			this.TmpFrom = Regex.Replace(this.TmpFrom, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpTo = Regex.Replace(this.TmpTo, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpCC = Regex.Replace(this.TmpCC, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpBcc = Regex.Replace(this.TmpBcc, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpSubject = Regex.Replace(this.TmpSubject, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			this.TmpBody = Regex.Replace(this.TmpBody, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd + "\r\n", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			this.TmpBody = Regex.Replace(this.TmpBody, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			if (this.CanSendHtml)
			{
				this.TmpBodyHtml = Regex.Replace(
					this.TmpBodyHtml,
					string.Format(
						"{0}(?:(?!{1}|{2}).)*{3}\r\n",
						strTagBgn,
						strTagBgn,
						strTagEnd,
						strTagEnd),
					string.Empty,
					RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpBodyHtml = Regex.Replace(
					this.TmpBodyHtml,
					string.Format(
						"{0}(?:(?!{1}|{2}).)*{3}",
						strTagBgn,
						strTagBgn,
						strTagEnd,
						strTagEnd),
					string.Empty,
					RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			if (this.UseSms)
			{
				this.TmpSmsText = Regex.Replace(this.TmpSmsText, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd + "\r\n", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				this.TmpSmsText = Regex.Replace(this.TmpSmsText, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			if (this.UseLine && (this.TmpLineText.Length > 0))
			{
				this.TmpLineText = this.TmpLineText.Select(
					text =>
					{
						var tempText = text;
						tempText = Regex.Replace(tempText, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd + VeriTransConst.NEWLINE_CHARACTER, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
						tempText = Regex.Replace(tempText, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
						return tempText;
					}).ToArray();
			}
		}

		/// <summary>
		/// Set tag disabled
		/// </summary>
		/// <param name="tagName">Tag name</param>
		/// <param name="source">Source</param>
		/// <returns>Source</returns>
		private string SetTagDisabled(string tagName, string source)
		{
			var tagBegin = "<@@" + tagName + "@@>";
			var tagEnd = "</@@" + tagName + "@@>";
			source = Regex.Replace(
				source,
				string.Format(
					"{0}(?:(?!{1}|{2}).)*{3}\r\n",
					tagBegin,
					tagBegin,
					tagEnd,
					tagEnd),
				string.Empty,
				RegexOptions.Singleline | RegexOptions.IgnoreCase);
			source = Regex.Replace(
				source,
				string.Format(
					"{0}(?:(?!{1}|{2}).)*{3}",
					tagBegin,
					tagBegin,
					tagEnd,
					tagEnd),
				string.Empty,
				RegexOptions.Singleline | RegexOptions.IgnoreCase);
			return source;
		}

		/// <summary>
		/// メール送信関数
		/// </summary>
		public new bool SendMail(bool userSendFlg = true)
		{
			// SMSオプションが有効であり、メールテンプレートのSMS利用フラグがONの場合にのみSMS送る
			if (Constants.GLOBAL_SMS_OPTION_ENABLED && this.UseSms)
			{
				SMSHelper.SendSms(this.UserId, this.TmpSmsText);
			}

			// メールの送信実行判定
			// true : 送信実行がされる
			// false : 送信実行がされない
			var mailSendExe = CheckMailSendExe();
			// メール送信実行判定がtrueの場合のみメールを送信
			if (mailSendExe)
			{
				SendMessagingAppContents();

				// メール送信用メソッドを定義
				Func<bool> funcSendMail = () => base.SendMail(this.UserId, this.UserMailAddress);

				// モール連携OPがONかつ、楽天マスクアドレスが含まれている場合、楽天安心メルアドサーバ用送信メソッドを定義する
				if ((MallOptionUtility.CheckRakutenMaskAddress(this.Message)) && (Constants.MALLCOOPERATION_OPTION_ENABLED))
				{
					funcSendMail = () => SendMailRakuten();
				}

				// メール送信
				var result = funcSendMail();

				// ユーザーIDが存在する AND CSオプションが有効の場合はメール送信ログ登録
				if ((string.IsNullOrEmpty(this.UserId) == false)
					&& Constants.CS_OPTION_ENABLED
					&& userSendFlg)
				{
					var service = new MailSendLogService();
					var model = new MailSendLogModel
					{
						UserId = this.UserId,
						DeptId = "",
						MailtextId = "",
						MailtextName = "",
						MaildistId = "",
						MaildistName = "",
						ShopId = this.ShopId,
						MailId = this.MailId,
						MailName = this.TmpName,
						MailFromName = this.Message.From.DisplayName,
						MailFrom = this.Message.From.Address,
						MailTo = string.Join(",", this.Message.To.Select(m => m.Address)),
						MailCc = string.Join(",", this.Message.CC.Select(m => m.Address)),
						MailBcc = string.Join(",", this.Message.Bcc.Select(m => m.Address)),
						MailSubject = this.Subject,
						MailBody = this.Body,
						MailBodyHtml = this.BodyHtml,
						ErrorMessage = (this.MailSendException != null) ? this.MailSendException.ToString() : "",
						DateSendMail = DateTime.Now,
						ReadFlg = Constants.FLG_MAILSENDLOG_READ_FLG_UNREAD
					};
					service.Insert(model);
				}
				// メール送信失敗したらログに情報を書き込む
				if (result == false)
				{
					var errorMessage = "メール送信失敗!\r\n" + Environment.NewLine + CreateCommonMailSendErrorLogMessage();
					w2.Common.Logger.AppLogger.Write("mail", errorMessage.ToString(), this.MailSendException);
					w2.Common.Logger.AppLogger.WriteError(this.MailSendException);
				}
				return result;
			}
			else
			{
				var errorMessage = "自動送信フラグが「送信しない」のため、送信されませんでした。\r\n" + Environment.NewLine + CreateCommonMailSendErrorLogMessage();
				w2.Common.Logger.AppLogger.Write("mail", errorMessage.ToString());

				// メール自動送信フラグが「送信しない」の場合でもtrueを返す
				return true;
			}
		}

		/// <summary>
		/// メッセージアプリ向けコンテンツの送信
		/// </summary>
		private void SendMessagingAppContents()
		{
			if (Line.Constants.LINE_DIRECT_OPTION_ENABLED && this.UseLine)
			{
				var extender = new UserService().GetUserExtend(this.UserId);
				var lineId = extender.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE];
				if (string.IsNullOrEmpty(lineId) == false)
				{
					var messages = this.TmpLineText.Select(
						msg => new LineMessageText
						{
							MessageText = msg
						}).ToArray();
					new LineDirectConnectManager().SendPushMessage(lineId, messages, this.UserId);
				}
			}
		}

		/// <summary>
		/// メール送信失敗結果ログ共通部分作成
		/// </summary>
		/// <returns></returns>
		private StringBuilder CreateCommonMailSendErrorLogMessage()
		{
			var errorMessage = new StringBuilder();
			errorMessage.Append("メール送信失敗!\r\n");
			errorMessage.Append("From:").Append(this.TmpFrom).Append("\r\n");
			errorMessage.Append("To:").Append(this.TmpTo).Append("\r\n");
			errorMessage.Append("Cc:").Append(this.TmpCC).Append("\r\n");
			errorMessage.Append("Bcc:").Append(this.TmpBcc).Append("\r\n");
			errorMessage.Append("Subject:").Append(this.TmpSubject).Append("\r\n");
			errorMessage.Append("Body:\r\n").Append(this.TmpBody).Append("\r\n");
			errorMessage.Append("BodyHtml:\r\n").Append(this.TmpBodyHtml).Append("\r\n");
			errorMessage.Append("------------------------------------------------------------------------------------------------------\r\n");
			return errorMessage;
		}

		/// <summary>
		/// メール送信関数(楽天あんしんメルアドサーバ用)
		/// </summary>
		private bool SendMailRakuten()
		{
			//------------------------------------------------------
			// 楽天あんしんメルアドサーバには、.NET標準のクラスを用いてメール送信する。
			// (PKG標準のクラスが、SMTP-AUTH,STARTTLSに対応していないため)
			//------------------------------------------------------									

			//------------------------------------------------------
			// w2.Common→.Netにメールの情報を移動
			//------------------------------------------------------
			System.Net.Mail.MailMessage mmMessage = new System.Net.Mail.MailMessage();

			// To
			foreach (MailAddress maAddress in this.Message.To)
			{
				mmMessage.To.Add(new System.Net.Mail.MailAddress(maAddress.Address));
			}

			// CC
			foreach (MailAddress maAddress in this.Message.CC)
			{
				mmMessage.CC.Add(new System.Net.Mail.MailAddress(maAddress.Address));
			}

			// BCC
			foreach (MailAddress maAddress in this.Message.Bcc)
			{
				mmMessage.Bcc.Add(new System.Net.Mail.MailAddress(maAddress.Address));
			}

			// From
			mmMessage.From = new System.Net.Mail.MailAddress(this.Message.From.Address);

			// Subject,Body
			mmMessage.Subject = this.Subject;
			mmMessage.Body = this.CanSendHtml ? this.BodyHtml : this.Body;

			//------------------------------------------------------
			// メール送信
			//------------------------------------------------------
			bool blSend = true;
			try
			{
				System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();

				// SMTPサーバ設定を取得,設定(注文ID毎にSMTPサーバ設定が異なる可能性がある)
				MallSmtpServerSettingRakuten msssr = new MallSmtpServerSettingRakuten(MallOptionUtility.GetSmtpServerSetting(this.MallId));

				// ポート設定について、変換できない場合は0として扱う
				int iSmtpServerPort;
				int.TryParse(msssr.SmtpServerPort, out iSmtpServerPort);

				sc.Host = msssr.SmtpServerName;
				sc.Port = iSmtpServerPort;

				// SMTP-ID,SMTP-PASSが入力されている場合、SMTP認証を行う
				// (どちらが未入力の場合、行わない。個別にIP申請することでSMTP認証を回避することもできるため)
				if ((msssr.SmtpAuthId != "") && (msssr.SmtpAuthPassword != ""))
				{
					sc.Credentials = new System.Net.NetworkCredential(msssr.SmtpAuthId, msssr.SmtpAuthPassword);
				}

				// 楽天メールサーバにはSSL接続を行う
				sc.EnableSsl = true;

				// 店舗連絡先メールアドレスが存在している場合、Fromを書き換える
				// (楽天安心メルアドサービスに送信する場合、店舗連絡先メールアドレス以外からの送信は拒否されるため）
				if (msssr.RakutenStoreMailAddress != "")
				{
					mmMessage.From = new System.Net.Mail.MailAddress(msssr.RakutenStoreMailAddress);
				}

				sc.Send(mmMessage);
			}
			catch (Exception ex)
			{
				this.MailSendException = ex;
				blSend = false;
			}

			mmMessage.Dispose();
			return blSend;
		}

		/// <summary>
		/// メール送信実行チェック
		/// </summary>
		/// <returns>送信実行の判定</returns>
		private bool CheckMailSendExe()
		{
			// メールIDが存在しない、もしくは自動送信フラグがON、もしくは自動送信フラグがOFFでメール送信方法がマニュアルの場合true
			var mailSendExe = (string.IsNullOrEmpty(this.MailId)
				|| (this.AutoSendFlgCheck
				|| ((this.AutoSendFlgCheck == false)
					&& (this.MailSendMethod == Constants.MailSendMethod.Manual))));
			return mailSendExe;
		}

		/// <summary>
		/// Replace product image link
		/// </summary>
		/// <param name="replaceTarget">Replace target</param>
		/// <param name="product">Product</param>
		/// <returns>Replaced target</returns>
		private string ReplaceProductImageLink(string replaceTarget, Hashtable product)
		{
			var replaceTags = new string[] {
				Constants.MAILTAG_ORDER_ITEM_PRODUCT_IMAGE,
				Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_IMAGE,
				Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_IMAGE,
				Constants.MAILTAG_ORDER_ITEM_PRODUCT_VARIATION_IMAGE,
				Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_VARIATION_IMAGE,
				Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_VARIATION_IMAGE};
			foreach (var tag in replaceTags)
			{
				if (product.ContainsKey(tag))
				{
					var tagBegin = string.Format("@@ {0}:", tag);
					var tagEnd = " @@";
					var isVariation = tag.Contains("variation");
					foreach (Match match in GetTagMatches(replaceTarget, tagBegin, tagEnd))
					{
						var size = match.Value.Replace(tagBegin, string.Empty).Replace(tagEnd, string.Empty);
						string fileNameFoot = null;
						switch (size)
						{
							case "S":
								fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_S;
								break;

							case "M":
								fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_M;
								break;

							case "L":
								fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_L;
								break;

							case "LL":
								fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_LL;
								break;

							default:
								fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_M;
								break;
						}

						var imageUrl = GetProductImageUrl(
							StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SHOP_ID]),
							StringUtility.ToEmpty(product[tag]),
							fileNameFoot);
						var imageLink = string.Format(
							"<p><img src=\"{0}\" alt=\"{1}\" border=\"0\" /></p>",
							imageUrl,
							HtmlSanitizer.HtmlEncode(product[Constants.MAILTAG_ORDER_ITEM_PRODUCT_NAME]));
						replaceTarget = replaceTarget.Replace(match.Value, imageLink);
					}
				}
			}
			return replaceTarget;
		}

		/// <summary>
		/// Replace order items loop
		/// </summary>
		/// <param name="replaceTarget">Replace target</param>
		/// <param name="orderItems">Order items</param>
		/// <param name="changeToBrTag">改行コードをbrタグに変換するか</param>
		/// <returns>Replaced target</returns>
		private string ReplaceOrderItemsLoop(string replaceTarget, Hashtable[] orderItems, bool changeToBrTag = false)
		{
			var tagBegin = "<@@" + Constants.MAILTAG_ORDER_ITEMS_LOOP + "@@>";
			var tagEnd = "</@@" + Constants.MAILTAG_ORDER_ITEMS_LOOP + "@@>";
			foreach (Match match in GetTagMatches(replaceTarget, tagBegin, tagEnd))
			{
				var replaceBuilder = new StringBuilder();
				foreach (var data in orderItems)
				{
					var source = match.Value;
					var type = StringUtility.ToEmpty(data[Constants.MAILTAG_ORDER_ITEM_TYPE]);

					// Replace order set products
					if (type == Constants.MAILTAG_ORDER_SET_PRODUCTS_LOOP)
					{
						source = SetTagDisabled(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT, source);
						source = SetTagDisabled(Constants.MAILTAG_ORDER_NORMAL_PRODUCT, source);
						source = SetTagEnabled(Constants.MAILTAG_ORDER_SET_PRODUCT, source);
						source = ReplaceLoop(
							source,
							Constants.MAILTAG_ORDER_SET_PRODUCTS_LOOP,
							new Hashtable
							{
								{ Constants.MAILTAG_ORDER_SET_PRODUCTS_LOOP, (IList)data[Constants.MAILTAG_ORDER_ITEM_PRODUCT_ITEMS] },
							},
							changeToBrTag);
					}

					// Replace order set promotion product
					if (type == Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCTS_LOOP)
					{
						source = SetTagDisabled(Constants.MAILTAG_ORDER_SET_PRODUCT, source);
						source = SetTagDisabled(Constants.MAILTAG_ORDER_NORMAL_PRODUCT, source);
						source = SetTagEnabled(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT, source);
						source = ReplaceLoop(
							source,
							Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCTS_LOOP,
							new Hashtable
							{
								{ Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCTS_LOOP, (IList)data[Constants.MAILTAG_ORDER_ITEM_PRODUCT_ITEMS] },
							},
							changeToBrTag);
					}

					// Replace normal product
					if (type == Constants.MAILTAG_ORDER_NORMAL_PRODUCT)
					{
						source = SetTagDisabled(Constants.MAILTAG_ORDER_SET_PRODUCT, source);
						source = SetTagDisabled(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT, source);
						source = SetTagEnabled(Constants.MAILTAG_ORDER_NORMAL_PRODUCT, source);
					}
					source = ReplaceTag(source, data, changeToBrTag);
					source = ReplaceProductImageLink(source, data);
					source = source.Replace(tagBegin + "\r\n", string.Empty)
						.Replace(tagBegin, string.Empty)
						.Replace(tagEnd + "\r\n", string.Empty)
						.Replace(tagEnd, string.Empty);
					replaceBuilder.Append(source);
				}
				replaceTarget = replaceTarget.Replace(match.Value, replaceBuilder.ToString());
			}
			return replaceTarget;
		}

		/// <summary>
		/// Replace set promotion and product set loop
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="replaceTarget">Replace target</param>
		/// <param name="changeToBrTag">改行コードをbrタグに変換するか</param>
		/// <returns>Replaced target</returns>
		private string ReplaceSetPromotionAndProductSetLoop(Hashtable product, string replaceTarget, bool changeToBrTag = false)
		{
			if (product.ContainsKey(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP))
			{
				replaceTarget = ReplaceLoop(
					replaceTarget,
					Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP,
					new Hashtable
					{
						{
							Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP,
							(IList)product[Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP]
						},
					},
					changeToBrTag);
			}

			// Order set product variation
			var isOrderSetVariation = (product.ContainsKey(Constants.MAILTAG_IS_ORDER_SET_PRODUCT_VARIATION)
				&& (bool)product[Constants.MAILTAG_IS_ORDER_SET_PRODUCT_VARIATION]);

			replaceTarget = SetTagEnabled(
				isOrderSetVariation
					? Constants.MAILTAG_IS_ORDER_SET_PRODUCT_VARIATION
					: Constants.MAILTAG_IS_NOT_ORDER_SET_PRODUCT_VARIATION,
				replaceTarget);

			replaceTarget = SetTagDisabled(
				isOrderSetVariation
					? Constants.MAILTAG_IS_NOT_ORDER_SET_PRODUCT_VARIATION
					: Constants.MAILTAG_IS_ORDER_SET_PRODUCT_VARIATION,
				replaceTarget);

			// Order set promotion product variation
			var isOrderSetPromotionVariation = (product.ContainsKey(Constants.MAILTAG_IS_ORDER_SET_PROMOTION_PRODUCT_VARIATION)
				&& (bool)product[Constants.MAILTAG_IS_ORDER_SET_PROMOTION_PRODUCT_VARIATION]);

			replaceTarget = SetTagEnabled(
				isOrderSetPromotionVariation
					? Constants.MAILTAG_IS_ORDER_SET_PROMOTION_PRODUCT_VARIATION
					: Constants.MAILTAG_IS_NOT_ORDER_SET_PROMOTION_PRODUCT_VARIATION,
				replaceTarget);

			replaceTarget = SetTagDisabled(
				isOrderSetPromotionVariation
					? Constants.MAILTAG_IS_NOT_ORDER_SET_PROMOTION_PRODUCT_VARIATION
					: Constants.MAILTAG_IS_ORDER_SET_PROMOTION_PRODUCT_VARIATION,
				replaceTarget);
			return replaceTarget;
		}

		/// <summary>
		/// Replace tag
		/// </summary>
		/// <param name="replaceTarget">Replace target</param>
		/// <param name="data">Data</param>
		/// <param name="changeToBrTag">改行コードをbrタグに変換するか</param>
		/// <returns>Replaced target</returns>
		private string ReplaceTag(string replaceTarget, Hashtable data, bool changeToBrTag = false)
		{
			replaceTarget = data.ContainsKey(Constants.MAILTAG_ORDER_PRODUCT_SERIAL_KEYS_LOOP)
				? ReplaceLoop(replaceTarget, Constants.MAILTAG_ORDER_PRODUCT_SERIAL_KEYS_LOOP, data, changeToBrTag)
				: SetTagDisabled(Constants.MAILTAG_ORDER_PRODUCT_SERIAL_KEYS_LOOP, replaceTarget);
			replaceTarget = data.ContainsKey(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP)
				? ReplaceLoop(replaceTarget, Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP, data, changeToBrTag)
				: SetTagDisabled(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP, replaceTarget);

			foreach (string key in data.Keys)
			{
				if (key == Constants.MAILTAG_IS_ORDER_PRODUCT_VARIATION)
				{
					if ((bool)data[key])
					{
						replaceTarget = SetTagEnabled(Constants.MAILTAG_IS_ORDER_PRODUCT_VARIATION, replaceTarget);
						replaceTarget = SetTagDisabled(Constants.MAILTAG_IS_NOT_ORDER_PRODUCT_VARIATION, replaceTarget);
					}
					else
					{
						replaceTarget = SetTagDisabled(Constants.MAILTAG_IS_ORDER_PRODUCT_VARIATION, replaceTarget);
						replaceTarget = SetTagEnabled(Constants.MAILTAG_IS_NOT_ORDER_PRODUCT_VARIATION, replaceTarget);
					}
				}
				else
				{
					var value = StringUtility.ToEmpty(data[key]);
					replaceTarget = replaceTarget.Replace(
						"@@ " + key + " @@",
						changeToBrTag ? StringUtility.ChangeToBrTag(value) : value);
				}
			}
			return replaceTarget;
		}

		/// <summary>
		/// Get product image url
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="imageHead">Image head</param>
		/// <param name="imageFooter">Image footer</param>
		/// <returns>Image url</returns>
		private string GetProductImageUrl(string shopId, string imageHead, string imageFooter)
		{
			var baseUrl = string.Format(
				"{0}{1}{2}{3}",
				Constants.PROTOCOL_HTTPS,
				(string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN) == false)
					? Constants.WEBHOOK_DOMAIN
					: Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				Constants.PATH_PRODUCTIMAGES);
			var imageUrl = (string.IsNullOrEmpty(imageHead) == false)
				? string.Format(
					"{0}{1}/{2}{3}",
					baseUrl,
					shopId,
					imageHead,
					imageFooter)
				: string.Format(
					"{0}{1}{2}",
					baseUrl,
					Constants.PRODUCTIMAGE_NOIMAGE_HEADER,
					imageFooter);
			return new UrlCreator(imageUrl).CreateUrl();
		}

		/// <summary>
		/// Replace store pickup tags
		/// </summary>
		/// <param name="inputParam">Input param</param>
		private void ReplaceStorePickupTags(Hashtable inputParam)
		{
			// Replace store pickup order tag
			void ReplaceStorePickupOrderTag(Hashtable inputParamOrderTag)
			{
				if (inputParamOrderTag.ContainsKey(Constants.MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER))
				{
					SetTagEnabled(Constants.MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER);
				}
				else
				{
					SetTagDisabled(Constants.MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER);
				}
			}

			// Replace store pickup order loop tag
			void ReplaceStorePickupOrderLoopTag(Hashtable inputParamOrderLoopTag)
			{
				if (inputParamOrderLoopTag.ContainsKey(Constants.MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP) == false) return;

				this.TmpBody = ReplaceLoop(
					this.TmpBody,
					Constants.MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP,
					inputParamOrderLoopTag);

				if (this.CanSendHtml)
				{
					this.TmpBodyHtml = ReplaceLoop(
						this.TmpBodyHtml,
						Constants.MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP,
						inputParamOrderLoopTag,
						changeToBrTag: true);
				}

				if (this.UseSms)
				{
					this.TmpSmsText = ReplaceLoop(
						this.TmpSmsText,
						Constants.MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP,
						inputParamOrderLoopTag);
				}

				if (this.UseLine && this.TmpLineText.Length > 0)
				{
					this.TmpLineText = this.TmpLineText.Select(
						text => ReplaceLoop(
							text,
							Constants.MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP,
							inputParamOrderLoopTag))
						.ToArray();
				}
			}

			if (Constants.STORE_PICKUP_OPTION_ENABLED == false) return;

			ReplaceStorePickupOrderTag(inputParam);
			ReplaceStorePickupOrderLoopTag(inputParam);
		}

		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>メールテンプレートID</summary>
		public string MailId { get; set; }
		/// <summary>ユーザーID</summary>
		public string UserId { get; set; }
		/// <summary>ユーザーID</summary>
		public new string UserMailAddress { get; set; }
		/// <summary>メールテンプレート名テンポラリ</summary>
		public string TmpName { get; set; }
		/// <summary>送信元テンポラリ</summary>
		public string TmpFrom { get; set; }
		/// <summary>送信先TOテンポラリ</summary>
		public string TmpTo { get; set; }
		/// <summary>送信先CCテンポラリ</summary>
		public string TmpCC { get; set; }
		/// <summary>送信先Bccテンポラリ</summary>
		public string TmpBcc { get; set; }
		/// <summary>送信件名テンポラリ</summary>
		public string TmpSubject { get; set; }
		/// <summary>送信本文テンポラリ</summary>
		public string TmpBody { get; set; }
		/// <summary>モールID</summary>
		public string MallId { get; set; }
		/// <summary> 自動送信フラグ判定 </summary>
		public bool AutoSendFlgCheck { get; private set; }
		/// <summary>メール送信例外</summary>
		public new Exception MailSendException
		{
			get { return (m_MailSendException == null) ? base.MailSendException : m_MailSendException; }
			private set { m_MailSendException = value; }
		}
		/// <summary>
		/// 送信方法
		/// Auto : 自動送信（オペレーターが送信選択できない）
		/// Manual : マニュアル送信（オペレーターが送信を選択できる）
		/// </summary>
		public Constants.MailSendMethod MailSendMethod { get; private set; }
		private Exception m_MailSendException;
		/// <summary>メール配信フラグがメール拒否か</summary>
		public bool IsRefuseMail { get; set; }
		/// <summary> SMS利用か否か </summary>
		public bool UseSms { get; set; }
		/// <summary> SMSテキストテンポラリ </summary>
		public string TmpSmsText { get; set; }
		/// <summary> LINE利用か否か </summary>
		public bool UseLine { get; set; }
		/// <summary> LINEテキストテンポラリ </summary>
		public string[] TmpLineText { get; set; }
		/// <summary>メール本文HTML</summary>
		public string TmpBodyHtml { get; set; }
		/// <summary>Can Send Html</summary>
		public bool CanSendHtml { get; set; }
	}
}
