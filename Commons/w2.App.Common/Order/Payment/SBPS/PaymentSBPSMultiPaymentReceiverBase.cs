/*
=========================================================================================================
  Module      : ソフトバンクペイメント マルチペイメントレスポンス受信基底クラス(PaymentSBPSMultiPaymentReceiverBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using w2.App.Common.Order.Payment;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチペイメントレスポンス受信基底クラス
	/// </summary>
	public class PaymentSBPSMultiPaymentReceiverBase : PaymentSBPSBaseLink
	{
		/// <summary>処理結果ステータス</summary>
		public enum ResResultType { OK };

		/// <summary>
		/// 受信対象フィールド一覧（チェックサム用）
		/// </summary>
		protected List<string> m_receivefields = new List<string>()
		{
			"pay_method", "merchant_id", "service_id",	"cust_code", "sps_cust_no", "sps_payment_no", "order_id", "item_id", "pay_item_id", "item_name", "tax", "amount", "pay_type", "auto_charge_type", "service_type", "div_settele", "last_charge_month", "camp_type", "tracking_id", "terminal_type", "free1", "free2", "free3", 
			"request_date", "res_pay_method", "res_result", "res_tracking_id", "res_sps_cust_no", "res_sps_payment_no", "res_payinfo_key", "res_payment_date", "res_err_code", "res_date", "limit_second"
		};

		/// <summary>受取POST内容</summary>
		protected NameValueCollection m_form = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="hashEncoding">ハッシュエンコーディング</param>
		/// <param name="form">受取POST内容</param>
		public PaymentSBPSMultiPaymentReceiverBase(
			PaymentSBPSSetting settings,
			Encoding hashEncoding,
			NameValueCollection form)
			: base(settings, hashEncoding)
		{
			m_form = form;
		}

		/// <summary>
		/// 受け取りデータ検証（エラーはSBPSException例外発生）
		/// </summary>
		/// <returns>成功/失敗</returns>
		protected void ValidateReceiveDatas()
		{
			// チェックサム判定
			if (ValidateCheckSum(m_receivefields) == false)
			{
				throw new SBPSException("チェックサムエラー。");
			}

			// サービス判定
			if ((m_form["merchant_id"] != this.Settings.MerchantId)
				|| (m_form["service_id"] != this.Settings.ServiceId))
			{
				throw new SBPSException("マーチャントID、サービスIDが異なります。");
			}
		}

		/// <summary>
		/// チェックサム検証
		/// </summary>
		/// <param name="fields">受信対象フィールド一覧</param>
		/// <returns>検証結果</returns>
		protected bool ValidateCheckSum(List<string> fields)
		{
			foreach (string key in fields)
			{
				this.HashCalculator.Add(m_form[key]);
			}
			string checkSum = this.HashCalculator.ComputeHashSHA1AndClearBuffer();

			return (checkSum == m_form["sps_hashcode"].ToLower());
		}

		/// <summary>
		/// エラーログ出力
		/// </summary>
		/// <param name="message">エラーメッセージ</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="paymentType">決済種別</param>
		/// <param name="idDictionary">ID名とIDのディクショナリ。例:{orderId,〇〇〇}</param>
		protected void WriteErrorLog(string message, PaymentFileLogger.PaymentProcessingType processingContent, string paymentType, Dictionary<string, string> idDictionary = null)
		{
			base.WriteLog(false, paymentType, processingContent, message, CreateFormString(), idDictionary);
		}

		/// <summary>
		/// フォーム文字列作成
		/// </summary>
		/// <returns></returns>
		protected string CreateFormString()
		{
			StringBuilder formString = new StringBuilder();
			foreach (string key in m_form.Keys)
			{
				formString.Append("[").Append(key).Append("] ").Append(m_form[key]).Append("\t");
			}
			return formString.ToString();
		}

		/// <summary>
		/// SBPS例外
		/// </summary>
		internal class SBPSException : Exception
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="message"></param>
			internal SBPSException(string message)
				: base(message)
			{
			}
		}

		/// <summary>決済注文ID</summary>
		public string PaymentOrderId { get { return (string)m_form["order_id"]; } }
		/// <summary>支払い方法</summary>
		public PaymentSBPSTypes.PayMethodTypes PayMethod { get { return (PaymentSBPSTypes.PayMethodTypes)Enum.Parse(typeof(PaymentSBPSTypes.PayMethodTypes), (string)m_form["pay_method"]); } }
		/// <summary>顧客ID(連携ID)</summary>
		public string CustomerCode { get { return (string)m_form["cust_code"]; } }
		/// <summary>決済種別ID</summary>
		public string PaymentId { get { return PaymentSBPSUtil.ConvertPayMethodTypeToPaymentId(this.PayMethod); } }
		/// <summary>SBPS処理結果</summary>
		public ResResultType? ResResult { get { return (ResResultType?)Enum.Parse(typeof(ResResultType), m_form["res_result"]); } }
		/// <summary>トラッキングID</summary>
		public string TrackingId { get { return (string)m_form["res_tracking_id"]; } }
		/// <summary>ユーザーID</summary>
		public string UserId { get { return (string)this.CustomerCode.Substring(0, Constants.CONST_USER_ID_HEADER.Length + Constants.CONST_USER_ID_LENGTH); } }
		/// <summary>顧客決済情報</summary>
		public string PayinfoKey { get { return StringUtility.ToEmpty(m_form["res_payinfo_key"]); } }
	}
}
