/*
=========================================================================================================
  Module      : ソフトバンクペイメント マルチペイメント「エラー情報」受信クラス(PaymentSBPSMultiPaymentReceiverOrderError.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチペイメント「エラー情報」受信クラス
	/// </summary>
	public class PaymentSBPSMultiPaymentReceiverOrderError : PaymentSBPSMultiPaymentReceiverBase
	{
		/// <summary>エラーコードタイプ</summary>
		public enum ErroCodeTypes
		{
			/// <summary>カードNG</summary>
			CardNG,
			/// <summary>カードキャンセル</summary>
			CardCancel,
			/// <summary>その他</summary>
			Others
		}

		/// <summary>カードNGエラーコード</summary>
		readonly static string[] CARD_NG_ERRORCODE =
		{
			// 通常クレジット
			"7202", "7203", "7204", "7205", "7212", "7230", "7242", "7254", "7255", "7256", "7260",
			"7261", "7265", "7267", "7268", "7269", "7270", "7271", "7272", "7273", "7274", "7275", 
			"7276", "7277", "7278", "7280", "7281", "7283", "7284", "7292", "7294", "7295", "7297",
			"7298", "7299", "7103", "7115", "7220", "7221", "7222",
			// 3Dセキュア
			"7244", "7245", "7246", "7702", "7703", "7704", "7705", "7716", "7971", "7800", "7801",
			"7802", "7904", "7905",
		};

		/// <summary>カードキャンセルエラーコード</summary>
		readonly static string[] CARD_CANCEL_ERRORCODE =
		{
			// 3Dセキュア
			"7717"
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="form">受取POST内容</param>
		public PaymentSBPSMultiPaymentReceiverOrderError(NameValueCollection form)
			: this(PaymentSBPSSetting.GetDefaultSetting(), form)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="form">受取POST内容</param>
		public PaymentSBPSMultiPaymentReceiverOrderError(
			PaymentSBPSSetting settings,
			NameValueCollection form)
			: base(settings, Encoding.GetEncoding("Shift_JIS"), form)
		{
			if (CARD_NG_ERRORCODE.Contains(this.ErrorCode))
			{
				this.ErrorCodeType = ErroCodeTypes.CardNG;
			}
			else if (CARD_CANCEL_ERRORCODE.Contains(this.ErrorCode))
			{
				this.ErrorCodeType = ErroCodeTypes.CardCancel;
			}
			else
			{
				this.ErrorCodeType = ErroCodeTypes.Others;
				WriteErrorLog(
					"ERROR,エラーコード：" + this.ErrorCode,
					PaymentFileLogger.PaymentProcessingType.ReceiveErrorCode,
					"マルチペイメント");
			}
		}

		/// <summary>エラーコード</summary>
		public string ErrorCode { get { return m_form["res_err_code"]; } }
		/// <summary>エラーコードタイプ</summary>
		public ErroCodeTypes ErrorCodeType { get; private set; }
	}
}
