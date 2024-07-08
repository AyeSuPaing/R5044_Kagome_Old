/*
=========================================================================================================
  Module      : ソフトバンクペイメント マルチペイメント「購入結果」受信クラス(PaymentSBPSMultiPaymentReceiverOrderResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using w2.App.Common.Order.Payment;
using w2.Common.Helper;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチペイメント「購入結果」受信クラス
	/// </summary>
	public class PaymentSBPSMultiPaymentReceiverOrderResult : PaymentSBPSMultiPaymentReceiverBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="form">受取POST内容</param>
		public PaymentSBPSMultiPaymentReceiverOrderResult(NameValueCollection form)
			: this(PaymentSBPSSetting.GetDefaultSetting(), form)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="form">受取POST内容</param>
		public PaymentSBPSMultiPaymentReceiverOrderResult(
			PaymentSBPSSetting settings,
			NameValueCollection form)
			: base(settings, Encoding.GetEncoding("Shift_JIS"), form)
		{
		}

		/// <summary>
		/// アクション（レスポンスメッセージ生成）
		/// </summary>
		/// <param name="form">受取POST内容</param>
		/// <returns>解析結果</returns>
		public bool Action()
		{
			try
			{
				// 検証
				ValidateReceiveDatas();

				WriteLog(
					true,
					PaymentFileLogger.PaymentProcessingType.PurchaseSuccessNotification.ToText(),
					PaymentFileLogger.PaymentProcessingType.ReceivePurchaseResults,
					PaymentFileLogger.PaymentProcessingType.ReceivePurchaseResults.ToText(),
					"",
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, m_form["order_id"] },
						{ "sps_payment_no", m_form["sps_payment_no"] },
					});

				// 処理結果判定
				return true;
			}
			catch (SBPSException)
			{
				WriteErrorLog(
					PaymentFileLogger.PaymentProcessingType.ReceivePurchaseResults.ToText(),
					PaymentFileLogger.PaymentProcessingType.ReceivePurchaseResults,
					m_form["pay_type"] ?? "",
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, m_form["order_id"] },
						{ "sps_payment_no", m_form["sps_payment_no"] },
					});
			}

			return false;
		}
	}
}
