/*
=========================================================================================================
  Module      : ヤマト決済(後払い) レスポンスデータ基底クラス(PaymentYamatoFinancialBaseResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) レスポンスデータ基底クラス
	/// </summary>
	public abstract class PaymentYamatoKaBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal PaymentYamatoKaBaseResponseData(string responseString)
		{
			this.ResponseString = responseString;
			this.ResponseXml = XDocument.Parse(responseString);

			LoadXml(this.ResponseXml);
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public virtual void LoadXml(XDocument responseXml)
		{
			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "ycfStrCode":
						this.YcfStrCode = element.Value;
						break;

					case "orderNo":
						this.OrderNo = element.Value;
						break;

					case "returnCode":
						this.ReturnCode = element.Value;
						break;

					case "errorCode":
						this.ErrorCode = element.Value;
						break;

					case "requestDate":
						this.RequestDate = DateTime.ParseExact(
							element.Value,
							"yyyyMMddHHmmss",
							System.Globalization.DateTimeFormatInfo.InvariantInfo,
							System.Globalization.DateTimeStyles.None);
						break;
				}
			}
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="errorCode">エラーコード</param>
		/// <returns>エラーメッセージ</returns>
		private string GetErrorMessages(string errorCode)
		{
			if (errorCode == "")
			{
				return "";
			}
			var document = XDocument.Parse(Properties.Resources.YamatoKaErrorMessages);
			var errorMessage = document.Root.Elements("Message")
				.Where(e => e.Attributes("code").First().Value == errorCode)
				.Select(e => e.Value).FirstOrDefault();

			if (errorMessage == null)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.GetErrorMessage,
					string.Format("エラーが変換できませんでした。エラーコード:{0}", errorCode));
			}

			return errorMessage;
		}

		/// <summary>レスポンス文字列</summary>
		public string ResponseString { get; protected set; }
		/// <summary>レスポンスXML</summary>
		public XDocument ResponseXml { get; protected set; }
		/// <summary>加盟店コード</summary>
		public string YcfStrCode { get; protected set; }
		/// <summary>受注番号</summary>
		public string OrderNo { get; protected set; }
		/// <summary>結果コード</summary>
		public string ReturnCode { get; protected set; }
		/// <summary>成功したか</summary>
		public bool IsSuccess { get { return this.ReturnCode == "0"; } }
		/// <summary>送信日時</summary>
		public DateTime RequestDate { get; protected set; }
		/// <summary>エラーコード</summary>
		public string ErrorCode
		{
			get { return m_errorCode; }
			protected set
			{
				m_errorCode = value;
				m_errorMessages = null;
			}
		}
		private string m_errorCode = null;
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessages
		{
			get
			{
				if (m_errorMessages == null)
				{
					m_errorMessages = GetErrorMessages(this.ErrorCode);
				}
				return m_errorMessages;
			}
		}
		private string m_errorMessages = null;
	}
}
