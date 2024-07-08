/*
=========================================================================================================
  Module      : ソフトバンクペイメント レスポンスデータ基底クラス(PaymentSBPSBaseResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント レスポンスデータ基底クラス
	/// </summary>
	public abstract class PaymentSBPSBaseResponseData
	{
		/// <summary>レスポンスXML</summary>
		private XDocument m_responseXml = null;
		/// <summary>3DESオブジェクト</summary>
		private PaymentSBPSTripleDESCrypto m_tripleDES = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSBaseResponseData(PaymentSBPSSetting settings)
		{
			if (settings.TripleDESKeyAndIV.HasValue)
			{
				m_tripleDES = new PaymentSBPSTripleDESCrypto(
					settings.TripleDESKeyAndIV.Value.Key,
					settings.TripleDESKeyAndIV.Value.Value);
			}
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public virtual void LoadXml(XDocument responseXml)
		{
			m_responseXml = responseXml;

			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "res_result":
						this.ResResult = element.Value;
						break;

					case "res_sps_transaction_id":
						this.ResSpsTransactionId = element.Value;
						break;

					case "res_tracking_id":
						this.ResTrackingId = element.Value;
						break;

					case "res_process_date":
						this.ResProcessDate = DateTime.ParseExact(
							element.Value,
							"yyyyMMddHHmmss",
							System.Globalization.DateTimeFormatInfo.InvariantInfo,
							System.Globalization.DateTimeStyles.None);
						break;

					case "res_err_code":
						this.ResErrCode = element.Value;
						break;

					case "res_date":
						this.ResDate = DateTime.ParseExact(
							element.Value,
							"yyyyMMddHHmmss",
							System.Globalization.DateTimeFormatInfo.InvariantInfo,
							System.Globalization.DateTimeStyles.None);
						break;
				}
			}
		}

		/// <summary>
		/// 復号化データ取得
		/// </summary>
		/// <param name="source">対象</param>
		/// <returns>復号化データ</returns>
		protected string GetDecryptedData(string source)
		{
			if (m_tripleDES == null)
			{
				return source;
			}
			return m_tripleDES.GetDecryptedData(source);
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="resErrorCode">エラーコード</param>
		/// <returns>エラーメッセージ</returns>
		private string GetErrorMessages(string resErrorCode)
		{
			if (resErrorCode == "")
			{
				return "";
			}
			string paymentMethodCode = resErrorCode.Substring(0, 3);
			string errorTypeCode = resErrorCode.Substring(3, 2);
			string errorFieldCode = resErrorCode.Substring(5, 3);

			XDocument document = XDocument.Parse(Properties.Resources.SBPSApiMessages);
			var errorMessage = (from elem in document.Root.Elements("ErrorTypes")
								where (elem.Attribute("method").Value.Contains("*") || elem.Attribute("method").Value.Contains(paymentMethodCode))
								&& (elem.Elements("Message").Any(e => e.Attribute("code").Value == errorTypeCode))
								select elem.Elements("Message").Where(e => e.Attribute("code").Value == errorTypeCode).First().Value).ToArray();
			var errorField = (from elem in document.Root.Elements("ErrorFields")
							  where (elem.Attribute("method").Value.Contains("*") || elem.Attribute("method").Value.Contains(paymentMethodCode))
							  && (elem.Elements("Message").Any(e => e.Attribute("code").Value == errorFieldCode))
							  select elem.Elements("Message").Where(e => e.Attribute("code").Value == errorFieldCode).First().Value).ToArray();

			StringBuilder result = new StringBuilder();
			foreach (string message in errorMessage)
			{
				result.Append(message);
			}
			foreach (string message in errorField)
			{
				result.Append("[").Append(message).Append("]");
			}

			if (result.Length == 0)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					"",
					PaymentFileLogger.PaymentType.Sbps,
					PaymentFileLogger.PaymentProcessingType.GetErrorMessage,
					string.Format("SBPSエラーが変換できませんでした。エラーコード{0}", resErrorCode));
			}

			return result.ToString();
		}

		/// <summary>処理結果ステータス</summary>
		public string ResResult { get; protected set; }
		/// <summary>処理SPS トランザクションID</summary>
		public string ResSpsTransactionId { get; protected set; }
		/// <summary>処理トラッキングID</summary>
		public string ResTrackingId { get; protected set; }
		/// <summary>処理完了日時</summary>
		public DateTime ResProcessDate { get; protected set; }
		/// <summary>エラーコード</summary>
		public string ResErrCode
		{
			get { return m_resErrCode; }
			protected set
			{
				m_resErrCode = value;
				m_resErrMessages = null;
			}
		}
		/// <summary>エラー種別コード（エラーコードの4-5桁目）</summary>
		public string ErrorTypeCode
		{
			get
			{
				if (string.IsNullOrEmpty(this.ResErrCode)) return "";

				var errorTypeCode = this.ResErrCode.Substring(3, 2);
				return errorTypeCode;
			}
		}
		private string m_resErrCode = null;
		/// <summary>エラーメッセージ</summary>
		public string ResErrMessages
		{
			get
			{
				if (m_resErrMessages == null)
				{
					m_resErrMessages = GetErrorMessages(this.ResErrCode);
				}
				return m_resErrMessages;
			}
		}
		private string m_resErrMessages = null;
		/// <summary>レスポンス日時</summary>
		public DateTime ResDate { get; protected set; }
	}
}
