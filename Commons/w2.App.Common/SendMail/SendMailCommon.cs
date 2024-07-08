/*
=========================================================================================================
  Module      : メール送信共通処理(SendMailCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Global.Region;
using w2.App.Common.Mail;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.User;
using w2.Domain.UserShipping;

namespace w2.App.Common.SendMail
{
	/// <summary>
	/// メール送信共通処理
	/// </summary>
	public class SendMailCommon
	{
		/// <summary> 購入履歴変更箇所 </summary>
		public enum PurchaseHistoryModify
		{
			/// <summary>支払方法</summary>
			PaymentMethod,
			/// <summary>お届け日</summary>
			ShippingDate,
			/// <summary>お届け先</summary>
			Shipping,
			/// <summary>利用ポイント</summary>
			OrderPoint,
			/// <summary>商品</summary>
			Product
		}
		/// <summary> 定期購入情報変更箇所 </summary>
		public enum FixedPurchaseModify
		{
			/// <summary>支払方法</summary>
			PaymentMethod,
			/// <summary>お届け日</summary>
			ShippingDate,
			/// <summary>お届け先</summary>
			Shipping,
			/// <summary>利用ポイント</summary>
			OrderPoint,
			/// <summary>配送スキップ</summary>
			SkipNextShipping,
			/// <summary>キャンセル</summary>
			OrderCancell,
			/// <summary>キャンセル</summary>
			Suspend,
			/// <summary>次回配送日</summary>
			NextShippingDate,
			/// <summary>商品</summary>
			Product,
		}
		/// <summary> 管理者向けの文字列定義 </summary>
		public const string STRING_FOR_OPERATOR = "_for_operator";

		#region 再与信エラーメール送信
		/// <summary>
		/// 再与信エラーメール送信
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="message">メッセージ</param>
		public static void SendReauthErrorMail(string orderId, string message)
		{
			var mailData = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
				{"message", message}
			};
			SendMailToOperator(Constants.CONST_MAIL_ID_REAUTH_ERROR_ADMIN, mailData);
		}
		#endregion

		#region マイページ変更案内メールの送信
		/// <summary>
		/// クレジットカード情報変更案内メール送信
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		public static void SendModifyCreditCardMail(string userId)
		{
			var user = new UserService().Get(userId);
			var mailDate = new Hashtable
			{
				{Constants.FIELD_USER_NAME1, user.Name1},
				{Constants.FIELD_USER_NAME2, user.Name2},
				{Constants.FIELD_USER_MAIL_ADDR, user.MailAddr},
				{Constants.FIELD_USER_MAIL_ADDR2, user.MailAddr2},
			};
			var isPc = (string.IsNullOrEmpty(user.MailAddr) == false);

			// ユーザー向け
			SendMailToUser(
				Constants.CONST_MAIL_ID_CREDITCARD_REGIST_FOR_USER,
				userId,
				mailDate,
				isPc,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
			// 管理者向け
			SendMailToOperator(Constants.CONST_MAIL_ID_CREDITCARD_REGIST_FOR_OPERATOR, mailDate);
		}

		/// <summary>
		/// アドレス帳変更案内メール送信
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="address">変更後のアドレス帳情報</param>
		public static void SendModifyUserShippingMail(string userId, UserShippingModel address)
		{
			var user = new UserService().Get(userId);
			var mailDate = address.DataSource;
			mailDate.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1, address.ShippingZip1);
			mailDate.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2, address.ShippingZip2);
			mailDate.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1, address.ShippingTel1_1);
			mailDate.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2, address.ShippingTel1_2);
			mailDate.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3, address.ShippingTel1_3);
			mailDate.Add(Constants.FIELD_USER_NAME1, user.Name1);
			mailDate.Add(Constants.FIELD_USER_NAME2, user.Name2);
			mailDate.Add(Constants.FIELD_USER_MAIL_ADDR, user.MailAddr);
			mailDate.Add(Constants.FIELD_USER_MAIL_ADDR2, user.MailAddr2);
			var isPc = (string.IsNullOrEmpty(user.MailAddr) == false);

			// ユーザー向け
			SendMailToUser(
				Constants.CONST_MAIL_ID_CHANGE_ACCOUNT_ADDRESS_FOR_USER,
				userId,
				mailDate,
				isPc,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
			// 管理者向け
			SendMailToOperator(Constants.CONST_MAIL_ID_CHANGE_ACCOUNT_ADDRESS_FOR_OPERATOR, mailDate);
		}

		/// <summary>
		/// お客様情報変更案内メール送信
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		public static void SendModifyUserMail(string userId)
		{
			var user = new UserService().Get(userId);
			var mailData = (Hashtable)user.DataSource.Clone();
			// 生年月日の時間削除
			mailData[Constants.FIELD_USER_BIRTH] = DateTimeUtility.ToString(
				user.Birth,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
				(string)mailData[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]);
			// 生年月日についての管理者向けのタグ用追加
			mailData[Constants.FIELD_USER_BIRTH + STRING_FOR_OPERATOR] = DateTimeUtility.ToStringForManager(
				user.Birth,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);

			// パスワード複合
			mailData[Constants.FIELD_USER_PASSWORD] = user.PasswordDecrypted;

			// ユーザー拡張項目追加
			var UserExtendSettings = new UserService().GetUserExtendSettingList();
			mailData[Constants.TABLE_USEREXTENDSETTING] = UserExtendSettings;
			mailData[Constants.TABLE_USEREXTEND] = user.UserExtend;

			var isPc = (string.IsNullOrEmpty(user.MailAddr) == false);

			// ユーザー向け
			SendMailToUser(Constants.CONST_MAIL_ID_CHANGE_ACCOUNT_FOR_USER, user.UserId, mailData, isPc, user.DispLanguageCode, user.DispLanguageLocaleId);
			// 管理者向け
			SendMailToOperator(Constants.CONST_MAIL_ID_CHANGE_ACCOUNT_FOR_OPERATOR, mailData);
		}
		#endregion

		#region 購入履歴情報変更メールの送信
		/// <summary>
		///  購入履歴変更メール送信
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="modify">変更箇所</param>
		public static void SendModifyPurchaseHistoryMail(string orderId, PurchaseHistoryModify modify)
		{
			var mailDate = CreateOrderHistoryMailDate(orderId);
			var isPc = (string.IsNullOrEmpty((string)mailDate[Constants.FIELD_USER_MAIL_ADDR]) == false);

			// メールテンプレートID振り分け
			var mailIdForUser = "";
			var mailIdForOperator = "";
			switch (modify)
			{
				case PurchaseHistoryModify.PaymentMethod:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FOR_OPERATOR;
					break;

				case PurchaseHistoryModify.ShippingDate:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FOR_OPERATOR;
					break;

				case PurchaseHistoryModify.Shipping:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FOR_OPERATOR;
					break;

				case PurchaseHistoryModify.OrderPoint:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_POINTS_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_POINTS_FOR_OPERATOR;
					break;

				case PurchaseHistoryModify.Product:
					mailIdForUser = Constants.CONST_MAIL_ID_MYPAGE_ORDER_MODIFY_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_MYPAGE_ORDER_MODIFY_FOR_OPERATOR;
					break;
			}

			// ユーザー向け
			SendMailToUser(
				mailIdForUser,
				(string)mailDate[Constants.FIELD_USER_USER_ID],
				mailDate,
				isPc,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
			// 管理者向け
			SendMailToOperator(mailIdForOperator, mailDate);
		}
		#endregion

		#region 定期購入情報変更案内メールの送信
		/// <summary>
		///  定期購入情報変更メール送信
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="modify">変更箇所</param>
		public static void SendModifyFixedPurchaseMail(string fixedPurchaseId, FixedPurchaseModify modify)
		{
			var mailDate = CreateFixedPurchaseMailDate(fixedPurchaseId);
			var isPc = (string.IsNullOrEmpty((string)mailDate[Constants.FIELD_USER_MAIL_ADDR]) == false);

			// メールテンプレートID振り分け
			var mailIdForUser = "";
			var mailIdForOperator = "";
			switch (modify)
			{
				case FixedPurchaseModify.PaymentMethod:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FIXEDPURCHASE_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FIXEDPURCHASE_FOR_OPERATOR;
					break;

				case FixedPurchaseModify.ShippingDate:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FIXEDPURCHASE_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FIXEDPURCHASE_FOR_OPERATOR;
					break;

				case FixedPurchaseModify.Shipping:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FIXEDPURCHASE_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FIXEDPURCHASE_FOR_OPERATOR;
					break;

				case FixedPurchaseModify.OrderPoint:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_POINTS_FIXEDPURCHASE_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_POINTS_FIXEDPURCHASE_FOR_OPERATOR;
					break;

				case FixedPurchaseModify.SkipNextShipping:
					mailIdForUser = Constants.CONST_MAIL_ID_SKIP_FIXEDPURCHASE;
					break;

				case FixedPurchaseModify.OrderCancell:
					mailIdForUser = Constants.CONST_MAIL_ID_CANCEL_FIXEDPURCHASE;
					break;

				case FixedPurchaseModify.Suspend:
					mailIdForUser = Constants.CONST_MAIL_ID_SUSPEND_FIXEDPURCHASE;
					break;
				case FixedPurchaseModify.NextShippingDate:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_SHIPPING_DATE_FIXEDPURCHASE_FOR_USER;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_SHIPPING_DATE_FIXEDPURCHASE_FOR_OPERATOR;
					break;

				case FixedPurchaseModify.Product:
					mailIdForUser = Constants.CONST_MAIL_ID_CHANGE_FIXEDPURCHASE_PRODUCT;
					mailIdForOperator = Constants.CONST_MAIL_ID_CHANGE_FIXEDPURCHASE_PRODUCT_FOR_OPERATOR;
					break;
			}

			// ユーザー向け
			SendMailToUser(
				mailIdForUser,
				(string)mailDate[Constants.FIELD_USER_USER_ID],
				mailDate,
				isPc,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
			// 管理者向け
			if (string.IsNullOrEmpty(mailIdForOperator) == false)
			{
				SendMailToOperator(mailIdForOperator, mailDate);
			}
		}
		#endregion

		/// <summary>
		/// 購入履歴 メールデータ取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>メールデータ</returns>
		public static Hashtable CreateOrderHistoryMailDate(string orderId)
		{
			var mailDate = new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(orderId);
			return mailDate;
		}

		/// <summary>
		/// 定期購入情報 メールデータ取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>メールデータ</returns>
		public static Hashtable CreateFixedPurchaseMailDate(string fixedPurchaseId)
		{
			var mailDate = new MailTemplateDataCreaterForFixedPurchase(true).GetFixedPurchaseMailDatas(fixedPurchaseId);
			mailDate[Constants.FIELD_USER_MAIL_ADDR] = mailDate[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
			mailDate[Constants.FIELD_USER_MAIL_ADDR2] = mailDate[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];
			return mailDate;
		}

		/// <summary>
		/// メール送信(ユーザー向け)
		/// </summary>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailData">メール用ハッシュテーブル</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="isPc">送信対象がPCか</param>
		/// <returns>メール配信フラグによって送信を拒否されたか</returns>
		public static bool SendMailToUser(string mailId, string userId, Hashtable mailData, bool isPc, string languageCode = null, string languageLocaleId = null)
		{
			MailSendUtility msMailSend = null;
			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
			{
				if ((string)mailData[Constants.FIELD_USER_MAIL_ADDR] != string.Empty)
				{
					msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, true, Constants.MailSendMethod.Auto, languageCode, languageLocaleId, (string)mailData[Constants.FIELD_USER_MAIL_ADDR]);
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR].ToString());
				}

				if ((string)mailData[Constants.FIELD_USER_MAIL_ADDR2] != string.Empty)
				{
					msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, false, Constants.MailSendMethod.Auto, userMailAddress: (string)mailData[Constants.FIELD_USER_MAIL_ADDR]);
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR2].ToString());
				}
			}
			else
			{
				msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, languageCode, languageLocaleId, userMailAddress: (string)mailData[Constants.FIELD_USER_MAIL_ADDR]);
				// PC宛の場合とモバイル宛の場合で送信先メールアドレスを分岐
				if (isPc)
				{
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR].ToString());
				}
				else
				{
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR2].ToString());
				}
			}
			// メール送信
			msMailSend.SendMail();
			return msMailSend.IsRefuseMail;
		}

		/// <summary>
		/// メール送信(管理者向け)
		/// </summary>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="mailData">メールデータ</param>
		public static void SendMailToOperator(string mailId, Hashtable mailData)
		{
			using (MailSendUtility msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, "", mailData, true, Constants.MailSendMethod.Auto))
			{
				// Toが設定されている場合にのみメール送信
				if (msMailSend.TmpTo != string.Empty)
				{
					msMailSend.SendMail();
				}
			}
		}

		/// <summary>
		/// Send reauth success mail
		/// </summary>
		/// <param name="execMode">Exec mode</param>
		/// <param name="date">Date</param>
		/// <param name="extend">Extend</param>
		/// <param name="successCount">Success count</param>
		/// <param name="failureCount">Failure count</param>
		/// <param name="isExecutionSuccess">成功か失敗(デフォルト：成功)</param>
		public static void SendReauthSuccessMail(
			string execMode,
			string date,
			string extend,
			int successCount,
			int failureCount,
			bool isExecutionSuccess = true)
		{
			var mailData = new Hashtable
			{
				{ Constants.CONST_REAUTH_TOTAL_COUNT, (successCount + failureCount) },
				{ Constants.CONST_REAUTH_MODE, execMode },
				{ Constants.CONST_REAUTH_DATE, date },
				{ Constants.CONST_REAUTH_EXTEND, extend },
				{ Constants.CONST_REAUTH_SUCESS_COUNT, successCount },
				{ Constants.CONST_REAUTH_FAILURE_COUNT, failureCount },
				{ Constants.CONST_REAUTH_EXECUTION_RESULT, isExecutionSuccess ? "正常" : "異常" },
				{ Constants.CONST_REAUTH_EXECUTION_FAILURE_MESSAGE, isExecutionSuccess ? string.Empty : "理由：予期せぬエラーが発生しました。" },
			};

			SendMailToOperator(Constants.CONST_MAIL_ID_REAUTH_FOR_OPERATOR, mailData);
		}

		/// <summary>
		///  領収書発行メール送信
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="tmpFilePath">領収書PDF</param>
		public static void SendReceipFiletMail(string orderId, string tmpFilePath)
		{
			var mailDate = CreateOrderHistoryMailDate(orderId);

			// メールテンプレートID振り分け
			var mailIdForOperator = Constants.CONST_MAIL_ID_RECEIPT;

			// 管理者向け
			using (MailSendUtility msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailIdForOperator, "", mailDate, true, Constants.MailSendMethod.Auto))
			{
				//領収書PDF添付
				msMailSend.Message.AttachmentFilePath.Add(tmpFilePath);

				// Toが設定されている場合にのみメール送信
				if (msMailSend.TmpTo != string.Empty)
				{
					msMailSend.SendMail();
				}
			}
		}

		/// <summary>
		/// Send 2-step authentication code
		/// </summary>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="mailData">メール用ハッシュテーブル</param>
		public void Send2StepAuthenticationCode(string mailId, Hashtable mailData)
		{
			using (var mailSend = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				mailId,
				string.Empty,
				mailData,
				true,
				Constants.MailSendMethod.Auto))
			{
				mailSend.AddTo(StringUtility.ToEmpty(mailData[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR]));
				mailSend.SendMail();
			}
		}
	}
}
