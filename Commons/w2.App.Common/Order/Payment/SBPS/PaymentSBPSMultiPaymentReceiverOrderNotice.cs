/*
=========================================================================================================
  Module      : ソフトバンクペイメント マルチペイメント「購入結果通知」受信クラス(PaymentSBPSMultiPaymentReceiverOrderNotice.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using w2.App.Common.Order.Payment;
using w2.Common.Helper;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチペイメント「購入結果通知」受信クラス
	/// </summary>
	public class PaymentSBPSMultiPaymentReceiverOrderNotice : PaymentSBPSMultiPaymentReceiverBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="form">受取POST内容</param>
		public PaymentSBPSMultiPaymentReceiverOrderNotice(NameValueCollection form)
			: this(PaymentSBPSSetting.GetDefaultSetting(), form)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="form">受取POST内容</param>
		public PaymentSBPSMultiPaymentReceiverOrderNotice(
			PaymentSBPSSetting settings,
			NameValueCollection form)
			: base(settings, Encoding.UTF8, form)
		{
		}

		/// <summary>
		/// アクション（レスポンスメッセージ生成）
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>結果</returns>
		public bool Action(string errorMessage = "")
		{
			try
			{
				// 検証
				ValidateReceiveDatas();

				// エラーメッセージがあればNGとする
				if (StringUtility.ToEmpty(errorMessage) != "")
				{
					throw new SBPSException(errorMessage);
				}

				// 処理結果判定
				switch (this.ResResult)
				{
					// OKであれば該当処理実行
					case ResResultType.OK:	// ※キャリア決済ではOKのみ。
						this.ResponseMessage = "OK,";
						WriteLog(
							true,
							m_form["pay_type"] ?? "",
							PaymentFileLogger.PaymentProcessingType.ReceivePurchaseResults,
							PaymentFileLogger.PaymentProcessingType.ReceivePurchaseResults.ToText(),
							"",
							new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, m_form["order_id"] },
								{ "sps_payment_no", m_form["sps_payment_no"] },
							});
						break;
				}
			}
			catch (SBPSException ex)
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
				this.ResponseMessage = "NG," + ex.Message;
				return false;
			}

			return true;
		}

		/// <summary>返却メッセージ</summary>
		public string ResponseMessage { get; private set; }
	}
}
