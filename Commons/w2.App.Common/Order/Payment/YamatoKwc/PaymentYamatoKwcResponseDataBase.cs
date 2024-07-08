/*
=========================================================================================================
  Module      : ヤマトKWC レスポンスデータ基底クラス(PaymentYamatoKwcResponseDataBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC レスポンスデータ基底クラス
	/// </summary>
	public abstract class PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		protected PaymentYamatoKwcResponseDataBase(string responseString)
		{
			this.ResponseString = responseString;
			this.ResponseXml = XDocument.Parse(responseString);

			SetPropertyFromXml(this.ResponseXml);
		}

		/// <summary>
		/// XMLをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public virtual void SetPropertyFromXml(XDocument responseXml)
		{
			// 固有の値をセット
			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "returnCode":
						this.ReturnCode = element.Value;
						break;

					case "errorCode":
						this.ErrorCode = element.Value;
						break;

					case "returnDate":
						this.ReturnDate = DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
						break;
				}
			}
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="errorCode">エラーコード</param>
		/// <returns>エラーメッセージ</returns>
		private string GetErrorMessage(string errorCode)
		{
			var errorMessage = GetMessage(Properties.Resources.YamatoKwcErrorMessage, errorCode);
			return errorMessage;
		}

		/// <summary>
		/// Ｃ、Ｇメッセージ取得
		/// </summary>
		/// <param name="creditErrorCode">与信結果エラーコード</param>
		/// <returns>エラーメッセージ</returns>
		protected string GetCgMessage(string creditErrorCode)
		{
			var errorMessage = GetMessage(Properties.Resources.YamatoKwcCGMessage, creditErrorCode);
			return errorMessage;
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="messageXmlString">メッセージXML文字列</param>
		/// <param name="code">コード</param>
		/// <returns>エラーメッセージ</returns>
		private string GetMessage(string messageXmlString, string code)
		{
			if (string.IsNullOrEmpty(code)) return "";

			var document = XDocument.Parse(messageXmlString);
			var errorMessage = document.Root.Elements("Message")
				.Where(e => e.Attributes("code").First().Value == code)
				.Select(e => e.Value).FirstOrDefault();

			if (errorMessage == null)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					"",
					PaymentFileLogger.PaymentType.Yamatokwc,
					PaymentFileLogger.PaymentProcessingType.GetErrorMessage,
					"エラーメッセージが変換できませんでした。");
			}


			return errorMessage;
		}

		/// <summary>レスポンス文字列</summary>
		public string ResponseString { get; set; }
		/// <summary>レスポンスXML</summary>
		public XDocument ResponseXml { get; set; }

		/// <summary>結果コード</summary>
		public string ReturnCode { get; set; }
		/// <summary>エラーコード</summary>
		public string ErrorCode { get; set; }
		/// <summary>送信日時</summary>
		public DateTime ReturnDate { get; private set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return m_errMessages ?? (m_errMessages = GetErrorMessage(this.ErrorCode)); }
		}
		private string m_errMessages = null;

		/// <summary>ログ出力用エラー情報</summary>
		public string ErrorInfoForLog
		{
			get
			{
				if (string.IsNullOrEmpty(this.ErrorCode)) return "";
				return string.Format("{0} ({1})", this.ErrorMessage, this.ErrorCode);
			}
		}
		/// <summary>成功か</summary>
		public bool Success
		{
			get { return this.ReturnCode == "0"; }
		}
	}
}
