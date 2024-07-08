/*
=========================================================================================================
  Module      : SBPS Webコンビニ決済向け入金ステータスアップデータ(OrderPaymentStatusUpdaterForSBPSCvs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using w2.App.Common.Order.Payment;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS Webコンビニ決済向け入金ステータスアップデータ
	/// </summary>
	public class OrderPaymentStatusUpdaterForSBPSCvs
	{
		/// <summary>
		/// 入力ストーリムを解析して入金ステータスを更新、レスポンス文字を作成する
		/// </summary>
		/// <param name="inputStream">入力ストリーム</param>
		/// <returns>レスポンス文字列</returns>
		public string Exec(Stream inputStream)
		{
			/// <summary>ソフトバンクペイメント WEBコンビニ「入金通知処理」受取APIクラス</summary>
			PaymentSBPSCvsPaymentApiRcv apiRcv = new PaymentSBPSCvsPaymentApiRcv();

			string requestXml = null;
			try
			{
				// リクエストXML取得
				requestXml = GetRequestXml(inputStream);

				// 解析
				apiRcv.Receive(requestXml);

				// 該当注文取得・チェック
				DataView order = GetTargetOrder(apiRcv.ResponseData.OrderId, apiRcv.ResponseData.TrackingId);

				// 入金ステータス更新
				UpdatePaymentStatus(apiRcv.ResponseData.RecType, apiRcv.ResponseData.RecDatetime, order);

				// 外部決済連携ログ
				PaymentFileLogger.WritePaymentLog(
					true,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
					PaymentFileLogger.PaymentType.Sbps,
					PaymentFileLogger.PaymentProcessingType.PaymentStatusUpForWebCvs,
					"",
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, apiRcv.ResponseData.OrderId },
						{ Constants.FOR_LOG_KEY_TRACKING_ID, apiRcv.ResponseData.TrackingId }
					}
					);
			}
			catch (Exception ex)
			{
				return CreateResponseString(false, ex.Message + ((ex.InnerException != null) ? ex.Message : ""), apiRcv);
			}

			// OKを返す
			return CreateResponseString(true, "", apiRcv);
		}

		/// <summary>
		/// ターゲット注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="trackingId">トラッキングID</param>
		/// <returns>注文情報</returns>
		private static DataView GetTargetOrder(string orderId, string trackingId)
		{
			DataView order = OrderCommon.GetOrder(orderId);
			if ((order.Count == 0)
				|| ((string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID] != trackingId))
			{
				throw new Exception("該当注文が見つかりませんでした：" + orderId + " " + trackingId);
			}
			return order;
		}

		/// <summary>
		/// リクエストXML取得
		/// </summary>
		/// <param name="inputStream">入力ストリーム</param>
		/// <returns>リクエストXML</returns>
		private string GetRequestXml(Stream inputStream)
		{
			if (inputStream.CanRead == false)
			{
				throw new Exception("XMLが読み込めません。");
			}
			using (StreamReader reader = new StreamReader(inputStream))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// 入金ステータス更新
		/// </summary>
		/// <param name="recType">入金タイプ</param>
		/// <param name="paymentDate">入金日</param>
		/// <param name="order">注文情報</param>
		private void UpdatePaymentStatus(
			PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes recType,
			DateTime paymentDate,
			DataView order)
		{
			// 処理
			switch (recType)
			{
				// 入金処理（未入金→入金済み）
				case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.PrompReport:
				case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.FixReport:
					if ((string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)
					{
						var updated = new OrderService().UpdatePaymentStatusForCvs(
							(string)order[0][Constants.FIELD_ORDER_ORDER_ID],
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
							paymentDate,
							(string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID],
							Constants.FLG_LASTCHANGED_CGI,
							UpdateHistoryAction.Insert);
						if (updated == 0) throw new Exception("入金済に更新できませんでした。:" + (string)order[0][Constants.FIELD_ORDER_ORDER_ID]);

						// Update FixPurchaseMemberFlg by Settings and Order Fixed purchase
						if (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE
							&& (String.IsNullOrEmpty(StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_FIXED_PURCHASE_ID])) == false))
						{
							new UserService().UpdateFixedPurchaseMemberFlg(
								(string)order[0][Constants.FIELD_ORDER_USER_ID],
								Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
								Constants.FLG_LASTCHANGED_CGI,
								UpdateHistoryAction.DoNotInsert);
						}
					}
					break;

				// 入金戻し（入金済み→未入金）
				case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.PrompReportCansel:
				case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.FixReportCansel:
					if ((string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
					{
						new OrderService().UpdatePaymentStatusForCvs(
							(string)order[0][Constants.FIELD_ORDER_ORDER_ID],
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM,
							null,
							(string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID],
							Constants.FLG_LASTCHANGED_CGI,
							UpdateHistoryAction.Insert);
					}
					break;
			}
		}

		/// <summary>
		/// レスポンス文字列作成
		/// </summary>
		/// <param name="result">結果</param>
		/// <param name="message">メッセージ</param>
		/// <param name="apiRcv">ソフトバンクペイメント WEBコンビニ「入金通知処理」受取APIクラス</param>
		private string CreateResponseString(bool result, string message, PaymentSBPSCvsPaymentApiRcv apiRcv)
		{
			return "<?xml version=\"1.0\" encoding=\"Shift_JIS\"?>\r\n"
				+ apiRcv.CreateResponseXml(result, message);
		}
	}
}
