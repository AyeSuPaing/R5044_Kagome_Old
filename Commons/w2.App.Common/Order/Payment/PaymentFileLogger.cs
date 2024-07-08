/*
=========================================================================================================
  Module      : 決済専用ファイルロガー(PaymentFileLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment
{
	/// <summary>
	/// 決済専用ファイルロガー
	/// </summary>
	public class PaymentFileLogger
	{
		/// <summary>
		/// 決済代行会社名、個別の決済名(AmazonPay,PayPalなど)の列挙体。決済種別名は含まない(クレジットカードやコンビニ後払いなど)
		/// </summary>
		public enum PaymentType
		{
			[EnumTextName("GMO")]
			Gmo,
			[EnumTextName("ヤマトKWC")]
			Yamatokwc,
			[EnumTextName("ヤマトKWCコンビニ決済受信")]
			YamatoKwcCvsPaymentRecv,
			[EnumTextName("ヤマトka")]
			Yamatoka,
			[EnumTextName("クロネコ後払い")]
			KuronekoPostPay,
			[EnumTextName("SBPS")]
			Sbps,
			[EnumTextName("ZEUS")]
			Zeus,
			[EnumTextName("ゼウス入金お任せサービス")]
			ZeusDepositService,
			[EnumTextName("DSK")]
			Dsk,
			[EnumTextName("AmazonPay")]
			Amazon,
			[EnumTextName("AmazonPay(CV2)")]
			AmazonCv2,
			[EnumTextName("TriLink後払い")]
			TriLink,
			[EnumTextName("Atodene")]
			Atodene,
			[EnumTextName("ZCOM")]
			Zcom,
			[EnumTextName("EScott")]
			EScott,
			[EnumTextName("PayPal")]
			PayPal,
			[EnumTextName("楽天ペイ")]
			RakutenId,
			[EnumTextName("Paidy")]
			Paidy,
			[EnumTextName("atone翌月払い")]
			Atone,
			[EnumTextName("aftee翌月払い")]
			Aftee,
			[EnumTextName("LINE Pay")]
			LinePay,
			[EnumTextName("NP後払い")]
			NpAfterPay,
			[EnumTextName("EcPay")]
			EcPay,
			[EnumTextName("藍新Pay")]
			NewebPay,
			[EnumTextName("ベリトランス4G")]
			VeriTrans,
			[EnumTextName("PayTG")]
			PayTg,
			[EnumTextName("DSK後払い")]
			DskDeferred,
			[EnumTextName("PayPay")]
			PayPay,
			[EnumTextName("Rakuten")]
			Rakuten,
			[EnumTextName("Atobaraicom")]
			Atobaraicom,
			[EnumTextName("Boku")]
			Boku,
			[EnumTextName("Score")]
			Score,
			[EnumTextName("ペイジェント")]
			Paygent,
			[EnumTextName("Paygent")]
			PaygentEng,
			[EnumTextName("GMOアトカラ")]
			GmoAtokara,
			[EnumTextName("")]
			Unknown //ログ格納処理の時点で決済の処理名がわからなかったときに使用
		}

		/// <summary>
		/// 決済の処理名の列挙体
		/// </summary>
		public enum PaymentProcessingType
		{
			[EnumTextName("審査")]
			Examination,
			[EnumTextName("入金通知")]
			PaymentNotification,
			[EnumTextName("エラーメッセージ取得")]
			GetErrorMessage,
			[EnumTextName("SecureApi決済で本人確認")]
			IdentificationBySecureApi,
			[EnumTextName("実売上処理")]
			RealSalesProcessing,
			[EnumTextName("APIリクエスト開始")]
			ApiRequestStart,
			[EnumTextName("APIリクエスト")]
			ApiRequest,
			[EnumTextName("APIリクエスト終了")]
			ApiRequestEnd,
			[EnumTextName("APIリクエスト前")]
			BeforeApiRequest,
			[EnumTextName("APIリクエスト後")]
			AfterApiRequest,
			[EnumTextName("リクエスト")]
			Request,
			[EnumTextName("コミット")]
			Commit,
			[EnumTextName("リクエストエラー")]
			RequestError,
			[EnumTextName("APIエラー")]
			ApiError,
			[EnumTextName("再与信実行")]
			ReAuthExec,
			[EnumTextName("Sale")]
			Sale,
			[EnumTextName("SubmitForSettlement")]
			SubmitForSettlement,
			[EnumTextName("Refund")]
			Refund,
			[EnumTextName("Void")]
			Void,
			[EnumTextName("レスポンス出力")]
			ResponseOutput,
			[EnumTextName("レスポンス取得")]
			GetResponse,
			[EnumTextName("キャンセル処理")]
			Cancel,
			[EnumTextName("返品処理")]
			Return,
			[EnumTextName("Webコンビニ決済向け入金ステータスアップ")]
			PaymentStatusUpForWebCvs,
			[EnumTextName("ゼウスセキュアリンクバッチ決済実行")]
			ZeusSecureBatchPaymentExec,
			[EnumTextName("ゼウスセキュアリンク決済実行")]
			PaymentExecByZeusSecure,
			[EnumTextName("CREATE PAYPAL CUSTOMER")]
			CreatePaypalCustomer,
			[EnumTextName("クレジットカード登録")]
			RegistCreditCard,
			[EnumTextName("定期台帳クレジットカード登録")]
			FixedPurchaseCreditRegist,
			[EnumTextName("決済実行")]
			ExecPayment,
			[EnumTextName("1円与信キャンセル処理")]
			OneYenAuthCancel,
			[EnumTextName("連携ID更新")]
			CooperationIdUpdate,
			[EnumTextName("売上確定処理")]
			SalesConfirmation,
			[EnumTextName("カード実売上処理失敗エラー")]
			CardRealSalesProcessingFailureError,
			[EnumTextName("クレジット売上確定処理")]
			CreditSalesSettlementProcessing,
			[EnumTextName("クレジット決済処理")]
			CreditPaymentProcessing,
			[EnumTextName("コンビニ決済処理")]
			CvsPaymentProcessing,
			[EnumTextName("コンビニ決済処理(セブンイレブン)")]
			CvsPaymentProcessingBySevenEleven,
			[EnumTextName("コンビニ決済処理(ファミリーマート)")]
			CvsPaymentProcessingByFamilyMart,
			[EnumTextName("コンビニ決済処理(ローソン、サークルKサンクス、ミニストップ、セイコーマート)")]
			CvsPaymentProcessingByOtherCvs,
			[EnumTextName("コンビニ前払い決済処理")]
			CvsPaygentPaymentProcessing,
			[EnumTextName("支払契約を設定")]
			PaymentContractSetting,
			[EnumTextName("支払契約の承認")]
			PaymentContractApproval,
			[EnumTextName("注文生成")]
			OrderGeneration,
			[EnumTextName("注文情報設定")]
			OrderInfoSetting,
			[EnumTextName("注文情報の承認")]
			OrderInfoApproval,
			[EnumTextName("オーソリ処理")]
			OthoriProcessing,
			[EnumTextName("即時決済")]
			ImmediateSettlement,
			[EnumTextName("注文審査[フロント用]")]
			OrderReviewForFornt,
			[EnumTextName("注文確定依頼[フロント用]")]
			OrderConfirmationRequestForFront,
			[EnumTextName("注文取り込み(新規注文)")]
			OrderCaptureForNewOrder,
			[EnumTextName("クレジット登録確定処理")]
			CreditRegistrationConfirmationProcessing,
			[EnumTextName("注文キャンセル")]
			OrderCancel,
			[EnumTextName("SMS認証")]
			SmsAuth,
			[EnumTextName("注文情報更新")]
			OrderInfoUpdate,
			[EnumTextName("注文確定通知取得")]
			GetOrderConfirmationNotice,
			[EnumTextName("購入成功通知")]
			PurchaseSuccessNotification,
			[EnumTextName("Response")]
			Response,
			[EnumTextName("受信")]
			Receive,
			[EnumTextName("クレジット金額変更")]
			ChangeCreditAmount,
			[EnumTextName("取引情報照会")]
			TransactionInformationInquiry,
			[EnumTextName("出荷情報取り消し")]
			CancelShippingInformation,
			[EnumTextName("クレジットキャンセル処理")]
			CreditChancel,
			[EnumTextName("出荷情報登録")]
			ShippingInformationRegistratio,
			[EnumTextName("クレジット情報取得")]
			GetCreditInfo,
			[EnumTextName("クレジット情報削除")]
			DeleteCreditInfo,
			[EnumTextName("クレジットカード更新")]
			UpdateCreditInfo,
			[EnumTextName("クレジットカード登録レスポンス")]
			CreditCardRegistResponse,
			[EnumTextName("Cvs1")]
			PaymentByCvs1,
			[EnumTextName("Cvs2")]
			PaymentByCvs2,
			[EnumTextName("Cvs3")]
			PaymentByOtherCvs3,
			[EnumTextName("クレジット認証")]
			CreditAuth,
			[EnumTextName("クレジット決済処理(3Dセキュア認証)")]
			CreditPaymentWithThreeDSecure,
			[EnumTextName("Exec")]
			Exec,
			[EnumTextName("3DSecureRequest")]
			ThreeDSecureRequest,
			[EnumTextName("3Dセキュア対象外")]
			NotApplicableForThreeDSecure,
			[EnumTextName("Check3dAuth")]
			Outside,
			[EnumTextName("購入結果受信")]
			ReceivePurchaseResults,
			[EnumTextName("エラーコード受信")]
			ReceiveErrorCode,
			[EnumTextName("請求書印字情報取得")]
			AcquisitionOfInvoicePrintInformation,
			[EnumTextName("決済取消")]
			CancelPayment,
			[EnumTextName("出荷情報依頼")]
			ShippingInformationRequest,
			[EnumTextName("請求金額変更（減額）依頼")]
			ChargeChangeForReductionRequest,
			[EnumTextName("ヤマト決済(後払い)決済依頼API実行")]
			YamatoPaymentForPostpaidSettlementRequest,
			[EnumTextName("請求書再発行")]
			InvoiceReissue,
			[EnumTextName("更新スキップ")]
			SkipUpdate,
			[EnumTextName("受注ワークフロー決済連携処理")]
			OrderWorkFlowSettlementLinkageProcessing,
			[EnumTextName("ドコモケータイ払い決済")]
			DocomoTelePhonePayment,
			[EnumTextName("S!まとめて支払い決済")]
			SoftBankPaymentTogether,
			[EnumTextName("出荷報告")]
			ShippingReport,
			[EnumTextName("後払い出荷報告")]
			ShippingReportForPostpaid,
			[EnumTextName("最終請求金")]
			FinalBillAmount,
			[EnumTextName("受注ワークフロー決済連携処理(仮売り上げ⇒実売上")]
			OrderWorkFlowSettlementLinkageProcessingForVirtualToReal,
			[EnumTextName("入金／外部決済ステータスを変更しました。")]
			ChangePaymentAndExternalPaymentStatus,
			[EnumTextName("決済カード支払回数文言")]
			SettlementCardPaymentNumberOfTimesStatement,
			[EnumTextName("")]
			Unknown, //ログ格納処理の時点で決済の処理名がわからなかったときに使用
			[EnumTextName("ゼウス3Dセキュア認証結果通知")]
			Zeus3DSecureAuthResultNotification,
			[EnumTextName("ゼウス3Dセキュア認証データ送信")]
			Zeus3DSecureAuthResultSend,
			[EnumTextName("Zcom 3DSecure auth result notification")]
			Zcom3DSecureAuthResultNotification,
			[EnumTextName("ベリトランスセキュア認証結果通知")]
			VeritransSecureAuthResultNotification,
			[EnumTextName("ベリトランス会員とカード登録")]
			VeritransMembersAndCardRegistration,
			[EnumTextName("楽天3Dセキュア認証結果通知")]
			Rakuten3DSecureAuthResultNotification,
			[EnumTextName("楽天3Dセキュア認証データ送信")]
			Rakuten3DSecureAuthResultSend,
			[EnumTextName("後払い.com")]
			Atobaraicom,
			[EnumTextName("AmazonPay(CV2)ユーザ取得")]
			AmazonCv2GetBuyer,
			[EnumTextName("AmazonPay(CV2)チェックアウトセッション取得")]
			AmazonCv2GetCheckoutSession,
			[EnumTextName("AmazonPay(CV2)チェックアウトセッション更新")]
			AmazonCv2UpdateCheckoutSession,
			[EnumTextName("AmazonPay(CV2)定期用チェックアウトセッション更新")]
			AmazonCv2UpdateCheckoutSessionForFixedPurchase,
			[EnumTextName("AmazonPay(CV2)チェックアウトセッション完了")]
			AmazonCv2CompleteCheckoutSession,
			[EnumTextName("AmazonPay(CV2)チャージパーミッション注文ID更新")]
			AmazonCv2UpdateChargePermissionOrderId,
			[EnumTextName("AmazonPay(CV2)注文取消")]
			AmazonCv2CloseChargePermission,
			[EnumTextName("AmazonPay(CV2)チャージ生成")]
			AmazonCv2CreateCharge,
			[EnumTextName("AmazonPay(CV2)チャージ取得")]
			AmazonCv2GetCharge,
			[EnumTextName("AmazonPay(CV2)売上確定")]
			AmazonCv2CaptureCharge,
			[EnumTextName("AmazonPay(CV2)チャージキャンセル（注文取消）")]
			AmazonCv2CancelCharge,
			[EnumTextName("AmazonPay(CV2)返金")]
			AmazonCv2CreateRefund,
			[EnumTextName("ペイジェントカード決済オーソリ電文")]
			PaygentCardAuthApi,
			[EnumTextName("ペイジェントカード決済オーソリキャンセル電文")]
			PaygentCardAuthCancelApi,
			[EnumTextName("ペイジェントカード決済売上電文")]
			PaygentCardRealSaleApi,
			[EnumTextName("ペイジェントカード決済売上キャンセル電文")]
			PaygentCardRealSaleCancelApi,
			[EnumTextName("ペイジェントカード情報設定電文")]
			PaygentCardRegisterApi,
			[EnumTextName("ペイジェントカード情報削除電文")]
			PaygentCardDeleteApi,
			[EnumTextName("ペイジェントEMV3Dセキュア認証電文")]
			PaygentCard3DSecureAuthApi,
			[EnumTextName("オーソリキャンセル処理")]
			AuthorizeCancel,
			[EnumTextName("返金処理処理")]
			PaidyRefund,
			[EnumTextName("ATM決済")]
			AtmPayment,
			[EnumTextName("ATM決済処理")]
			AtmProcessing
		}

		/// <summary>
		/// 外部決済連携用ログ書き込みメソッド（取得できるIDがないとき利用）
		/// </summary>
		/// <param name="success">成功時のログであればtrue、成功時でも失敗時でもないただのログであればnull</param>
		/// <param name="paymentDetailType">決済種別</param>
		/// <param name="accountSettlementCompanyName">決済代行会社</param>
		/// <param name="processingContent">処理内容(再与信など…)</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		/// <param name="idKeyAndValueDictionary">キーがID名(注文IDなど)とバリューのディクショナリ</param>
		public static void WritePaymentLog(
			bool? success,
			string paymentDetailType,
			PaymentType accountSettlementCompanyName,
			PaymentProcessingType processingContent,
			string externalPaymentCooperationLog,
			Dictionary<string, string> idKeyAndValueDictionary = null)
		{
			var resultWord = success.HasValue ? success.Value ? "成功" : "失敗" : "ログ";

			string message;

			if ((paymentDetailType == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (accountSettlementCompanyName == PaymentType.Rakuten)
				&& Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE)
			{
				message = string.Empty;
			}
			else
			{
				message = ((success.HasValue && (success.Value == false)) ? "エラー" : "") + "メッセージ";
			}
			Encoding encoding = null;

			if (paymentDetailType == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) encoding = Encoding.UTF8;

			FileLogger.Write(
				"payment",
				string.Format(
					message == string.Empty
						? "[{0}] {1} {2} {3} {4} {5} {7}"
						: "[{0}] {1} {2} {3} {4} {5}{6}{7}",
					resultWord,
					paymentDetailType,
					accountSettlementCompanyName.ToText(),
					processingContent.ToText(),
					CreateIdString(idKeyAndValueDictionary),
					(string.IsNullOrEmpty(externalPaymentCooperationLog)) ? "" : message,
					(string.IsNullOrEmpty(externalPaymentCooperationLog)
						? string.Empty
						: ":"),
					(string.IsNullOrEmpty(externalPaymentCooperationLog))
						? ""
						: externalPaymentCooperationLog.Replace("\r\n", "\t")),
				false,
				encoding);
		}

		/// <summary>
		/// ID名(order_idやpayment_order_idなど)とIDの値の文字列を作成する
		/// </summary>
		/// <param name="idKeyAndValueDictionary">キーがID名(order_idなど)とバリューのディクショナリ</param>
		/// <returns>ID名とIDの値の文字列 例"order_id:〇〇〇"</returns>
		private static string CreateIdString(Dictionary<string, string> idKeyAndValueDictionary)
		{
			if ((idKeyAndValueDictionary == null) || (idKeyAndValueDictionary.Count == 0)) return "";
			var keyValuePairArray = idKeyAndValueDictionary.Where(
					keyValuePair =>
					{
						return ((string.IsNullOrEmpty(keyValuePair.Key) == false) && (string.IsNullOrEmpty(keyValuePair.Value) == false));
					})
				.ToArray();

			var result = string.Join(
				"\t",
				keyValuePairArray.Select(
					keyValuePair => string.Format("{0}：{1}", keyValuePair.Key, keyValuePair.Value)));
			return result;
		}
	}
}
